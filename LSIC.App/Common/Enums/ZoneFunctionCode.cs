namespace LSIC.App
{
    public enum ZoneFunctionCode : byte
    {
        /// <summary>
        /// 查询 & 上传版本信息
        /// </summary>
        Version = 0x00,

        /// <summary>
        /// 老化控制 & 状态上报
        /// </summary>
        ControlAndState = 0x01,

        /// <summary>
        /// 插板状态
        /// </summary>
        BurnInBoardInsertState = 0x02,

        /// <summary>
        /// 电源状态查询 & 状态上报
        /// </summary>
        DpsState = 0x03,

        /// <summary>
        /// 上传当前使用的电源方案
        /// </summary>
        CurrentDpsPlan = 0x04,

        /// <summary>
        /// 上传老化进程
        /// </summary>
        BurnState = 0x05,

        /// <summary>
        /// 上报回检报错
        /// </summary>
        CompareError = 0x06,

        /// <summary>
        /// 切换 & 上报Trigger通道
        /// </summary>
        Trigger = 0x07,

        /// <summary>
        /// 配置电源方案数量
        /// </summary>
        DpsPlanCount = 0x50,

        /// <summary>
        /// 配置单个电源方案
        /// </summary>
        DpsPlan = 0x51,

        /// <summary>
        /// 配置向量数量
        /// </summary>
        PatternCount = 0x52,

        /// <summary>
        /// 配置单个向量
        /// </summary>
        Pattern = 0x53,

        /// <summary>
        /// 配置REG数量
        /// </summary>
        RegisterCount = 0x54,

        /// <summary>
        /// 配置单个REG
        /// </summary>
        Register = 0x55,

        /// <summary>
        /// 上电设置
        /// </summary>
        UpSet = 0x56,

        /// <summary>
        /// 步次设置
        /// </summary>
        StepSet = 0x57,

        /// <summary>
        /// 下电设置
        /// </summary>
        DownSet = 0x58,

        ///// <summary>
        ///// SPI设置
        ///// </summary>
        //SpiSet = 0x59,

        /// <summary>
        /// 器件发送标识
        /// </summary>
        ConfigFinish = 0x5F,

        /// <summary>
        /// 配置在线升级 & 上报在线升级进程
        /// </summary>
        Upgrade = 0xA0,

        /// <summary>
        /// 上报在线升级结束状态
        /// </summary>
        UpgradeFinish = 0xA1,

        /// <summary>
        /// 启动设备自检 & 上报自检条目
        /// </summary>
        DeviceCheck = 0xF0,

        /// <summary>
        /// 启动DPS自检
        /// </summary>
        DpsCheck = 0xF1,

        /// <summary>
        /// 温控配置 & 推送温控信息
        /// </summary>
        Thermostat = 0xF2,

        /// <summary>
        /// 温控PID参数 & 推送温控PID参数
        /// </summary>
        ThermostatPID = 0xF3,
    }
}
