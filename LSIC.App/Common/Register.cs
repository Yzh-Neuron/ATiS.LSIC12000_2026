namespace LSIC.App
{
    public class Register
    {
        public enum OutputFormat
        {
            NORMAL = 1,
            ONE,
            ZERO,
            TRISTATE,
            NOTEST,
            DNRZ
        }

        public enum ChipSelectOutputFormat
        {
            ONE = 1,
            ZERO
        }

        public enum CompareMode
        {
            DOUBLE = 0x4,
            SINGLEVRL = 0x5,
            SINGLEVRH = 0x6,
            DISABLE = 0x8
        }

        public enum Command
        {
            VIH,
            VIHCLAMP,
            VH_SEL,
            VRH,
            VRL,
            RRADR,
            CLOCK,
            CLOCKTE, // 负负得正
            CLOCKLE,
            STROBE,
            FORMAT,
            MODSTADR,
            MODEOF,
            CSDRVID,
            CSDEFALT,
            HFCFREQ,
            COMPMODE,
            REP_RATE = 0xF0,
        }

        public Dictionary<int, float> VIH { get; } = new Dictionary<int, float>();

        public Dictionary<int, float> VIHClamp { get; } = new Dictionary<int, float>();

        public List<KeyValuePair<int[], int>> VHSel { get; } = new List<KeyValuePair<int[], int>>();

        public Dictionary<int, float> VRH { get; } = new Dictionary<int, float>();

        public Dictionary<int, float> VRL { get; } = new Dictionary<int, float>();

        public Dictionary<int, int> Period { get; } = new Dictionary<int, int>();

        public Dictionary<int, List<KeyValuePair<int[], KeyValuePair<int, int>>>> Clock { get; } = new Dictionary<int, List<KeyValuePair<int[], KeyValuePair<int, int>>>>();

        public Dictionary<int, List<KeyValuePair<int[], int>>> ClockLE { get; } = new Dictionary<int, List<KeyValuePair<int[], int>>>();

        public Dictionary<int, List<KeyValuePair<int[], int>>> ClockTE { get; } = new Dictionary<int, List<KeyValuePair<int[], int>>>();

        public Dictionary<int, List<KeyValuePair<int[], int>>> Strobe { get; } = new Dictionary<int, List<KeyValuePair<int[], int>>>();

        public List<KeyValuePair<int[], OutputFormat>> Format { get; } = new List<KeyValuePair<int[], OutputFormat>>();

        public int? ModStart { get; set; }

        public int? ModEof { get; set; }

        public List<KeyValuePair<int[], int>> CsDrvid { get; } = new List<KeyValuePair<int[], int>>();

        public List<KeyValuePair<int[], ChipSelectOutputFormat>> CsDefault { get; } = new List<KeyValuePair<int[], ChipSelectOutputFormat>>();

        public Dictionary<int, int> HightFrequency { get; } = new Dictionary<int, int>();

        public List<KeyValuePair<int[], CompareMode>> CompareModes { get; } = new List<KeyValuePair<int[], CompareMode>>();

        private static byte[] Serialize(Command command, int[] channels, int arg0, int arg1, int arg2)
        {
            // 32：4(命令) + 4(arg0) + 16(2板掩码) + 4(arg1) + 4(arg2)
            // 48：4(命令) + 4(arg0) + 32(4板掩码) + 4(arg1) + 4(arg2)
            byte[] data = new byte[48];

            int index = 0;
            Array.Copy(((int)command).ToBytes(Endianness.Big), 0, data, index, 4);
            index += 4;

            Array.Copy(arg0.ToBytes(Endianness.Big), 0, data, index, 4);
            index += 4;

            // 4 张板卡，每张 64 通道
            long[] boards = new long[4];
            if (channels is not null)
            {
                foreach (int channel in channels)
                {
                    int i = channel / 64;
                    int bits = channel % 64;
                    boards[i] |= (long)1 << bits;
                }
            }
            // 4 个 long，大端序拍平 = 32 字节
            byte[] mask = boards.Select(i => i.ToBytes(Endianness.Big)).Flatten().ToArray();
            Array.Copy(mask, 0, data, index, mask.Length);
            index += mask.Length;

            Array.Copy(arg1.ToBytes(Endianness.Big), 0, data, index, 4);
            index += 4;

            Array.Copy(arg2.ToBytes(Endianness.Big), 0, data, index, 4);

            return data;
        }

        public byte[] Serialize(out int count)
        {
            count = 0;

            using MemoryStream stream = new MemoryStream();
            byte[] data;

            foreach (KeyValuePair<int, float> pair in VIH)
            {
                data = Serialize(Command.VIH, null, pair.Key, (int)(decimal)(pair.Value * 1000), 0);
                stream.Write(data);
                count++;
            }

            foreach (KeyValuePair<int, float> pair in VIHClamp)
            {
                data = Serialize(Command.VIHCLAMP, null, pair.Key, (int)(decimal)(pair.Value * 1000), 0);
                stream.Write(data);
                count++;
            }

            foreach (KeyValuePair<int[], int> pair in VHSel)
            {
                data = Serialize(Command.VH_SEL, pair.Key, (int)(decimal)(pair.Value * 1000), 0, 0);
                stream.Write(data);
                count++;
            }

            foreach (KeyValuePair<int, float> pair in VRH)
            {
                data = Serialize(Command.VRH, null, pair.Key, (int)(decimal)(pair.Value * 1000), 0);
                stream.Write(data);
                count++;
            }

            foreach (KeyValuePair<int, float> pair in VRL)
            {
                data = Serialize(Command.VRL, null, pair.Key, (int)(decimal)(pair.Value * 1000), 0);
                stream.Write(data);
                count++;
            }

            foreach (KeyValuePair<int, int> pair in Period)
            {
                data = Serialize(Command.RRADR, null, pair.Key, pair.Value, 0);
                stream.Write(data);
                count++;
            }

            foreach (KeyValuePair<int, List<KeyValuePair<int[], KeyValuePair<int, int>>>> clock in Clock)
            {
                foreach (KeyValuePair<int[], KeyValuePair<int, int>> pair in clock.Value)
                {
                    data = Serialize(Command.CLOCK, pair.Key, clock.Key, pair.Value.Key, pair.Value.Value);
                    stream.Write(data);
                    count++;
                }
            }

            foreach (KeyValuePair<int, List<KeyValuePair<int[], int>>> clock in ClockLE)
            {
                foreach (KeyValuePair<int[], int> pair in clock.Value)
                {
                    data = Serialize(Command.CLOCKLE, pair.Key, clock.Key, pair.Value, 0);
                    stream.Write(data);
                    count++;
                }
            }

            foreach (KeyValuePair<int, List<KeyValuePair<int[], int>>> clock in ClockTE)
            {
                foreach (KeyValuePair<int[], int> pair in clock.Value)
                {
                    data = Serialize(Command.CLOCKTE, pair.Key, clock.Key, pair.Value, 0);
                    stream.Write(data);
                    count++;
                }
            }

            foreach (KeyValuePair<int, List<KeyValuePair<int[], int>>> clock in Strobe)
            {
                foreach (KeyValuePair<int[], int> pair in clock.Value)
                {
                    data = Serialize(Command.STROBE, pair.Key, clock.Key, pair.Value, 0);
                    stream.Write(data);
                    count++;
                }
            }

            foreach (KeyValuePair<int[], OutputFormat> pair in Format)
            {
                data = Serialize(Command.FORMAT, pair.Key, 0, (int)pair.Value, 0);
                stream.Write(data);
                count++;
            }

            if (ModStart.HasValue)
            {
                data = Serialize(Command.MODSTADR, null, 0, ModStart.Value, 0);
                stream.Write(data);
                count++;
            }

            if (ModEof.HasValue)
            {
                data = Serialize(Command.MODEOF, null, 0, ModEof.Value, 0);
                stream.Write(data);
                count++;
            }

            foreach (KeyValuePair<int[], int> pair in CsDrvid)
            {
                data = Serialize(Command.CSDRVID, pair.Key, 0, pair.Value, 0);
                stream.Write(data);
                count++;
            }

            foreach (KeyValuePair<int[], ChipSelectOutputFormat> pair in CsDefault)
            {
                data = Serialize(Command.CSDEFALT, pair.Key, 0, (int)pair.Value, 0);
                stream.Write(data);
                count++;
            }

            foreach (KeyValuePair<int, int> pair in HightFrequency)
            {
                data = Serialize(Command.HFCFREQ, null, pair.Key, pair.Value, 0);
                stream.Write(data);
                count++;
            }

            foreach (KeyValuePair<int[], CompareMode> pair in CompareModes)
            {
                data = Serialize(Command.COMPMODE, pair.Key, (int)pair.Value, 0, 0);
                stream.Write(data);
                count++;
            }

            return stream.ToArray();
        }
    }
}
