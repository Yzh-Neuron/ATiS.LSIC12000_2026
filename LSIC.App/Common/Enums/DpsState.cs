namespace LSIC.App
{
    /// <summary>
    /// DPS 状态位
    /// </summary>
    public enum DpsState:byte
    {
        /// <summary>
        /// 关闭
        /// </summary>
        Off,

        /// <summary>
        /// 输出过压故障
        /// </summary>
        OutputOv,

        /// <summary>
        /// 输出过流故障
        /// </summary>
        OutputOc,

        /// <summary>
        /// 输入电压OV/UV故障
        /// </summary>
        InputOvUv,

        /// <summary>
        /// 芯片过温故障
        /// </summary>
        ChipOt,

        /// <summary>
        /// 通信/命令逻辑故障
        /// </summary>
        CommLogicFault,

        /// <summary>
        /// 厂商自定义故障
        /// </summary>
        VendorCustomFault,

        /// <summary>
        /// PG电源正常信号异常
        /// </summary>
        PgAbnormal,

        /// <summary>
        /// 总故障汇总位，任意故障置1
        /// </summary>
        FaultSummary,

        /// <summary>
        /// 输出欠压
        /// </summary>
        OutputUv,

        /// <summary>
        /// 输出电流告警
        /// </summary>
        OutputCurrentWarning,

        /// <summary>
        /// 温度告警
        /// </summary>
        TemperatureWarning,

        /// <summary>
        /// MOS管功率回路故障
        /// </summary>
        MosPowerLoopFault,

        /// <summary>
        /// 其他故障
        /// </summary>
        OtherFault,

        /// <summary>
        /// 运行
        /// </summary>
        Runing = 0xFF,
    }
}