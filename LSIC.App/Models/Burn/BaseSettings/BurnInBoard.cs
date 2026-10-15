namespace LSIC.App.Models
{
    /// <summary>
    /// 老化板（运行时）
    /// </summary>
    public class BurnInBoard : ObservableObject
    {
        public int Row { get; }
        public int Column { get; }
        public BurnDut[] Duts { get; }
        public BurnThermostat[] Thermostats { get; }
        public BurnInBoard(BurnInBoardSetting setting)
        {
            Row = setting.Row;
            Column = setting.Column;

            List<BurnThermostat> thermostats = new List<BurnThermostat>();
            if (setting.ThermostatSettings != null)
            {
                foreach (ThermostatSetting thermostatSetting in setting.ThermostatSettings)
                {
                    thermostats.Add(new BurnThermostat(thermostatSetting));
                }
            }
            Thermostats = thermostats.ToArray();

            Dictionary<int, BurnThermostat> thermostatById = new Dictionary<int, BurnThermostat>();
            foreach (BurnThermostat thermostat in Thermostats)
            {
                thermostatById[thermostat.Id] = thermostat;
            }

            Duts = new BurnDut[setting.DutSettings.Length];
            for (int i = 0; i < setting.DutSettings.Length; i++)
            {
                DutSetting dutSetting = setting.DutSettings[i];
                BurnThermostat thermostat = null;
                if (dutSetting.ThermostatSetting != null && thermostatById.TryGetValue(dutSetting.ThermostatSetting.Id, out BurnThermostat found))
                {
                    thermostat = found;
                }
                Duts[i] = new BurnDut(dutSetting, thermostat);
            }
        }
        private ulong _dutStates;
        public ulong DutStates
        {
            get => _dutStates;
            private set => SetProperty(ref _dutStates, value);
        }

        /// <summary>
        /// 工位屏蔽掩码（位序对应 BurnDut.Id）
        /// </summary>
        public byte[] DutMask
        {
            get => BuildDutMask();
            set
            {
                int[] integers = MaskToIntegers(value ?? Array.Empty<byte>());
                HashSet<int> masked = [.. integers];
                foreach (BurnDut dut in Duts)
                {
                    dut.Masked = masked.Contains(dut.Id);
                }
                DutStates = AggregateStates();
            }
        }
        public void Burning()
        {
            foreach (BurnDut dut in Duts)
            {
                dut.Finished = false;
                dut.Burning = true;
            }
            DutStates = AggregateStates();
        }
        public void Finished()
        {
            foreach (BurnDut dut in Duts)
            {
                dut.Burning = false;
                dut.Finished = true;
            }
            DutStates = AggregateStates();
        }
        public void CompareError(int cs, int dr)
        {
            foreach (BurnDut dut in Duts)
            {
                if (!dut.Masked && dut.ChipSelect == cs && dut.Drs.Contains(dr))
                {
                    dut.Error = true;
                }
            }
            DutStates = AggregateStates();
        }
        private const int MaskBytes = 32;
        private byte[] BuildDutMask()
        {
            List<int> masked = new List<int>();
            foreach (BurnDut dut in Duts)
            {
                if (dut.Masked)
                {
                    masked.Add(dut.Id);
                }
            }
            return IntegersToMask(masked, MaskBytes);
        }
        private ulong AggregateStates()
        {
            ulong states = 0;
            foreach (BurnDut dut in Duts)
            {
                states += (ulong)dut.State;
            }
            return states;
        }
        private static byte[] IntegersToMask(IEnumerable<int> values, int length)
        {
            byte[] mask = new byte[length];
            foreach (int value in values)
            {
                if (value / 8 < length)
                {
                    mask[value / 8] |= (byte)(1 << value % 8);
                }
            }
            return mask;
        }
        private static int[] MaskToIntegers(byte[] mask)
        {
            List<int> values = new List<int>();
            for (int i = 0; i < mask.Length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((mask[i] & (1 << j)) != 0)
                    {
                        values.Add(i * 8 + j);
                    }
                }
            }
            return values.ToArray();
        }
    }
}