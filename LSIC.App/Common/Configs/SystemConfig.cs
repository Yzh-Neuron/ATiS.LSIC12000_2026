namespace LSIC.App
{
    /// <summary>
    /// 系统配置
    /// </summary>
    public class SystemConfig
    {
        /// <summary>
        /// 设备信息
        /// </summary>
        public DeviceInfo DeviceInfo { get; private set; }

        /// <summary>
        /// 强电控制板信息
        /// </summary>
        public HVControlBoardInfo HVControlBoardInfo { get; private set; }

        /// <summary>
        /// 弱电控制板信息
        /// </summary>
        [SectionName(nameof(LVControlBoardInfo))]
        public LVControlBoardInfo[] LVControlBoardInfos { get; private set; }

        /// <summary>
        /// 烘箱信息
        /// </summary>
        [SectionName(nameof(OvenInfo))]
        public OvenInfo[] OvenInfos { get; private set; }

        /// <summary>
        /// 老化区信息
        /// </summary>
        [SectionName(nameof(ZoneInfo))]
        public ZoneInfo[] ZoneInfos { get; private set; }

        /// <summary>
        /// 竖向转接板信息
        /// </summary>
        [SectionName(nameof(VerticalAdapterBoardInfo))]
        public VerticalAdapterBoardInfo[] VerticalAdapterBoardInfos { get; private set; }

        /// <summary>
        /// 一级电源规格信息
        /// </summary>
        [SectionName(nameof(PpsInfo))]
        public PpsInfo[] PpsInfos { get; private set; }

        /// <summary>
        /// 二级电源规格信息
        /// </summary>
        [SectionName(nameof(DpsInfo))]
        public DpsInfo[] DpsInfos { get; private set; }

        /// <summary>
        /// BMC 电源规格信息
        /// </summary>
        [SectionName(nameof(BpsInfo))]
        public BpsInfo[] BpsInfos { get; private set; }

        /// <summary>
        /// VIH 电源规格信息
        /// </summary>
        [SectionName(nameof(VIHInfo))]
        public VIHInfo[] VIHInfos { get; private set; }

        /// <summary>
        /// 恒温器信息
        /// </summary>
        public ThermostatInfo ThermostatInfo { get; private set; }

        /// <summary>
        /// 急停按钮信息
        /// </summary>
        [SectionName(nameof(EMOInfo))]
        public EMOInfo[] EMOInfos { get; private set; }

        /// <summary>
        /// 门信息
        /// </summary>
        [SectionName(nameof(DoorInfo))]
        public DoorInfo[] DoorInfos { get; private set; }

        /// <summary>
        /// 交流检测点信息
        /// </summary>
        [SectionName(nameof(ACBoardInfo))]
        public ACBoardInfo[] ACBoardInfos { get; private set; }

        /// <summary>
        /// 日志规则
        /// </summary>
        [SectionName(nameof(LogRule))]
        public LogRule[] LogRules { get; private set; }

        /// <summary>
        /// 日志列规则
        /// </summary>
        [SectionName(nameof(LogColumn))]
        public LogColumn[] LogColumns { get; private set; }
    }
}
