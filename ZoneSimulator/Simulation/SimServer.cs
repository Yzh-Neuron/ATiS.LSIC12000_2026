using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ZoneSimulator
{
    /// <summary>模拟服务端：监听一个或多个端口，每个 TCP 连接对应一个老化区</summary>
    public sealed class SimServer : IDisposable
    {
        private readonly List<TcpListener> _listeners = new();
        private readonly List<Task> _acceptTasks = new();
        private readonly List<ZoneSim> _sessions = new();
        private readonly object _sync = new();
        private readonly CancellationTokenSource _cts = new();
        private int _nextId;
        private bool _disposed;

        public bool Auto { get; set; } = true;
        public int Speed { get; set; } = 5;
        public bool Verbose { get; set; }

        public event Action<string> Logged;
        public event Action<ZoneSim> SessionOpened;

        public IReadOnlyList<ZoneSim> Sessions
        {
            get
            {
                lock (_sync)
                {
                    return _sessions.ToArray();
                }
            }
        }

        public void Start(IEnumerable<int> ports)
        {
            foreach (int port in ports.Distinct())
            {
                TcpListener listener = new(IPAddress.Any, port);
                listener.Start(16);
                _listeners.Add(listener);
                Log($"监听 0.0.0.0:{port}（老化区模拟）");
                _acceptTasks.Add(Task.Run(() => AcceptLoopAsync(listener, port)));
            }
        }

        private async Task AcceptLoopAsync(TcpListener listener, int port)
        {
            while (!_cts.IsCancellationRequested)
            {
                TcpClient client;
                try
                {
                    client = await listener.AcceptTcpClientAsync(_cts.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (ObjectDisposedException)
                {
                    return;
                }
                catch (Exception ex)
                {
                    Log($"{port} 接受连接失败：{ex.Message}");
                    continue;
                }

                ZoneSim sim;
                lock (_sync)
                {
                    sim = new ZoneSim(_nextId++, client)
                    {
                        Auto = Auto,
                        Speed = Speed,
                        Verbose = Verbose,
                    };
                    _sessions.Add(sim);
                }

                sim.Closed += Remove;
                Log($"老化区 Z{sim.Id} 已连接：{sim.Remote}（端口 {port}）");
                SessionOpened?.Invoke(sim);
                sim.Start();
            }
        }

        private void Remove(ZoneSim sim)
        {
            lock (_sync)
            {
                _sessions.Remove(sim);
            }
        }

        private void Log(string message)
        {
            string line = $"[服务] {message}";
            Console.WriteLine(line);
            Logged?.Invoke(line);
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            try
            {
                _cts.Cancel();
            }
            catch
            {
            }

            foreach (TcpListener listener in _listeners)
            {
                try
                {
                    listener.Stop();
                }
                catch
                {
                }
            }

            foreach (ZoneSim sim in Sessions)
            {
                sim.Dispose();
            }
        }
    }
}
