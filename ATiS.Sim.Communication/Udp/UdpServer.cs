using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ATiS.Sim.Communication
{
    public class UdpServer : NetBase
    {
        public UdpServer(ushort localPort, int receiveBufferSize = 1472) : base(receiveBufferSize)
        {
            LocalEndPoint = new IPEndPoint(IPAddress.Any, localPort);
            _receiveBuffer = new byte[ReceiveBufferSize];
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

        private readonly SemaphoreSlim _sendLock = new SemaphoreSlim(ConcurrentControl.MaxSendTaskCount, ConcurrentControl.MaxSendTaskCount);
        private readonly byte[] _receiveBuffer;
        private Task _receiveTask;
        public Socket _socket;

        public IPEndPoint LocalEndPoint { get; private set; }

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
            _socket.Bind(LocalEndPoint);
            LocalEndPoint = (IPEndPoint)_socket.LocalEndPoint;
            _receiveTask = ReceiveJob();
        }

        public int SendTo(IPEndPoint remote, byte[] buffer)
        {
            return SendTo(remote, buffer, 0, buffer?.Length ?? 0);
        }

        public int SendTo(IPEndPoint remote, byte[] buffer, int offset, int count)
        {
            bool semaphoreHeld = false;
            try
            {
                ThrowIfDisposed();
                if (remote == null)
                {
                    throw new ArgumentNullException(nameof(remote));
                }

                ThrowIfBufferArgsError(buffer, offset, count);

                _sendLock.Wait(LifeToken);
                semaphoreHeld = true;

                _socket.SendTimeout = SendTimeout;
                return _socket.SendTo(buffer, offset, count, SocketFlags.None, remote);
            }
            catch (Exception ex)
            {
                OnException(_socket.LocalEndPoint as IPEndPoint, remote, ex);
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

        public async ValueTask<int> SendToAsync(IPEndPoint remote, byte[] buffer, CancellationToken cancellationToken = default)
        {
            return await SendToAsync(remote, buffer, 0, buffer?.Length ?? 0, cancellationToken).ConfigureAwait(false);
        }

        public async ValueTask<int> SendToAsync(IPEndPoint remote, byte[] buffer, int offset, int count, CancellationToken cancellationToken = default)
        {
            bool semaphoreHeld = false;
            try
            {
                ThrowIfDisposed();
                if (remote == null)
                {
                    throw new ArgumentNullException(nameof(remote));
                }

                ThrowIfBufferArgsError(buffer, offset, count);

                using CancellationTokenSource linked = CancellationTokenSource.CreateLinkedTokenSource(LifeToken, cancellationToken);

                await _sendLock.WaitAsync(linked.Token).ConfigureAwait(false);
                semaphoreHeld = true;

                return await _socket.SendToAsync(buffer.AsMemory(offset, count), SocketFlags.None, remote, linked.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return 0;
            }
            catch (Exception ex)
            {
                OnException(_socket.LocalEndPoint as IPEndPoint, remote, ex);
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

        private async Task ReceiveJob()
        {
            SocketReceiveFromResult result = default;
            while (!LifeToken.IsCancellationRequested)
            {
                try
                {
                    var anyIpEnd = new IPEndPoint(IPAddress.Any, 0);
                    result = await _socket.ReceiveFromAsync(_receiveBuffer.AsMemory(0, _receiveBuffer.Length), SocketFlags.None, anyIpEnd, LifeToken).ConfigureAwait(false);
                    if (result.ReceivedBytes == 0)
                    {
                        throw new SocketException((int)SocketError.ConnectionAborted);
                    }
                    byte[] data = new byte[result.ReceivedBytes];
                    Buffer.BlockCopy(_receiveBuffer, 0, data, 0, result.ReceivedBytes);
                    OnReceivedData(_socket.LocalEndPoint as IPEndPoint, result.RemoteEndPoint as IPEndPoint, data);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    // 将异常上报，避免任务以 Faulted 结束未被观察
                    OnException(_socket.LocalEndPoint as IPEndPoint, result.RemoteEndPoint as IPEndPoint, ex);
                }
            }
        }

        protected override async ValueTask InternalDisposeAsync()
        {
            if (_receiveTask != null)
            {
                await _receiveTask.ConfigureAwait(false);
            }
            // 固定次数获取，排干所有在途发送
            for (int i = 0; i < ConcurrentControl.MaxSendTaskCount; i++)
            {
                await _sendLock.WaitAsync().ConfigureAwait(false);
            }
            _sendLock.Dispose();
            _socket.Dispose();
        }
    }
}
