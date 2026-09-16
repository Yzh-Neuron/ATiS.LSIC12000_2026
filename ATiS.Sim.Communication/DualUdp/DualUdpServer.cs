using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ATiS.Sim.Communication
{
    public class DualUdpServer : DualUdpBase
    {
        private const int MaxPacketSize = 1024 * 1024;
        private static readonly IPEndPoint _epAnyStatic = new IPEndPoint(IPAddress.Any, 0);

        public DualUdpServer(ushort localPort)
        {
            LocalEndPoint = new IPEndPoint(IPAddress.Any, localPort);
        }

        private readonly ConcurrentDictionary<IPEndPoint, ClientWrapper> _clients = new ConcurrentDictionary<IPEndPoint, ClientWrapper>();
        private readonly ConcurrentDictionary<IPEndPoint, ClientWrapper> _deads = new ConcurrentDictionary<IPEndPoint, ClientWrapper>();

        private Task _receiveTask;
        private Task _checkTask;
        private Task _cleanTask;

        public IPEndPoint LocalEndPoint { get; private set; }

        protected override void InternalStart()
        {
            Sender.Bind(_epAnyStatic);
            Receiver.Bind(LocalEndPoint);
            LocalEndPoint = (IPEndPoint)Receiver.LocalEndPoint;

            _receiveTask = ReceiveJob();
            _checkTask = CheckJob();
            _cleanTask = CleanJob();
        }

        public async Task<bool> SendToAsync(IPEndPoint remote, byte code, byte[] data = null, int retryTimes = 3, int timeout = 3000)
        {
            if (code is HandShakeCode or HeartbeatCode)
            {
                throw new ArgumentException("0xE0, 0xE1 code has been used in internal.", nameof(code));
            }
            return await InternalSendToAsync(remote, code, data, retryTimes, timeout).ConfigureAwait(false);
        }

        protected override async ValueTask InternalDisposeAsync()
        {
            try { if (_receiveTask != null) await _receiveTask.ConfigureAwait(false); }
            catch (OperationCanceledException) { }
            try { if (_checkTask != null) await _checkTask.ConfigureAwait(false); }
            catch (OperationCanceledException) { }
            try { if (_cleanTask != null) await _cleanTask.ConfigureAwait(false); }
            catch (OperationCanceledException) { }

            List<Task> tasks = new List<Task>();
            foreach (var pair in _deads)
            {
                tasks.Add(pair.Value.DisposeAsync().AsTask());
            }
            foreach (var pair in _clients)
            {
                OnStateChanged(LocalEndPoint, pair.Key, false);
                tasks.Add(pair.Value.DisposeAsync().AsTask());
            }

            try
            {
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            catch
            {
                // ignore dispose errors
            }

            _deads.Clear();
            _clients.Clear();

            Sender?.Dispose();
            Receiver?.Dispose();
        }

        #region 私有方法

        private async Task ReceiveJob()
        {
            byte[] buffer = ArrayPool<byte>.Shared.Rent(ReceiveBufferSize);

            try
            {
                while (!LifeToken.IsCancellationRequested)
                {
                    EndPoint remoteEp = new IPEndPoint(IPAddress.Any, 0);
                    SocketReceiveFromResult result = await Receiver.ReceiveFromAsync(buffer, SocketFlags.None, remoteEp, LifeToken).ConfigureAwait(false);

                    IPEndPoint remoteIpep = result.RemoteEndPoint as IPEndPoint;
                    if (remoteIpep == null)
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
                    await Receiver.SendToAsync(ackSegment, SocketFlags.None, result.RemoteEndPoint, LifeToken).ConfigureAwait(false);

                    if (code == HandShakeCode)
                    {
                        // 握手：payload 为 2 字节的接收端口
                        if (payloadLength == 2 && !_clients.ContainsKey(remoteIpep))
                        {
                            int p = payloadIndex;
                            ushort receivePort = (ushort)((buffer[p] << 8) | buffer[p + 1]);
                            if (receivePort != 0)
                            {
                                ClientWrapper client = new ClientWrapper(remoteIpep, receivePort);
                                if (_clients.TryAdd(remoteIpep, client))
                                {
                                    OnStateChanged(LocalEndPoint, remoteIpep, true);
                                }
                                else
                                {
                                    // 已存在，回滚
                                    _ = client.DisposeAsync();
                                }
                            }
                        }
                        continue;
                    }

                    if (code == HeartbeatCode || total == 0)
                    {
                        // 心跳/空包不参与重组，也清空别的客户端重组的大包
                        continue;
                    }

                    if (!_clients.TryGetValue(remoteIpep, out ClientWrapper c))
                    {
                        // 未握手客户端发的业务包直接丢弃
                        continue;
                    }

                    if (offset == 0)
                    {
                        c.ReceivedData = new byte[total];
                        c.LastFragmentUtc = DateTime.UtcNow;
                    }

                    if (c.ReceivedData == null || offset + payloadLength > c.ReceivedData.Length)
                    {
                        c.ReceivedData = null;
                        continue;
                    }

                    Array.Copy(buffer, payloadIndex, c.ReceivedData, offset, payloadLength);
                    c.LastFragmentUtc = DateTime.UtcNow;

                    if (offset + payloadLength >= total)
                    {
                        byte[] received = c.ReceivedData;
                        c.ReceivedData = null;
                        OnReceivedPacket(LocalEndPoint, remoteIpep, code, received);
                    }
                    else if (DateTime.UtcNow - c.LastFragmentUtc > TimeSpan.FromMilliseconds(3000))
                    {
                        c.ReceivedData = null;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // ignore
            }
            catch (Exception e)
            {
                OnException(LocalEndPoint, _epAnyStatic, e);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        private async Task CheckJob()
        {
            try
            {
                while (!LifeToken.IsCancellationRequested)
                {
                    foreach (var pair in _clients.ToArray())
                    {
                        bool ok = !pair.Value.Life.IsCancellationRequested && await HeartbeatAsync(pair.Value).ConfigureAwait(false);
                        if (ok)
                        {
                            continue;
                        }

                        if (_clients.TryRemove(pair.Key, out var client) && _deads.TryAdd(pair.Key, client))
                        {
                            OnStateChanged(LocalEndPoint, pair.Key, false);
                        }
                    }
                    await Task.Delay(CheckInterval, LifeToken).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private async Task CleanJob()
        {
            try
            {
                while (!LifeToken.IsCancellationRequested)
                {
                    foreach (var pair in _deads.ToArray())
                    {
                        if (_deads.TryRemove(pair.Key, out var client))
                        {
                            await client.DisposeAsync().ConfigureAwait(false);
                        }
                    }
                    await Task.Delay(CheckInterval, LifeToken).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private async Task<bool> HeartbeatAsync(ClientWrapper client)
        {
            return await InternalSendToAsync(client.RemoteAddress, HeartbeatCode, null, 2, 1000).ConfigureAwait(false);
        }

        private async Task<bool> InternalSendToAsync(IPEndPoint remote, byte code, byte[] data, int retryTimes, int timeout)
        {
            bool connected = _clients.TryGetValue(remote, out ClientWrapper client);
            if (!connected)
            {
                return false;
            }

            bool globalSemaphoreHeld = false;
            bool clientSemaphoreHeld = false;
            byte[] discardBuffer = ArrayPool<byte>.Shared.Rent(ReceiveBufferSize);
            byte[] ackBuffer = new byte[5];
            try
            {
                await _semaphore.WaitAsync(LifeToken).ConfigureAwait(false);
                globalSemaphoreHeld = true;

                if (code == HeartbeatCode)
                {
                    clientSemaphoreHeld = await client.Semaphore.WaitAsync(0, client.Life.Token).ConfigureAwait(false);
                    if (!clientSemaphoreHeld)
                    {
                        return true;
                    }
                }
                else
                {
                    await client.Semaphore.WaitAsync(client.Life.Token).ConfigureAwait(false);
                    clientSemaphoreHeld = true;
                }

                data ??= Array.Empty<byte>();
                int totalLen = data.Length;
                if (totalLen > 0xFFFFFF)
                {
                    throw new ArgumentException("Data length exceeds 3-byte length field.", nameof(data));
                }

                // 空数据也要发一个空分片（例如心跳）
                int fragCount = totalLen == 0 ? 1 : ((totalLen + 1023) / 1024);

                for (int frag = 0; frag < fragCount; frag++)
                {
                    int offset = frag * 1024;
                    int segLen = totalLen == 0 ? 0 : Math.Min(totalLen - offset, 1024);

                    // 每个分片独立 SN
                    int sn = client.SN;
                    client.SN = sn + 1;

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
                    while (!client.Life.IsCancellationRequested && retried < retryTimes)
                    {
                        try
                        {
                            // 清空 Sender 残留包
                            int discardCount = 0;
                            EndPoint discardEp = new IPEndPoint(IPAddress.Any, 0);
                            while (Sender.Available > 0 && discardCount < 32)
                            {
                                discardCount++;
                                int readLen = Math.Min(ReceiveBufferSize, Sender.Available);
                                await Sender.ReceiveFromAsync(discardBuffer.AsMemory(0, readLen), SocketFlags.None, discardEp, client.Life.Token).ConfigureAwait(false);
                            }

                            await Sender.SendToAsync(pack, SocketFlags.None, client.ReceiverAddress, client.Life.Token).ConfigureAwait(false);

                            using var ctsAck = new CancellationTokenSource(timeout);
                            EndPoint ackEp = new IPEndPoint(IPAddress.Any, 0);
                            SocketReceiveFromResult ackResult = await Sender.ReceiveFromAsync(ackBuffer, SocketFlags.None, ackEp, ctsAck.Token).ConfigureAwait(false);

                            if (ackResult.RemoteEndPoint is IPEndPoint ackRemote
                                && ackRemote.Equals(client.ReceiverAddress)
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
                        catch (OperationCanceledException) when (client.Life.IsCancellationRequested)
                        {
                            return false;
                        }
                        catch (OperationCanceledException)
                        {
                            // ACK 超时，继续重试
                        }
                        catch
                        {
                            // 其它异常也继续重试
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
            catch (Exception e)
            {
                client.Life.Cancel();
                if (_clients.TryRemove(remote, out var temp) && _deads.TryAdd(remote, temp))
                {
                    OnStateChanged(LocalEndPoint, remote, false);
                }
                OnException(LocalEndPoint, client.ReceiverAddress, e);
                return false;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(discardBuffer);

                if (clientSemaphoreHeld)
                {
                    client.Semaphore.Release();
                }
                if (globalSemaphoreHeld)
                {
                    _semaphore.Release();
                }
            }
        }

        #endregion

        private class ClientWrapper : IAsyncDisposable
        {
            public ClientWrapper(IPEndPoint remoteAddress, ushort receivePort)
            {
                RemoteAddress = remoteAddress;
                ReceiverAddress = new IPEndPoint(remoteAddress.Address, receivePort);
                LastFragmentUtc = DateTime.UtcNow;
            }

            private bool _disposed;

            public IPEndPoint RemoteAddress { get; }
            public IPEndPoint ReceiverAddress { get; }

            public byte[] ReceivedData { get; set; }
            public DateTime LastFragmentUtc { get; set; }

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

            public CancellationTokenSource Life { get; } = new CancellationTokenSource();
            public SemaphoreSlim Semaphore { get; } = new SemaphoreSlim(1, 1);

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
                    await Semaphore.WaitAsync().ConfigureAwait(false);
                }
                catch (ObjectDisposedException)
                {
                }

                Semaphore.Dispose();
                Life.Dispose();
                ReceivedData = null;
            }
        }
    }
}