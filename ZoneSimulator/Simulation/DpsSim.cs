namespace ZoneSimulator
{
    /// <summary>单路电源（DPS / VIH）模拟状态。电压 mV、电流 mA、温度 0.1℃</summary>
    public sealed class DpsSim
    {
        public DpsSim(byte id, DpsType type)
        {
            Id = id;
            Type = type;
            Name = type == DpsType.DPS ? $"DPS{id}" : $"VIH{id}";
        }

        public byte Id { get; }
        public DpsType Type { get; }
        public string Name { get; }

        public int InputVoltage { get; set; } = 12000;
        public int OutputVoltage { get; set; }
        public int OutputCurrent { get; set; }
        public int Temperature { get; set; } = 300;
        public DpsState State { get; set; } = DpsState.Off;

        public int TargetVoltage { get; set; }
        public int TargetCurrent { get; set; }

        public bool IsOn => State == DpsState.Runing;
    }
}
