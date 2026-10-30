using System.Collections.Generic;
using System.Text;

namespace ZoneSimulator
{
    /// <summary>大端字节序写入器</summary>
    public sealed class ByteWriter
    {
        private readonly List<byte> _bytes = new();

        public int Length => _bytes.Count;

        public void U8(int value) => _bytes.Add((byte)value);

        public void I16(int value)
        {
            _bytes.Add((byte)(value >> 8));
            _bytes.Add((byte)value);
        }

        public void I32(int value)
        {
            _bytes.Add((byte)(value >> 24));
            _bytes.Add((byte)(value >> 16));
            _bytes.Add((byte)(value >> 8));
            _bytes.Add((byte)value);
        }

        public void I64(long value)
        {
            for (int shift = 56; shift >= 0; shift -= 8)
            {
                _bytes.Add((byte)(value >> shift));
            }
        }

        public void Bytes(byte[] value)
        {
            if (value is not null)
            {
                _bytes.AddRange(value);
            }
        }

        public void FixedString(string value, int length)
        {
            byte[] raw = Encoding.ASCII.GetBytes(value ?? string.Empty);
            for (int i = 0; i < length; i++)
            {
                _bytes.Add(i < raw.Length ? raw[i] : (byte)0);
            }
        }

        public byte[] ToArray() => _bytes.ToArray();
    }
}
