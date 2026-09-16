using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ATiS.Sim.Communication
{
    public class TcpServer : TcpBase
    {
        public TcpServer(ushort localPort, int backlog = int.MaxValue, int receiveBufferSize = 4 * 1024) : base(receiveBufferSize)
        {
            _backlog = backlog;
            LocalEndPoint = new IPEndPoint(IPAddress.Any, localPort);
        }

        private readonly int _backlog;
        private readonly ConcurrentDictionary<IPEndPoint, ClientWrapper> _clients = new ConcurrentDictionary<IPEndPoint, ClientWrapper>();
        private readonly ConcurrentDictionary<IPEndPoint, ClientWrapper> _deads = new ConcurrentDictionary<IPEndPoint, ClientWrapper>();

        private Task _acceptTask;
        private Task _checkTask;
        private Task _cleanTask;

        public IPEndPoint LocalEndPoint { get; private set; }

        protected override void InternalStart()
        {
            Socket.Bind(LocalEndPoint);
            Socket.Listen(_backlog);
            LocalEndPoint = (IPEndPoint)Socket.LocalEndPoint;
            _acceptTask = AcceptJob();
            _checkTask = CheckJob();
            _cleanTask = CleanJob();
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
                ThrowIfBufferArgsError(buffer, offset, count);

                if (!_clients.TryGetValue(remote, out ClientWrapper client) || client.Life.IsCancellationRequested)
                {
                    return 0;
                }

                client.Semaphore.Wait(client.Life.Token);
                semaphoreHeld = true;

                client.Socket.SendTimeout = SendTimeout;
                return client.Socket.Send(buffer, offset, count, SocketFlags.None);
            }
            catch (OperationCanceledException)
            {
                return 0;
            }
            catch (Exception e)
            {
                MarkClientDead(remote, e);
                return 0;
            }
            finally
            {
                if (semaphoreHeld && _clients.TryGetValue(remote, out ClientWrapper client))
                {
                    client.Semaphore.Release();
                }
            }
        }

        public async Task<int> SendToAsync(IPEndPoint remote, byte[] buffer, CancellationToken cancellationToken = default)
        {
            return await SendToAsync(remote, buffer, 0, buffer?.Length ?? 0, cancellationToken).ConfigureAwait(false);
        }

        public async Task<int> SendToAsync(IPEndPoint remote, byte[] buffer, int offset, int count, CancellationToken cancellationToken = default)
        {
            bool semaphoreHeld = false;
            try
            {
                ThrowIfDisposed();
                ThrowIfBufferArgsError(buffer, offset, count);

                if (!_clients.TryGetValue(remote, out ClientWrapper client) || client.Life.IsCancellationRequested)
                {
                    return 0;
                }

                using CancellationTokenSource linked = CancellationTokenSource.CreateLinkedTokenSource(client.Life.Token, cancellationToken);

                await client.Semaphore.WaitAsync(linked.Token).ConfigureAwait(false);
                semaphoreHeld = true;

                return await client.Socket.SendAsync(buffer.AsMemory(offset, count), SocketFlags.None, linked.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return 0;
            }
            catch (Exception e)
            {
                MarkClientDead(remote, e);
                return 0;
            }
            finally
            {
                if (semaphoreHeld && _clients.TryGetValue(remote, out ClientWrapper client))
                {
                    client.Semaphore.Release();
                }
            }
        }

        private async Task AcceptJob()
        {
            try
            {
                while (!LifeToken.IsCancellationRequested)
                {
                    Socket accept = await Socket.AcceptAsync(LifeToken).ConfigureAwait(false);

                    // client 生命周期与 server 总生命周期并联，server dispose 时可联动取消
                    CancellationTokenSource connectLife = CancellationTokenSource.CreateLinkedTokenSource(LifeToken);

                    Task receiveTask = ReceiveJob(accept, connectLife);

                    ClientWrapper client = new ClientWrapper(accept, receiveTask, connectLife);

                    _clients[(IPEndPoint)accept.RemoteEndPoint] = client;
                    OnStateChanged(LocalEndPoint, accept.RemoteEndPoint as IPEndPoint, true);
                }
            }
            catch (OperationCanceledException)
            {
                // ignore
            }
            catch (Exception e)
            {
                OnException(LocalEndPoint, null, e);
            }
        }

        private async Task CheckJob()
        {
            try
            {
                while (!LifeToken.IsCancellationRequested)
                {
                    foreach (KeyValuePair<IPEndPoint, ClientWrapper> pair in _clients.ToArray())
                    {
                        if (!pair.Value.Life.IsCancellationRequested && await pair.Key.PingAsync().ConfigureAwait(false))
                        {
                            continue;
                        }

                        MarkClientDead(pair.Key, null);
                    }

                    await Task.Delay(CheckInterval, LifeToken).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                // ignore
            }
        }

        private async Task CleanJob()
        {
            try
            {
                while (!LifeToken.IsCancellationRequested)
                {
                    foreach (KeyValuePair<IPEndPoint, ClientWrapper> pair in _deads.ToArray())
                    {
                        if (_deads.TryRemove(pair.Key, out ClientWrapper client))
                        {
                            await client.DisposeAsync().ConfigureAwait(false);
                        }
                    }

                    await Task.Delay(CheckInterval, LifeToken).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                // ignore
            }
        }

        private async Task ReceiveJob(Socket accept, CancellationTokenSource connectLife)
        {
            byte[] buffer = new byte[ReceiveBufferSize];
            EndPoint remote = null;
            try
            {
                remote = accept.RemoteEndPoint;

                while (!connectLife.Token.IsCancellationRequested && accept.Connected)
                {
                    int count = await accept.ReceiveAsync(buffer, SocketFlags.None, connectLife.Token).ConfigureAwait(false);
                    if (count == 0)
                    {
                        throw new SocketException((int)SocketError.ConnectionAborted);
                    }

                    byte[] data = new byte[count];
                    Buffer.BlockCopy(buffer, 0, data, 0, count);
                    OnReceivedData(LocalEndPoint, remote as IPEndPoint, data);
                }
            }
            catch (OperationCanceledException)
            {
                // ignore
            }
            catch (Exception e)
            {
                remote ??= accept.RemoteEndPoint;
                MarkClientDead(remote as IPEndPoint, e);
            }
        }

        private void MarkClientDead(IPEndPoint remote, Exception exception)
        {
            if (remote == null)
            {
                return;
            }
            if (_clients.TryGetValue(remote, out ClientWrapper wrapper))
            {
                wrapper.Life.Cancel();
            }
            if (_clients.TryRemove(remote, out ClientWrapper client) && _deads.TryAdd(remote, client))
            {
                OnStateChanged(LocalEndPoint, remote, false);
            }
            if (exception != null)
            {
                // 与 TcpClient 类似：只上报一次断连异常（若已经标记为 dead 则不重复报）
                if (_deads.TryGetValue(remote, out ClientWrapper dead) && dead.TryReportDisconnectOnce())
                {
                    OnException(LocalEndPoint, remote, exception);
                }
            }
        }

        protected override async ValueTask InternalDisposeAsync()
        {
            if (_acceptTask != null)
            {
                await _acceptTask.ConfigureAwait(false);
            }
            if (_checkTask != null)
            {
                await _checkTask.ConfigureAwait(false);
            }
            if (_cleanTask != null)
            {
                await _cleanTask.ConfigureAwait(false);
            }
            List<Task> tasks = new List<Task>();
            foreach (KeyValuePair<IPEndPoint, ClientWrapper> pair in _deads)
            {
                tasks.Add(pair.Value.DisposeAsync().AsTask());
            }
            foreach (KeyValuePair<IPEndPoint, ClientWrapper> pair in _clients)
            {
                OnStateChanged(LocalEndPoint, pair.Key, false);
                tasks.Add(pair.Value.DisposeAsync().AsTask());
            }
            await Task.WhenAll(tasks).ConfigureAwait(false);
            _deads.Clear();
            _clients.Clear();

            Socket?.Dispose();
            _socketLock.Dispose();
        }

        private class ClientWrapper : IAsyncDisposable
        {
            public ClientWrapper(Socket socket, Task receiveTask, CancellationTokenSource life)
            {
                Socket = socket ?? throw new ArgumentNullException(nameof(socket));
                ReceiveTask = receiveTask ?? throw new ArgumentNullException(nameof(receiveTask));
                Life = life ?? throw new ArgumentNullException(nameof(life));
            }

            private int _disposed; // 0 = no, 1 = yes
            private int _reportedDisconnect; // 0 = no, 1 = yes

            public Socket Socket { get; }

            public Task ReceiveTask { get; }

            public CancellationTokenSource Life { get; }

            public SemaphoreSlim Semaphore { get; } = new SemaphoreSlim(ConcurrentControl.MaxSendTaskCount, ConcurrentControl.MaxSendTaskCount);

            public bool TryReportDisconnectOnce()
            {
                return Interlocked.Exchange(ref _reportedDisconnect, 1) == 0;
            }

            public async ValueTask DisposeAsync()
            {
                if (Interlocked.Exchange(ref _disposed, 1) == 1)
                {
                    return;
                }
                try
                {
                    Life.Cancel();
                    try
                    {
                        await ReceiveTask.ConfigureAwait(false);
                    }
                    catch
                    {
                        // Dispose 场景不传播接收任务异常，避免影响整体释放
                    }
                    // 固定次数获取，排干所有在途发送
                    for (int i = 0; i < ConcurrentControl.MaxSendTaskCount; i++)
                    {
                        await Semaphore.WaitAsync().ConfigureAwait(false);
                    }
                }
                finally
                {
                    Semaphore.Dispose();
                    Life.Dispose();
                    Socket.Dispose();
                }
            }
        }
    }
}
