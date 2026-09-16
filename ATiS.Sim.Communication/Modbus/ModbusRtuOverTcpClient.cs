using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ATiS.Sim.Communication
{
    public class ModbusRtuOverTcpClient : TcpBase, IModbusClient
    {
        public ModbusRtuOverTcpClient(byte station, string ip, ushort port = 502) : base(256)
        {
            this._station = station;

            RemoteAddress = new IPEndPoint(IPAddress.Parse(ip), port);
        }

        private readonly byte _station;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

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
            Response resp = await ReadAsync(ModbusFunction.ReadCoils, address, quantity).ConfigureAwait(false);
            if (resp == null)
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
            Response resp = await ReadAsync(ModbusFunction.ReadDiscreteInputs, address, quantity).ConfigureAwait(false);
            if (resp == null)
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
            Response resp = await ReadAsync(ModbusFunction.ReadHoldingRegisters, address, quantity).ConfigureAwait(false);
            if (resp == null)
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
            Response resp = await ReadAsync(ModbusFunction.ReadInputRegisters, address, quantity).ConfigureAwait(false);
            if (resp == null)
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

        private async Task<Response> ReadAsync(ModbusFunction func, ushort address, ushort quantity)
        {
            ThrowIfDisposed();

            CancellationTokenSource connectLife = ConnectLife;
            if (connectLife == null || connectLife.IsCancellationRequested)
            {
                return null;
            }

            bool semaphoreHeld = false;
            try
            {
                await _semaphore.WaitAsync(connectLife.Token).ConfigureAwait(false);
                semaphoreHeld = true;
                await ClearReceiveBufferAsync(connectLife.Token).ConfigureAwait(false);

                byte[] pack = new byte[8];
                int index = 0;

                pack[index++] = _station;
                pack[index++] = (byte)func;

                pack[index++] = (byte)(address >> 8);

                pack[index++] = (byte)(address & 0xFF);

                pack[index++] = (byte)(quantity >> 8);
                pack[index++] = (byte)(quantity & 0xFF);

                ushort crc = CRC16(pack, 0, index);
                pack[index++] = (byte)(crc >> 8);
                pack[index] = (byte)(crc & 0xFF);

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

                int dataLength;
                switch (func)
                {
                    case ModbusFunction.ReadCoils:
                    case ModbusFunction.ReadDiscreteInputs:
                        dataLength = (quantity + 7) / 8;
                        break;
                    case ModbusFunction.ReadHoldingRegisters:
                    case ModbusFunction.ReadInputRegisters:
                        dataLength = quantity * 2;
                        break;
                    case ModbusFunction.WriteSingleCoil:
                    case ModbusFunction.WriteSingleRegister:
                    case ModbusFunction.WriteMultipleCoils:
                    case ModbusFunction.WriteMultipleRegisters:
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                int rtu = 2 + 1 + dataLength + 2;

                using (CancellationTokenSource cancellationToken = new CancellationTokenSource(TimeSpan.FromMilliseconds(ReceiveTimeout)))
                {
                    byte[] buffer = new byte[rtu];

                    int offset = 0;
                    while (offset < 2)
                    {
                        ArraySegment<byte> segment = new ArraySegment<byte>(buffer, offset, 2 - offset);
                        int received = await Socket.ReceiveAsync(segment, SocketFlags.None, cancellationToken.Token).ConfigureAwait(false);
                        if (received == 0)
                        {
                            throw new SocketException((int)SocketError.Shutdown);
                        }
                        offset += received;
                    }

                    index = 0;

                    if (buffer[index++] != _station)
                    {
                        throw new Exception("response error.");
                    }

                    if (buffer[index++] != (byte)func)
                    {
                        throw new Exception("response error.");
                    }

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

                    if (buffer[index++] != dataLength)
                    {
                        throw new Exception("response error.");
                    }

                    int dataOffset = index;
                    index += dataLength;

                    ushort crc16 = CRC16(buffer, 0, index);
                    ushort rcrc16 = (ushort)(buffer[index++] << 8 | buffer[index]);

                    if (crc16 != rcrc16)
                    {
                        throw new Exception("response error.");
                    }

                    return new Response(buffer, dataOffset);
                }
            }
            catch (Exception e)
            {
                connectLife.Cancel();
                Connected = false;
                OnException(Socket.LocalEndPoint as IPEndPoint, RemoteAddress, e);
                return null;
            }
            finally
            {
                if (semaphoreHeld)
                {
                    _semaphore.Release();
                }
            }
        }

        private async Task<bool> WriteAsync(ushort address, bool value)
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

                byte[] pack = new byte[8];
                int index = 0;

                pack[index++] = _station;

                pack[index++] = (byte)ModbusFunction.WriteSingleCoil;

                pack[index++] = (byte)(address >> 8);
                pack[index++] = (byte)(address & 0xFF);

                short val = (short)(value ? 0xFF00 : 0x0000);

                pack[index++] = (byte)(val >> 8);
                pack[index++] = (byte)(val & 0xFF);

                ushort crc = CRC16(pack, 0, index);
                pack[index++] = (byte)(crc >> 8);
                pack[index] = (byte)(crc & 0xFF);

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

                int rtu = 8;

                using (CancellationTokenSource cancellationToken = new CancellationTokenSource(TimeSpan.FromMilliseconds(ReceiveTimeout)))
                {
                    byte[] buffer = new byte[rtu];

                    int offset = 0;
                    while (offset < 2)
                    {
                        ArraySegment<byte> segment = new ArraySegment<byte>(buffer, offset, 2 - offset);
                        int received = await Socket.ReceiveAsync(segment, SocketFlags.None, cancellationToken.Token).ConfigureAwait(false);
                        if (received == 0)
                        {
                            throw new SocketException((int)SocketError.Shutdown);
                        }
                        offset += received;
                    }

                    index = 0;

                    if (buffer[index++] != _station)
                    {
                        throw new Exception("response error.");
                    }

                    if (buffer[index++] != (byte)ModbusFunction.WriteSingleCoil)
                    {
                        throw new Exception("response error.");
                    }

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

                    int dataOffset = index;
                    index += 4;

                    ushort crc16 = CRC16(buffer, 0, index);
                    ushort rcrc16 = (ushort)(buffer[index++] << 8 | buffer[index]);

                    if (crc16 != rcrc16)
                    {
                        throw new Exception("response error.");
                    }
                    index = dataOffset;
                    return buffer[index++] == pack[2] &&
                           buffer[index++] == pack[3] &&
                           buffer[index++] == pack[4] &&
                           buffer[index] == pack[5];
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

        private async Task<bool> WriteAsync(ushort address, bool[] values)
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

                byte[] pack = new byte[9 + bytes.Length];
                int index = 0;

                pack[index++] = _station;

                pack[index++] = (byte)ModbusFunction.WriteMultipleCoils;

                pack[index++] = (byte)(address >> 8);
                pack[index++] = (byte)(address & 0xFF);

                pack[index++] = (byte)(quantity >> 8);
                pack[index++] = (byte)(quantity & 0xFF);

                pack[index++] = (byte)(bytes.Length);

                Array.Copy(bytes, 0, pack, index, bytes.Length);
                index += bytes.Length;

                ushort crc = CRC16(pack, 0, index);
                pack[index++] = (byte)(crc >> 8);
                pack[index] = (byte)(crc & 0xFF);

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

                int rtu = 8;

                using (CancellationTokenSource cancellationToken = new CancellationTokenSource(TimeSpan.FromMilliseconds(ReceiveTimeout)))
                {
                    byte[] buffer = new byte[rtu];

                    int offset = 0;
                    while (offset < 2)
                    {
                        ArraySegment<byte> segment = new ArraySegment<byte>(buffer, offset, 2 - offset);
                        int received = await Socket.ReceiveAsync(segment, SocketFlags.None, cancellationToken.Token).ConfigureAwait(false);
                        if (received == 0)
                        {
                            throw new SocketException((int)SocketError.Shutdown);
                        }
                        offset += received;
                    }

                    index = 0;

                    if (buffer[index++] != _station)
                    {
                        throw new Exception("response error.");
                    }

                    if (buffer[index++] != (byte)ModbusFunction.WriteMultipleCoils)
                    {
                        throw new Exception("response error.");
                    }

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

                    int dataOffset = index;
                    index += 4;

                    ushort crc16 = CRC16(buffer, 0, index);
                    ushort rcrc16 = (ushort)(buffer[index++] << 8 | buffer[index]);

                    if (crc16 != rcrc16)
                    {
                        throw new Exception("response error.");
                    }
                    index = dataOffset;
                    return buffer[index++] == pack[2] &&
                           buffer[index++] == pack[3] &&
                           buffer[index++] == pack[4] &&
                           buffer[index] == pack[5];
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

        private async Task<bool> WriteAsync(ushort address, short value)
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

                byte[] pack = new byte[8];
                int index = 0;

                pack[index++] = _station;

                pack[index++] = (byte)ModbusFunction.WriteSingleRegister;

                pack[index++] = (byte)(address >> 8);
                pack[index++] = (byte)(address & 0xFF);

                pack[index++] = (byte)(value >> 8);
                pack[index++] = (byte)(value & 0xFF);

                ushort crc = CRC16(pack, 0, index);
                pack[index++] = (byte)(crc >> 8);
                pack[index] = (byte)(crc & 0xFF);

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

                int rtu = 8;

                using (CancellationTokenSource cancellationToken = new CancellationTokenSource(TimeSpan.FromMilliseconds(ReceiveTimeout)))
                {
                    byte[] buffer = new byte[rtu];

                    int offset = 0;
                    while (offset < 2)
                    {
                        ArraySegment<byte> segment = new ArraySegment<byte>(buffer, offset, 2 - offset);
                        int received = await Socket.ReceiveAsync(segment, SocketFlags.None, cancellationToken.Token).ConfigureAwait(false);
                        if (received == 0)
                        {
                            throw new SocketException((int)SocketError.Shutdown);
                        }
                        offset += received;
                    }

                    index = 0;

                    if (buffer[index++] != _station)
                    {
                        throw new Exception("response error.");
                    }

                    if (buffer[index++] != (byte)ModbusFunction.WriteSingleRegister)
                    {
                        throw new Exception("response error.");
                    }

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

                    int dataOffset = index;
                    index += 4;

                    ushort crc16 = CRC16(buffer, 0, index);
                    ushort rcrc16 = (ushort)(buffer[index++] << 8 | buffer[index]);

                    if (crc16 != rcrc16)
                    {
                        throw new Exception("response error.");
                    }
                    index = dataOffset;
                    return buffer[index++] == pack[2] &&
                           buffer[index++] == pack[3] &&
                           buffer[index++] == pack[4] &&
                           buffer[index] == pack[5];
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

        private async Task<bool> WriteAsync(ushort address, short[] values)
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
                byte[] pack = new byte[9 + quantity * 2];
                int index = 0;

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

                ushort crc = CRC16(pack, 0, index);
                pack[index++] = (byte)(crc >> 8);
                pack[index] = (byte)(crc & 0xFF);

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

                int rtu = 8;
                using (CancellationTokenSource cancellationToken = new CancellationTokenSource(TimeSpan.FromMilliseconds(ReceiveTimeout)))
                {
                    byte[] buffer = new byte[rtu];

                    int offset = 0;
                    while (offset < 2)
                    {
                        ArraySegment<byte> segment = new ArraySegment<byte>(buffer, offset, 2 - offset);
                        int received = await Socket.ReceiveAsync(segment, SocketFlags.None, cancellationToken.Token).ConfigureAwait(false);
                        if (received == 0)
                        {
                            throw new SocketException((int)SocketError.Shutdown);
                        }
                        offset += received;
                    }
                    index = 0;
                    if (buffer[index++] != _station)
                    {
                        throw new Exception("response error.");
                    }
                    if (buffer[index++] != (byte)ModbusFunction.WriteMultipleRegisters)
                    {
                        throw new Exception("response error.");
                    }
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
                    int dataOffset = index;
                    index += 4;

                    ushort crc16 = CRC16(buffer, 0, index);
                    ushort rcrc16 = (ushort)(buffer[index++] << 8 | buffer[index]);

                    if (crc16 != rcrc16)
                    {
                        throw new Exception("response error.");
                    }
                    index = dataOffset;
                    return buffer[index++] == pack[2] &&
                           buffer[index++] == pack[3] &&
                           buffer[index++] == pack[4] &&
                           buffer[index] == pack[5];
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

        private async Task ClearReceiveBufferAsync(CancellationToken cancellationToken)
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
            await _connectTask.ConfigureAwait(false);
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
        }

        #region CRC16/modbus

        /// <summary>
        /// 256 element CRC16 lookup table for Modbus
        /// </summary>
        private static readonly ushort[] _crc16Table = new ushort[]
        {
            0x0000, 0xc0c1, 0xc181, 0x0140, 0xc301, 0x03c0, 0x0280, 0xc241,
            0xc601, 0x06c0, 0x0780, 0xc741, 0x0500, 0xc5c1, 0xc481, 0x0440,
            0xcc01, 0x0cc0, 0x0d80, 0xcd41, 0x0f00, 0xcfc1, 0xce81, 0x0e40,
            0x0a00, 0xcac1, 0xcb81, 0x0b40, 0xc901, 0x09c0, 0x0880, 0xc841,
            0xd801, 0x18c0, 0x1980, 0xd941, 0x1b00, 0xdbc1, 0xda81, 0x1a40,
            0x1e00, 0xdec1, 0xdf81, 0x1f40, 0xdd01, 0x1dc0, 0x1c80, 0xdc41,
            0x1400, 0xd4c1, 0xd581, 0x1540, 0xd701, 0x17c0, 0x1680, 0xd641,
            0xd201, 0x12c0, 0x1380, 0xd341, 0x1100, 0xd1c1, 0xd081, 0x1040,
            0xf001, 0x30c0, 0x3180, 0xf141, 0x3300, 0xf3c1, 0xf281, 0x3240,
            0x3600, 0xf6c1, 0xf781, 0x3740, 0xf501, 0x35c0, 0x3480, 0xf441,
            0x3c00, 0xfcc1, 0xfd81, 0x3d40, 0xff01, 0x3fc0, 0x3e80, 0xfe41,
            0xfa01, 0x3ac0, 0x3b80, 0xfb41, 0x3900, 0xf9c1, 0xf881, 0x3840,
            0x2800, 0xe8c1, 0xe981, 0x2940, 0xeb01, 0x2bc0, 0x2a80, 0xea41,
            0xee01, 0x2ec0, 0x2f80, 0xef41, 0x2d00, 0xedc1, 0xec81, 0x2c40,
            0xe401, 0x24c0, 0x2580, 0xe541, 0x2700, 0xe7c1, 0xe681, 0x2640,
            0x2200, 0xe2c1, 0xe381, 0x2340, 0xe101, 0x21c0, 0x2080, 0xe041,
            0xa001, 0x60c0, 0x6180, 0xa141, 0x6300, 0xa3c1, 0xa281, 0x6240,
            0x6600, 0xa6c1, 0xa781, 0x6740, 0xa501, 0x65c0, 0x6480, 0xa441,
            0x6c00, 0xacc1, 0xad81, 0x6d40, 0xaf01, 0x6fc0, 0x6e80, 0xae41,
            0xaa01, 0x6ac0, 0x6b80, 0xab41, 0x6900, 0xa9c1, 0xa881, 0x6840,
            0x7800, 0xb8c1, 0xb981, 0x7940, 0xbb01, 0x7bc0, 0x7a80, 0xba41,
            0xbe01, 0x7ec0, 0x7f80, 0xbf41, 0x7d00, 0xbdc1, 0xbc81, 0x7c40,
            0xb401, 0x74c0, 0x7580, 0xb541, 0x7700, 0xb7c1, 0xb681, 0x7640,
            0x7200, 0xb2c1, 0xb381, 0x7340, 0xb101, 0x71c0, 0x7080, 0xb041,
            0x5000, 0x90c1, 0x9181, 0x5140, 0x9301, 0x53c0, 0x5280, 0x9241,
            0x9601, 0x56c0, 0x5780, 0x9741, 0x5500, 0x95c1, 0x9481, 0x5440,
            0x9c01, 0x5cc0, 0x5d80, 0x9d41, 0x5f00, 0x9fc1, 0x9e81, 0x5e40,
            0x5a00, 0x9ac1, 0x9b81, 0x5b40, 0x9901, 0x59c0, 0x5880, 0x9841,
            0x8801, 0x48c0, 0x4980, 0x8941, 0x4b00, 0x8bc1, 0x8a81, 0x4a40,
            0x4e00, 0x8ec1, 0x8f81, 0x4f40, 0x8d01, 0x4dc0, 0x4c80, 0x8c41,
            0x4400, 0x84c1, 0x8581, 0x4540, 0x8701, 0x47c0, 0x4680, 0x8641,
            0x8201, 0x42c0, 0x4380, 0x8341, 0x4100, 0x81c1, 0x8081, 0x4040
        };

        private static ushort CRC16(byte[] data, int offset, int count)
        {
            ushort crc = 0xFFFF;
            int end = offset + count;
            for (int i = offset; i < end; i++)
            {
                crc = (ushort)((crc >> 8) ^ _crc16Table[(crc ^ data[i]) & 0xFF]);
            }
            return (ushort)(((crc & 0xFF) << 8) | (crc >> 8));
        }

        #endregion

        private class Response
        {
            public byte[] Buffer { get; }

            public int Offset { get; }

            public Response(byte[] buffer, int offset)
            {
                Buffer = buffer;
                Offset = offset;
            }
        }
    }
}
