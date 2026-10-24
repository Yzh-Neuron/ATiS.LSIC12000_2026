using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ATiS.Sim.Communication
{
    public class ModbusTcpClient : TcpBase, IModbusClient
    {
        public ModbusTcpClient(byte station, string ip, int port = 502) : base(260)
        {
            this._station = station;
            RemoteAddress = new IPEndPoint(IPAddress.Parse(ip), port);
        }

        private readonly byte _station;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        private ushort _sn = 0;
        private Task _connectTask;
        private bool _isReportedDisconnect;

        public IPEndPoint RemoteAddress { get; }

        private CancellationTokenSource _connectLife;
        private CancellationTokenSource ConnectLife
        {
            get => Volatile.Read(ref _connectLife);
            set
            {
                CancellationTokenSource old = Volatile.Read(ref _connectLife);
                if (!ReferenceEquals(old, value))
                {
                    Volatile.Write(ref _connectLife, value);
                    Connected = value != null;
                }
            }
        }

        private volatile bool _connected;
        public bool Connected
        {
            get => _connected;
            private set
            {
                if (_connected != value)
                {
                    _connected = value;
                    OnStateChanged(Socket.LocalEndPoint as IPEndPoint, RemoteAddress, value);
                }
            }
        }

        protected override void InternalStart()
        {
            _connectTask = ConnectJob();
        }

        #region 读写操作

        public async Task<bool?> ReadCoilAsync(ushort address)
        {
            bool[] values = await ReadCoilsAsync(address, 1).ConfigureAwait(false);
            return values?[0];
        }

        public async Task<bool[]> ReadCoilsAsync(ushort address, ushort quantity)
        {
            if (quantity > 2000)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity), "quantity <= 2000");
            }
            ModbusResponse resp = await ReadAsync(ModbusFunction.ReadCoils, address, quantity).ConfigureAwait(false);
            if (!resp.Ok)
            {
                return null;
            }
            bool[] values = new bool[quantity];
            for (int i = 0; i < quantity; i++)
            {
                values[i] = ((resp.Buffer[resp.Offset + i / 8] >> (i % 8)) & 0x1) == 0x1;
            }
            return values;
        }

        public async Task<bool?> ReadDiscreteInputAsync(ushort address)
        {
            bool[] values = await ReadDiscreteInputsAsync(address, 1).ConfigureAwait(false);
            return values?[0];
        }

        public async Task<bool[]> ReadDiscreteInputsAsync(ushort address, ushort quantity)
        {
            if (quantity > 2000)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity), "quantity <= 2000");
            }
            ModbusResponse resp = await ReadAsync(ModbusFunction.ReadDiscreteInputs, address, quantity).ConfigureAwait(false);
            if (!resp.Ok)
            {
                return null;
            }
            bool[] values = new bool[quantity];
            for (int i = 0; i < quantity; i++)
            {
                values[i] = ((resp.Buffer[resp.Offset + i / 8] >> (i % 8)) & 0x1) == 0x1;
            }
            return values;
        }

        public async Task<short?> ReadHoldingRegisterAsync(ushort address)
        {
            short[] values = await ReadHoldingRegistersAsync(address, 1).ConfigureAwait(false);
            return values?[0];
        }

        public async Task<short[]> ReadHoldingRegistersAsync(ushort address, ushort quantity)
        {
            if (quantity > 125)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity), "quantity <= 125");
            }
            ModbusResponse resp = await ReadAsync(ModbusFunction.ReadHoldingRegisters, address, quantity).ConfigureAwait(false);
            if (!resp.Ok)
            {
                return null;
            }
            short[] values = new short[quantity];
            for (int i = 0; i < quantity; i++)
            {
                values[i] = (short)(resp.Buffer[resp.Offset + i * 2] << 8 | resp.Buffer[resp.Offset + i * 2 + 1]);
            }
            return values;
        }

        public async Task<short?> ReadInputRegisterAsync(ushort address)
        {
            short[] values = await ReadInputRegistersAsync(address, 1).ConfigureAwait(false);
            return values?[0];
        }

        public async Task<short[]> ReadInputRegistersAsync(ushort address, ushort quantity)
        {
            if (quantity > 125)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity), "quantity <= 125");
            }
            ModbusResponse resp = await ReadAsync(ModbusFunction.ReadInputRegisters, address, quantity).ConfigureAwait(false);
            if (!resp.Ok)
            {
                return null;
            }
            short[] values = new short[quantity];
            for (int i = 0; i < quantity; i++)
            {
                values[i] = (short)(resp.Buffer[resp.Offset + i * 2] << 8 | resp.Buffer[resp.Offset + i * 2 + 1]);
            }
            return values;
        }

        public async Task<bool> WriteSingleCoilAsync(ushort address, bool value)
        {
            return await WriteAsync(address, value).ConfigureAwait(false);
        }

        public async Task<bool> WriteMultipleCoilsAsync(ushort address, bool[] values)
        {
            if (values.Length > 1968)
            {
                throw new ArgumentOutOfRangeException(nameof(values.Length), "values.Length <= 1968");
            }
            return await WriteAsync(address, values).ConfigureAwait(false);
        }

        public async Task<bool> WriteSingleRegisterAsync(ushort address, short value)
        {
            return await WriteAsync(address, value).ConfigureAwait(false);
        }

        public async Task<bool> WriteMultipleRegistersAsync(ushort address, short[] values)
        {
            if (values.Length > 123)
            {
                throw new ArgumentOutOfRangeException(nameof(values.Length), "values.Length <= 123");
            }
            return await WriteAsync(address, values).ConfigureAwait(false);
        }

        #endregion

        private async Task ConnectJob()
        {
            while (!LifeToken.IsCancellationRequested)
            {
                try
                {
                    CancellationTokenSource connectLife = ConnectLife;
                    if (connectLife is { IsCancellationRequested: false } && await RemoteAddress.PingAsync().ConfigureAwait(false))
                    {
                        continue;
                    }
                    if (connectLife != null)
                    {
                        connectLife.Cancel();
                        bool semaphoreHeld = false;
                        try
                        {
                            await _semaphore.WaitAsync(LifeToken).ConfigureAwait(false);
                            semaphoreHeld = true;
                        }
                        finally
                        {
                            if (semaphoreHeld)
                            {
                                _semaphore.Release();
                            }
                            connectLife.Dispose();
                            ConnectLife = null;
                        }
                        await Socket.DisconnectAsync(false, LifeToken).ConfigureAwait(false);
                        ResetSocket();
                        continue;
                    }

                    await Socket.ConnectAsync(RemoteAddress, LifeToken).ConfigureAwait(false);
                    ConnectLife = CancellationTokenSource.CreateLinkedTokenSource(LifeToken);
                    _isReportedDisconnect = false;
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception e)
                {
                    if (!_isReportedDisconnect)
                    {
                        OnException(Socket.LocalEndPoint as IPEndPoint, RemoteAddress, e);
                        _isReportedDisconnect = true;
                    }
                }
                finally
                {
                    try
                    {
                        await Task.Delay(CheckInterval, LifeToken).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        // ignore
                    }
                }
            }
        }

        public async Task<ModbusResponse> ReadAsync(ModbusFunction func, ushort address, ushort quantity)
        {
            ThrowIfDisposed();

            CancellationTokenSource connectLife = ConnectLife;
            if (connectLife == null || connectLife.IsCancellationRequested)
            {
                return ModbusResponse.Fail();
            }

            bool semaphoreHeld = false;
            try
            {
                await _semaphore.WaitAsync(connectLife.Token).ConfigureAwait(false);
                semaphoreHeld = true;
                await ClearReceiveBufferAsync(connectLife.Token).ConfigureAwait(false);

                byte[] pack = new byte[12];
                int index = 0;

                pack[index++] = (byte)(_sn >> 8);
                pack[index++] = (byte)(_sn++ & 0xFF);

                pack[index++] = 0x00;
                pack[index++] = 0x00;

                pack[index++] = 0x00;
                pack[index++] = 0x06;

                pack[index++] = _station;

                pack[index++] = (byte)func;

                pack[index++] = (byte)(address >> 8);
                pack[index++] = (byte)(address & 0xFF);

                pack[index++] = (byte)(quantity >> 8);
                pack[index] = (byte)(quantity & 0xFF);

                using (CancellationTokenSource cancellationToken = new CancellationTokenSource(TimeSpan.FromMilliseconds(SendTimeout)))
                {
                    int offset = 0;
                    while (offset < pack.Length)
                    {
                        ArraySegment<byte> segment = new ArraySegment<byte>(pack, offset, pack.Length - offset);
                        int sent = await Socket.SendAsync(segment, SocketFlags.None, cancellationToken.Token).ConfigureAwait(false);
                        offset += sent;
                    }
                }

                int mbap = 7;
                int pdu;
                switch (func)
                {
                    case ModbusFunction.ReadCoils:
                    case ModbusFunction.ReadDiscreteInputs:
                        pdu = 2 + (quantity + 7) / 8;
                        break;
                    case ModbusFunction.ReadHoldingRegisters:
                    case ModbusFunction.ReadInputRegisters:
                        pdu = 2 + quantity * 2;
                        break;
                    case ModbusFunction.WriteSingleCoil:
                    case ModbusFunction.WriteSingleRegister:
                    case ModbusFunction.WriteMultipleCoils:
                    case ModbusFunction.WriteMultipleRegisters:
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                using (CancellationTokenSource cancellationToken = new CancellationTokenSource(TimeSpan.FromMilliseconds(ReceiveTimeout)))
                {
                    byte[] buffer = new byte[mbap];
                    int offset = 0;
                    while (offset < buffer.Length)
                    {
                        ArraySegment<byte> segment = new ArraySegment<byte>(buffer, offset, buffer.Length - offset);
                        int received = await Socket.ReceiveAsync(segment, SocketFlags.None, cancellationToken.Token).ConfigureAwait(false);
                        if (received == 0)
                        {
                            throw new SocketException((int)SocketError.Shutdown);
                        }
                        offset += received;
                    }

                    index = 0;
                    if (buffer[index] != pack[index++] ||
                        buffer[index] != pack[index++] ||
                        buffer[index++] != 0x00 ||
                        buffer[index++] != 0x00)
                    {
                        throw new Exception("response error.");
                    }

                    int length = buffer[index++] << 8 | buffer[index++];
                    byte stat = buffer[index];

                    buffer = new byte[length - 1];
                    offset = 0;
                    while (offset < buffer.Length)
                    {
                        ArraySegment<byte> segment = new ArraySegment<byte>(buffer, offset, buffer.Length - offset);
                        int received = await Socket.ReceiveAsync(segment, SocketFlags.None, cancellationToken.Token).ConfigureAwait(false);
                        if (received == 0)
                        {
                            throw new SocketException((int)SocketError.Shutdown);
                        }
                        offset += received;
                    }

                    // Modbus 异常响应（功能码最高位已置位）：属正常协议交互，不断连，仅按失败返回
                    if ((buffer[0] & 0x80) != 0 && (buffer[0] & 0x7F) == (byte)func)
                    {
                        return ModbusResponse.Fail();
                    }

                    if (length != 1 + pdu)
                    {
                        throw new Exception("response error.");
                    }

                    if (stat != _station)
                    {
                        throw new Exception("response error.");
                    }

                    index = 0;

                    if (buffer[index++] != (byte)func ||
                        buffer[index++] != pdu - 2)
                    {
                        throw new Exception("response error.");
                    }

                    return ModbusResponse.Success(buffer, index);
                }
            }
            catch (Exception e)
            {
                connectLife.Cancel();
                Connected = false;
                OnException(Socket.LocalEndPoint as IPEndPoint, RemoteAddress, e);
                return ModbusResponse.Fail();
            }
            finally
            {
                if (semaphoreHeld)
                {
                    _semaphore.Release();
                }
            }
        }

        public async Task<bool> WriteAsync(ushort address, bool value)
        {
            ThrowIfDisposed();

            CancellationTokenSource connectLife = ConnectLife;
            if (connectLife == null || connectLife.IsCancellationRequested)
            {
                return false;
            }

            bool semaphoreHeld = false;
            try
            {
                await _semaphore.WaitAsync(connectLife.Token).ConfigureAwait(false);
                semaphoreHeld = true;
                await ClearReceiveBufferAsync(connectLife.Token).ConfigureAwait(false);

                byte[] pack = new byte[12];
                int index = 0;

                pack[index++] = (byte)(_sn >> 8);
                pack[index++] = (byte)(_sn++ & 0xFF);

                pack[index++] = 0x00;
                pack[index++] = 0x00;

                pack[index++] = 0x00;
                pack[index++] = 0x06;

                pack[index++] = _station;

                pack[index++] = (byte)ModbusFunction.WriteSingleCoil;

                pack[index++] = (byte)(address >> 8);
                pack[index++] = (byte)(address & 0xFF);

                short val = (short)(value ? 0xFF00 : 0x0000);

                pack[index++] = (byte)(val >> 8);
                pack[index] = (byte)(val & 0xFF);

                using (CancellationTokenSource cancellationToken = new CancellationTokenSource(TimeSpan.FromMilliseconds(SendTimeout)))
                {
                    int offset = 0;
                    while (offset < pack.Length)
                    {
                        ArraySegment<byte> segment = new ArraySegment<byte>(pack, offset, pack.Length - offset);
                        int sent = await Socket.SendAsync(segment, SocketFlags.None, cancellationToken.Token).ConfigureAwait(false);
                        offset += sent;
                    }
                }

                int mbap = 7;
                int pdu = 5;

                using (CancellationTokenSource cancellationToken = new CancellationTokenSource(TimeSpan.FromMilliseconds(ReceiveTimeout)))
                {
                    byte[] buffer = new byte[mbap];
                    int offset = 0;
                    while (offset < buffer.Length)
                    {
                        ArraySegment<byte> segment = new ArraySegment<byte>(buffer, offset, buffer.Length - offset);
                        int received = await Socket.ReceiveAsync(segment, SocketFlags.None, cancellationToken.Token).ConfigureAwait(false);
                        if (received == 0)
                        {
                            throw new SocketException((int)SocketError.Shutdown);
                        }
                        offset += received;
                    }

                    index = 0;
                    if (buffer[index] != pack[index++] ||
                        buffer[index] != pack[index++] ||
                        buffer[index++] != 0x00 ||
                        buffer[index++] != 0x00)
                    {
                        throw new Exception("response error.");
                    }

                    int length = buffer[index++] << 8 | buffer[index++];
                    byte stat = buffer[index];

                    buffer = new byte[length - 1];
                    offset = 0;
                    while (offset < buffer.Length)
                    {
                        ArraySegment<byte> segment = new ArraySegment<byte>(buffer, offset, buffer.Length - offset);
                        int received = await Socket.ReceiveAsync(segment, SocketFlags.None, cancellationToken.Token).ConfigureAwait(false);
                        if (received == 0)
                        {
                            throw new SocketException((int)SocketError.Shutdown);
                        }
                        offset += received;
                    }

                    // Modbus 异常响应（功能码最高位已置位）：属正常协议交互，不断连，仅按失败返回
                    if ((buffer[0] & 0x80) != 0 && (buffer[0] & 0x7F) == (byte)ModbusFunction.WriteSingleCoil)
                    {
                        return false;
                    }

                    if (length != 1 + pdu)
                    {
                        throw new Exception("response error.");
                    }

                    if (stat != _station)
                    {
                        throw new Exception("response error.");
                    }

                    index = 0;

                    if (buffer[index++] != (byte)ModbusFunction.WriteSingleCoil)
                    {
                        throw new Exception("response error.");
                    }

                    return buffer[index++] == pack[8] &&
                           buffer[index++] == pack[9] &&
                           buffer[index++] == pack[10] &&
                           buffer[index] == pack[11];
                }
            }
            catch (Exception e)
            {
                connectLife.Cancel();
                Connected = false;
                OnException(Socket.LocalEndPoint as IPEndPoint, RemoteAddress, e);
                return false;
            }
            finally
            {
                if (semaphoreHeld)
                {
                    _semaphore.Release();
                }
            }
        }

        public async Task<bool> WriteAsync(ushort address, bool[] values)
        {
            ThrowIfDisposed();

            CancellationTokenSource connectLife = ConnectLife;
            if (connectLife == null || connectLife.IsCancellationRequested)
            {
                return false;
            }

            bool semaphoreHeld = false;
            try
            {
                await _semaphore.WaitAsync(connectLife.Token).ConfigureAwait(false);
                semaphoreHeld = true;
                await ClearReceiveBufferAsync(connectLife.Token).ConfigureAwait(false);

                ushort quantity = (ushort)values.Length;
                byte[] bytes = new byte[(quantity + 7) / 8];
                for (int i = 0; i < values.Length; i++)
                {
                    if (values[i])
                    {
                        bytes[i / 8] = (byte)(bytes[i / 8] | (0x1 << (i % 8)));
                    }
                }
                byte[] pack = new byte[12 + 1 + bytes.Length];
                int index = 0;

                pack[index++] = (byte)(_sn >> 8);
                pack[index++] = (byte)(_sn++ & 0xFF);

                pack[index++] = 0x00;
                pack[index++] = 0x00;

                int thenLength = (7 + bytes.Length);
                pack[index++] = (byte)(thenLength >> 8);
                pack[index++] = (byte)(thenLength & 0xFF);

                pack[index++] = _station;

                pack[index++] = (byte)ModbusFunction.WriteMultipleCoils;

                pack[index++] = (byte)(address >> 8);
                pack[index++] = (byte)(address & 0xFF);

                pack[index++] = (byte)(quantity >> 8);
                pack[index++] = (byte)(quantity & 0xFF);

                pack[index++] = (byte)(bytes.Length);

                Array.Copy(bytes, 0, pack, index, bytes.Length);

                using (CancellationTokenSource cancellationToken = new CancellationTokenSource(TimeSpan.FromMilliseconds(SendTimeout)))
                {
                    int offset = 0;
                    while (offset < pack.Length)
                    {
                        ArraySegment<byte> segment = new ArraySegment<byte>(pack, offset, pack.Length - offset);
                        int sent = await Socket.SendAsync(segment, SocketFlags.None, cancellationToken.Token).ConfigureAwait(false);
                        offset += sent;
                    }
                }

                int mbap = 7;
                int pdu = 5;

                using (CancellationTokenSource cancellationToken = new CancellationTokenSource(TimeSpan.FromMilliseconds(ReceiveTimeout)))
                {
                    byte[] buffer = new byte[mbap];
                    int offset = 0;
                    while (offset < buffer.Length)
                    {
                        ArraySegment<byte> segment = new ArraySegment<byte>(buffer, offset, buffer.Length - offset);
                        int received = await Socket.ReceiveAsync(segment, SocketFlags.None, cancellationToken.Token).ConfigureAwait(false);
                        if (received == 0)
                        {
                            throw new SocketException((int)SocketError.Shutdown);
                        }
                        offset += received;
                    }

                    index = 0;
                    if (buffer[index] != pack[index++] ||
                        buffer[index] != pack[index++] ||
                        buffer[index++] != 0x00 ||
                        buffer[index++] != 0x00)
                    {
                        throw new Exception("response error.");
                    }

                    int length = buffer[index++] << 8 | buffer[index++];
                    byte stat = buffer[index];

                    buffer = new byte[length - 1];
                    offset = 0;
                    while (offset < buffer.Length)
                    {
                        ArraySegment<byte> segment = new ArraySegment<byte>(buffer, offset, buffer.Length - offset);
                        int received = await Socket.ReceiveAsync(segment, SocketFlags.None, cancellationToken.Token).ConfigureAwait(false);
                        if (received == 0)
                        {
                            throw new SocketException((int)SocketError.Shutdown);
                        }
                        offset += received;
                    }

                    // Modbus 异常响应（功能码最高位已置位）：属正常协议交互，不断连，仅按失败返回
                    if ((buffer[0] & 0x80) != 0 && (buffer[0] & 0x7F) == (byte)ModbusFunction.WriteMultipleCoils)
                    {
                        return false;
                    }

                    if (length != 1 + pdu)
                    {
                        throw new Exception("response error.");
                    }

                    if (stat != _station)
                    {
                        throw new Exception("response error.");
                    }

                    index = 0;

                    if (buffer[index++] != (byte)ModbusFunction.WriteMultipleCoils)
                    {
                        throw new Exception("response error.");
                    }

                    return buffer[index++] == pack[8] &&
                           buffer[index++] == pack[9] &&
                           buffer[index++] == pack[10] &&
                           buffer[index] == pack[11];
                }
            }
            catch (Exception e)
            {
                connectLife.Cancel();
                Connected = false;
                OnException(Socket.LocalEndPoint as IPEndPoint, RemoteAddress, e);
                return false;
            }
            finally
            {
                if (semaphoreHeld)
                {
                    _semaphore.Release();
                }
            }
        }

        public async Task<bool> WriteAsync(ushort address, short value)
        {
            ThrowIfDisposed();

            CancellationTokenSource connectLife = ConnectLife;
            if (connectLife == null || connectLife.IsCancellationRequested)
            {
                return false;
            }

            bool semaphoreHeld = false;
            try
            {
                await _semaphore.WaitAsync(connectLife.Token).ConfigureAwait(false);
                semaphoreHeld = true;
                await ClearReceiveBufferAsync(connectLife.Token).ConfigureAwait(false);

                byte[] pack = new byte[12];
                int index = 0;

                pack[index++] = (byte)(_sn >> 8);
                pack[index++] = (byte)(_sn++ & 0xFF);

                pack[index++] = 0x00;
                pack[index++] = 0x00;

                pack[index++] = 0x00;
                pack[index++] = 0x06;

                pack[index++] = _station;

                pack[index++] = (byte)ModbusFunction.WriteSingleRegister;

                pack[index++] = (byte)(address >> 8);
                pack[index++] = (byte)(address & 0xFF);

                pack[index++] = (byte)(value >> 8);
                pack[index] = (byte)(value & 0xFF);

                using (CancellationTokenSource cancellationToken = new CancellationTokenSource(TimeSpan.FromMilliseconds(SendTimeout)))
                {
                    int offset = 0;
                    while (offset < pack.Length)
                    {
                        ArraySegment<byte> segment = new ArraySegment<byte>(pack, offset, pack.Length - offset);
                        int sent = await Socket.SendAsync(segment, SocketFlags.None, cancellationToken.Token).ConfigureAwait(false);
                        offset += sent;
                    }
                }

                int mbap = 7;
                int pdu = 5;

                using (CancellationTokenSource cancellationToken = new CancellationTokenSource(TimeSpan.FromMilliseconds(ReceiveTimeout)))
                {
                    byte[] buffer = new byte[mbap];
                    int offset = 0;
                    while (offset < buffer.Length)
                    {
                        ArraySegment<byte> segment = new ArraySegment<byte>(buffer, offset, buffer.Length - offset);
                        int received = await Socket.ReceiveAsync(segment, SocketFlags.None, cancellationToken.Token).ConfigureAwait(false);
                        if (received == 0)
                        {
                            throw new SocketException((int)SocketError.Shutdown);
                        }
                        offset += received;
                    }

                    index = 0;
                    if (buffer[index] != pack[index++] ||
                        buffer[index] != pack[index++] ||
                        buffer[index++] != 0x00 ||
                        buffer[index++] != 0x00)
                    {
                        throw new Exception("response error.");
                    }

                    int length = buffer[index++] << 8 | buffer[index++];
                    byte stat = buffer[index];

                    buffer = new byte[length - 1];
                    offset = 0;
                    while (offset < buffer.Length)
                    {
                        ArraySegment<byte> segment = new ArraySegment<byte>(buffer, offset, buffer.Length - offset);
                        int received = await Socket.ReceiveAsync(segment, SocketFlags.None, cancellationToken.Token).ConfigureAwait(false);
                        if (received == 0)
                        {
                            throw new SocketException((int)SocketError.Shutdown);
                        }
                        offset += received;
                    }

                    // Modbus 异常响应（功能码最高位已置位）：属正常协议交互，不断连，仅按失败返回
                    if ((buffer[0] & 0x80) != 0 && (buffer[0] & 0x7F) == (byte)ModbusFunction.WriteSingleRegister)
                    {
                        return false;
                    }

                    if (length != 1 + pdu)
                    {
                        throw new Exception("response error.");
                    }

                    if (stat != _station)
                    {
                        throw new Exception("response error.");
                    }

                    index = 0;

                    if (buffer[index++] != (byte)ModbusFunction.WriteSingleRegister)
                    {
                        throw new Exception("response error.");
                    }

                    return buffer[index++] == pack[8] &&
                           buffer[index++] == pack[9] &&
                           buffer[index++] == pack[10] &&
                           buffer[index] == pack[11];
                }
            }
            catch (Exception e)
            {
                connectLife.Cancel();
                Connected = false;
                OnException(Socket.LocalEndPoint as IPEndPoint, RemoteAddress, e);
                return false;
            }
            finally
            {
                if (semaphoreHeld)
                {
                    _semaphore.Release();
                }
            }
        }

        public async Task<bool> WriteAsync(ushort address, short[] values)
        {
            ThrowIfDisposed();

            CancellationTokenSource connectLife = ConnectLife;
            if (connectLife == null || connectLife.IsCancellationRequested)
            {
                return false;
            }

            bool semaphoreHeld = false;
            try
            {
                await _semaphore.WaitAsync(connectLife.Token).ConfigureAwait(false);
                semaphoreHeld = true;
                await ClearReceiveBufferAsync(connectLife.Token).ConfigureAwait(false);

                ushort quantity = (ushort)values.Length;
                byte[] pack = new byte[12 + 1 + quantity * 2];
                int index = 0;

                pack[index++] = (byte)(_sn >> 8);
                pack[index++] = (byte)(_sn++ & 0xFF);

                pack[index++] = 0x00;
                pack[index++] = 0x00;

                int thenLength = (7 + quantity * 2);
                pack[index++] = (byte)(thenLength >> 8);
                pack[index++] = (byte)(thenLength & 0xFF);

                pack[index++] = _station;

                pack[index++] = (byte)ModbusFunction.WriteMultipleRegisters;

                pack[index++] = (byte)(address >> 8);
                pack[index++] = (byte)(address & 0xFF);

                pack[index++] = (byte)(quantity >> 8);
                pack[index++] = (byte)(quantity & 0xFF);

                pack[index++] = (byte)(quantity * 2);

                foreach (short value in values)
                {
                    pack[index++] = (byte)(value >> 8);
                    pack[index++] = (byte)(value & 0xFF);
                }

                using (CancellationTokenSource cancellationToken = new CancellationTokenSource(TimeSpan.FromMilliseconds(SendTimeout)))
                {
                    int offset = 0;
                    while (offset < pack.Length)
                    {
                        ArraySegment<byte> segment = new ArraySegment<byte>(pack, offset, pack.Length - offset);
                        int sent = await Socket.SendAsync(segment, SocketFlags.None, cancellationToken.Token).ConfigureAwait(false);
                        offset += sent;
                    }
                }

                int mbap = 7;
                int pdu = 5;

                using (CancellationTokenSource cancellationToken = new CancellationTokenSource(TimeSpan.FromMilliseconds(ReceiveTimeout)))
                {
                    byte[] buffer = new byte[mbap];
                    int offset = 0;
                    while (offset < buffer.Length)
                    {
                        ArraySegment<byte> segment = new ArraySegment<byte>(buffer, offset, buffer.Length - offset);
                        int received = await Socket.ReceiveAsync(segment, SocketFlags.None, cancellationToken.Token).ConfigureAwait(false);
                        if (received == 0)
                        {
                            throw new SocketException((int)SocketError.Shutdown);
                        }
                        offset += received;
                    }

                    index = 0;
                    if (buffer[index] != pack[index++] ||
                        buffer[index] != pack[index++] ||
                        buffer[index++] != 0x00 ||
                        buffer[index++] != 0x00)
                    {
                        throw new Exception("response error.");
                    }

                    int length = buffer[index++] << 8 | buffer[index++];
                    byte stat = buffer[index];

                    buffer = new byte[length - 1];
                    offset = 0;
                    while (offset < buffer.Length)
                    {
                        ArraySegment<byte> segment = new ArraySegment<byte>(buffer, offset, buffer.Length - offset);
                        int received = await Socket.ReceiveAsync(segment, SocketFlags.None, cancellationToken.Token).ConfigureAwait(false);
                        if (received == 0)
                        {
                            throw new SocketException((int)SocketError.Shutdown);
                        }
                        offset += received;
                    }

                    // Modbus 异常响应（功能码最高位已置位）：属正常协议交互，不断连，仅按失败返回
                    if ((buffer[0] & 0x80) != 0 && (buffer[0] & 0x7F) == (byte)ModbusFunction.WriteMultipleRegisters)
                    {
                        return false;
                    }

                    if (length != 1 + pdu)
                    {
                        throw new Exception("response error.");
                    }
                    if (stat != _station)
                    {
                        throw new Exception("response error.");
                    }

                    index = 0;

                    if (buffer[index++] != (byte)ModbusFunction.WriteMultipleRegisters)
                    {
                        throw new Exception("response error.");
                    }

                    return buffer[index++] == pack[8] &&
                           buffer[index++] == pack[9] &&
                           buffer[index++] == pack[10] &&
                           buffer[index] == pack[11];
                }
            }
            catch (Exception e)
            {
                connectLife.Cancel();
                Connected = false;
                OnException(Socket.LocalEndPoint as IPEndPoint, RemoteAddress, e);
                return false;
            }
            finally
            {
                if (semaphoreHeld)
                {
                    _semaphore.Release();
                }
            }
        }

        public async Task ClearReceiveBufferAsync(CancellationToken cancellationToken)
        {
            int available = Socket.Available;
            if (available == 0)
            {
                return;
            }
            byte[] buffer = new byte[ReceiveBufferSize];
            while (available > 0)
            {
                int received = await Socket.ReceiveAsync(buffer, SocketFlags.None, cancellationToken).ConfigureAwait(false);
                available -= received;
            }
        }

        protected override async ValueTask InternalDisposeAsync()
        {
            if (_connectTask != null)
            {
                await _connectTask.ConfigureAwait(false);
            }
            if (_connectLife != null)
            {
                _connectLife.Cancel();
                try
                {
                    await _semaphore.WaitAsync().ConfigureAwait(false);
                }
                finally
                {
                    _connectLife.Dispose();
                    _connectLife = null;
                }
            }
            // 释放连接 Socket 与锁，避免句柄泄漏
            Socket?.Dispose();
            _socketLock.Dispose();
        }
    }

    public struct ModbusResponse
    {
        private ModbusResponse(byte[] buffer, int offset)
        {
            Ok = true;
            Buffer = buffer;
            Offset = offset;
        }
        public ModbusResponse()
        {
            Ok = false;
        }

        private static readonly ModbusResponse FailResponse = new ModbusResponse();

        public bool Ok { get; }

        public byte[] Buffer { get; }

        public int Offset { get; }

        public static ModbusResponse Success(byte[] buffer, int offset) => new ModbusResponse(buffer, offset);

        public static ModbusResponse Fail() => FailResponse;
    }
}
