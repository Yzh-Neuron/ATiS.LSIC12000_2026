namespace LSIC.App.Common
{
    public class CircularBuffer<T> : IEnumerable<T>
    {
        private readonly T[] _buffer;
        private int _head;
        private int _count;
        public int Capacity { get; }
        public int Count => _count;
        public CircularBuffer(int capacity)
        {
            Capacity = capacity;
            _buffer = new T[capacity];
        }
        public void Add(T item)
        {
            if (_count < Capacity)
            {
                _count++;
            }
            else
            {
                _head = (_head + 1) % Capacity;
            }
            int tail = (_head + _count - 1) % Capacity;
            _buffer[tail] = item;
        }
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _count; i++)
            {
                int idx = (_head + i) % Capacity;
                yield return _buffer[idx];
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

}
