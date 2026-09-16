using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ATiS.Sim.Communication
{
    public abstract class TcpBase : NetBase
    {
        protected TcpBase(int receiveBufferSize) : base(receiveBufferSize)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _cts = new CancellationTokenSource();
        }

        protected readonly ReaderWriterLockSlim _socketLock = new ReaderWriterLockSlim();
        protected CancellationTokenSource _cts;

        private Socket _socket;
        protected Socket Socket
        {
            get
            {
                _socketLock.EnterReadLock();
                try
                {
                    return _socket;
                }
                finally
                {
                    _socketLock.ExitReadLock();
                }
            }
        }

        private int _checkInterval = 3000;
        public int CheckInterval
        {
            get => _checkInterval;
            set
            {
                if (_checkInterval != value)
                {
                    _checkInterval = Math.Max(value, 100);
                }
            }
        }

        private readonly AsyncSyncEvent<SocketStateChangedArgs> _stateChangedEvent = new AsyncSyncEvent<SocketStateChangedArgs>();
        private readonly AsyncSyncEvent<SocketDataReceivedArgs> _receivedDataEvent = new AsyncSyncEvent<SocketDataReceivedArgs>();
        private readonly AsyncSyncEvent<SocketExceptionArgs> _thrownExceptionEvent = new AsyncSyncEvent<SocketExceptionArgs>();

        /// <summary>
        /// 通信状态改变，纯异步事件
        /// </summary>
        public event Func<object, SocketStateChangedArgs, Task> StateChanged
        {
            add => _stateChangedEvent.Add(value);
            remove => _stateChangedEvent.Remove(value);
        }

        /// <summary>
        /// 收到数据，纯异步事件
        /// </summary>
        public event Func<object, SocketDataReceivedArgs, Task> ReceivedData
        {
            add => _receivedDataEvent.Add(value);
            remove => _receivedDataEvent.Remove(value);
        }

        /// <summary>
        /// 抛出异常，纯异步事件
        /// </summary>
        public event Func<object, SocketExceptionArgs, Task> ThrownException
        {
            add => _thrownExceptionEvent.Add(value);
            remove => _thrownExceptionEvent.Remove(value);
        }

        /// <summary>
        /// 状态变更触发（即发即忘，不阻塞IO循环）
        /// </summary>
        protected virtual void OnStateChanged(IPEndPoint local, IPEndPoint remote, bool connected)
        {
            var args = new SocketStateChangedArgs(local, remote, connected);
            _stateChangedEvent.InvokeFireAndForget(this, args);
        }

        /// <summary>
        /// 收到数据触发（即发即忘，不阻塞IO循环）
        /// </summary>
        protected virtual void OnReceivedData(IPEndPoint local, IPEndPoint remote, byte[] data)
        {
            var args = new SocketDataReceivedArgs(local, remote, data);
            _receivedDataEvent.InvokeFireAndForget(this, args);
        }

        /// <summary>
        /// 异常触发（即发即忘，不阻塞IO循环）
        /// </summary>
        protected virtual void OnException(IPEndPoint local, IPEndPoint remote, Exception exception)
        {
            var args = new SocketExceptionArgs(local, remote, exception);
            _thrownExceptionEvent.InvokeFireAndForget(this, args);
        }

        /// <summary>
        /// 重建Socket，销毁旧实例
        /// </summary>
        protected void ResetSocket()
        {
            ThrowIfDisposed();
            _socketLock.EnterWriteLock();
            try
            {
                Socket old = _socket;
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                old?.Dispose();
            }
            finally
            {
                _socketLock.ExitWriteLock();
            }
        }

    }
}
