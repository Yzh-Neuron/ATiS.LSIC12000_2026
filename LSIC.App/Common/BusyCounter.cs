namespace LSIC.App
{
    public sealed class BusyCounter
    {
        private readonly Action<bool> _set;
        private int _count;
        public BusyCounter(Action<bool> set)
        {
            _set = set;
        }
        public Scope Enter()
        {
            if (Interlocked.Increment(ref _count) == 1)
            {
                _set(true);
            }
            return new Scope(this);
        }
        public readonly struct Scope : IDisposable
        {
            private readonly BusyCounter _o;
            internal Scope(BusyCounter o)
            {
                _o = o;
            }
            public void Dispose()
            {
                if (Interlocked.Decrement(ref _o._count) == 0)
                {
                    _o._set(false);
                }
            }
        }
    }
}
