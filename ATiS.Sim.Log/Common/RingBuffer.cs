using System;
using System.Threading;

namespace ATiS.Sim.Log
{
    public sealed class RingBuffer<T>
    {
        private readonly T[] _buffer;
        private readonly long[] _sequences;
        private readonly int _mask;
        private long _writeIndex;
        private long _readIndex;

        public int Capacity => _buffer.Length;

        public RingBuffer(int capacity)
        {
            if (capacity < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }

            int cap = 1;
            while (cap < capacity)
            {
                cap <<= 1;
            }

            _buffer = new T[cap];
            _sequences = new long[cap];
            for (int i = 0; i < cap; i++)
            {
                _sequences[i] = i;
            }
            _mask = cap - 1;
        }

        public bool TryEnqueue(T item)
        {
            long pos = Volatile.Read(ref _writeIndex);
            while (true)
            {
                long seq = Volatile.Read(ref _sequences[pos & _mask]);
                long diff = seq - pos;

                if (diff == 0)
                {
                    if (Interlocked.CompareExchange(ref _writeIndex, pos + 1, pos) == pos)
                    {
                        break;
                    }
                }
                else if (diff < 0)
                {
                    return false;
                }
                else
                {
                    pos = Volatile.Read(ref _writeIndex);
                }
            }

            _buffer[pos & _mask] = item;
            Volatile.Write(ref _sequences[pos & _mask], pos + 1);
            return true;
        }

        public bool TryDequeue(out T item)
        {
            long pos = Volatile.Read(ref _readIndex);
            while (true)
            {
                long seq = Volatile.Read(ref _sequences[pos & _mask]);
                long diff = seq - (pos + 1);

                if (diff == 0)
                {
                    if (Interlocked.CompareExchange(ref _readIndex, pos + 1, pos) == pos)
                    {
                        break;
                    }
                }
                else if (diff < 0)
                {
                    item = default!;
                    return false;
                }
                else
                {
                    pos = Volatile.Read(ref _readIndex);
                }
            }

            int slot = (int)(pos & _mask);
            item = _buffer[slot];
            _buffer[slot] = default!;
            Volatile.Write(ref _sequences[slot], pos + _buffer.Length);
            return true;
        }

        /// <summary>
        /// 近似元素个数
        /// </summary>
        public long Count
        {
            get
            {
                long w = Volatile.Read(ref _writeIndex);
                long r = Volatile.Read(ref _readIndex);
                long n = w - r;
                return n < 0 ? 0 : n;
            }
        }
    }
}
