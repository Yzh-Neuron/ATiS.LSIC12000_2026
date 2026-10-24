using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ATiS.Sim.Communication
{
    public class TcpClient : TcpBase
    {
        public TcpClient(string remoteIp, ushort remotePort, int receiveBufferSize = 4 * 1024) : this(new IPEndPoint(IPAddress.Parse(remoteIp), remotePort), receiveBufferSize)
        {

        }
        public TcpClient(IPEndPoint remote, int receiveBufferSize = 4 * 1024) : base(receiveBufferSize)
        {
            RemoteEndPoint = remote;
            _receiveBuffer = new byte[ReceiveBufferSize];
        }

        private readonly SemaphoreSlim _sendLock = new SemaphoreSlim(ConcurrentControl.MaxSendTaskCount, ConcurrentControl.MaxSendTaskCount);
        private readonly SemaphoreSlim _receiveLock = new SemaphoreSlim(1, 1);
        private readonly byte[] _receiveBuffer;
        private Task _connectTask;
        private Task _receiveTask;
        private volatile int _receiving; // 0 = no, 1 = yes
        private CancellationTokenSource _receiveLife;
        private bool _isReportedDisconnect;

        public IPEndPoint RemoteEndPoint { get; }

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
                    OnStateChanged(Socket.LocalEndPoint as IPEndPoint, RemoteEndPoint, _connected);
                }
            }
        }

        protected override void InternalStart()
        {
            _connectTask = ConnectJob();
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
            // 独立的接收生命周期：StopAutoReceive 可主动取消挂起的 ReceiveAsync，避免无数据时永久阻塞
            CancellationTokenSource receiveLife = CancellationTokenSource.CreateLinkedTokenSource(LifeToken);
            Volatile.Write(ref _receiveLife, receiveLife);
            _receiveTask = ReceiveJob(receiveLife.Token);
        }

        /// <summary>
        /// 停止自动接收并等待接收任务退出。注意：取消瞬间到达的数据可能被丢弃。
        /// </summary>
        public void StopAutoReceive()
        {
            if (Interlocked.Exchange(ref _receiving, 0) == 0)
            {
                return;
            }
            // 取消挂起的接收，令 ReceiveJob 及时退出，而不是等待下一包数据到达
            CancellationTokenSource receiveLife = Volatile.Read(ref _receiveLife);
            try
            {
                receiveLife?.Cancel();
            }
            catch (ObjectDisposedException)
            {
            }
            Task t = _receiveTask;
            t?.GetAwaiter().GetResult();
            _receiveTask = null;
            receiveLife?.Dispose();
            Volatile.Write(ref _receiveLife, null);
        }

        private async Task ConnectJob()
        {
            while (!LifeToken.IsCancellationRequested)
            {
                try
                {
                    // 连接状态检查
                    ClientConnectState state = ConnectState;
                    if (state != null &&
                        !state.Life.IsCancellationRequested &&
                        await RemoteEndPoint.PingAsync().ConfigureAwait(false))
                    {
                        continue;
                    }
                    // 连接断开或 Ping 不通，重新连接，先释放旧连接
                    if (state != null)
                    {
                        await state.DisposeAsync().ConfigureAwait(false);
                        ConnectState = null;
                        await Socket.DisconnectAsync(false).ConfigureAwait(false);
                        ResetSocket();
                        continue;
                    }
                    await Socket.ConnectAsync(RemoteEndPoint, LifeToken).ConfigureAwait(false);
                    CancellationTokenSource connectLife = CancellationTokenSource.CreateLinkedTokenSource(LifeToken);
                    ConnectState = new ClientConnectState(connectLife);
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
                        OnException(Socket.LocalEndPoint as IPEndPoint, RemoteEndPoint, e);
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

        public int Send(byte[] buffer)
        {
            return Send(buffer, 0, buffer?.Length ?? 0);
        }

        public int Send(byte[] buffer, int offset, int count)
        {
            bool semaphoreHeld = false;
            ClientConnectState state = null;
            try
            {
                ThrowIfDisposed();
                ThrowIfBufferArgsError(buffer, offset, count);

                state = ConnectState;
                if (state == null || state.Life.IsCancellationRequested)
                {
                    return 0;
                }

                _sendLock.Wait(LifeToken);
                semaphoreHeld = true;

                Socket.SendTimeout = SendTimeout;
                return Socket.Send(buffer, offset, count, SocketFlags.None);
            }
            catch (OperationCanceledException)
            {
                return 0;
            }
            catch (Exception e)
            {
                MarkConnectStateDead(state, e);
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

        public async ValueTask<int> SendAsync(byte[] buffer, CancellationToken cancellationToken = default)
        {
            return await SendAsync(buffer, 0, buffer?.Length ?? 0, cancellationToken).ConfigureAwait(false);
        }

        public async ValueTask<int> SendAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken = default)
        {
            bool semaphoreHeld = false;
            ClientConnectState state = null;
            try
            {
                ThrowIfDisposed();
                ThrowIfBufferArgsError(buffer, offset, count);

                state = ConnectState;
                if (state == null || state.Life.IsCancellationRequested)
                {
                    return 0;
                }

                using CancellationTokenSource linked = CancellationTokenSource.CreateLinkedTokenSource(LifeToken, cancellationToken);

                await _sendLock.WaitAsync(linked.Token).ConfigureAwait(false);
                semaphoreHeld = true;

                return await Socket.SendAsync(buffer.AsMemory(offset, count), SocketFlags.None, linked.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return 0;
            }
            catch (Exception e)
            {
                MarkConnectStateDead(state, e);
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

                _receiveLock.Wait(LifeToken);
                semaphoreHeld = true;

                Socket.ReceiveTimeout = ReceiveTimeout;

                int count = Socket.Receive(_receiveBuffer, 0, _receiveBuffer.Length, SocketFlags.None);
                if (count == 0)
                {
                    throw new SocketException((int)SocketError.ConnectionAborted);
                }
                byte[] data = new byte[count];
                Buffer.BlockCopy(_receiveBuffer, 0, data, 0, count);
                return data;
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            catch (Exception e)
            {
                OnException(Socket.LocalEndPoint as IPEndPoint, RemoteEndPoint, e);
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

        public async ValueTask<byte[]> ReceiveAsync(CancellationToken cancellationToken = default)
        {
            bool semaphoreHeld = false;
            try
            {
                ThrowIfDisposed();
                ThrowIfAutoReceiving();

                using CancellationTokenSource linked = CancellationTokenSource.CreateLinkedTokenSource(LifeToken, cancellationToken);

                await _receiveLock.WaitAsync(linked.Token).ConfigureAwait(false);
                semaphoreHeld = true;

                int count = await Socket.ReceiveAsync(_receiveBuffer.AsMemory(0, _receiveBuffer.Length), SocketFlags.None, linked.Token).ConfigureAwait(false);
                if (count == 0)
                {
                    throw new SocketException((int)SocketError.ConnectionAborted);
                }
                byte[] data = new byte[count];
                Buffer.BlockCopy(_receiveBuffer, 0, data, 0, count);
                return data;
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            catch (Exception e)
            {
                OnException(Socket.LocalEndPoint as IPEndPoint, RemoteEndPoint, e);
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

        private async Task ReceiveJob(CancellationToken receiveLife)
        {
            while (!receiveLife.IsCancellationRequested && _receiving == 1)
            {
                try
                {
                    if (!Connected)
                    {
                        await Task.Delay(CheckInterval, receiveLife).ConfigureAwait(false);
                        continue;
                    }
                    int count = await Socket.ReceiveAsync(_receiveBuffer.AsMemory(0, _receiveBuffer.Length), SocketFlags.None, receiveLife).ConfigureAwait(false);
                    if (count == 0)
                    {
                        throw new SocketException((int)SocketError.ConnectionAborted);
                    }
                    byte[] data = new byte[count];
                    Buffer.BlockCopy(_receiveBuffer, 0, data, 0, count);
                    OnReceivedData(Socket.LocalEndPoint as IPEndPoint, RemoteEndPoint, data);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception e)
                {
                    MarkConnectStateDead(ConnectState, e);
                }
            }
        }

        /// <summary>
        /// 标记连接为死连接并上报异常。该方法与 ConnectJob 重连并发执行，
        /// 期间 CTS 可能已被 ConnectJob 释放、Socket 可能已被重建，
        /// 清理动作必须吞掉二次异常，否则接收循环会被异常杀死，重连后永久收不到数据
        /// </summary>
        private void MarkConnectStateDead(ClientConnectState stateAtError, Exception e)
        {
            if (stateAtError is not null)
            {
                try
                {
                    stateAtError.Life.Cancel();
                }
                catch (ObjectDisposedException)
                {
                    // ConnectJob 已释放该连接，无需重复处理
                }
            }

            // 仅当当前连接仍是报错的连接时才置断开，避免旧连接的延迟异常误杀已建立的新连接
            if (ReferenceEquals(ConnectState, stateAtError))
            {
                try
                {
                    Connected = false;
                }
                catch
                {
                    // Socket 可能已被重建/释放，状态通知失败可忽略
                }
            }

            try
            {
                OnException(Socket.LocalEndPoint as IPEndPoint, RemoteEndPoint, e);
            }
            catch
            {
            }
        }

        protected override async ValueTask InternalDisposeAsync()
        {
            if (_connectTask != null)
            {
                await _connectTask.ConfigureAwait(false);
            }
            if (_receiveTask != null)
            {
                Interlocked.Exchange(ref _receiving, 0);
                CancellationTokenSource receiveLife = Volatile.Read(ref _receiveLife);
                try
                {
                    receiveLife?.Cancel();
                }
                catch (ObjectDisposedException)
                {
                }
                await _receiveTask.ConfigureAwait(false);
                receiveLife?.Dispose();
                Volatile.Write(ref _receiveLife, null);
            }

            ClientConnectState state = ConnectState;
            if (state != null)
            {
                await state.DisposeAsync().ConfigureAwait(false);
            }

            for (int i = 0; i < ConcurrentControl.MaxSendTaskCount; i++)
            {
                await _sendLock.WaitAsync().ConfigureAwait(false);
            }

            _sendLock.Dispose();
            _receiveLock.Dispose();

            Socket?.Dispose();
            _socketLock.Dispose();
        }

        protected void ThrowIfAutoReceiving()
        {
            if (Interlocked.CompareExchange(ref _receiving, 0, 0) == 1)
            {
                throw new InvalidOperationException("Invalid operation while auto receiving.");
            }
        }

        private class ClientConnectState : IAsyncDisposable
        {
            public ClientConnectState(CancellationTokenSource life)
            {
                Life = life ?? throw new ArgumentNullException(nameof(life));
            }

            private int _disposed; // 0 = no, 1 = yes

            public CancellationTokenSource Life { get; }

            public ValueTask DisposeAsync()
            {
                if (Interlocked.Exchange(ref _disposed, 1) == 1)
                {
                    return new ValueTask(Task.CompletedTask);
                }
                Life.Cancel();
                Life.Dispose();
                return new ValueTask(Task.CompletedTask);
            }
        }
    }
}
