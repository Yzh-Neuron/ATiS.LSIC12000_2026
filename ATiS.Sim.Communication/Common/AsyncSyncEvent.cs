using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ATiS.Sim.Communication
{
    public sealed class AsyncSyncEvent<TEventArgs> where TEventArgs : EventArgs
    {
        private readonly object _gate = new();

        private struct Subscription
        {
            public SynchronizationContext Context;
            public Func<object, TEventArgs, Task> Handler;
        }

        private readonly List<Subscription> _handlers = new();

        public void Add(Func<object, TEventArgs, Task> handler)
        {
            if (handler == null)
            {
                return;
            }
            var ctx = SynchronizationContext.Current;
            lock (_gate)
            {
                _handlers.Add(new Subscription { Context = ctx, Handler = handler });
            }
        }

        public void Remove(Func<object, TEventArgs, Task> handler)
        {
            if (handler == null)
            {
                return;
            }
            lock (_gate)
            {
                for (int i = _handlers.Count - 1; i >= 0; i--)
                {
                    if (_handlers[i].Handler == handler)
                    {
                        _handlers.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 触发事件，不等待订阅任务执行完成（即发即忘）
        /// </summary>
        public void InvokeFireAndForget(object sender, TEventArgs args)
        {
            Subscription[] snapshot;
            lock (_gate)
            {
                if (_handlers.Count == 0)
                {
                    return;
                }

                snapshot = _handlers.ToArray();
            }

            foreach (var sub in snapshot)
            {
                if (sub.Context != null)
                {
                    sub.Context.Post(async _ => await SafeInvoke(sub.Handler, sender, args), null);
                }
                else
                {
                    _ = SafeInvoke(sub.Handler, sender, args);
                }
            }
        }

        /// <summary>
        /// 触发事件，await等待所有订阅全部执行完毕，会阻塞调用方
        /// </summary>
        public async Task InvokeAwaitAll(object sender, TEventArgs args)
        {
            Subscription[] snapshot;
            lock (_gate)
            {
                if (_handlers.Count == 0)
                {
                    return;
                }
                snapshot = _handlers.ToArray();
            }

            var tasks = new List<Task>();
            foreach (var sub in snapshot)
            {
                tasks.Add(SafeInvoke(sub.Handler, sender, args));
            }
            await Task.WhenAll(tasks);
        }

        private static async Task SafeInvoke(Func<object, TEventArgs, Task> handler, object sender, TEventArgs args)
        {
            try
            {
                await handler(sender, args);
            }
            catch
            {
                // 交给调用者去解决
            }
        }
    }

}
