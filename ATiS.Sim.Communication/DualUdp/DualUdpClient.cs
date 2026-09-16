using System;
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ATiS.Sim.Communication
{
    public class DualUdpClient : DualUdpBase
    {
        private const int MaxPacketSize = 1024 * 1024;

        public DualUdpClient(string remoteIp, ushort port)
            : this(new IPEndPoint(IPAddress.Parse(remoteIp), port))
        {
        }

        public DualUdpClient(IPEndPoint remote)
        {
            RemoteAddress = remote ?? throw new ArgumentNullException(nameof(remote));
        }

        private bool _isReportedDisconnect;
        private Task _connectTask;
        private ClientConnectState _connectState;
        private ClientConnectState ConnectState
        {
            get => Volatile.Read(ref _connectState);
            set
            {
                ClientConnectState old = Volatile.Read(ref _connectState);
                if (!ReferenceEquals(old, value))
                {
                    Volatile.Write(ref _connectState, value);
                    Connected = value != null;
                }
            }
        }

        private bool _connected;
        public bool Connected
        {
            get => _connected;
            private set
            {
                if (_connected != value)
                {
                    _connected = value;
                    OnStateChanged(Sender.LocalEndPoint as IPEndPoint, RemoteAddress, _connected);
                }
            }
        }

        public IPEndPoint RemoteAddress { get; }

        private int _sn;
        internal int SN
        {
            get => _sn;
            set
            {
                if (_sn != value)
                {
                    _sn = value > 0xFFFFFF ? 0 : value;
                }
            }
        }

        protected override void InternalStart()
        {
            Sender.Bind(new IPEndPoint(IPAddress.Any, 0));
            Receiver.Bind(new IPEndPoint(IPAddress.Any, 0));
            _connectTask = ConnectJob();
        }

        public async Task<bool> SendAsync(byte code, byte[] data = null, int retryTimes = 3, int timeout = 3000)
        {
            if (code is HandShakeCode or HeartbeatCode)
            {
                throw new ArgumentException("0xE0, 0xE1 code has been used in internal.", nameof(code));
            }
            return await InternalSendAsync(code, data, true, retryTimes, timeout).ConfigureAwait(false);
        }

        protected override async ValueTask InternalDisposeAsync()
        {
            if (_connectTask != null)
            {
                await _connectTask.ConfigureAwait(false);
            }
            ClientConnectState state = ConnectState;
            if (state != null)
            {
                await state.DisposeAsync().ConfigureAwait(false);
            }
            await _semaphore.WaitAsync().ConfigureAwait(false);
            Sender?.Dispose();
            Receiver?.Dispose();
        }

        #region 私有方法

        private async Task ConnectJob()
        {
            try
            {
                await Sender.ConnectAsync(RemoteAddress, LifeToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }

            while (!LifeToken.IsCancellationRequested)
            {
                try
                {
                    ClientConnectState state = ConnectState;
                    if (state != null && !state.Life.IsCancellationRequested && await HeartbeatAsync().ConfigureAwait(false))
                    {
                        try
                        {
                            await Task.Delay(CheckInterval, LifeToken).ConfigureAwait(false);
                            continue;
                        }
                        catch (OperationCanceledException)
                        {
                        }
                    }

                    if (state != null)
                    {
                        await state.DisposeAsync().ConfigureAwait(false);
                        ConnectState = null;
                        continue;
                    }

                    // 握手包：把 Receiver 监听端口发给服务端
                    int receiverPort = (Receiver.LocalEndPoint as IPEndPoint).Port;
                    byte[] data =
                    [
                        (byte)((receiverPort >> 8) & 0xFF),
                        (byte)(receiverPort & 0xFF)
                    ];
                    bool success = await InternalSendAsync(HandShakeCode, data, false, 3, 3000).ConfigureAwait(false);
                    if (success)
                    {
                        CancellationTokenSource connectLife = new CancellationTokenSource();
                        ConnectState = new ClientConnectState(connectLife, ReceiveJob(connectLife));
                        _isReportedDisconnect = false;
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception e)
                {
                    if (!_isReportedDisconnect)
                    {
                        OnException(Sender.LocalEndPoint as IPEndPoint, RemoteAddress, e);
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

        private async Task ReceiveJob(CancellationTokenSource connectLife)
        {
            byte[] buffer = ArrayPool<byte>.Shared.Rent(ReceiveBufferSize);
            byte[] discardBuffer = ArrayPool<byte>.Shared.Rent(ReceiveBufferSize);
            try
            {
                // 清空 Sender 残留包，限制丢弃次数
                int discardCount = 0;
                EndPoint discardEp = new IPEndPoint(IPAddress.Any, 0);
                while (Sender.Available > 0 && discardCount < 32)
                {
                    discardCount++;
                    int readLen = Math.Min(ReceiveBufferSize, Sender.Available);
                    await Sender.ReceiveFromAsync(discardBuffer.AsMemory(0, readLen), SocketFlags.None, discardEp, connectLife.Token).ConfigureAwait(false);
                }

                byte[] received = null;
                DateTime lastFragmentUtc = DateTime.UtcNow;

                while (!connectLife.IsCancellationRequested)
                {
                    EndPoint remoteEp = new IPEndPoint(IPAddress.Any, 0);
                    SocketReceiveFromResult result = await Receiver.ReceiveFromAsync(buffer, SocketFlags.None, remoteEp, connectLife.Token).ConfigureAwait(false);

                    IPEndPoint remoteIpep = result.RemoteEndPoint as IPEndPoint;
                    if (remoteIpep == null || !remoteIpep.Address.Equals(RemoteAddress.Address))
                    {
                        continue;
                    }

                    if (result.ReceivedBytes < 18)
                    {
                        continue;
                    }

                    int index = 0;
                    ushort header = (ushort)((buffer[index++] << 8) | buffer[index++]);
                    if (header != Header)
                    {
                        continue;
                    }

                    int sn = (buffer[index++] << 16) | (buffer[index++] << 8) | buffer[index++];
                    byte code = buffer[index++];
                    int total = (buffer[index++] << 16) | (buffer[index++] << 8) | buffer[index++];
                    int offset = (buffer[index++] << 16) | (buffer[index++] << 8) | buffer[index++];
                    ushort payloadLength = (ushort)((buffer[index++] << 8) | buffer[index++]);

                    if (result.ReceivedBytes != 18 + payloadLength)
                    {
                        continue;
                    }

                    if (total < 0 || total > MaxPacketSize || offset < 0 || offset + payloadLength > total)
                    {
                        continue;
                    }

                    int payloadIndex = index;
                    int crcIndex = payloadIndex + payloadLength;
                    if (crcIndex + 4 > result.ReceivedBytes)
                    {
                        continue;
                    }

                    uint crc32 = (uint)(buffer[crcIndex++] << 24) | (uint)(buffer[crcIndex++] << 16) | (uint)(buffer[crcIndex++] << 8) | buffer[crcIndex];
                    uint calcCrc32 = CRC32(buffer, payloadIndex, payloadLength);
                    if (crc32 != calcCrc32)
                    {
                        continue;
                    }

                    // 回复 ACK (Header + SN)
                    byte[] ackSegment = new byte[5];
                    Array.Copy(buffer, 0, ackSegment, 0, 5);
                    await Receiver.SendToAsync(ackSegment, SocketFlags.None, result.RemoteEndPoint, connectLife.Token).ConfigureAwait(false);

                    // 心跳 / 空包：不参与数据重组，也不会覆盖正在重组的大包缓冲区
                    if (code == HeartbeatCode || total == 0)
                    {
                        continue;
                    }

                    if (offset == 0)
                    {
                        received = new byte[total];
                    }

                    if (received == null || offset + payloadLength > received.Length)
                    {
                        received = null;
                        continue;
                    }

                    Array.Copy(buffer, payloadIndex, received, offset, payloadLength);
                    lastFragmentUtc = DateTime.UtcNow;

                    if (offset + payloadLength >= total)
                    {
                        OnReceivedPacket(Sender.LocalEndPoint as IPEndPoint, remoteIpep, code, received);
                        received = null;
                    }
                    else if (DateTime.UtcNow - lastFragmentUtc > TimeSpan.FromMilliseconds(3000))
                    {
                        received = null;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // ignore
            }
            catch (Exception e)
            {
                connectLife.Cancel();
                OnException(Sender.LocalEndPoint as IPEndPoint, RemoteAddress, e);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
                ArrayPool<byte>.Shared.Return(discardBuffer);
            }
        }

        private async Task<bool> HeartbeatAsync()
        {
            try
            {
                return await InternalSendAsync(HeartbeatCode, null, true, 3, 3000).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception e)
            {
                OnException(Sender.LocalEndPoint as IPEndPoint, RemoteAddress, e);
            }
            return false;
        }

        private async Task<bool> InternalSendAsync(byte code, byte[] data, bool check, int retryTimes, int timeout)
        {
            ClientConnectState state = ConnectState;
            CancellationToken token;
            if (state == null || state.Life.IsCancellationRequested)
            {
                if (check)
                {
                    return false;
                }
                token = LifeToken;
            }
            else
            {
                token = state.Life.Token;
            }

            bool semaphoreHeld = false;
            byte[] discardBuffer = ArrayPool<byte>.Shared.Rent(ReceiveBufferSize);
            byte[] ackBuffer = new byte[5];
            try
            {
                if (code == HeartbeatCode)
                {
                    semaphoreHeld = await _semaphore.WaitAsync(0, token).ConfigureAwait(false);
                    if (!semaphoreHeld)
                    {
                        return true;
                    }
                }
                else
                {
                    await _semaphore.WaitAsync(token).ConfigureAwait(false);
                    semaphoreHeld = true;
                }

                data ??= Array.Empty<byte>();
                int totalLen = data.Length;
                if (totalLen > 0xFFFFFF)
                {
                    throw new ArgumentException("Data length exceeds 3-byte length field.", nameof(data));
                }

                int fragCount = totalLen == 0 ? 1 : ((totalLen + 1023) / 1024);

                for (int frag = 0; frag < fragCount; frag++)
                {
                    int offset = frag * 1024;
                    int segLen = totalLen == 0 ? 0 : Math.Min(totalLen - offset, 1024);

                    // 每个分片使用独立的 SN，重试期间保持不变（服务端会把 SN 原样 ACK 回来）
                    int sn = SN;
                    SN = sn + 1;

                    byte[] pack = new byte[segLen + 18];
                    int index = 0;
                    pack[index++] = (byte)((Header >> 8) & 0xFF);
                    pack[index++] = (byte)(Header & 0xFF);
                    pack[index++] = (byte)((sn >> 16) & 0xFF);
                    pack[index++] = (byte)((sn >> 8) & 0xFF);
                    pack[index++] = (byte)(sn & 0xFF);
                    pack[index++] = code;
                    pack[index++] = (byte)((totalLen >> 16) & 0xFF);
                    pack[index++] = (byte)((totalLen >> 8) & 0xFF);
                    pack[index++] = (byte)(totalLen & 0xFF);
                    pack[index++] = (byte)((offset >> 16) & 0xFF);
                    pack[index++] = (byte)((offset >> 8) & 0xFF);
                    pack[index++] = (byte)(offset & 0xFF);
                    pack[index++] = (byte)((segLen >> 8) & 0xFF);
                    pack[index++] = (byte)(segLen & 0xFF);
                    if (segLen > 0)
                    {
                        Array.Copy(data, offset, pack, index, segLen);
                        index += segLen;
                    }

                    uint crc32 = segLen > 0 ? CRC32(data, offset, segLen) : CRC32(Array.Empty<byte>());
                    pack[index++] = (byte)((crc32 >> 24) & 0xFF);
                    pack[index++] = (byte)((crc32 >> 16) & 0xFF);
                    pack[index++] = (byte)((crc32 >> 8) & 0xFF);
                    pack[index] = (byte)(crc32 & 0xFF);

                    int retried = 0;
                    bool ackOk = false;
                    while (!token.IsCancellationRequested && retried < retryTimes)
                    {
                        try
                        {
                            // 清空 Sender 残留包，限制最大丢弃次数
                            int discardCount = 0;
                            EndPoint discardEp = new IPEndPoint(IPAddress.Any, 0);
                            while (Sender.Available > 0 && discardCount < 32)
                            {
                                discardCount++;
                                int readLen = Math.Min(ReceiveBufferSize, Sender.Available);
                                await Sender.ReceiveFromAsync(discardBuffer.AsMemory(0, readLen), SocketFlags.None, discardEp, token).ConfigureAwait(false);
                            }

                            await Sender.SendToAsync(pack, SocketFlags.None, RemoteAddress, token).ConfigureAwait(false);

                            using var ctsAck = new CancellationTokenSource(timeout);
                            EndPoint ackEp = new IPEndPoint(IPAddress.Any, 0);
                            SocketReceiveFromResult ackResult = await Sender.ReceiveFromAsync(ackBuffer, SocketFlags.None, ackEp, ctsAck.Token).ConfigureAwait(false);

                            if (ackResult.RemoteEndPoint is IPEndPoint ackRemote
                                && ackRemote.Equals(RemoteAddress)
                                && ackResult.ReceivedBytes == 5
                                && ackBuffer[0] == pack[0]
                                && ackBuffer[1] == pack[1]
                                && ackBuffer[2] == pack[2]
                                && ackBuffer[3] == pack[3]
                                && ackBuffer[4] == pack[4])
                            {
                                ackOk = true;
                                break;
                            }
                        }
                        catch (OperationCanceledException) when (token.IsCancellationRequested)
                        {
                            // 连接/客户端被取消
                            return false;
                        }
                        catch
                        {
                            // ACK 超时或其它异常
                        }

                        retried++;
                    }

                    if (!ackOk)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(discardBuffer);
                if (semaphoreHeld)
                {
                    _semaphore.Release();
                }
            }
        }

        #endregion

        private class ClientConnectState : IAsyncDisposable
        {
            public ClientConnectState(CancellationTokenSource cancellationToken, Task task)
            {
                Life = cancellationToken;
                ReceiveTask = task;
            }

            private bool _disposed;
            public CancellationTokenSource Life { get; }
            public Task ReceiveTask { get; }

            public async ValueTask DisposeAsync()
            {
                if (_disposed)
                {
                    return;
                }
                _disposed = true;

                Life.Cancel();
                try
                {
                    await ReceiveTask.ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                }
                catch
                {
                    // 忽略接收任务的其它异常，避免 Dispose 抛出
                }
                Life.Dispose();
            }
        }
    }
}