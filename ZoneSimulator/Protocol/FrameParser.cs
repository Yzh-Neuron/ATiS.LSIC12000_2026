using System;
using System.Collections.Generic;
using ATiS.Base;

namespace ZoneSimulator
{
    /// <summary>
    /// 帧解析器，解析规则与 Zone.OnReceivedData 一致：
    /// 逐字节重同步、非法长度丢弃帧头重同步、CRC32/帧尾校验失败丢弃整帧。
    /// </summary>
    public sealed class FrameParser
    {
        private readonly List<byte> _buffer = new();

        /// <summary>code, data(数据段), crc(帧内 CRC32 字段)</summary>
        public event Action<ZoneFunctionCode, byte[], uint> FrameReceived;

        public void Append(byte[] data)
        {
            if (data is not null && data.Length > 0)
            {
                _buffer.AddRange(data);
            }

            while (_buffer.Count > 7)
            {
                if (ReadU16(0) != FrameProtocol.Header)
                {
                    _buffer.RemoveAt(0);
                    continue;
                }

                int length = ReadI32(3);
                if (length < 0 || length > FrameProtocol.MaxPayloadLength)
                {
                    _buffer.RemoveAt(0);
                    continue;
                }

                int total = FrameProtocol.OverheadLength + length;
                if (_buffer.Count < total)
                {
                    break;
                }

                byte code = _buffer[2];
                byte[] payload = _buffer.GetRange(7, length).ToArray();
                uint crc = ReadU32(7 + length);
                ushort ending = ReadU16(7 + length + 4);

                _buffer.RemoveRange(0, total);

                if (SecurityUtil.CRC32(payload) == crc && ending == FrameProtocol.Ending)
                {
                    FrameReceived?.Invoke((ZoneFunctionCode)code, payload, crc);
                }
            }
        }

        private ushort ReadU16(int index)
        {
            return (ushort)((_buffer[index] << 8) | _buffer[index + 1]);
        }

        private int ReadI32(int index)
        {
            return (_buffer[index] << 24) | (_buffer[index + 1] << 16) | (_buffer[index + 2] << 8) | _buffer[index + 3];
        }

        private uint ReadU32(int index)
        {
            return ((uint)_buffer[index] << 24) | ((uint)_buffer[index + 1] << 16) | ((uint)_buffer[index + 2] << 8) | _buffer[index + 3];
        }
    }
}
