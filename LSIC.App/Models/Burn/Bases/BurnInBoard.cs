namespace LSIC.App.Models
{
    public class BurnInBoard : ObservableObject
    {
        public int Rows { get; }
        public int Cols { get; }
        public Dut[] Duts { get; }
        public Thermostat[] Thermostats { get; }
        public BurnInBoard(BurnInBoardSetting setting)
        {
            Rows = setting.Row;
            Cols = setting.Column;
            Thermostats = (setting.ThermostatSettings ?? Array.Empty<ThermostatSetting>()).Select(t => new Thermostat(t)).ToArray();
            var thermostatById = Thermostats.ToDictionary(t => t.Id);
            Duts = setting.DutSettings.Select(s => new Dut(s, s.ThermostatSetting is { } ts && thermostatById.TryGetValue(ts.Id, out var t) ? t : null)).ToArray();
        }
        private ulong _dutStates;
        public ulong DutStates
        {
            get => _dutStates;
            private set => SetProperty(ref _dutStates, value);
        }
        public byte[] DutMask
        {
            get => IntegersToMask(Duts.Where(d => d.Masked).Select(d => d.Id), 32);
            set
            {
                var masked = new HashSet<int>(MaskToIntegers(value ?? Array.Empty<byte>()));
                ulong states = 0;
                foreach (Dut dut in Duts)
                {
                    dut.Masked = masked.Contains(dut.Id);
                    states += (ulong)dut.State;
                }
                DutStates = states;
            }
        }
        public void Burning()
        {
            ulong states = 0;
            foreach (Dut dut in Duts)
            {
                dut.Finished = false;
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
                dut.Burning = false;
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
                if (!dut.Masked && dut.ChipSelect == cs && dut.Drs.Contains(dr))
                {
                    dut.Error = true;
                }
                states += (ulong)dut.State;
            }
            DutStates = states;
        }
        private static byte[] IntegersToMask(IEnumerable<int> values, int length)
        {
            var mask = new byte[length];
            foreach (int value in values)
            {
                if (value / 8 < length)
                {
                    mask[value / 8] |= (byte)(1 << value % 8);
                }
            }
            return mask;
        }
        private static int[] MaskToIntegers(byte[] mask) =>
            mask.SelectMany((b, i) => Enumerable.Range(0, 8)
                .Where(j => (b & (1 << j)) != 0)
                .Select(j => i * 8 + j))
                .ToArray();
    }
}