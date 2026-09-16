using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ATiS.Sim.Communication
{
    public class UdpClient : NetBase
    {
        public UdpClient(string remoteIp, ushort remotePort, int receiveBufferSize = 1472) : this(new IPEndPoint(IPAddress.Parse(remoteIp), remotePort), receiveBufferSize)
        {

        }
        public UdpClient(IPEndPoint remoteAddress, int receiveBufferSize = 1472) : base(receiveBufferSize)
        {
            RemoteEndPoint = remoteAddress;
            _receiveBuffer = new byte[ReceiveBufferSize];
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

        private readonly SemaphoreSlim _sendLock = new SemaphoreSlim(ConcurrentControl.MaxSendTaskCount, ConcurrentControl.MaxSendTaskCount);
        private readonly SemaphoreSlim _receiveLock = new SemaphoreSlim(1, 1);
        private readonly byte[] _receiveBuffer;
        private Task _receiveTask;
        private volatile int _receiving; // 0 = no, 1 = yes
        private readonly Socket _socket;

        public IPEndPoint RemoteEndPoint { get; }

        private readonly AsyncSyncEvent<SocketDataReceivedArgs> _receivedDataEvent = new AsyncSyncEvent<SocketDataReceivedArgs>();
        private readonly AsyncSyncEvent<SocketExceptionArgs> _thrownExceptionEvent = new AsyncSyncEvent<SocketExceptionArgs>();

        /// <summary>
        /// 收到数据触发事件（按订阅者 SynchronizationContext 分发）
        /// </summary>
        public event Func<object, SocketDataReceivedArgs, Task> ReceivedData
        {
            add => _receivedDataEvent.Add(value);
            remove => _receivedDataEvent.Remove(value);
        }

        /// <summary>
        /// 抛出异常触发事件（按订阅者 SynchronizationContext 分发）
        /// </summary>
        public event Func<object, SocketExceptionArgs, Task> ThrownException
        {
            add => _thrownExceptionEvent.Add(value);
            remove => _thrownExceptionEvent.Remove(value);
        }

        protected virtual void OnReceivedData(IPEndPoint local, IPEndPoint remote, byte[] data)
        {
            SocketDataReceivedArgs args = new SocketDataReceivedArgs(local, remote, data);
            _receivedDataEvent.InvokeFireAndForget(this, args);
        }

        protected virtual void OnException(IPEndPoint local, IPEndPoint remote, Exception exception)
        {
            SocketExceptionArgs args = new SocketExceptionArgs(local, remote, exception);
            _thrownExceptionEvent.InvokeFireAndForget(this, args);
        }

        protected override void InternalStart()
        {
            try
            {
                _socket.Connect(RemoteEndPoint);
            }
            catch (OperationCanceledException)
            {
                // ignore
            }
            catch (Exception ex)
            {
                OnException(_socket.LocalEndPoint as IPEndPoint, RemoteEndPoint, ex);
            }
        }

        /// <summary>
        /// Auto receive data from remote endpoint, need register ReceivedData event to get data.
        /// </summary>
        public void StartAutoReceive()
        {
            ThrowIfDisposed();
            if (Interlocked.Exchange(ref _receiving, 1) == 1)
            {
                return;
            }
            _receiveTask = ReceiveJob();
        }

        public void StopAutoReceive()
        {
            if (Interlocked.Exchange(ref _receiving, 0) == 0)
            {
                return;
            }
            Task t = _receiveTask;
            t?.GetAwaiter().GetResult();
            _receiveTask = null;
        }

        public int Send(byte[] buffer)
        {
            return Send(buffer, 0, buffer?.Length ?? 0);
        }

        public int Send(byte[] buffer, int offset, int count)
        {
            bool semaphoreHeld = false;
            try
            {
                ThrowIfDisposed();
                ThrowIfBufferArgsError(buffer, offset, count);

                _sendLock.Wait(LifeToken);
                semaphoreHeld = true;
                _socket.SendTimeout = SendTimeout;
                return _socket.SendTo(buffer, offset, count, SocketFlags.None, RemoteEndPoint);
            }
            catch (Exception ex)
            {
                // 将异常上报，避免任务以 Faulted 结束未被观察
                OnException(_socket.LocalEndPoint as IPEndPoint, RemoteEndPoint, ex);
            }
            finally
            {
                if (semaphoreHeld)
                {
                    _sendLock.Release();
                }
            }
            return 0;
        }

        public async ValueTask<int> SendAsync(byte[] buffer, CancellationToken cancellationToken = default)
        {
            return await SendAsync(buffer, 0, buffer?.Length ?? 0, cancellationToken).ConfigureAwait(false);
        }

        public async ValueTask<int> SendAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken = default)
        {
            bool semaphoreHeld = false;
            try
            {
                ThrowIfDisposed();
                ThrowIfBufferArgsError(buffer, offset, count);

                using CancellationTokenSource linked = CancellationTokenSource.CreateLinkedTokenSource(LifeToken, cancellationToken);

                await _sendLock.WaitAsync(linked.Token).ConfigureAwait(false);
                semaphoreHeld = true;

                return await _socket.SendToAsync(buffer.AsMemory(offset, count), SocketFlags.None, RemoteEndPoint, linked.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return 0;
            }
            catch (Exception ex)
            {
                // 将异常上报，避免任务以 Faulted 结束未被观察
                OnException(_socket.LocalEndPoint as IPEndPoint, RemoteEndPoint, ex);
                return 0;
            }
            finally
            {
                if (semaphoreHeld)
                {
                    _sendLock.Release();
                }
            }
        }

        public byte[] Receive()
        {
            bool semaphoreHeld = false;
            try
            {
                ThrowIfDisposed();
                ThrowIfAutoReceiving();

                _receiveLock.Wait();
                semaphoreHeld = true;

                _socket.ReceiveTimeout = ReceiveTimeout;

                EndPoint remote = new IPEndPoint(IPAddress.Any, 0);
                int count = _socket.ReceiveFrom(_receiveBuffer, 0, _receiveBuffer.Length, SocketFlags.None, ref remote);
                if (count == 0)
                {
                    throw new SocketException((int)SocketError.ConnectionAborted);
                }
                if (!RemoteEndPoint.Equals(remote))
                {
                    throw new SocketException((int)SocketError.ConnectionReset, "Received data from unexpected remote endpoint.");
                }
                byte[] data = new byte[count];
                Buffer.BlockCopy(_receiveBuffer, 0, data, 0, count);
                return data;
            }
            catch (Exception e)
            {
                OnException(_socket.LocalEndPoint as IPEndPoint, RemoteEndPoint, e);
                return null;
            }
            finally
            {
                if (semaphoreHeld)
                {
                    _receiveLock.Release();
                }
            }
        }

        public async Task<byte[]> ReceiveAsync(CancellationToken cancellationToken = default)
        {
            bool semaphoreHeld = false;
            try
            {
                ThrowIfDisposed();
                ThrowIfAutoReceiving();

                using CancellationTokenSource linked = CancellationTokenSource.CreateLinkedTokenSource(LifeToken, cancellationToken);

                await _receiveLock.WaitAsync(linked.Token).ConfigureAwait(false);
                semaphoreHeld = true;

                SocketReceiveFromResult result = await _socket.ReceiveFromAsync(_receiveBuffer.AsMemory(0, _receiveBuffer.Length), SocketFlags.None, RemoteEndPoint, linked.Token).ConfigureAwait(false);
                if (result.ReceivedBytes == 0)
                {
                    throw new SocketException((int)SocketError.ConnectionAborted);
                }
                if (!RemoteEndPoint.Equals(result.RemoteEndPoint))
                {
                    throw new SocketException((int)SocketError.ConnectionReset, "Received data from unexpected remote endpoint.");
                }
                byte[] data = new byte[result.ReceivedBytes];
                Buffer.BlockCopy(_receiveBuffer, 0, data, 0, result.ReceivedBytes);
                return data;
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            catch (Exception e)
            {
                OnException(_socket.LocalEndPoint as IPEndPoint, RemoteEndPoint, e);
                return null;
            }
            finally
            {
                if (semaphoreHeld)
                {
                    _receiveLock.Release();
                }
            }
        }

        /// <summary>
        /// 指令式读取，发送指令并等待响应返回，请勿使用其他方式发送或接收数据以免破坏语义
        /// </summary>
        public async ValueTask<string> ReadAsync(string command, Encoding encoding, CancellationToken cancellationToken = default)
        {
            bool semaphoreHeld = false;
            try
            {
                ThrowIfDisposed();
                ThrowIfAutoReceiving();

                using CancellationTokenSource linked = CancellationTokenSource.CreateLinkedTokenSource(LifeToken, cancellationToken);
                await _receiveLock.WaitAsync(linked.Token).ConfigureAwait(false);
                semaphoreHeld = true;

                // 清除 Available 数据（仅清理来自目标 RemoteEndPoint 的积压数据）
                while (_socket.Available > 0)
                {
                    SocketReceiveFromResult drained = await _socket.ReceiveFromAsync(_receiveBuffer.AsMemory(0, _receiveBuffer.Length), SocketFlags.None, RemoteEndPoint, linked.Token).ConfigureAwait(false);
                    if (drained.ReceivedBytes == 0)
                    {
                        throw new SocketException((int)SocketError.ConnectionAborted);
                    }
                    if (!RemoteEndPoint.Equals(drained.RemoteEndPoint))
                    {
                        // 非预期远端的数据不应污染本连接语义；丢弃后继续清理
                        continue;
                    }
                }

                byte[] commandBytes = encoding.GetBytes(command);
                await _socket.SendToAsync(commandBytes.AsMemory(0, commandBytes.Length), SocketFlags.None, RemoteEndPoint, linked.Token).ConfigureAwait(false);
                SocketReceiveFromResult result = await _socket.ReceiveFromAsync(_receiveBuffer.AsMemory(0, _receiveBuffer.Length), SocketFlags.None, RemoteEndPoint, linked.Token).ConfigureAwait(false);
                if (result.ReceivedBytes == 0)
                {
                    throw new SocketException((int)SocketError.ConnectionAborted);
                }
                if (!RemoteEndPoint.Equals(result.RemoteEndPoint))
                {
                    throw new SocketException((int)SocketError.ConnectionReset, "Received data from unexpected remote endpoint.");
                }
                return encoding.GetString(_receiveBuffer, 0, result.ReceivedBytes);
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            catch (Exception e)
            {
                OnException(_socket.LocalEndPoint as IPEndPoint, RemoteEndPoint, e);
                return null;
            }
            finally
            {
                if (semaphoreHeld)
                {
                    _receiveLock.Release();
                }
            }
        }

        /// <summary>
        /// 指令式写入，发送指令到远端，请勿使用其他方式发送数据以免破坏语义
        /// </summary>
        public async ValueTask WriteAsync(string command, Encoding encoding, CancellationToken cancellationToken = default)
        {
            bool semaphoreHeld = false;
            try
            {
                ThrowIfDisposed();
                ThrowIfAutoReceiving();
                using CancellationTokenSource linked = CancellationTokenSource.CreateLinkedTokenSource(LifeToken, cancellationToken);
                await _receiveLock.WaitAsync(linked.Token).ConfigureAwait(false);
                semaphoreHeld = true;
                byte[] commandBytes = encoding.GetBytes(command);
                await _socket.SendToAsync(commandBytes.AsMemory(0, commandBytes.Length), SocketFlags.None, RemoteEndPoint, linked.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // ignore
            }
            catch (Exception e)
            {
                OnException(_socket.LocalEndPoint as IPEndPoint, RemoteEndPoint, e);
            }
            finally
            {
                if (semaphoreHeld)
                {
                    _receiveLock.Release();
                }
            }
        }

        private async Task ReceiveJob()
        {
            while (_receiving == 1 && !LifeToken.IsCancellationRequested)
            {
                try
                {
                    int count = await _socket.ReceiveAsync(_receiveBuffer.AsMemory(0, _receiveBuffer.Length), SocketFlags.None, LifeToken).ConfigureAwait(false);
                    if (count == 0)
                    {
                        throw new SocketException((int)SocketError.ConnectionAborted);
                    }
                    byte[] data = new byte[count];
                    Buffer.BlockCopy(_receiveBuffer, 0, data, 0, count);
                    OnReceivedData(_socket.LocalEndPoint as IPEndPoint, RemoteEndPoint, data);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    OnException(_socket.LocalEndPoint as IPEndPoint, RemoteEndPoint, ex);
                }
            }
        }

        protected override async ValueTask InternalDisposeAsync()
        {
            if (_receiveTask != null)
            {
                Interlocked.Exchange(ref _receiving, 0);
                await _receiveTask.ConfigureAwait(false);
            }
            // 固定次数获取，排干所有在途发送
            for (int i = 0; i < ConcurrentControl.MaxSendTaskCount; i++)
            {
                await _sendLock.WaitAsync().ConfigureAwait(false);
            }
            // 释放信号量资源
            _sendLock.Dispose();
            _receiveLock.Dispose();
            _socket.Dispose();
        }

        protected void ThrowIfAutoReceiving()
        {
            if (Interlocked.CompareExchange(ref _receiving, 0, 0) == 1)
            {
                throw new InvalidOperationException("Invalid operation while auto receiving.");
            }
        }
    }
}
