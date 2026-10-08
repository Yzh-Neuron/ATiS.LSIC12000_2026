namespace LSIC.App
{
    public enum ZoneFunctionCode : byte
    {
        /// <remarks>查询 & 上传版本信息</remarks>
        Version = 0x00,

        /// <remarks>老化控制 & 状态上报</remarks>
        ControlAndState = 0x01,

        /// <remarks>插板状态</remarks>
        BurnInBoardInsertState = 0x02,

        /// <remarks>电源状态查询 & 状态上报</remarks>
        DpsState = 0x03,

        /// <remarks>上传当前使用的电源组</remarks>
        CurrentDpsGroup = 0x04,

        /// <remarks>上传老化进程</remarks>
        BurnState = 0x05,

        /// <remarks>上报回检报错</remarks>
        CompareError = 0x06,

        /// <remarks>切换 & 上报Trigger通道</remarks>
        Trigger = 0x07,

        /// <remarks>配置电源组数量</remarks>
        DpsGroupCount = 0x50,

        /// <remarks>配置单个电源组</remarks>
        DpsGroup = 0x51,

        /// <remarks>配置向量数量</remarks>
        PatternCount = 0x52,

        /// <remarks>配置单个向量</remarks>
        Pattern = 0x53,

        /// <remarks>配置REG数量</remarks>
        RegisterCount = 0x54,

        /// <remarks>配置单个REG</remarks>
        Register = 0x55,

        /// <remarks>上电设置</remarks>
        UpSet = 0x56,

        /// <remarks>步次设置</remarks>
        StepSet = 0x57,

        /// <remarks>下电设置</remarks>
        DownSet = 0x58,

        /// <remarks>SPI设置</remarks>
        SpiSet = 0x59,

        /// <remarks>器件发送标识</remarks>
        ConfigFinish = 0x5F,

        /// <remarks>配置在线升级 & 上报在线升级进程</remarks>
        Upgrade = 0xA0,

        /// <remarks>上报在线升级结束状态</remarks>
        UpgradeFinish = 0xA1,

        /// <remarks>启动设备自检 & 上报自检条目</remarks>
        DeviceCheck = 0xF0,

        /// <remarks>启动DPS自检</remarks>
        DpsCheck = 0xF1,

        /// <remarks>温控配置 & 推送温控信息</remarks>
        Thermostat = 0xF2,

        /// <remarks>温控PID参数 & 推送温控PID参数</remarks>
        ThermostatPID = 0xF3,
    }
}
