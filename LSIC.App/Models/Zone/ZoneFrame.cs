namespace LSIC.App.Models
{
    /// <summary>
    /// 老化区通讯帧编解码：帧头 0x55AA / 帧尾 0xEAA5 / CRC32，多字节大端
    /// </summary>
    public static class ZoneFrame
    {
        public const ushort FrameHeader = 0x55AA;
        public const ushort FrameEnding = 0xEAA5;

        /// <summary>
        /// 封装一帧
        /// </summary>
        public static byte[] Build(byte code, byte[] data)
        {
            data ??= Array.Empty<byte>();
            int length = data.Length;
            byte[] frame = new byte[13 + length];

            int index = 0;
            Copy(frame, ref index, FrameHeader.ToBytes(Endianness.Big));
            frame[index++] = code;
            Copy(frame, ref index, length.ToBytes(Endianness.Big));
            Copy(frame, ref index, data);
            Copy(frame, ref index, SecurityUtil.CRC32(data).ToBytes(Endianness.Big));
            Copy(frame, ref index, FrameEnding.ToBytes(Endianness.Big));
            return frame;
        }

        /// <summary>
        /// 从接收缓冲区取出一帧（原地丢弃无效字节），返回 false 表示数据不足
        /// </summary>
        public static bool TryExtractFrame(List<byte> buffer, out byte code, out byte[] data)
        {
            code = 0;
            data = null;

            while (buffer.Count > 7)
            {
                if (buffer.ToUInt16(0, Endianness.Big) != FrameHeader)
                {
                    buffer.RemoveAt(0);
                    continue;
                }

                int length = buffer.ToInt32(3, Endianness.Big);
                int total = 13 + length;
                if (buffer.Count < total)
                {
                    return false;
                }

                data = buffer.Skip(7).Take(length).ToArray();
                if (buffer.ToUInt32(7 + length, Endianness.Big) != SecurityUtil.CRC32(data))
                {
                    buffer.RemoveRange(0, total);
                    continue;
                }
                if (buffer.ToUInt16(7 + length + 4, Endianness.Big) != FrameEnding)
                {
                    buffer.RemoveRange(0, total);
                    continue;
                }

                code = buffer[2];
                buffer.RemoveRange(0, total);
                return true;
            }

            return false;
        }

        private static void Copy(byte[] target, ref int index, byte[] source)
        {
            Array.Copy(source, 0, target, index, source.Length);
            index += source.Length;
        }
    }
}
