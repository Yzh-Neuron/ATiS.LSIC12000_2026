using System;
using ATiS.Base;

namespace ZoneSimulator
{
    /// <summary>
    /// 帧编解码，格式与 Zone.SendAsync / Zone.OnReceivedData 一致：
    /// 55 AA | Code(1) | Len(4,大端) | Data(N) | CRC32(4,大端,仅数据段) | EA A5
    /// </summary>
    public static class FrameProtocol
    {
        public const ushort Header = 0x55AA;
        public const ushort Ending = 0xEAA5;
        public const int MaxPayloadLength = 4 * 1024 * 1024;
        public const int OverheadLength = 13;

        public static byte[] BuildFrame(ZoneFunctionCode code, byte[] payload)
        {
            payload ??= Array.Empty<byte>();
            uint crc = SecurityUtil.CRC32(payload);

            byte[] frame = new byte[OverheadLength + payload.Length];
            frame[0] = 0x55;
            frame[1] = 0xAA;
            frame[2] = (byte)code;

            byte[] length = ((uint)payload.Length).ToBytes(Endianness.Big);
            Array.Copy(length, 0, frame, 3, 4);

            if (payload.Length > 0)
            {
                Array.Copy(payload, 0, frame, 7, payload.Length);
            }

            byte[] checkBytes = crc.ToBytes(Endianness.Big);
            Array.Copy(checkBytes, 0, frame, 7 + payload.Length, 4);

            frame[^2] = 0xEA;
            frame[^1] = 0xA5;
            return frame;
        }
    }
}
