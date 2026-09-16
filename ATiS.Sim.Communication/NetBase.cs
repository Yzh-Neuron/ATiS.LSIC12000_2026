using System;
using System.Threading;
using System.Threading.Tasks;

namespace ATiS.Sim.Communication
{
    public abstract class NetBase : IDisposable, IAsyncDisposable
    {
        protected NetBase(int receiveBufferSize)
        {
            if (receiveBufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(receiveBufferSize));
            }
            ReceiveBufferSize = receiveBufferSize;
        }


        protected readonly CancellationTokenSource _life = new CancellationTokenSource();
        private int _disposed; // 0 = not disposed, 1 = disposed
        private int _started;  // 0 = not started, 1 = started

        public int ReceiveBufferSize { get; }

        /// <summary>
        /// 同步方法生效, 异步方法请使用 CancellationToken 控制超时
        /// </summary>
        public int SendTimeout { get; set; } = 3000;

        /// <summary>
        /// 同步方法生效, 异步方法请使用 CancellationToken 控制超时
        /// </summary>
        public int ReceiveTimeout { get; set; } = 3000;

        protected CancellationToken LifeToken => _life.Token;

        public void Start()
        {
            ThrowIfDisposed();
            // 保证并发情况下只启动一次
            if (Interlocked.Exchange(ref _started, 1) == 1)
            {
                return;
            }
            InternalStart();
        }

        protected abstract void InternalStart();

        public void Dispose()
        {
            DisposeAsync().GetAwaiter().GetResult();
        }

        public async ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 1)
            {
                return;
            }
            try
            {
                _life.Cancel();
                await InternalDisposeAsync().ConfigureAwait(false);
            }
            finally
            {
                Interlocked.Exchange(ref _disposed, 1);
                _life.Dispose();
                GC.SuppressFinalize(this);
            }
        }

        protected virtual async ValueTask InternalDisposeAsync()
        {
            await Task.CompletedTask;
        }

        #region 异常处理

        protected void ThrowIfDisposed()
        {
            if (Interlocked.CompareExchange(ref _disposed, 0, 0) == 1)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        protected void ThrowIfBufferArgsError(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
            if (buffer.Length - offset < count)
            {
                throw new ArgumentException("The sum of offset and count is larger than the buffer length.");
            }
        }

        #endregion
    }
}
