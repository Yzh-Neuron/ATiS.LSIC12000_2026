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
        /// 二级电源规格信息
        /// </summary>
        [SectionName(nameof(DpsInfo))]
        public DpsInfo[] DpsInfos { get; private set; }

        /// <summary>
        /// 恒温器信息
        /// </summary>
        public ThermostatInfo ThermostatInfo { get; private set; }

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
