namespace LSIC.App.Models
{
    /// <summary>
    /// 老化板工位信息
    /// </summary>
    public class DutInfo : ObservableObject, ICloneable
    {
        public DutInfo(int? zoneId, int rows, int cols)
        {
            ZoneId = zoneId;
            Rows = rows;
            Cols = cols;
            Duts = new Dut[rows * cols];
        }

        private DutInfo()
        {

        }

        public int? ZoneId { get; private init; }

        public int Rows { get; private init; }

        public int Cols { get; private init; }

        public Dut[] Duts { get; private init; }

        private ulong _dutStates;
        public ulong DutStates
        {
            get => _dutStates;
            private set => SetProperty(ref _dutStates, value);
        }

        public byte[] DutMask
        {
            get
            {
                int[] dutIds = Duts.Where(i => i.Masked).Select(i => i.Id).ToArray();
                return IntegersToMask(dutIds, 32);
            }
            set
            {
                if (value != null)
                {
                    int[] dutIds = MaskToIntegers(value);
                    ulong states = 0;
                    foreach (Dut dut in Duts)
                    {
                        dut.Masked = dutIds.Contains(dut.Id);
                        states += (ulong)dut.State;
                    }
                    DutStates = states;
                }
            }
        }

        public void Burning()
        {
            ulong states = 0;
            foreach (Dut dut in Duts)
            {
                dut.Burning = true;
                states += (ulong)dut.State;
            }
            DutStates = states;
        }

        public void Finished()
        {
            ulong states = 0;
            foreach (Dut dut in Duts)
            {
                dut.Finished = true;
                states += (ulong)dut.State;
            }
            DutStates = states;
        }

        public void CompareError(int cs, int dr)
        {
            ulong states = 0;
            foreach (Dut dut in Duts)
            {
                if (!dut.Masked &&
                    dut.ChipSelect == cs &&
                    dut.Drs.Contains(dr))
                {
                    dut.Error = true;
                }
                states += (ulong)dut.State;
            }
            DutStates = states;
        }

        public object Clone()
        {
            return new DutInfo()
            {
                ZoneId = ZoneId,
                Rows = Rows,
                Cols = Cols,
                Duts = Duts.Select(i => (Dut)i.Clone()).ToArray()
            };
        }

        public byte[] Serialize()
        {
            /*
             rows{byte}
             cols{byte}
             row{byte} col{byte} id{byte} drs{byte[32]} cs{byte} dpss{byte[4]} temp{byte} tag{byte}
             length = 2 + 42 * [dut count]
             */
            byte[] data = new byte[2 + 42 * Duts.Length];
            int index = 0;
            data[index++] = (byte)Rows;
            data[index++] = (byte)Cols;
            foreach (Dut dut in Duts)
            {
                data[index++] = (byte)dut.Id;

                data[index++] = (byte)dut.Row;

                data[index++] = (byte)dut.Column;

                byte[] mask = IntegersToMask(dut.Drs, 32);
                Array.Copy(mask, 0, data, index, 32);
                index += 32;

                data[index++] = (byte)dut.ChipSelect;

                mask = IntegersToMask(dut.Dpss, 4);
                Array.Copy(mask, 0, data, index, 4);
                index += 4;

                data[index++] = dut.ThermostatId != null
                              ? (byte)(0x80 | (byte)dut.ThermostatId)
                              : (byte)0;

                index++;
            }
            return data;
        }

        private static byte[] IntegersToMask(int[] values, int length)
        {
            byte[] mask = new byte[length];
            foreach (int value in values)
            {
                int index = value / 8;
                int bits = value % 8;
                if (index < length)
                {
                    mask[index] |= (byte)(0x1 << bits);
                }
            }
            return mask;
        }

        private static int[] MaskToIntegers(byte[] mask)
        {
            List<int> values = new List<int>();
            for (int i = 0; i < mask.Length; i++)
            {
                byte temp = mask[i];
                for (int j = 0; j < 8; j++)
                {
                    if (((temp >> j) & 0x1) == 0x1)
                    {
                        values.Add(i * 8 + j);
                    }
                }
            }
            return values.ToArray();
        }

        public static DutInfo Deserialize(int zoneId, byte[] data)
        {
            int index = 0;
            int rows = data[index++];
            int cols = data[index++];
            DutInfo board = new DutInfo(zoneId, rows, cols);
            for (int i = 0; i < board.Duts.Length; i++)
            {
                int id = data[index++];
                int row = data[index++];
                int col = data[index++];

                byte[] mask = new byte[32];
                Array.Copy(data, index, mask, 0, 32);
                int[] drs = MaskToIntegers(mask);
                index += 32;

                int cs = data[index++];

                mask = new byte[4];
                Array.Copy(data, index, mask, 0, 4);
                int[] dpss = MaskToIntegers(mask);
                index += 4;

                int? thermostatId = null;
                byte value = data[index++];
                if (value >> 7 == 1)
                {
                    thermostatId = value & 0x7F;
                }

                index++;

                board.Duts[row * cols + col] = new Dut(zoneId, id, row, col, cs, drs, dpss, thermostatId);
            }

            return board;
        }
    }
}
