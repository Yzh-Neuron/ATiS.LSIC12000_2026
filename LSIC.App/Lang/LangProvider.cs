namespace LSIC.App
{
    public sealed partial class LangProvider : LanguageProviderBase
    {
        public static LangProvider Instance { get; } = new();
        public UiLanguage CurrentLanguage => GlobalLanguageManager.CurrentLanguage;

        /// <summary>生产模式</summary>
        public string Enum_DeviceMode_Production => GetString("Enum.DeviceMode.Production");
        /// <summary>工程模式</summary>
        public string Enum_DeviceMode_Engineering => GetString("Enum.DeviceMode.Engineering");
        /// <summary>管理员模式</summary>
        public string Enum_DeviceMode_Administrator => GetString("Enum.DeviceMode.Administrator");
        /// <summary>待机中</summary>
        public string Enum_OvenMode_Off => GetString("Enum.OvenMode.Off");
        /// <summary>待机中</summary>
        public string Enum_OvenMode_Standby => GetString("Enum.OvenMode.Standby");
        /// <summary>定值运行中</summary>
        public string Enum_OvenMode_Constant => GetString("Enum.OvenMode.Constant");
        /// <summary>程式运行中</summary>
        public string Enum_OvenMode_Run => GetString("Enum.OvenMode.Run");
        /// <summary>离线</summary>
        public string Enum_OvenMode_Offline => GetString("Enum.OvenMode.Offline");
        /// <summary>秒</summary>
        public string Enum_TimeUnit_Second => GetString("Enum.TimeUnit.Second");
        /// <summary>分钟</summary>
        public string Enum_TimeUnit_Minute => GetString("Enum.TimeUnit.Minute");
        /// <summary>小时</summary>
        public string Enum_TimeUnit_Hour => GetString("Enum.TimeUnit.Hour");
        /// <summary>天</summary>
        public string Enum_TimeUnit_Day => GetString("Enum.TimeUnit.Day");
        /// <summary>时间</summary>
        public string Enum_RunMode_Time => GetString("Enum.RunMode.Time");
        /// <summary>次数</summary>
        public string Enum_RunMode_Times => GetString("Enum.RunMode.Times");
        /// <summary>风冷</summary>
        public string Enum_CoolingMode_Air => GetString("Enum.CoolingMode.Air");
        /// <summary>水冷</summary>
        public string Enum_CoolingMode_Water => GetString("Enum.CoolingMode.Water");
        /// <summary>查询版本</summary>
        public string Enum_ZoneFunctionCode_Version => GetString("Enum.ZoneFunctionCode.Version");
        /// <summary>同步时间</summary>
        public string Enum_ZoneFunctionCode_SyncTime => GetString("Enum.ZoneFunctionCode.SyncTime");
        /// <summary>插板状态</summary>
        public string Enum_ZoneFunctionCode_PlugedState => GetString("Enum.ZoneFunctionCode.PlugedState");
        /// <summary>老化控制</summary>
        public string Enum_ZoneFunctionCode_BurnControl => GetString("Enum.ZoneFunctionCode.BurnControl");
        /// <summary>老化流程</summary>
        public string Enum_ZoneFunctionCode_BurnProcess => GetString("Enum.ZoneFunctionCode.BurnProcess");
        /// <summary>向量错误</summary>
        public string Enum_ZoneFunctionCode_PatternError => GetString("Enum.ZoneFunctionCode.PatternError");
        /// <summary>电源状态</summary>
        public string Enum_ZoneFunctionCode_PowerState => GetString("Enum.ZoneFunctionCode.PowerState");
        /// <summary>触发通道</summary>
        public string Enum_ZoneFunctionCode_TriggerChannel => GetString("Enum.ZoneFunctionCode.TriggerChannel");
        /// <summary>温控系数</summary>
        public string Enum_ZoneFunctionCode_ThermostatPID => GetString("Enum.ZoneFunctionCode.ThermostatPID");
        /// <summary>器件发送标志</summary>
        public string Enum_ZoneFunctionCode_ConfigFlag => GetString("Enum.ZoneFunctionCode.ConfigFlag");
        /// <summary>设置电源方案</summary>
        public string Enum_ZoneFunctionCode_SetDpsPlan => GetString("Enum.ZoneFunctionCode.SetDpsPlan");
        /// <summary>设置向量数据</summary>
        public string Enum_ZoneFunctionCode_SetPattern => GetString("Enum.ZoneFunctionCode.SetPattern");
        /// <summary>设置地址数据</summary>
        public string Enum_ZoneFunctionCode_SetRegister => GetString("Enum.ZoneFunctionCode.SetRegister");
        /// <summary>设置老化步次</summary>
        public string Enum_ZoneFunctionCode_SetStep => GetString("Enum.ZoneFunctionCode.SetStep");
        /// <summary>设置上电方案</summary>
        public string Enum_ZoneFunctionCode_SetPowerUp => GetString("Enum.ZoneFunctionCode.SetPowerUp");
        /// <summary>设置下电方案</summary>
        public string Enum_ZoneFunctionCode_SetPowerDown => GetString("Enum.ZoneFunctionCode.SetPowerDown");
        /// <summary>设置独立温控</summary>
        public string Enum_ZoneFunctionCode_Thermostat => GetString("Enum.ZoneFunctionCode.Thermostat");
        /// <summary>固件自检</summary>
        public string Enum_ZoneFunctionCode_SelfCheck => GetString("Enum.ZoneFunctionCode.SelfCheck");
        /// <summary>查询版本</summary>
        public string Enum_LVControlBoardCode_Version => GetString("Enum.LVControlBoardCode.Version");
        /// <summary>同步时间</summary>
        public string Enum_LVControlBoardCode_SyncTime => GetString("Enum.LVControlBoardCode.SyncTime");
        /// <summary>三色报警灯</summary>
        public string Enum_LVControlBoardCode_LightAlarm => GetString("Enum.LVControlBoardCode.LightAlarm");
        /// <summary>烟雾探测器</summary>
        public string Enum_LVControlBoardCode_Smoke => GetString("Enum.LVControlBoardCode.Smoke");
        /// <summary>急停按钮</summary>
        public string Enum_LVControlBoardCode_Emergency => GetString("Enum.LVControlBoardCode.Emergency");
        /// <summary>烘箱门状态</summary>
        public string Enum_LVControlBoardCode_OvenDoor => GetString("Enum.LVControlBoardCode.OvenDoor");
        /// <summary>强电门状态</summary>
        public string Enum_LVControlBoardCode_PowerDoor => GetString("Enum.LVControlBoardCode.PowerDoor");
        /// <summary>系统供电</summary>
        public string Enum_LVControlBoardCode_SystemPower => GetString("Enum.LVControlBoardCode.SystemPower");
        /// <summary>触发通道</summary>
        public string Enum_LVControlBoardCode_TriggerChannel => GetString("Enum.LVControlBoardCode.TriggerChannel");
        /// <summary>固件自检</summary>
        public string Enum_LVControlBoardCode_SelfCheck => GetString("Enum.LVControlBoardCode.SelfCheck");
        /// <summary>查询版本</summary>
        public string Enum_VerticalAdapterBoardCode_Version => GetString("Enum.VerticalAdapterBoardCode.Version");
        /// <summary>同步时间</summary>
        public string Enum_VerticalAdapterBoardCode_SyncTime => GetString("Enum.VerticalAdapterBoardCode.SyncTime");
        /// <summary>BMC供电</summary>
        public string Enum_VerticalAdapterBoardCode_BmcPower => GetString("Enum.VerticalAdapterBoardCode.BmcPower");
        /// <summary>一级电源</summary>
        public string Enum_VerticalAdapterBoardCode_PrePower => GetString("Enum.VerticalAdapterBoardCode.PrePower");
        /// <summary>触发通道</summary>
        public string Enum_VerticalAdapterBoardCode_TriggerChannel => GetString("Enum.VerticalAdapterBoardCode.TriggerChannel");
        /// <summary>固件自检</summary>
        public string Enum_VerticalAdapterBoardCode_SelfCheck => GetString("Enum.VerticalAdapterBoardCode.SelfCheck");
        /// <summary>初始化中</summary>
        public string Enum_ZoneState_Init => GetString("Enum.ZoneState.Init");
        /// <summary>空闲</summary>
        public string Enum_ZoneState_Idle => GetString("Enum.ZoneState.Idle");
        /// <summary>就绪</summary>
        public string Enum_ZoneState_Ready => GetString("Enum.ZoneState.Ready");
        /// <summary>老化中</summary>
        public string Enum_ZoneState_Burning => GetString("Enum.ZoneState.Burning");
        /// <summary>老化暂停</summary>
        public string Enum_ZoneState_Suspend => GetString("Enum.ZoneState.Suspend");
        /// <summary>老化停止</summary>
        public string Enum_ZoneState_Stop => GetString("Enum.ZoneState.Stop");
        /// <summary>老化完成</summary>
        public string Enum_ZoneState_Finish => GetString("Enum.ZoneState.Finish");
        /// <summary>上电中</summary>
        public string Enum_ZoneState_PowerUp => GetString("Enum.ZoneState.PowerUp");
        /// <summary>下电中</summary>
        public string Enum_ZoneState_PowerDown => GetString("Enum.ZoneState.PowerDown");
        /// <summary>自检中</summary>
        public string Enum_ZoneState_SelfCheck => GetString("Enum.ZoneState.SelfCheck");
        /// <summary>升级中</summary>
        public string Enum_ZoneState_Upgrading => GetString("Enum.ZoneState.Upgrading");
        /// <summary>离线</summary>
        public string Enum_ZoneState_Offline => GetString("Enum.ZoneState.Offline");
        /// <summary>传输中</summary>
        public string Enum_ZoneState_IcSending => GetString("Enum.ZoneState.IcSending");
        /// <summary>错误</summary>
        public string Enum_ZoneState_Error => GetString("Enum.ZoneState.Error");
        /// <summary>空闲</summary>
        public string Enum_DutState_Idle => GetString("Enum.DutState.Idle");
        /// <summary>测试中</summary>
        public string Enum_DutState_Test => GetString("Enum.DutState.Test");
        /// <summary>通过</summary>
        public string Enum_DutState_Pass => GetString("Enum.DutState.Pass");
        /// <summary>错误</summary>
        public string Enum_DutState_Fail => GetString("Enum.DutState.Fail");
        /// <summary>忽略</summary>
        public string Enum_DutState_Ignore => GetString("Enum.DutState.Ignore");
        /// <summary>驱动板</summary>
        public string Enum_BoardType_DriverBoard => GetString("Enum.BoardType.DriverBoard");
        /// <summary>底板</summary>
        public string Enum_BoardType_BaseBoard => GetString("Enum.BoardType.BaseBoard");
        /// <summary>数字板</summary>
        public string Enum_BoardType_DigitalBoard => GetString("Enum.BoardType.DigitalBoard");
        /// <summary>模拟板</summary>
        public string Enum_BoardType_AnalogBoard => GetString("Enum.BoardType.AnalogBoard");
        /// <summary>二级电源控制板</summary>
        public string Enum_BoardType_DPSControlBoard => GetString("Enum.BoardType.DPSControlBoard");
        /// <summary>DPS</summary>
        public string Enum_BoardType_DPS => GetString("Enum.BoardType.DPS");
        /// <summary>VIH</summary>
        public string Enum_BoardType_VIH => GetString("Enum.BoardType.VIH");
        /// <summary>一级电源控制板</summary>
        public string Enum_BoardType_PPSControlBoard => GetString("Enum.BoardType.PPSControlBoard");
        /// <summary>温度采集板</summary>
        public string Enum_BoardType_TempAcquisitionBoard => GetString("Enum.BoardType.TempAcquisitionBoard");
        /// <summary>BMC</summary>
        public string Enum_BoardType_BMC => GetString("Enum.BoardType.BMC");
        /// <summary>全砖 10V10A</summary>
        public string Enum_BoardType_FB10V10A => GetString("Enum.BoardType.FB10V10A");
        /// <summary>全砖 06V25A</summary>
        public string Enum_BoardType_FB06V25A => GetString("Enum.BoardType.FB06V25A");
        /// <summary>全砖 06V50A</summary>
        public string Enum_BoardType_FB06V50A => GetString("Enum.BoardType.FB06V50A");
        /// <summary>半砖 20V20A</summary>
        public string Enum_BoardType_HB20V20A => GetString("Enum.BoardType.HB20V20A");
        /// <summary>半砖 45V10A</summary>
        public string Enum_BoardType_HB45V10A => GetString("Enum.BoardType.HB45V10A");
        /// <summary>竖向转接板</summary>
        public string Enum_BoardType_VerticalAdapterBoard => GetString("Enum.BoardType.VerticalAdapterBoard");
        /// <summary>老化转接板</summary>
        public string Enum_BoardType_BIBAdapterBoard => GetString("Enum.BoardType.BIBAdapterBoard");
        /// <summary>I/O控制板</summary>
        public string Enum_BoardType_IOControllerBoard => GetString("Enum.BoardType.IOControllerBoard");
        /// <summary>辅助板</summary>
        public string Enum_BoardType_AuxiliaryBoard => GetString("Enum.BoardType.AuxiliaryBoard");
        /// <summary>强电控制板</summary>
        public string Enum_BoardType_HVControlBoard => GetString("Enum.BoardType.HVControlBoard");
        /// <summary>弱电控制板</summary>
        public string Enum_BoardType_LVControlBoard => GetString("Enum.BoardType.LVControlBoard");
        /// <summary>老化板</summary>
        public string Enum_BoardType_BurnInBoard => GetString("Enum.BoardType.BurnInBoard");
        /// <summary>工作中</summary>
        public string Enum_DpsState_Output => GetString("Enum.DpsState.Output");
        /// <summary>过压</summary>
        public string Enum_DpsState_Ovp => GetString("Enum.DpsState.Ovp");
        /// <summary>欠压</summary>
        public string Enum_DpsState_Uvp => GetString("Enum.DpsState.Uvp");
        /// <summary>过流</summary>
        public string Enum_DpsState_Ocp => GetString("Enum.DpsState.Ocp");
        /// <summary>关闭</summary>
        public string Enum_DpsState_Idle => GetString("Enum.DpsState.Idle");
        /// <summary>向量加载错误</summary>
        public string Enum_PatternErrorType_Load => GetString("Enum.PatternErrorType.Load");
        /// <summary>向量解码错误</summary>
        public string Enum_PatternErrorType_Decode => GetString("Enum.PatternErrorType.Decode");
        /// <summary>向量运行超时</summary>
        public string Enum_PatternErrorType_Timeout => GetString("Enum.PatternErrorType.Timeout");
        /// <summary>关闭</summary>
        public string Enum_TriggerOutputType_Close => GetString("Enum.TriggerOutputType.Close");
        /// <summary>信号</summary>
        public string Enum_TriggerOutputType_Signal => GetString("Enum.TriggerOutputType.Signal");
        /// <summary>Sync</summary>
        public string Enum_TriggerOutputType_Sync => GetString("Enum.TriggerOutputType.Sync");
        /// <summary>升级中</summary>
        public string Enum_UpgradeState_Updating => GetString("Enum.UpgradeState.Updating");
        /// <summary>升级完成</summary>
        public string Enum_UpgradeState_Finish => GetString("Enum.UpgradeState.Finish");
        /// <summary>擦除中</summary>
        public string Enum_UpgradeState_Erase => GetString("Enum.UpgradeState.Erase");
        /// <summary>切换板卡失败</summary>
        public string Enum_UpgradeState_SwitchBoardFail => GetString("Enum.UpgradeState.SwitchBoardFail");
        /// <summary>写入更新时间失败</summary>
        public string Enum_UpgradeState_WriteTimeFail => GetString("Enum.UpgradeState.WriteTimeFail");
        /// <summary>读取FLASH ID失败</summary>
        public string Enum_UpgradeState_ReadFlashIdFail => GetString("Enum.UpgradeState.ReadFlashIdFail");
        /// <summary>更新超时</summary>
        public string Enum_UpgradeState_TimeOut => GetString("Enum.UpgradeState.TimeOut");
        /// <summary>其他</summary>
        public string Enum_UpgradeState_Other => GetString("Enum.UpgradeState.Other");
        /// <summary>通信错误</summary>
        public string Enum_UpgradeState_CommunicateFail => GetString("Enum.UpgradeState.CommunicateFail");
        /// <summary>数据校验失败</summary>
        public string Enum_UpgradeState_DataVerifyFail => GetString("Enum.UpgradeState.DataVerifyFail");
        /// <summary>错误</summary>
        public string Enum_UpgradeState_Error => GetString("Enum.UpgradeState.Error");
        /// <summary>红灯</summary>
        public string Enum_LightAlarmState_Red => GetString("Enum.LightAlarmState.Red");
        /// <summary>黄灯</summary>
        public string Enum_LightAlarmState_Yellow => GetString("Enum.LightAlarmState.Yellow");
        /// <summary>绿灯</summary>
        public string Enum_LightAlarmState_Green => GetString("Enum.LightAlarmState.Green");
        /// <summary>报警</summary>
        public string Enum_LightAlarmState_Alarm => GetString("Enum.LightAlarmState.Alarm");
        /// <summary>默认</summary>
        public string Enum_DpsPlanPurpose_Default => GetString("Enum.DpsPlanPurpose.Default");
        /// <summary>上电</summary>
        public string Enum_DpsPlanPurpose_PowerUp => GetString("Enum.DpsPlanPurpose.PowerUp");
        /// <summary>下电</summary>
        public string Enum_DpsPlanPurpose_PowerDown => GetString("Enum.DpsPlanPurpose.PowerDown");
        /// <summary>跟踪</summary>
        public string Enum_LogLevel_Trace => GetString("Enum.LogLevel.Trace");
        /// <summary>调试</summary>
        public string Enum_LogLevel_Debug => GetString("Enum.LogLevel.Debug");
        /// <summary>消息</summary>
        public string Enum_LogLevel_Info => GetString("Enum.LogLevel.Info");
        /// <summary>警告</summary>
        public string Enum_LogLevel_Warn => GetString("Enum.LogLevel.Warn");
        /// <summary>错误</summary>
        public string Enum_LogLevel_Error => GetString("Enum.LogLevel.Error");
        /// <summary>异常</summary>
        public string Enum_LogLevel_Fatal => GetString("Enum.LogLevel.Fatal");
        /// <summary>查询版本</summary>
        public string Enum_HVControlBoardCode_Version => GetString("Enum.HVControlBoardCode.Version");
        /// <summary>同步时间</summary>
        public string Enum_HVControlBoardCode_SyncTime => GetString("Enum.HVControlBoardCode.SyncTime");
        /// <summary>三色报警灯</summary>
        public string Enum_HVControlBoardCode_LightAlarm => GetString("Enum.HVControlBoardCode.LightAlarm");
        /// <summary>烟雾报警器</summary>
        public string Enum_HVControlBoardCode_Smoke => GetString("Enum.HVControlBoardCode.Smoke");
        /// <summary>急停按钮</summary>
        public string Enum_HVControlBoardCode_Emergency => GetString("Enum.HVControlBoardCode.Emergency");
        /// <summary>门的状态</summary>
        public string Enum_HVControlBoardCode_Door => GetString("Enum.HVControlBoardCode.Door");
        /// <summary>强电控制</summary>
        public string Enum_HVControlBoardCode_HighVoltage => GetString("Enum.HVControlBoardCode.HighVoltage");
        /// <summary>固件自检</summary>
        public string Enum_HVControlBoardCode_SelfCheck => GetString("Enum.HVControlBoardCode.SelfCheck");
        /// <summary>检测板 1</summary>
        public string Enum_HVStateErrorType_AC1 => GetString("Enum.HVStateErrorType.AC1");
        /// <summary>检测板 2</summary>
        public string Enum_HVStateErrorType_AC2 => GetString("Enum.HVStateErrorType.AC2");
        /// <summary>检测板 3</summary>
        public string Enum_HVStateErrorType_AC3 => GetString("Enum.HVStateErrorType.AC3");
        /// <summary>检测板 4</summary>
        public string Enum_HVStateErrorType_AC4 => GetString("Enum.HVStateErrorType.AC4");
        /// <summary>检测板 5</summary>
        public string Enum_HVStateErrorType_AC5 => GetString("Enum.HVStateErrorType.AC5");
        /// <summary>检测板 6</summary>
        public string Enum_HVStateErrorType_AC6 => GetString("Enum.HVStateErrorType.AC6");
        /// <summary>关闭</summary>
        public string Enum_HVState_Close => GetString("Enum.HVState.Close");
        /// <summary>上电中</summary>
        public string Enum_HVState_PowerUp => GetString("Enum.HVState.PowerUp");
        /// <summary>下电中</summary>
        public string Enum_HVState_PowerDown => GetString("Enum.HVState.PowerDown");
        /// <summary>开启</summary>
        public string Enum_HVState_Open => GetString("Enum.HVState.Open");
        /// <summary>离线</summary>
        public string Enum_HVState_Offline => GetString("Enum.HVState.Offline");
        /// <summary>错误</summary>
        public string Enum_HVState_Error => GetString("Enum.HVState.Error");
        /// <summary>设备门</summary>
        public string Enum_DoorType_Default => GetString("Enum.DoorType.Default");
        /// <summary>烘箱门</summary>
        public string Enum_DoorType_Oven => GetString("Enum.DoorType.Oven");
        /// <summary>强电门</summary>
        public string Enum_DoorType_Power => GetString("Enum.DoorType.Power");
        /// <summary>通讯状态</summary>
        public string Enum_FirmwareSelfCheckItem_ConnectionState => GetString("Enum.FirmwareSelfCheckItem.ConnectionState");
        /// <summary>环境变量</summary>
        public string Enum_FirmwareSelfCheckItem_EnviormentVariables => GetString("Enum.FirmwareSelfCheckItem.EnviormentVariables");
        /// <summary>配置文件</summary>
        public string Enum_FirmwareSelfCheckItem_ConfigFiles => GetString("Enum.FirmwareSelfCheckItem.ConfigFiles");
        /// <summary>输出电压</summary>
        public string Enum_FirmwareSelfCheckItem_OutputVoltage => GetString("Enum.FirmwareSelfCheckItem.OutputVoltage");
        /// <summary>输入电压</summary>
        public string Enum_FirmwareSelfCheckItem_InputVoltage => GetString("Enum.FirmwareSelfCheckItem.InputVoltage");
        /// <summary>输出电流</summary>
        public string Enum_FirmwareSelfCheckItem_OutputCurrent => GetString("Enum.FirmwareSelfCheckItem.OutputCurrent");
        /// <summary>输入电流</summary>
        public string Enum_FirmwareSelfCheckItem_InputCurrent => GetString("Enum.FirmwareSelfCheckItem.InputCurrent");
        /// <summary>输出温度</summary>
        public string Enum_FirmwareSelfCheckItem_OutputTemperature => GetString("Enum.FirmwareSelfCheckItem.OutputTemperature");
        /// <summary>扩展IO</summary>
        public string Enum_FirmwareSelfCheckItem_ExpandIO => GetString("Enum.FirmwareSelfCheckItem.ExpandIO");
        /// <summary>数模转换</summary>
        public string Enum_FirmwareSelfCheckItem_DAC => GetString("Enum.FirmwareSelfCheckItem.DAC");
        /// <summary>FPGA</summary>
        public string Enum_FirmwareSelfCheckItem_FPGA => GetString("Enum.FirmwareSelfCheckItem.FPGA");
        /// <summary>闪存</summary>
        public string Enum_FirmwareSelfCheckItem_Flash => GetString("Enum.FirmwareSelfCheckItem.Flash");
        /// <summary>模数转换</summary>
        public string Enum_FirmwareSelfCheckItem_ADC => GetString("Enum.FirmwareSelfCheckItem.ADC");
        /// <summary>差分信号</summary>
        public string Enum_FirmwareSelfCheckItem_LVDS => GetString("Enum.FirmwareSelfCheckItem.LVDS");
        /// <summary>EEPROM</summary>
        public string Enum_FirmwareSelfCheckItem_EEPROM => GetString("Enum.FirmwareSelfCheckItem.EEPROM");
        /// <summary>读寄存器</summary>
        public string Enum_FirmwareSelfCheckItem_ReadRegister => GetString("Enum.FirmwareSelfCheckItem.ReadRegister");
        /// <summary>写寄存器</summary>
        public string Enum_FirmwareSelfCheckItem_WriteRegister => GetString("Enum.FirmwareSelfCheckItem.WriteRegister");
        /// <summary>通过</summary>
        public string Enum_FirmwareSelfCheckResult_Pass => GetString("Enum.FirmwareSelfCheckResult.Pass");
        /// <summary>通讯异常</summary>
        public string Enum_FirmwareSelfCheckResult_ConnectionError => GetString("Enum.FirmwareSelfCheckResult.ConnectionError");
        /// <summary>环境变量丢失</summary>
        public string Enum_FirmwareSelfCheckResult_EnviormentVariablesLost => GetString("Enum.FirmwareSelfCheckResult.EnviormentVariablesLost");
        /// <summary>配置文件丢失</summary>
        public string Enum_FirmwareSelfCheckResult_ConfigFilesLost => GetString("Enum.FirmwareSelfCheckResult.ConfigFilesLost");
        /// <summary>配置文件错误</summary>
        public string Enum_FirmwareSelfCheckResult_ConfigFilesError => GetString("Enum.FirmwareSelfCheckResult.ConfigFilesError");
        /// <summary>电压过高</summary>
        public string Enum_FirmwareSelfCheckResult_VoltageTooHigh => GetString("Enum.FirmwareSelfCheckResult.VoltageTooHigh");
        /// <summary>电压过低</summary>
        public string Enum_FirmwareSelfCheckResult_VoltageTooLow => GetString("Enum.FirmwareSelfCheckResult.VoltageTooLow");
        /// <summary>电流过高</summary>
        public string Enum_FirmwareSelfCheckResult_CurrentTooHigh => GetString("Enum.FirmwareSelfCheckResult.CurrentTooHigh");
        /// <summary>电流过低</summary>
        public string Enum_FirmwareSelfCheckResult_CurrentTooLow => GetString("Enum.FirmwareSelfCheckResult.CurrentTooLow");
        /// <summary>温度过高</summary>
        public string Enum_FirmwareSelfCheckResult_TemperatureTooHigh => GetString("Enum.FirmwareSelfCheckResult.TemperatureTooHigh");
        /// <summary>温度过低</summary>
        public string Enum_FirmwareSelfCheckResult_TemperatureTooLow => GetString("Enum.FirmwareSelfCheckResult.TemperatureTooLow");
        /// <summary>内存地址错误</summary>
        public string Enum_FirmwareSelfCheckResult_MemoryAddressError => GetString("Enum.FirmwareSelfCheckResult.MemoryAddressError");
        /// <summary>数据读取错误</summary>
        public string Enum_FirmwareSelfCheckResult_DataReadError => GetString("Enum.FirmwareSelfCheckResult.DataReadError");
        /// <summary>数据写入错误</summary>
        public string Enum_FirmwareSelfCheckResult_DataWriteError => GetString("Enum.FirmwareSelfCheckResult.DataWriteError");
        /// <summary>位于左侧</summary>
        public string Enum_TriggerVerticalDirection_Left => GetString("Enum.TriggerVerticalDirection.Left");
        /// <summary>位于右侧</summary>
        public string Enum_TriggerVerticalDirection_Right => GetString("Enum.TriggerVerticalDirection.Right");
        /// <summary>未捕获到的异常, {0}</summary>
        public string UnhandledExceptionMessage => GetString("UnhandledExceptionMessage");
        /// <summary>程序已运行</summary>
        public string AppHasLaunchedMessage => GetString("AppHasLaunchedMessage");
        /// <summary>程序启动</summary>
        public string AppStartMessage => GetString("AppStartMessage");
        /// <summary>程序退出</summary>
        public string AppExitMessage => GetString("AppExitMessage");
        /// <summary>Config.ini 配置文件已发生修改。上次文件修改日期：{0}，当前文件修改日期：{1}</summary>
        public string ConfigModifiedMessage => GetString("ConfigModifiedMessage");
        /// <summary>加密锁和设备不匹配, 请联系售后人员</summary>
        public string IllegalDongleMessage => GetString("IllegalDongleMessage");
        /// <summary>设备授权已到期 [ {0:yyyy/MM/dd HH:mm:ss} ], 请联系售后人员</summary>
        public string DongleExpireMessage => GetString("DongleExpireMessage");
        /// <summary>文件导入成功</summary>
        public string FileImportSuccessMessage => GetString("FileImportSuccessMessage");
        /// <summary>文件导入失败</summary>
        public string FileImportFailureMessage => GetString("FileImportFailureMessage");
        /// <summary>文件导出成功</summary>
        public string FileExportSuccessMessage => GetString("FileExportSuccessMessage");
        /// <summary>文件导出失败</summary>
        public string FileExportFailureMessage => GetString("FileExportFailureMessage");
        /// <summary>文件创建成功</summary>
        public string FileCreatedSuccessMessage => GetString("FileCreatedSuccessMessage");
        /// <summary>文件创建失败</summary>
        public string FileCreatedFailureMessage => GetString("FileCreatedFailureMessage");
        /// <summary>文件删除成功</summary>
        public string FileDeletedSuccessMessage => GetString("FileDeletedSuccessMessage");
        /// <summary>文件删除失败</summary>
        public string FileDeletedFailureMessage => GetString("FileDeletedFailureMessage");
        /// <summary>文件不存在</summary>
        public string FileNotExistMessage => GetString("FileNotExistMessage");
        /// <summary>文件已存在</summary>
        public string FileExistMessage => GetString("FileExistMessage");
        /// <summary>文件保存成功</summary>
        public string FileSaveSuccessMessage => GetString("FileSaveSuccessMessage");
        /// <summary>文件保存失败</summary>
        public string FileSaveFailureMessage => GetString("FileSaveFailureMessage");
        /// <summary>文件另存成功</summary>
        public string FileSaveAsSuccessMessage => GetString("FileSaveAsSuccessMessage");
        /// <summary>文件另存失败</summary>
        public string FileSaveAsFailureMessage => GetString("FileSaveAsFailureMessage");
        /// <summary>文件刷新成功</summary>
        public string FileRefreshSuccessMessage => GetString("FileRefreshSuccessMessage");
        /// <summary>文件刷新失败</summary>
        public string FileRefreshFailureMessage => GetString("FileRefreshFailureMessage");
        /// <summary>{0} 区初始化失败，{1}</summary>
        public string ZoneInitFailureMessage => GetString("ZoneInitFailureMessage");
        /// <summary>确定删除所选项目?</summary>
        public string DeleteAskMessage => GetString("DeleteAskMessage");
        /// <summary>器件库可能包含重要的数据文件, 是否删除?</summary>
        public string DeleteIcLibraryMessage => GetString("DeleteIcLibraryMessage");
        /// <summary>已存在同名器件库, 是否重命名?</summary>
        public string ImportIcLibraryExistMessage => GetString("ImportIcLibraryExistMessage");
        /// <summary>已存在同名器件文件, 是否覆盖?</summary>
        public string ImportIcFileExistMessage => GetString("ImportIcFileExistMessage");
        /// <summary>器件 [ {0} ] 已被修改, 是否保存当前修改</summary>
        public string IcFileUnsavedMessage => GetString("IcFileUnsavedMessage");
        /// <summary>器件 [ {0} ] 不存在</summary>
        public string IcFileNotExistMessage => GetString("IcFileNotExistMessage");
        /// <summary>器件 [ {0} ] 正在执行老化, 请停止老化后重试</summary>
        public string IcFileInUsedMessage => GetString("IcFileInUsedMessage");
        /// <summary>器件库 [ {0} ] 中的器件正在执行老化, 请停止老化后重试</summary>
        public string IcLibraryInUsedMessage => GetString("IcLibraryInUsedMessage");
        /// <summary>器件库 [ {0} ] 不存在器件 [ {1} ]</summary>
        public string IcLibraryNotExistIcFileMessage => GetString("IcLibraryNotExistIcFileMessage");
        /// <summary>器件库 [ {0} ] 不存在</summary>
        public string IcLibraryNotExistMessage => GetString("IcLibraryNotExistMessage");
        /// <summary>器件 [ {0} ] 已被修改，状态将被重置到空闲，请重新选择器件</summary>
        public string IcFileModifiedReselectMessage => GetString("IcFileModifiedReselectMessage");
        /// <summary>器件 [ {0} ] 已被删除，状态将被重置到空闲，请重新选择器件</summary>
        public string IcFileDeletedReselectMessage => GetString("IcFileDeletedReselectMessage");
        /// <summary>名称不可以为空或包含空格</summary>
        public string NameNotNullMessage => GetString("NameNotNullMessage");
        /// <summary>名称不可以超过 {0} 位长度</summary>
        public string NameNotExceedMessage => GetString("NameNotExceedMessage");
        /// <summary>名称不能包含下列任何字符 \ / : * ? . " > < |</summary>
        public string NameIllegalMessage => GetString("NameIllegalMessage");
        /// <summary>名称已存在</summary>
        public string NameExistMessage => GetString("NameExistMessage");
        /// <summary>设备配置文件解析失败, {0}</summary>
        public string DeviceConfigParseFailureMessage => GetString("DeviceConfigParseFailureMessage");
        /// <summary>初始化中...</summary>
        public string InitMessage => GetString("InitMessage");
        /// <summary>初始化完成</summary>
        public string InitEndMessage => GetString("InitEndMessage");
        /// <summary>老化过程中不允许关闭</summary>
        public string ClosingAppWhenAnyZoneRunMessage => GetString("ClosingAppWhenAnyZoneRunMessage");
        /// <summary>值必须介于 {0}</summary>
        public string MustBeInRangMessage => GetString("MustBeInRangMessage");
        /// <summary>下电方案的设置电压必须设置为 0</summary>
        public string PowerDownValueMustZeroMessage => GetString("PowerDownValueMustZeroMessage");
        /// <summary>仅支持英文字母、数字和下划线</summary>
        public string ValidStringRuleMessage => GetString("ValidStringRuleMessage");
        /// <summary>文件 [ {0} ] 解析失败, 第 {1} 行 {2} 列</summary>
        public string FileParseErrorMessage => GetString("FileParseErrorMessage");
        /// <summary>文件 [ {0} ] 长度不正确, 预期长度为 {1} 字节</summary>
        public string FileLengthErrorMessage => GetString("FileLengthErrorMessage");
        /// <summary>ABM 文件 [ {0} ] 不匹配，请使用该上位机软件中的向量转换工具生成的 ABM 文件</summary>
        public string AbmFileNotMatchMessage => GetString("AbmFileNotMatchMessage");
        /// <summary>检测到循环配置，移动步次顺序会清除所有循环配置, 是否继续?</summary>
        public string AskMoveStepMessage => GetString("AskMoveStepMessage");
        /// <summary>检测到循环配置，删除任意步次会清除所有循环配置, 是否继续?</summary>
        public string AskDeleteStepMessage => GetString("AskDeleteStepMessage");
        /// <summary>检测到烟雾，所有老化即将停止，随后强电将关闭，消除烟雾报警状态后才可重新开启强电 !</summary>
        public string SmokyPromptMessage => GetString("SmokyPromptMessage");
        /// <summary>检测到烘箱离线，请立即检查烘箱通信状态 !</summary>
        public string OvenOfflinePromptMessage => GetString("OvenOfflinePromptMessage");
        /// <summary>检测到{0} [ {1} ] 已打开，请立即检查{2}状态 !</summary>
        public string DoorOpenedPromptMessage => GetString("DoorOpenedPromptMessage");
        /// <summary>{0} [ {1} ] 已关闭 !</summary>
        public string DoorClosedPromptMessage => GetString("DoorClosedPromptMessage");
        /// <summary>检测到烘箱异常报警 [ {0} ]，请立即检查烘箱状态 !</summary>
        public string OvenErrorPromptMessage => GetString("OvenErrorPromptMessage");
        /// <summary>检测到急停按钮 [ {0} ] 被按下，请确认是否人为操作 !</summary>
        public string EmoPressedPromptMessage => GetString("EmoPressedPromptMessage");
        /// <summary>是否执行 [ 停止烘箱 ] 操作 ?</summary>
        public string AskOvenPowerOffMessage => GetString("AskOvenPowerOffMessage");
        /// <summary>是否执行 [ 一键降温 ] 操作 ?</summary>
        public string AskOvenOneClickCoolMessage => GetString("AskOvenOneClickCoolMessage");
        /// <summary>是否执行 [ 开锁 ] 操作 ?</summary>
        public string AskOvenDoorUnlockMessage => GetString("AskOvenDoorUnlockMessage");
        /// <summary>自检错误，请联系售后人员</summary>
        public string DeviceCheckFailMessage => GetString("DeviceCheckFailMessage");
        /// <summary>报文解析错误，{0}</summary>
        public string AnalysisFrameErrorMessage => GetString("AnalysisFrameErrorMessage");
        /// <summary>CRC32不匹配，{0}</summary>
        public string Crc32NotMatchMessage => GetString("Crc32NotMatchMessage");
        /// <summary>帧尾不匹配</summary>
        public string FrameEndingNotMatchMessage => GetString("FrameEndingNotMatchMessage");
        /// <summary>老化板插入</summary>
        public string PlugedMessage => GetString("PlugedMessage");
        /// <summary>老化板卸下</summary>
        public string UnplugedMessage => GetString("UnplugedMessage");
        /// <summary>请设置 [ {0} ]</summary>
        public string PleaseSetMessage => GetString("PleaseSetMessage");
        /// <summary>至少需要配置一个步次</summary>
        public string AtleastOneStepMessage => GetString("AtleastOneStepMessage");
        /// <summary>至少需要配置一个子步次</summary>
        public string AtleastOneSubStepMessage => GetString("AtleastOneSubStepMessage");
        /// <summary>至少需要配置一个启用的电源</summary>
        public string AtleastEnableOneDpsMessage => GetString("AtleastEnableOneDpsMessage");
        /// <summary>当前状态 [ {0} ]</summary>
        public string StateMessage => GetString("StateMessage");
        /// <summary>向量 [ {0} ] 运行错误, 请检查步次 [ {1} ], 子步次 [ {2} ] 中的向量配置</summary>
        public string PatternErrorMessage => GetString("PatternErrorMessage");
        /// <summary>设备异常; 请联系售后人员! 错误码 [ {0} ]</summary>
        public string DeviceErrorMessage => GetString("DeviceErrorMessage");
        /// <summary>器件配置错误, 请检查后重试! {0}</summary>
        public string IcErrorMessage => GetString("IcErrorMessage");
        /// <summary>发送器件数据 [ {0} ]</summary>
        public string SendIcDataMessage => GetString("SendIcDataMessage");
        /// <summary>当前步次 [ {0} ] ; 子步次 [ {1} ]</summary>
        public string CurrentStepMessage => GetString("CurrentStepMessage");
        /// <summary>BMC供电异常, {0} {1}V</summary>
        public string BmcPowerErrorMessage => GetString("BmcPowerErrorMessage");
        /// <summary>一级电源异常, PPS {0} {1}</summary>
        public string PrePowerErrorMessage => GetString("PrePowerErrorMessage");
        /// <summary>检测到烟雾，老化已停止</summary>
        public string StopOnSmokyMeesage => GetString("StopOnSmokyMeesage");
        /// <summary>检测到烘箱异常，老化已停止</summary>
        public string StopOnOvenErrorMessage => GetString("StopOnOvenErrorMessage");
        /// <summary>检测到急停按钮被按下，老化已停止</summary>
        public string StopOnEmergencyMessage => GetString("StopOnEmergencyMessage");
        /// <summary>检测到烘箱门 [ {0} ] 被打开，老化已暂停</summary>
        public string SuspendOnOvenDoorOpenMessage => GetString("SuspendOnOvenDoorOpenMessage");
        /// <summary>检测到烘箱离线，老化已暂停</summary>
        public string SuspendOnOvenOfflineMessage => GetString("SuspendOnOvenOfflineMessage");
        /// <summary>检测到强电门 [ {0} ] 被打开，老化已暂停</summary>
        public string SuspendOnPowerDoorOpenMessage => GetString("SuspendOnPowerDoorOpenMessage");
        /// <summary>检测到设备门 [ {0} ] 被打开，老化已暂停</summary>
        public string SuspendOnDefaultDoorOpenMessage => GetString("SuspendOnDefaultDoorOpenMessage");
        /// <summary>三色报警灯 [ {0} ]</summary>
        public string LightAlarmMessage => GetString("LightAlarmMessage");
        /// <summary>系统供电异常: {0} {1}V</summary>
        public string SystemPowerErrorMessage => GetString("SystemPowerErrorMessage");
        /// <summary>确认终止所有区的老化吗?</summary>
        public string AbortAllZonesBurnAskMessage => GetString("AbortAllZonesBurnAskMessage");
        /// <summary>门 [ {0}] 的类型与配置文件中不匹配，请检查配置文件</summary>
        public string DoorTypeNotMatchMessage => GetString("DoorTypeNotMatchMessage");
        /// <summary>确认要开启强电吗？</summary>
        public string OpenHighVoltageAskMessage => GetString("OpenHighVoltageAskMessage");
        /// <summary>确认要关闭强电吗？</summary>
        public string CloseHighVoltageAskMessage => GetString("CloseHighVoltageAskMessage");
        /// <summary>急停按钮 [ {0} ] 被按下</summary>
        public string EMOIsPressedMessage => GetString("EMOIsPressedMessage");
        /// <summary>[ {0} ] 已被打开</summary>
        public string DoorIsOpenedMessage => GetString("DoorIsOpenedMessage");
        /// <summary>有老化区处于老化过程中不允许升级固件</summary>
        public string UpgradeFirmwareWhenAnyZoneRunMessage => GetString("UpgradeFirmwareWhenAnyZoneRunMessage");
        /// <summary>当前日志文件未找到对应的日志规则，无法查看。请检查配置文件或将该文件放到正确的路径下。</summary>
        public string LogRuleNotFoundErrorMessage => GetString("LogRuleNotFoundErrorMessage");
        /// <summary>当前日志文件与已经添加的日志文件规则不一致，无法合并查看。可以清除已经添加的日志文件后重试。</summary>
        public string LogRuleNotSameErrorMessage => GetString("LogRuleNotSameErrorMessage");
        /// <summary>当前日志文件所在文件夹与已经添加的日志文件所在文件夹不一致，无法合并查看。可以清除已经添加的日志文件后重试。</summary>
        public string LogFolderNotSameErrorMessage => GetString("LogFolderNotSameErrorMessage");
        /// <summary>指令 {0} 请求失败。{1}</summary>
        public string OvenCommandErrorMessage => GetString("OvenCommandErrorMessage");
        /// <summary>指令 {0} 请求成功。{1}</summary>
        public string OvenCommandSuccessMessage => GetString("OvenCommandSuccessMessage");
        /// <summary>连接成功</summary>
        public string ConnectedMessage => GetString("ConnectedMessage");
        /// <summary>断开连接</summary>
        public string DisconnectedMessage => GetString("DisconnectedMessage");
        /// <summary>自检信息：{0}</summary>
        public string FirmwareSelfCheckMessage => GetString("FirmwareSelfCheckMessage");
        /// <summary>工作线程已启动!</summary>
        public string WorkerThreadStartedMessage => GetString("WorkerThreadStartedMessage");
        /// <summary>工作线程已退出!</summary>
        public string WorkerThreadEndedMessage => GetString("WorkerThreadEndedMessage");
        /// <summary>见微知著·科技赋能·创造非凡价值</summary>
        public string ConceptText => GetString("ConceptText");
        /// <summary>许可认证</summary>
        public string LicenseCertificationText => GetString("LicenseCertificationText");
        /// <summary>机器码</summary>
        public string MachineCodeText => GetString("MachineCodeText");
        /// <summary>管理员密码</summary>
        public string AdminPasswordText => GetString("AdminPasswordText");
        /// <summary>密码错误</summary>
        public string InvalidPasswordText => GetString("InvalidPasswordText");
        /// <summary>切换语言</summary>
        public string SwitchLanguageText => GetString("SwitchLanguageText");
        /// <summary>语言</summary>
        public string LanguageText => GetString("LanguageText");
        /// <summary>中文</summary>
        public string ChineseText => GetString("ChineseText");
        /// <summary>English</summary>
        public string EnglishText => GetString("EnglishText");
        /// <summary>区</summary>
        public string ZoneText => GetString("ZoneText");
        /// <summary>{0} 区</summary>
        public string ZoneFormatText => GetString("ZoneFormatText");
        /// <summary>状态</summary>
        public string StateText => GetString("StateText");
        /// <summary>类型</summary>
        public string TypeText => GetString("TypeText");
        /// <summary>规格</summary>
        public string SpecText => GetString("SpecText");
        /// <summary>系统(_S)</summary>
        public string SystemMenuText => GetString("SystemMenuText");
        /// <summary>模式</summary>
        public string ModeText => GetString("ModeText");
        /// <summary>设置</summary>
        public string SettingText => GetString("SettingText");
        /// <summary>数据(_D)</summary>
        public string DataMenuText => GetString("DataMenuText");
        /// <summary>帮助(_H)</summary>
        public string HelpMenuText => GetString("HelpMenuText");
        /// <summary>控制(_C)</summary>
        public string ControlMenuText => GetString("ControlMenuText");
        /// <summary>弱电控制板</summary>
        public string LVControlBoardText => GetString("LVControlBoardText");
        /// <summary>确认</summary>
        public string ConfirmText => GetString("ConfirmText");
        /// <summary>取消</summary>
        public string CancelText => GetString("CancelText");
        /// <summary>启动</summary>
        public string StartText => GetString("StartText");
        /// <summary>停止</summary>
        public string StopText => GetString("StopText");
        /// <summary>开启</summary>
        public string OpenText => GetString("OpenText");
        /// <summary>关闭</summary>
        public string CloseText => GetString("CloseText");
        /// <summary>已锁闭</summary>
        public string LockedText => GetString("LockedText");
        /// <summary>未落锁</summary>
        public string UnlockedText => GetString("UnlockedText");
        /// <summary>锁门</summary>
        public string LockDoorText => GetString("LockDoorText");
        /// <summary>开锁</summary>
        public string OpenLockText => GetString("OpenLockText");
        /// <summary>检测到烟雾</summary>
        public string DetechedSmokeText => GetString("DetechedSmokeText");
        /// <summary>烘箱启动设置</summary>
        public string OvenStartupSettingText => GetString("OvenStartupSettingText");
        /// <summary>烘箱开门时自动暂停老化</summary>
        public string SuspendWhenOvenDoorOpenedText => GetString("SuspendWhenOvenDoorOpenedText");
        /// <summary>烘箱断连时自动暂停老化</summary>
        public string SuspendWhenOvenOfflineText => GetString("SuspendWhenOvenOfflineText");
        /// <summary>正常</summary>
        public string NormalText => GetString("NormalText");
        /// <summary>下限</summary>
        public string FloorText => GetString("FloorText");
        /// <summary>上限</summary>
        public string CeilingText => GetString("CeilingText");
        /// <summary>新建</summary>
        public string CreateNewText => GetString("CreateNewText");
        /// <summary>重命名</summary>
        public string RenameText => GetString("RenameText");
        /// <summary>保存</summary>
        public string SaveText => GetString("SaveText");
        /// <summary>另存为</summary>
        public string SaveAsText => GetString("SaveAsText");
        /// <summary>删除</summary>
        public string DeleteText => GetString("DeleteText");
        /// <summary>导入</summary>
        public string ImportText => GetString("ImportText");
        /// <summary>导出</summary>
        public string ExportText => GetString("ExportText");
        /// <summary>查看</summary>
        public string ViewText => GetString("ViewText");
        /// <summary>测试</summary>
        public string TestText => GetString("TestText");
        /// <summary>编辑</summary>
        public string EditText => GetString("EditText");
        /// <summary>名称</summary>
        public string NameText => GetString("NameText");
        /// <summary>上移</summary>
        public string MoveUpText => GetString("MoveUpText");
        /// <summary>下移</summary>
        public string MoveDownText => GetString("MoveDownText");
        /// <summary>复制</summary>
        public string CopyText => GetString("CopyText");
        /// <summary>粘贴</summary>
        public string PasteText => GetString("PasteText");
        /// <summary>清空</summary>
        public string ClearText => GetString("ClearText");
        /// <summary>发送</summary>
        public string SendText => GetString("SendText");
        /// <summary>是</summary>
        public string YesText => GetString("YesText");
        /// <summary>否</summary>
        public string NoText => GetString("NoText");
        /// <summary>提示</summary>
        public string TipsText => GetString("TipsText");
        /// <summary>成功</summary>
        public string SuccessText => GetString("SuccessText");
        /// <summary>失败</summary>
        public string FailureText => GetString("FailureText");
        /// <summary>关于</summary>
        public string AboutText => GetString("AboutText");
        /// <summary>日志</summary>
        public string LogText => GetString("LogText");
        /// <summary>全选</summary>
        public string AllSelectText => GetString("AllSelectText");
        /// <summary>在线</summary>
        public string OnlineText => GetString("OnlineText");
        /// <summary>离线</summary>
        public string OfflineText => GetString("OfflineText");
        /// <summary>使能</summary>
        public string EnableText => GetString("EnableText");
        /// <summary>器件名称</summary>
        public string IcNameText => GetString("IcNameText");
        /// <summary>器件编辑</summary>
        public string IcEditText => GetString("IcEditText");
        /// <summary>当前器件</summary>
        public string CurrentIcText => GetString("CurrentIcText");
        /// <summary>数据查看</summary>
        public string DataViewText => GetString("DataViewText");
        /// <summary>向量格式转换</summary>
        public string PatternFormatConvertText => GetString("PatternFormatConvertText");
        /// <summary>曲线查看</summary>
        public string ChartViewText => GetString("ChartViewText");
        /// <summary>触发设置</summary>
        public string TriggerSetText => GetString("TriggerSetText");
        /// <summary>输出模式</summary>
        public string OutputModeText => GetString("OutputModeText");
        /// <summary>输出通道</summary>
        public string OutputPinText => GetString("OutputPinText");
        /// <summary>触发输出模式</summary>
        public string TriggerOutputModeText => GetString("TriggerOutputModeText");
        /// <summary>触发输出通道</summary>
        public string TriggerOutputPinText => GetString("TriggerOutputPinText");
        /// <summary>发送</summary>
        public string SendParameterText => GetString("SendParameterText");
        /// <summary>开始</summary>
        public string StartBurnText => GetString("StartBurnText");
        /// <summary>暂停</summary>
        public string SuspendBurnText => GetString("SuspendBurnText");
        /// <summary>继续</summary>
        public string ContinueBurnText => GetString("ContinueBurnText");
        /// <summary>终止</summary>
        public string StopBurnText => GetString("StopBurnText");
        /// <summary>进入老化界面</summary>
        public string EnterBurnViewText => GetString("EnterBurnViewText");
        /// <summary>重置</summary>
        public string ReSelectIcText => GetString("ReSelectIcText");
        /// <summary>关闭报警</summary>
        public string CloseAlarmText => GetString("CloseAlarmText");
        /// <summary>烘箱控制</summary>
        public string OvenControlText => GetString("OvenControlText");
        /// <summary>实时温度</summary>
        public string RealTemperatureText => GetString("RealTemperatureText");
        /// <summary>降温方式</summary>
        public string CoolingModeText => GetString("CoolingModeText");
        /// <summary>试验箱进水温度</summary>
        public string TestChamberInTemperatureText => GetString("TestChamberInTemperatureText");
        /// <summary>试验箱出水温度</summary>
        public string TestChamberOutTemperatureText => GetString("TestChamberOutTemperatureText");
        /// <summary>机械室进水温度</summary>
        public string MachineRoomInTemperatureText => GetString("MachineRoomInTemperatureText");
        /// <summary>机械室出水温度</summary>
        public string MachineRoomOutTemperatureText => GetString("MachineRoomOutTemperatureText");
        /// <summary>驱动室温度</summary>
        public string DriveRoomTemperatureText => GetString("DriveRoomTemperatureText");
        /// <summary>环温</summary>
        public string AmbientTemperatureText => GetString("AmbientTemperatureText");
        /// <summary>换向器1温度</summary>
        public string Commutator1TemperatureText => GetString("Commutator1TemperatureText");
        /// <summary>换向器2温度</summary>
        public string Commutator2TemperatureText => GetString("Commutator2TemperatureText");
        /// <summary>驱动室湿度</summary>
        public string DriveRoomHumidityText => GetString("DriveRoomHumidityText");
        /// <summary>氮气流量</summary>
        public string NitrogenFlowText => GetString("NitrogenFlowText");
        /// <summary>氮气压力</summary>
        public string NitrogenPressureText => GetString("NitrogenPressureText");
        /// <summary>进水压力</summary>
        public string InletWaterPressureText => GetString("InletWaterPressureText");
        /// <summary>出水压力</summary>
        public string OutletWaterPressureText => GetString("OutletWaterPressureText");
        /// <summary>驱动室比例阀开度</summary>
        public string DriveRoomProportionalValveOpeningText => GetString("DriveRoomProportionalValveOpeningText");
        /// <summary>温度</summary>
        public string TemperatureText => GetString("TemperatureText");
        /// <summary>温度(℃)</summary>
        public string TemperatureWithUnitText => GetString("TemperatureWithUnitText");
        /// <summary>目标温度</summary>
        public string TargetTemperatureText => GetString("TargetTemperatureText");
        /// <summary>目标温度(℃)</summary>
        public string TargetTemperatureWithUnitText => GetString("TargetTemperatureWithUnitText");
        /// <summary>当前温度</summary>
        public string CurrentTemperatureText => GetString("CurrentTemperatureText");
        /// <summary>当前温度(℃)</summary>
        public string CurrentTemperatureWithUnitText => GetString("CurrentTemperatureWithUnitText");
        /// <summary>温度下限</summary>
        public string FloorTemperatureText => GetString("FloorTemperatureText");
        /// <summary>温度上限</summary>
        public string CeilTemperatureText => GetString("CeilTemperatureText");
        /// <summary>烘箱门</summary>
        public string OvenDoorText => GetString("OvenDoorText");
        /// <summary>烘箱锁</summary>
        public string OvenLockerText => GetString("OvenLockerText");
        /// <summary>报警信息</summary>
        public string AlarmMessageText => GetString("AlarmMessageText");
        /// <summary>报警窗口</summary>
        public string AlarmWindowText => GetString("AlarmWindowText");
        /// <summary>检测到设备异常报警，请立即处理</summary>
        public string AlarmWindowTitleText => GetString("AlarmWindowTitleText");
        /// <summary>关闭烘箱报警</summary>
        public string CloseOvenAlarmText => GetString("CloseOvenAlarmText");
        /// <summary>一键降温</summary>
        public string OneClickCoolText => GetString("OneClickCoolText");
        /// <summary>报警解除</summary>
        public string ClearAlarmText => GetString("ClearAlarmText");
        /// <summary>自动锁门温度</summary>
        public string AutoDoorLockTempText => GetString("AutoDoorLockTempText");
        /// <summary>器件库</summary>
        public string IcLibraryText => GetString("IcLibraryText");
        /// <summary>选择老化器件</summary>
        public string IcSelectText => GetString("IcSelectText");
        /// <summary>运行策略</summary>
        public string RunStrategyText => GetString("RunStrategyText");
        /// <summary>运行模式</summary>
        public string RunModeText => GetString("RunModeText");
        /// <summary>运行设置</summary>
        public string RunSetText => GetString("RunSetText");
        /// <summary>按时间运行</summary>
        public string RunByTimeText => GetString("RunByTimeText");
        /// <summary>按次数运行</summary>
        public string RunByTimesText => GetString("RunByTimesText");
        /// <summary>循环策略</summary>
        public string LoopStrategyText => GetString("LoopStrategyText");
        /// <summary>位于循环中</summary>
        public string InLoopText => GetString("InLoopText");
        /// <summary>循环运行中</summary>
        public string RunInTheLoopText => GetString("RunInTheLoopText");
        /// <summary>启用循环</summary>
        public string EnableLoopText => GetString("EnableLoopText");
        /// <summary>循环到目标步次</summary>
        public string CycleToTargetStepText => GetString("CycleToTargetStepText");
        /// <summary>数据记录间隔</summary>
        public string LogIntervalText => GetString("LogIntervalText");
        /// <summary>报错记录间隔</summary>
        public string ErrorLogIntervalText => GetString("ErrorLogIntervalText");
        /// <summary>批次号</summary>
        public string LotText => GetString("LotText");
        /// <summary>操作员</summary>
        public string OperatorText => GetString("OperatorText");
        /// <summary>实时数据</summary>
        public string RealDataText => GetString("RealDataText");
        /// <summary>电源</summary>
        public string DpsText => GetString("DpsText");
        /// <summary>设置电压</summary>
        public string SetVoltageText => GetString("SetVoltageText");
        /// <summary>设置电压(V)</summary>
        public string SetVoltageWithUnitText => GetString("SetVoltageWithUnitText");
        /// <summary>实时电压</summary>
        public string RealVoltageText => GetString("RealVoltageText");
        /// <summary>实时电压(V)</summary>
        public string RealVoltageWithUnitText => GetString("RealVoltageWithUnitText");
        /// <summary>输入电压</summary>
        public string InputVoltageText => GetString("InputVoltageText");
        /// <summary>输入电压(V)</summary>
        public string InputVoltageWithUnitText => GetString("InputVoltageWithUnitText");
        /// <summary>输入电流</summary>
        public string InputCurrentText => GetString("InputCurrentText");
        /// <summary>输入电流(A)</summary>
        public string InputCurrentWithUnitText => GetString("InputCurrentWithUnitText");
        /// <summary>输出电压</summary>
        public string OutputVoltageText => GetString("OutputVoltageText");
        /// <summary>输出电压(V)</summary>
        public string OutputVoltageWithUnitText => GetString("OutputVoltageWithUnitText");
        /// <summary>输出电流</summary>
        public string OutputCurrentText => GetString("OutputCurrentText");
        /// <summary>输出电流(A)</summary>
        public string OutputCurrentWithUnitText => GetString("OutputCurrentWithUnitText");
        /// <summary>实时电流</summary>
        public string RealCurrentText => GetString("RealCurrentText");
        /// <summary>实时电流(A)</summary>
        public string RealCurrentWithUnitText => GetString("RealCurrentWithUnitText");
        /// <summary>实时功率</summary>
        public string RealPowerText => GetString("RealPowerText");
        /// <summary>实时功率(W)</summary>
        public string RealPowerWithUnitText => GetString("RealPowerWithUnitText");
        /// <summary>电压下限</summary>
        public string VoltageFloorText => GetString("VoltageFloorText");
        /// <summary>电压下限(V)</summary>
        public string VoltageFloorWithUnitText => GetString("VoltageFloorWithUnitText");
        /// <summary>电压上限</summary>
        public string VoltageCeilingText => GetString("VoltageCeilingText");
        /// <summary>电压上限(V)</summary>
        public string VoltageCeilingWithUnitText => GetString("VoltageCeilingWithUnitText");
        /// <summary>电流上限</summary>
        public string CurrentCeilingText => GetString("CurrentCeilingText");
        /// <summary>电流上限(A)</summary>
        public string CurrentCeilingWithUnitText => GetString("CurrentCeilingWithUnitText");
        /// <summary>上升时间</summary>
        public string RaiseTimeText => GetString("RaiseTimeText");
        /// <summary>上升时间(ms)</summary>
        public string RaiseTimeWithUnitText => GetString("RaiseTimeWithUnitText");
        /// <summary>延时时间</summary>
        public string DelayText => GetString("DelayText");
        /// <summary>延时时间(ms)</summary>
        public string DelayWithUnitText => GetString("DelayWithUnitText");
        /// <summary>上电</summary>
        public string DpsUpText => GetString("DpsUpText");
        /// <summary>下电</summary>
        public string DpsDownText => GetString("DpsDownText");
        /// <summary>循环到</summary>
        public string LoopToText => GetString("LoopToText");
        /// <summary>数量</summary>
        public string QuantityText => GetString("QuantityText");
        /// <summary>用途</summary>
        public string PurposeText => GetString("PurposeText");
        /// <summary>时间</summary>
        public string TimeText => GetString("TimeText");
        /// <summary>修改时间</summary>
        public string UpdateTimeText => GetString("UpdateTimeText");
        /// <summary>次</summary>
        public string TimesText => GetString("TimesText");
        /// <summary>向量文件</summary>
        public string PatternFileText => GetString("PatternFileText");
        /// <summary>寄存器文件</summary>
        public string RegisterFileText => GetString("RegisterFileText");
        /// <summary>向量超时</summary>
        public string PatternTimeoutText => GetString("PatternTimeoutText");
        /// <summary>器件文件</summary>
        public string IcFileText => GetString("IcFileText");
        /// <summary>开始时间</summary>
        public string StartTimeText => GetString("StartTimeText");
        /// <summary>上电后总运行时间</summary>
        public string TotalTimeBeforeDpsUpText => GetString("TotalTimeBeforeDpsUpText");
        /// <summary>当前向量剩余时间</summary>
        public string CurrentPatternLeftTimeText => GetString("CurrentPatternLeftTimeText");
        /// <summary>当前向量运行时间</summary>
        public string CurrentPatternCostTimeText => GetString("CurrentPatternCostTimeText");
        /// <summary>当前向量设定时间</summary>
        public string CurrentPatternSetTimeText => GetString("CurrentPatternSetTimeText");
        /// <summary>当前向量</summary>
        public string CurrentPatternText => GetString("CurrentPatternText");
        /// <summary>当前寄存器</summary>
        public string CurrentRegistersText => GetString("CurrentRegistersText");
        /// <summary>当前步次</summary>
        public string CurrentStepText => GetString("CurrentStepText");
        /// <summary>当前电源方案</summary>
        public string CurrentDpsPlanText => GetString("CurrentDpsPlanText");
        /// <summary>工位</summary>
        public string DutText => GetString("DutText");
        /// <summary>工位编辑</summary>
        public string DutEditText => GetString("DutEditText");
        /// <summary>行</summary>
        public string RowText => GetString("RowText");
        /// <summary>列</summary>
        public string ColumnText => GetString("ColumnText");
        /// <summary>所属片选</summary>
        public string BelongChipSelectText => GetString("BelongChipSelectText");
        /// <summary>使用的电源</summary>
        public string UsedDpsText => GetString("UsedDpsText");
        /// <summary>关联的管脚</summary>
        public string LinkedDrText => GetString("LinkedDrText");
        /// <summary>管脚</summary>
        public string DrText => GetString("DrText");
        /// <summary>设定</summary>
        public string SetText => GetString("SetText");
        /// <summary>老化区</summary>
        public string BurnZoneText => GetString("BurnZoneText");
        /// <summary>弱电控制板 {0}</summary>
        public string LVControlBoardFormatText => GetString("LVControlBoardFormatText");
        /// <summary>烘箱</summary>
        public string OvenText => GetString("OvenText");
        /// <summary>竖向转接板</summary>
        public string VerticalAdapterBoardText => GetString("VerticalAdapterBoardText");
        /// <summary>竖向转接板 {0}</summary>
        public string VerticalAdapterBoardFormatText => GetString("VerticalAdapterBoardFormatText");
        /// <summary>选择数据文件</summary>
        public string ChooseDataFileText => GetString("ChooseDataFileText");
        /// <summary>导出为CSV文件</summary>
        public string ExportAsCsvFileText => GetString("ExportAsCsvFileText");
        /// <summary>数据文件</summary>
        public string DataFileText => GetString("DataFileText");
        /// <summary>日期筛选</summary>
        public string DateFilterText => GetString("DateFilterText");
        /// <summary>筛选</summary>
        public string FilterText => GetString("FilterText");
        /// <summary>首页</summary>
        public string FirstPageText => GetString("FirstPageText");
        /// <summary>上一页</summary>
        public string PreviousPageText => GetString("PreviousPageText");
        /// <summary>下一页</summary>
        public string NextPageText => GetString("NextPageText");
        /// <summary>尾页</summary>
        public string LastPageText => GetString("LastPageText");
        /// <summary>第 {0} 页</summary>
        public string CurrentPageFormatText => GetString("CurrentPageFormatText");
        /// <summary>共 {0} 页</summary>
        public string TotalPageFormatText => GetString("TotalPageFormatText");
        /// <summary>刷新</summary>
        public string RefreshText => GetString("RefreshText");
        /// <summary>控制节点</summary>
        public string ControlNodeText => GetString("ControlNodeText");
        /// <summary>老化区信息</summary>
        public string BurnZoneInfoText => GetString("BurnZoneInfoText");
        /// <summary>板卡信息</summary>
        public string BorderInfoText => GetString("BorderInfoText");
        /// <summary>固件信息</summary>
        public string FirmwareInfoText => GetString("FirmwareInfoText");
        /// <summary>查询固件信息</summary>
        public string QueryFirmwareInfoText => GetString("QueryFirmwareInfoText");
        /// <summary>导出固件信息</summary>
        public string ExportFirmwareInfoText => GetString("ExportFirmwareInfoText");
        /// <summary>全部</summary>
        public string AllText => GetString("AllText");
        /// <summary>板卡类型</summary>
        public string BorderTypeText => GetString("BorderTypeText");
        /// <summary>板卡索引</summary>
        public string BoardIndexText => GetString("BoardIndexText");
        /// <summary>索引</summary>
        public string IndexText => GetString("IndexText");
        /// <summary>板卡编号</summary>
        public string PnText => GetString("PnText");
        /// <summary>生产序号</summary>
        public string SnText => GetString("SnText");
        /// <summary>父板编号</summary>
        public string ParentIdText => GetString("ParentIdText");
        /// <summary>固件类型</summary>
        public string FirmwareTypeText => GetString("FirmwareTypeText");
        /// <summary>固件版本</summary>
        public string FirmwareVersionText => GetString("FirmwareVersionText");
        /// <summary>版本</summary>
        public string VersionText => GetString("VersionText");
        /// <summary>硬件版本</summary>
        public string HardwareVersionText => GetString("HardwareVersionText");
        /// <summary>软件版本</summary>
        public string SoftwareVersionText => GetString("SoftwareVersionText");
        /// <summary>编译时间</summary>
        public string CompilationTimeText => GetString("CompilationTimeText");
        /// <summary>升级时间</summary>
        public string UpgradeTimeText => GetString("UpgradeTimeText");
        /// <summary>固件自检</summary>
        public string FirmwareSelfCheckText => GetString("FirmwareSelfCheckText");
        /// <summary>开始固件自检</summary>
        public string StartSelfCheckText => GetString("StartSelfCheckText");
        /// <summary>自检信息</summary>
        public string SelfCheckInfoText => GetString("SelfCheckInfoText");
        /// <summary>导出自检信息</summary>
        public string ExportSelfCheckInfoText => GetString("ExportSelfCheckInfoText");
        /// <summary>是否自检</summary>
        public string IsSelfCheckText => GetString("IsSelfCheckText");
        /// <summary>检查项目</summary>
        public string CheckItemText => GetString("CheckItemText");
        /// <summary>期望值</summary>
        public string ExpectValueText => GetString("ExpectValueText");
        /// <summary>实际值</summary>
        public string ActualValueText => GetString("ActualValueText");
        /// <summary>结果</summary>
        public string ResultText => GetString("ResultText");
        /// <summary>备注</summary>
        public string RemarkText => GetString("RemarkText");
        /// <summary>设备信息</summary>
        public string DeviceInfoText => GetString("DeviceInfoText");
        /// <summary>查询版本信息</summary>
        public string GetVersionInfoText => GetString("GetVersionInfoText");
        /// <summary>导出版本信息 (所有区)</summary>
        public string ExportVersionInfoText => GetString("ExportVersionInfoText");
        /// <summary>板卡类型</summary>
        public string BoardTypeText => GetString("BoardTypeText");
        /// <summary>板卡</summary>
        public string BoardText => GetString("BoardText");
        /// <summary>固件</summary>
        public string FirmwareText => GetString("FirmwareText");
        /// <summary>固件升级</summary>
        public string FirmwareUpgradeText => GetString("FirmwareUpgradeText");
        /// <summary>升级状态</summary>
        public string UpgradeStateText => GetString("UpgradeStateText");
        /// <summary>升级</summary>
        public string UpgradeText => GetString("UpgradeText");
        /// <summary>进度</summary>
        public string ProgressText => GetString("ProgressText");
        /// <summary>注意：升级过程中请勿关闭上位机或设备电源！</summary>
        public string FirmwareUpgradePromptText => GetString("FirmwareUpgradePromptText");
        /// <summary>请检查当前区和I/O控制板通信</summary>
        public string CheckZoneControllerConnectionText => GetString("CheckZoneControllerConnectionText");
        /// <summary>三色报警灯</summary>
        public string ThiLightAlarmText => GetString("ThiLightAlarmText");
        /// <summary>烟雾报警器</summary>
        public string SmokeAlarmText => GetString("SmokeAlarmText");
        /// <summary>强电箱门</summary>
        public string MainPowerDoorText => GetString("MainPowerDoorText");
        /// <summary>急停按钮</summary>
        public string EmergencyButtonText => GetString("EmergencyButtonText");
        /// <summary>参数校验失败</summary>
        public string ParameterValidateErrorText => GetString("ParameterValidateErrorText");
        /// <summary>运行脚本</summary>
        public string RunScriptText => GetString("RunScriptText");
        /// <summary>批量导出</summary>
        public string BatchExportText => GetString("BatchExportText");
        /// <summary>导出到</summary>
        public string ExportToText => GetString("ExportToText");
        /// <summary>老化结束后自动导出老化数据</summary>
        public string AutoExportDataText => GetString("AutoExportDataText");
        /// <summary>老化数据导出文件夹</summary>
        public string AutoExportDataDirectoryText => GetString("AutoExportDataDirectoryText");
        /// <summary>数值</summary>
        public string ValueText => GetString("ValueText");
        /// <summary>单位</summary>
        public string UnitText => GetString("UnitText");
        /// <summary>输入名称</summary>
        public string InputNameText => GetString("InputNameText");
        /// <summary>器件目录</summary>
        public string IcCatalogText => GetString("IcCatalogText");
        /// <summary>日志目录</summary>
        public string LogCatalogText => GetString("LogCatalogText");
        /// <summary>只读模式</summary>
        public string ReadOnlyModeText => GetString("ReadOnlyModeText");
        /// <summary>基础信息</summary>
        public string BaseInfoText => GetString("BaseInfoText");
        /// <summary>工位信息</summary>
        public string DutInfoText => GetString("DutInfoText");
        /// <summary>工位配置文件</summary>
        public string DutConfigFileText => GetString("DutConfigFileText");
        /// <summary>选择工位配置文件</summary>
        public string SelectDutConfigFileText => GetString("SelectDutConfigFileText");
        /// <summary>选择向量文件</summary>
        public string SelectPatternFileText => GetString("SelectPatternFileText");
        /// <summary>选择寄存器文件</summary>
        public string SelectRegisterFileText => GetString("SelectRegisterFileText");
        /// <summary>输入器件库名称</summary>
        public string InputIcLibNameText => GetString("InputIcLibNameText");
        /// <summary>输入器件名称</summary>
        public string InputIcNameText => GetString("InputIcNameText");
        /// <summary>电源设置</summary>
        public string DpsSettingText => GetString("DpsSettingText");
        /// <summary>电源组设置</summary>
        public string DpsGroupSettingText => GetString("DpsGroupSettingText");
        /// <summary>输入电源组名称</summary>
        public string InputDpsGroupNameText => GetString("InputDpsGroupNameText");
        /// <summary>排序</summary>
        public string SequenceSettingText => GetString("SequenceSettingText");
        /// <summary>步次设置</summary>
        public string StepSettingText => GetString("StepSettingText");
        /// <summary>步次</summary>
        public string StepText => GetString("StepText");
        /// <summary>子步次</summary>
        public string SubstepText => GetString("SubstepText");
        /// <summary>子步次设置</summary>
        public string SubstepSettingText => GetString("SubstepSettingText");
        /// <summary>运行时间</summary>
        public string RunTimeText => GetString("RunTimeText");
        /// <summary>运行次数</summary>
        public string RunTimesText => GetString("RunTimesText");
        /// <summary>循环模式</summary>
        public string LoopModeText => GetString("LoopModeText");
        /// <summary>循环时间</summary>
        public string LoopTimeText => GetString("LoopTimeText");
        /// <summary>循环次数</summary>
        public string LoopTimesText => GetString("LoopTimesText");
        /// <summary>目标步次</summary>
        public string TargetStepText => GetString("TargetStepText");
        /// <summary>向量</summary>
        public string VectorText => GetString("VectorText");
        /// <summary>向量设置</summary>
        public string VectorSettingText => GetString("VectorSettingText");
        /// <summary>当前向量</summary>
        public string CurrentVectorText => GetString("CurrentVectorText");
        /// <summary>输入子步次名称</summary>
        public string InputSubstepNameText => GetString("InputSubstepNameText");
        /// <summary>输入步次名称</summary>
        public string InputStepNameText => GetString("InputStepNameText");
        /// <summary>器件</summary>
        public string IcText => GetString("IcText");
        /// <summary>本地</summary>
        public string LocalText => GetString("LocalText");
        /// <summary>标签</summary>
        public string LabelText => GetString("LabelText");
        /// <summary>路由</summary>
        public string RouteText => GetString("RouteText");
        /// <summary>错误信息</summary>
        public string ErrorMessageText => GetString("ErrorMessageText");
        /// <summary>基础设置</summary>
        public string BaseSettingText => GetString("BaseSettingText");
        /// <summary>电源设置</summary>
        public string PowerSettingText => GetString("PowerSettingText");
        /// <summary>流程设置</summary>
        public string ProcessSettingText => GetString("ProcessSettingText");
        /// <summary>电源方案</summary>
        public string DpsPlanText => GetString("DpsPlanText");
        /// <summary>步次[ {0} ]</summary>
        public string StepFormatText => GetString("StepFormatText");
        /// <summary>子步次[ {0} ]</summary>
        public string SubstepFormatText => GetString("SubstepFormatText");
        /// <summary>电源[ {0} ]</summary>
        public string DpsFormatText => GetString("DpsFormatText");
        /// <summary>电源方案[ {0} ]</summary>
        public string DpsPlanFormatText => GetString("DpsPlanFormatText");
        /// <summary>工位[ {0} ]</summary>
        public string DutFormatText => GetString("DutFormatText");
        /// <summary>日志信息</summary>
        public string LogInfosText => GetString("LogInfosText");
        /// <summary>日志级别</summary>
        public string LogLevelText => GetString("LogLevelText");
        /// <summary>显示所有</summary>
        public string ShowAllText => GetString("ShowAllText");
        /// <summary>显示错误</summary>
        public string ShowErrorText => GetString("ShowErrorText");
        /// <summary>显示警告</summary>
        public string ShowWarnText => GetString("ShowWarnText");
        /// <summary>显示信息</summary>
        public string ShowInfoText => GetString("ShowInfoText");
        /// <summary>显示调试</summary>
        public string ShowDebugText => GetString("ShowDebugText");
        /// <summary>清空记录</summary>
        public string ClearLogsText => GetString("ClearLogsText");
        /// <summary>跟踪</summary>
        public string TraceText => GetString("TraceText");
        /// <summary>调试</summary>
        public string DebugText => GetString("DebugText");
        /// <summary>消息</summary>
        public string InfoText => GetString("InfoText");
        /// <summary>警告</summary>
        public string WarnText => GetString("WarnText");
        /// <summary>错误</summary>
        public string ErrorText => GetString("ErrorText");
        /// <summary>异常</summary>
        public string FatalText => GetString("FatalText");
        /// <summary>帧过滤器</summary>
        public string FrameFilterText => GetString("FrameFilterText");
        /// <summary>日志文件</summary>
        public string LogFileText => GetString("LogFileText");
        /// <summary>CSV文件</summary>
        public string CsvFileText => GetString("CsvFileText");
        /// <summary>选择曲线数据</summary>
        public string SelectCurveDataText => GetString("SelectCurveDataText");
        /// <summary>系统供电</summary>
        public string SystemPowerText => GetString("SystemPowerText");
        /// <summary>温控索引</summary>
        public string ThermostatIndexText => GetString("ThermostatIndexText");
        /// <summary>独立温控监视</summary>
        public string ThermostatMonitorText => GetString("ThermostatMonitorText");
        /// <summary>独立温控使能</summary>
        public string ThermostatEnableText => GetString("ThermostatEnableText");
        /// <summary>独立温控使能关闭</summary>
        public string ThermostatEnableClosedText => GetString("ThermostatEnableClosedText");
        /// <summary>比列系数(Kp)</summary>
        public string ProportionalCoefficientText => GetString("ProportionalCoefficientText");
        /// <summary>积分系数(Ki)</summary>
        public string IntegralCoefficientText => GetString("IntegralCoefficientText");
        /// <summary>微分系数(Kd)</summary>
        public string DerivativeCoefficientText => GetString("DerivativeCoefficientText");
        /// <summary>设置温控系数</summary>
        public string SetThermostatCoefficientText => GetString("SetThermostatCoefficientText");
        /// <summary>强电控制板</summary>
        public string HVControlBoardText => GetString("HVControlBoardText");
        /// <summary>强电控制板 {0}</summary>
        public string HVControlBoardFormatText => GetString("HVControlBoardFormatText");
        /// <summary>急停按钮</summary>
        public string EMOText => GetString("EMOText");
        /// <summary>序号</summary>
        public string SequenceText => GetString("SequenceText");
        /// <summary>按下</summary>
        public string PressText => GetString("PressText");
        /// <summary>松开</summary>
        public string ReleaseText => GetString("ReleaseText");
        /// <summary>强电状态</summary>
        public string HVStateText => GetString("HVStateText");
        /// <summary>强电视图</summary>
        public string HighVoltageViewText => GetString("HighVoltageViewText");
        /// <summary>门的状态</summary>
        public string DoorStateText => GetString("DoorStateText");
        /// <summary>交流检测板</summary>
        public string ACBoardText => GetString("ACBoardText");
        /// <summary>片选</summary>
        public string CSText => GetString("CSText");
        /// <summary>错误行</summary>
        public string ErrorLineText => GetString("ErrorLineText");
        /// <summary>管脚</summary>
        public string DRText => GetString("DRText");
        /// <summary>查看列表</summary>
        public string ViewListText => GetString("ViewListText");
        /// <summary>文件路径</summary>
        public string FilePathText => GetString("FilePathText");
        /// <summary>添加当前日志</summary>
        public string AppendLogFileText => GetString("AppendLogFileText");
        /// <summary>添加所有日志</summary>
        public string AppendAllLogFileText => GetString("AppendAllLogFileText");
        /// <summary>移除当前日志</summary>
        public string RemoveLogFileText => GetString("RemoveLogFileText");
        /// <summary>清空查看列表</summary>
        public string ClearViewListText => GetString("ClearViewListText");
        /// <summary>老化日志</summary>
        public string BurnLogText => GetString("BurnLogText");
        /// <summary>烘箱日志</summary>
        public string OvenLogText => GetString("OvenLogText");
        /// <summary>数据日志</summary>
        public string DataLogText => GetString("DataLogText");
        /// <summary>流程日志</summary>
        public string FlowLogText => GetString("FlowLogText");
        /// <summary>系统日志</summary>
        public string SystemLogText => GetString("SystemLogText");
        /// <summary>单区日志</summary>
        public string ZoneLogText => GetString("ZoneLogText");
        /// <summary>设备日志</summary>
        public string DeviceLogText => GetString("DeviceLogText");
        /// <summary>固件工具日志</summary>
        public string FirmwareToolLogText => GetString("FirmwareToolLogText");
        /// <summary>向量工具日志</summary>
        public string PatternToolLogText => GetString("PatternToolLogText");
        /// <summary>强电控制板日志</summary>
        public string HVCtrlBoardLogText => GetString("HVCtrlBoardLogText");
        /// <summary>弱电控制板日志</summary>
        public string LVCtrlBoardLogText => GetString("LVCtrlBoardLogText");
        /// <summary>竖向转接板日志</summary>
        public string VertAdapterBoardLogText => GetString("VertAdapterBoardLogText");
        /// <summary>触发通道设置信息</summary>
        public string TriggerChannelSettingInfoText => GetString("TriggerChannelSettingInfoText");
        /// <summary>竖向板位置</summary>
        public string VerticalBoardDirectionText => GetString("VerticalBoardDirectionText");
        /// <summary>竖向板索引</summary>
        public string VerticalBoardIndexText => GetString("VerticalBoardIndexText");
        /// <summary>电源编辑</summary>
        public string DpsEditText => GetString("DpsEditText");

        protected override void OnLanguageChanged()
        {
            OnPropertyChanged(nameof(CurrentLanguage));
            OnPropertyChanged(nameof(Enum_DeviceMode_Production));
            OnPropertyChanged(nameof(Enum_DeviceMode_Engineering));
            OnPropertyChanged(nameof(Enum_DeviceMode_Administrator));
            OnPropertyChanged(nameof(Enum_OvenMode_Off));
            OnPropertyChanged(nameof(Enum_OvenMode_Standby));
            OnPropertyChanged(nameof(Enum_OvenMode_Constant));
            OnPropertyChanged(nameof(Enum_OvenMode_Run));
            OnPropertyChanged(nameof(Enum_OvenMode_Offline));
            OnPropertyChanged(nameof(Enum_TimeUnit_Second));
            OnPropertyChanged(nameof(Enum_TimeUnit_Minute));
            OnPropertyChanged(nameof(Enum_TimeUnit_Hour));
            OnPropertyChanged(nameof(Enum_TimeUnit_Day));
            OnPropertyChanged(nameof(Enum_RunMode_Time));
            OnPropertyChanged(nameof(Enum_RunMode_Times));
            OnPropertyChanged(nameof(Enum_CoolingMode_Air));
            OnPropertyChanged(nameof(Enum_CoolingMode_Water));
            OnPropertyChanged(nameof(Enum_ZoneFunctionCode_Version));
            OnPropertyChanged(nameof(Enum_ZoneFunctionCode_SyncTime));
            OnPropertyChanged(nameof(Enum_ZoneFunctionCode_PlugedState));
            OnPropertyChanged(nameof(Enum_ZoneFunctionCode_BurnControl));
            OnPropertyChanged(nameof(Enum_ZoneFunctionCode_BurnProcess));
            OnPropertyChanged(nameof(Enum_ZoneFunctionCode_PatternError));
            OnPropertyChanged(nameof(Enum_ZoneFunctionCode_PowerState));
            OnPropertyChanged(nameof(Enum_ZoneFunctionCode_TriggerChannel));
            OnPropertyChanged(nameof(Enum_ZoneFunctionCode_ThermostatPID));
            OnPropertyChanged(nameof(Enum_ZoneFunctionCode_ConfigFlag));
            OnPropertyChanged(nameof(Enum_ZoneFunctionCode_SetDpsPlan));
            OnPropertyChanged(nameof(Enum_ZoneFunctionCode_SetPattern));
            OnPropertyChanged(nameof(Enum_ZoneFunctionCode_SetRegister));
            OnPropertyChanged(nameof(Enum_ZoneFunctionCode_SetStep));
            OnPropertyChanged(nameof(Enum_ZoneFunctionCode_SetPowerUp));
            OnPropertyChanged(nameof(Enum_ZoneFunctionCode_SetPowerDown));
            OnPropertyChanged(nameof(Enum_ZoneFunctionCode_Thermostat));
            OnPropertyChanged(nameof(Enum_ZoneFunctionCode_SelfCheck));
            OnPropertyChanged(nameof(Enum_LVControlBoardCode_Version));
            OnPropertyChanged(nameof(Enum_LVControlBoardCode_SyncTime));
            OnPropertyChanged(nameof(Enum_LVControlBoardCode_LightAlarm));
            OnPropertyChanged(nameof(Enum_LVControlBoardCode_Smoke));
            OnPropertyChanged(nameof(Enum_LVControlBoardCode_Emergency));
            OnPropertyChanged(nameof(Enum_LVControlBoardCode_OvenDoor));
            OnPropertyChanged(nameof(Enum_LVControlBoardCode_PowerDoor));
            OnPropertyChanged(nameof(Enum_LVControlBoardCode_SystemPower));
            OnPropertyChanged(nameof(Enum_LVControlBoardCode_TriggerChannel));
            OnPropertyChanged(nameof(Enum_LVControlBoardCode_SelfCheck));
            OnPropertyChanged(nameof(Enum_VerticalAdapterBoardCode_Version));
            OnPropertyChanged(nameof(Enum_VerticalAdapterBoardCode_SyncTime));
            OnPropertyChanged(nameof(Enum_VerticalAdapterBoardCode_BmcPower));
            OnPropertyChanged(nameof(Enum_VerticalAdapterBoardCode_PrePower));
            OnPropertyChanged(nameof(Enum_VerticalAdapterBoardCode_TriggerChannel));
            OnPropertyChanged(nameof(Enum_VerticalAdapterBoardCode_SelfCheck));
            OnPropertyChanged(nameof(Enum_ZoneState_Init));
            OnPropertyChanged(nameof(Enum_ZoneState_Idle));
            OnPropertyChanged(nameof(Enum_ZoneState_Ready));
            OnPropertyChanged(nameof(Enum_ZoneState_Burning));
            OnPropertyChanged(nameof(Enum_ZoneState_Suspend));
            OnPropertyChanged(nameof(Enum_ZoneState_Stop));
            OnPropertyChanged(nameof(Enum_ZoneState_Finish));
            OnPropertyChanged(nameof(Enum_ZoneState_PowerUp));
            OnPropertyChanged(nameof(Enum_ZoneState_PowerDown));
            OnPropertyChanged(nameof(Enum_ZoneState_SelfCheck));
            OnPropertyChanged(nameof(Enum_ZoneState_Upgrading));
            OnPropertyChanged(nameof(Enum_ZoneState_Offline));
            OnPropertyChanged(nameof(Enum_ZoneState_IcSending));
            OnPropertyChanged(nameof(Enum_ZoneState_Error));
            OnPropertyChanged(nameof(Enum_DutState_Idle));
            OnPropertyChanged(nameof(Enum_DutState_Test));
            OnPropertyChanged(nameof(Enum_DutState_Pass));
            OnPropertyChanged(nameof(Enum_DutState_Fail));
            OnPropertyChanged(nameof(Enum_DutState_Ignore));
            OnPropertyChanged(nameof(Enum_BoardType_DriverBoard));
            OnPropertyChanged(nameof(Enum_BoardType_BaseBoard));
            OnPropertyChanged(nameof(Enum_BoardType_DigitalBoard));
            OnPropertyChanged(nameof(Enum_BoardType_AnalogBoard));
            OnPropertyChanged(nameof(Enum_BoardType_DPSControlBoard));
            OnPropertyChanged(nameof(Enum_BoardType_DPS));
            OnPropertyChanged(nameof(Enum_BoardType_VIH));
            OnPropertyChanged(nameof(Enum_BoardType_PPSControlBoard));
            OnPropertyChanged(nameof(Enum_BoardType_TempAcquisitionBoard));
            OnPropertyChanged(nameof(Enum_BoardType_BMC));
            OnPropertyChanged(nameof(Enum_BoardType_FB10V10A));
            OnPropertyChanged(nameof(Enum_BoardType_FB06V25A));
            OnPropertyChanged(nameof(Enum_BoardType_FB06V50A));
            OnPropertyChanged(nameof(Enum_BoardType_HB20V20A));
            OnPropertyChanged(nameof(Enum_BoardType_HB45V10A));
            OnPropertyChanged(nameof(Enum_BoardType_VerticalAdapterBoard));
            OnPropertyChanged(nameof(Enum_BoardType_BIBAdapterBoard));
            OnPropertyChanged(nameof(Enum_BoardType_IOControllerBoard));
            OnPropertyChanged(nameof(Enum_BoardType_AuxiliaryBoard));
            OnPropertyChanged(nameof(Enum_BoardType_HVControlBoard));
            OnPropertyChanged(nameof(Enum_BoardType_LVControlBoard));
            OnPropertyChanged(nameof(Enum_BoardType_BurnInBoard));
            OnPropertyChanged(nameof(Enum_DpsState_Output));
            OnPropertyChanged(nameof(Enum_DpsState_Ovp));
            OnPropertyChanged(nameof(Enum_DpsState_Uvp));
            OnPropertyChanged(nameof(Enum_DpsState_Ocp));
            OnPropertyChanged(nameof(Enum_DpsState_Idle));
            OnPropertyChanged(nameof(Enum_PatternErrorType_Load));
            OnPropertyChanged(nameof(Enum_PatternErrorType_Decode));
            OnPropertyChanged(nameof(Enum_PatternErrorType_Timeout));
            OnPropertyChanged(nameof(Enum_TriggerOutputType_Close));
            OnPropertyChanged(nameof(Enum_TriggerOutputType_Signal));
            OnPropertyChanged(nameof(Enum_TriggerOutputType_Sync));
            OnPropertyChanged(nameof(Enum_UpgradeState_Updating));
            OnPropertyChanged(nameof(Enum_UpgradeState_Finish));
            OnPropertyChanged(nameof(Enum_UpgradeState_Erase));
            OnPropertyChanged(nameof(Enum_UpgradeState_SwitchBoardFail));
            OnPropertyChanged(nameof(Enum_UpgradeState_WriteTimeFail));
            OnPropertyChanged(nameof(Enum_UpgradeState_ReadFlashIdFail));
            OnPropertyChanged(nameof(Enum_UpgradeState_TimeOut));
            OnPropertyChanged(nameof(Enum_UpgradeState_Other));
            OnPropertyChanged(nameof(Enum_UpgradeState_CommunicateFail));
            OnPropertyChanged(nameof(Enum_UpgradeState_DataVerifyFail));
            OnPropertyChanged(nameof(Enum_UpgradeState_Error));
            OnPropertyChanged(nameof(Enum_LightAlarmState_Red));
            OnPropertyChanged(nameof(Enum_LightAlarmState_Yellow));
            OnPropertyChanged(nameof(Enum_LightAlarmState_Green));
            OnPropertyChanged(nameof(Enum_LightAlarmState_Alarm));
            OnPropertyChanged(nameof(Enum_DpsPlanPurpose_Default));
            OnPropertyChanged(nameof(Enum_DpsPlanPurpose_PowerUp));
            OnPropertyChanged(nameof(Enum_DpsPlanPurpose_PowerDown));
            OnPropertyChanged(nameof(Enum_LogLevel_Trace));
            OnPropertyChanged(nameof(Enum_LogLevel_Debug));
            OnPropertyChanged(nameof(Enum_LogLevel_Info));
            OnPropertyChanged(nameof(Enum_LogLevel_Warn));
            OnPropertyChanged(nameof(Enum_LogLevel_Error));
            OnPropertyChanged(nameof(Enum_LogLevel_Fatal));
            OnPropertyChanged(nameof(Enum_HVControlBoardCode_Version));
            OnPropertyChanged(nameof(Enum_HVControlBoardCode_SyncTime));
            OnPropertyChanged(nameof(Enum_HVControlBoardCode_LightAlarm));
            OnPropertyChanged(nameof(Enum_HVControlBoardCode_Smoke));
            OnPropertyChanged(nameof(Enum_HVControlBoardCode_Emergency));
            OnPropertyChanged(nameof(Enum_HVControlBoardCode_Door));
            OnPropertyChanged(nameof(Enum_HVControlBoardCode_HighVoltage));
            OnPropertyChanged(nameof(Enum_HVControlBoardCode_SelfCheck));
            OnPropertyChanged(nameof(Enum_HVStateErrorType_AC1));
            OnPropertyChanged(nameof(Enum_HVStateErrorType_AC2));
            OnPropertyChanged(nameof(Enum_HVStateErrorType_AC3));
            OnPropertyChanged(nameof(Enum_HVStateErrorType_AC4));
            OnPropertyChanged(nameof(Enum_HVStateErrorType_AC5));
            OnPropertyChanged(nameof(Enum_HVStateErrorType_AC6));
            OnPropertyChanged(nameof(Enum_HVState_Close));
            OnPropertyChanged(nameof(Enum_HVState_PowerUp));
            OnPropertyChanged(nameof(Enum_HVState_PowerDown));
            OnPropertyChanged(nameof(Enum_HVState_Open));
            OnPropertyChanged(nameof(Enum_HVState_Offline));
            OnPropertyChanged(nameof(Enum_HVState_Error));
            OnPropertyChanged(nameof(Enum_DoorType_Default));
            OnPropertyChanged(nameof(Enum_DoorType_Oven));
            OnPropertyChanged(nameof(Enum_DoorType_Power));
            OnPropertyChanged(nameof(Enum_FirmwareSelfCheckItem_ConnectionState));
            OnPropertyChanged(nameof(Enum_FirmwareSelfCheckItem_EnviormentVariables));
            OnPropertyChanged(nameof(Enum_FirmwareSelfCheckItem_ConfigFiles));
            OnPropertyChanged(nameof(Enum_FirmwareSelfCheckItem_OutputVoltage));
            OnPropertyChanged(nameof(Enum_FirmwareSelfCheckItem_InputVoltage));
            OnPropertyChanged(nameof(Enum_FirmwareSelfCheckItem_OutputCurrent));
            OnPropertyChanged(nameof(Enum_FirmwareSelfCheckItem_InputCurrent));
            OnPropertyChanged(nameof(Enum_FirmwareSelfCheckItem_OutputTemperature));
            OnPropertyChanged(nameof(Enum_FirmwareSelfCheckItem_ExpandIO));
            OnPropertyChanged(nameof(Enum_FirmwareSelfCheckItem_DAC));
            OnPropertyChanged(nameof(Enum_FirmwareSelfCheckItem_FPGA));
            OnPropertyChanged(nameof(Enum_FirmwareSelfCheckItem_Flash));
            OnPropertyChanged(nameof(Enum_FirmwareSelfCheckItem_ADC));
            OnPropertyChanged(nameof(Enum_FirmwareSelfCheckItem_LVDS));
            OnPropertyChanged(nameof(Enum_FirmwareSelfCheckItem_EEPROM));
            OnPropertyChanged(nameof(Enum_FirmwareSelfCheckItem_ReadRegister));
            OnPropertyChanged(nameof(Enum_FirmwareSelfCheckItem_WriteRegister));
            OnPropertyChanged(nameof(Enum_FirmwareSelfCheckResult_Pass));
            OnPropertyChanged(nameof(Enum_FirmwareSelfCheckResult_ConnectionError));
            OnPropertyChanged(nameof(Enum_FirmwareSelfCheckResult_EnviormentVariablesLost));
            OnPropertyChanged(nameof(Enum_FirmwareSelfCheckResult_ConfigFilesLost));
            OnPropertyChanged(nameof(Enum_FirmwareSelfCheckResult_ConfigFilesError));
            OnPropertyChanged(nameof(Enum_FirmwareSelfCheckResult_VoltageTooHigh));
            OnPropertyChanged(nameof(Enum_FirmwareSelfCheckResult_VoltageTooLow));
            OnPropertyChanged(nameof(Enum_FirmwareSelfCheckResult_CurrentTooHigh));
            OnPropertyChanged(nameof(Enum_FirmwareSelfCheckResult_CurrentTooLow));
            OnPropertyChanged(nameof(Enum_FirmwareSelfCheckResult_TemperatureTooHigh));
            OnPropertyChanged(nameof(Enum_FirmwareSelfCheckResult_TemperatureTooLow));
            OnPropertyChanged(nameof(Enum_FirmwareSelfCheckResult_MemoryAddressError));
            OnPropertyChanged(nameof(Enum_FirmwareSelfCheckResult_DataReadError));
            OnPropertyChanged(nameof(Enum_FirmwareSelfCheckResult_DataWriteError));
            OnPropertyChanged(nameof(Enum_TriggerVerticalDirection_Left));
            OnPropertyChanged(nameof(Enum_TriggerVerticalDirection_Right));
            OnPropertyChanged(nameof(UnhandledExceptionMessage));
            OnPropertyChanged(nameof(AppHasLaunchedMessage));
            OnPropertyChanged(nameof(AppStartMessage));
            OnPropertyChanged(nameof(AppExitMessage));
            OnPropertyChanged(nameof(ConfigModifiedMessage));
            OnPropertyChanged(nameof(IllegalDongleMessage));
            OnPropertyChanged(nameof(DongleExpireMessage));
            OnPropertyChanged(nameof(FileImportSuccessMessage));
            OnPropertyChanged(nameof(FileImportFailureMessage));
            OnPropertyChanged(nameof(FileExportSuccessMessage));
            OnPropertyChanged(nameof(FileExportFailureMessage));
            OnPropertyChanged(nameof(FileCreatedSuccessMessage));
            OnPropertyChanged(nameof(FileCreatedFailureMessage));
            OnPropertyChanged(nameof(FileDeletedSuccessMessage));
            OnPropertyChanged(nameof(FileDeletedFailureMessage));
            OnPropertyChanged(nameof(FileNotExistMessage));
            OnPropertyChanged(nameof(FileExistMessage));
            OnPropertyChanged(nameof(FileSaveSuccessMessage));
            OnPropertyChanged(nameof(FileSaveFailureMessage));
            OnPropertyChanged(nameof(FileSaveAsSuccessMessage));
            OnPropertyChanged(nameof(FileSaveAsFailureMessage));
            OnPropertyChanged(nameof(FileRefreshSuccessMessage));
            OnPropertyChanged(nameof(FileRefreshFailureMessage));
            OnPropertyChanged(nameof(ZoneInitFailureMessage));
            OnPropertyChanged(nameof(DeleteAskMessage));
            OnPropertyChanged(nameof(DeleteIcLibraryMessage));
            OnPropertyChanged(nameof(ImportIcLibraryExistMessage));
            OnPropertyChanged(nameof(ImportIcFileExistMessage));
            OnPropertyChanged(nameof(IcFileUnsavedMessage));
            OnPropertyChanged(nameof(IcFileNotExistMessage));
            OnPropertyChanged(nameof(IcFileInUsedMessage));
            OnPropertyChanged(nameof(IcLibraryInUsedMessage));
            OnPropertyChanged(nameof(IcLibraryNotExistIcFileMessage));
            OnPropertyChanged(nameof(IcLibraryNotExistMessage));
            OnPropertyChanged(nameof(IcFileModifiedReselectMessage));
            OnPropertyChanged(nameof(IcFileDeletedReselectMessage));
            OnPropertyChanged(nameof(NameNotNullMessage));
            OnPropertyChanged(nameof(NameNotExceedMessage));
            OnPropertyChanged(nameof(NameIllegalMessage));
            OnPropertyChanged(nameof(NameExistMessage));
            OnPropertyChanged(nameof(DeviceConfigParseFailureMessage));
            OnPropertyChanged(nameof(InitMessage));
            OnPropertyChanged(nameof(InitEndMessage));
            OnPropertyChanged(nameof(ClosingAppWhenAnyZoneRunMessage));
            OnPropertyChanged(nameof(MustBeInRangMessage));
            OnPropertyChanged(nameof(PowerDownValueMustZeroMessage));
            OnPropertyChanged(nameof(ValidStringRuleMessage));
            OnPropertyChanged(nameof(FileParseErrorMessage));
            OnPropertyChanged(nameof(FileLengthErrorMessage));
            OnPropertyChanged(nameof(AbmFileNotMatchMessage));
            OnPropertyChanged(nameof(AskMoveStepMessage));
            OnPropertyChanged(nameof(AskDeleteStepMessage));
            OnPropertyChanged(nameof(SmokyPromptMessage));
            OnPropertyChanged(nameof(OvenOfflinePromptMessage));
            OnPropertyChanged(nameof(DoorOpenedPromptMessage));
            OnPropertyChanged(nameof(DoorClosedPromptMessage));
            OnPropertyChanged(nameof(OvenErrorPromptMessage));
            OnPropertyChanged(nameof(EmoPressedPromptMessage));
            OnPropertyChanged(nameof(AskOvenPowerOffMessage));
            OnPropertyChanged(nameof(AskOvenOneClickCoolMessage));
            OnPropertyChanged(nameof(AskOvenDoorUnlockMessage));
            OnPropertyChanged(nameof(DeviceCheckFailMessage));
            OnPropertyChanged(nameof(AnalysisFrameErrorMessage));
            OnPropertyChanged(nameof(Crc32NotMatchMessage));
            OnPropertyChanged(nameof(FrameEndingNotMatchMessage));
            OnPropertyChanged(nameof(PlugedMessage));
            OnPropertyChanged(nameof(UnplugedMessage));
            OnPropertyChanged(nameof(PleaseSetMessage));
            OnPropertyChanged(nameof(AtleastOneStepMessage));
            OnPropertyChanged(nameof(AtleastOneSubStepMessage));
            OnPropertyChanged(nameof(AtleastEnableOneDpsMessage));
            OnPropertyChanged(nameof(StateMessage));
            OnPropertyChanged(nameof(PatternErrorMessage));
            OnPropertyChanged(nameof(DeviceErrorMessage));
            OnPropertyChanged(nameof(IcErrorMessage));
            OnPropertyChanged(nameof(SendIcDataMessage));
            OnPropertyChanged(nameof(CurrentStepMessage));
            OnPropertyChanged(nameof(BmcPowerErrorMessage));
            OnPropertyChanged(nameof(PrePowerErrorMessage));
            OnPropertyChanged(nameof(StopOnSmokyMeesage));
            OnPropertyChanged(nameof(StopOnOvenErrorMessage));
            OnPropertyChanged(nameof(StopOnEmergencyMessage));
            OnPropertyChanged(nameof(SuspendOnOvenDoorOpenMessage));
            OnPropertyChanged(nameof(SuspendOnOvenOfflineMessage));
            OnPropertyChanged(nameof(SuspendOnPowerDoorOpenMessage));
            OnPropertyChanged(nameof(SuspendOnDefaultDoorOpenMessage));
            OnPropertyChanged(nameof(LightAlarmMessage));
            OnPropertyChanged(nameof(SystemPowerErrorMessage));
            OnPropertyChanged(nameof(AbortAllZonesBurnAskMessage));
            OnPropertyChanged(nameof(DoorTypeNotMatchMessage));
            OnPropertyChanged(nameof(OpenHighVoltageAskMessage));
            OnPropertyChanged(nameof(CloseHighVoltageAskMessage));
            OnPropertyChanged(nameof(EMOIsPressedMessage));
            OnPropertyChanged(nameof(DoorIsOpenedMessage));
            OnPropertyChanged(nameof(UpgradeFirmwareWhenAnyZoneRunMessage));
            OnPropertyChanged(nameof(LogRuleNotFoundErrorMessage));
            OnPropertyChanged(nameof(LogRuleNotSameErrorMessage));
            OnPropertyChanged(nameof(LogFolderNotSameErrorMessage));
            OnPropertyChanged(nameof(OvenCommandErrorMessage));
            OnPropertyChanged(nameof(OvenCommandSuccessMessage));
            OnPropertyChanged(nameof(ConnectedMessage));
            OnPropertyChanged(nameof(DisconnectedMessage));
            OnPropertyChanged(nameof(FirmwareSelfCheckMessage));
            OnPropertyChanged(nameof(WorkerThreadStartedMessage));
            OnPropertyChanged(nameof(WorkerThreadEndedMessage));
            OnPropertyChanged(nameof(ConceptText));
            OnPropertyChanged(nameof(LicenseCertificationText));
            OnPropertyChanged(nameof(MachineCodeText));
            OnPropertyChanged(nameof(AdminPasswordText));
            OnPropertyChanged(nameof(InvalidPasswordText));
            OnPropertyChanged(nameof(SwitchLanguageText));
            OnPropertyChanged(nameof(LanguageText));
            OnPropertyChanged(nameof(ChineseText));
            OnPropertyChanged(nameof(EnglishText));
            OnPropertyChanged(nameof(ZoneText));
            OnPropertyChanged(nameof(ZoneFormatText));
            OnPropertyChanged(nameof(StateText));
            OnPropertyChanged(nameof(TypeText));
            OnPropertyChanged(nameof(SpecText));
            OnPropertyChanged(nameof(SystemMenuText));
            OnPropertyChanged(nameof(ModeText));
            OnPropertyChanged(nameof(SettingText));
            OnPropertyChanged(nameof(DataMenuText));
            OnPropertyChanged(nameof(HelpMenuText));
            OnPropertyChanged(nameof(ControlMenuText));
            OnPropertyChanged(nameof(LVControlBoardText));
            OnPropertyChanged(nameof(ConfirmText));
            OnPropertyChanged(nameof(CancelText));
            OnPropertyChanged(nameof(StartText));
            OnPropertyChanged(nameof(StopText));
            OnPropertyChanged(nameof(OpenText));
            OnPropertyChanged(nameof(CloseText));
            OnPropertyChanged(nameof(LockedText));
            OnPropertyChanged(nameof(UnlockedText));
            OnPropertyChanged(nameof(LockDoorText));
            OnPropertyChanged(nameof(OpenLockText));
            OnPropertyChanged(nameof(DetechedSmokeText));
            OnPropertyChanged(nameof(OvenStartupSettingText));
            OnPropertyChanged(nameof(SuspendWhenOvenDoorOpenedText));
            OnPropertyChanged(nameof(SuspendWhenOvenOfflineText));
            OnPropertyChanged(nameof(NormalText));
            OnPropertyChanged(nameof(FloorText));
            OnPropertyChanged(nameof(CeilingText));
            OnPropertyChanged(nameof(CreateNewText));
            OnPropertyChanged(nameof(RenameText));
            OnPropertyChanged(nameof(SaveText));
            OnPropertyChanged(nameof(SaveAsText));
            OnPropertyChanged(nameof(DeleteText));
            OnPropertyChanged(nameof(ImportText));
            OnPropertyChanged(nameof(ExportText));
            OnPropertyChanged(nameof(ViewText));
            OnPropertyChanged(nameof(TestText));
            OnPropertyChanged(nameof(EditText));
            OnPropertyChanged(nameof(NameText));
            OnPropertyChanged(nameof(MoveUpText));
            OnPropertyChanged(nameof(MoveDownText));
            OnPropertyChanged(nameof(CopyText));
            OnPropertyChanged(nameof(PasteText));
            OnPropertyChanged(nameof(ClearText));
            OnPropertyChanged(nameof(SendText));
            OnPropertyChanged(nameof(YesText));
            OnPropertyChanged(nameof(NoText));
            OnPropertyChanged(nameof(TipsText));
            OnPropertyChanged(nameof(SuccessText));
            OnPropertyChanged(nameof(FailureText));
            OnPropertyChanged(nameof(AboutText));
            OnPropertyChanged(nameof(LogText));
            OnPropertyChanged(nameof(AllSelectText));
            OnPropertyChanged(nameof(OnlineText));
            OnPropertyChanged(nameof(OfflineText));
            OnPropertyChanged(nameof(EnableText));
            OnPropertyChanged(nameof(IcNameText));
            OnPropertyChanged(nameof(IcEditText));
            OnPropertyChanged(nameof(CurrentIcText));
            OnPropertyChanged(nameof(DataViewText));
            OnPropertyChanged(nameof(PatternFormatConvertText));
            OnPropertyChanged(nameof(ChartViewText));
            OnPropertyChanged(nameof(TriggerSetText));
            OnPropertyChanged(nameof(OutputModeText));
            OnPropertyChanged(nameof(OutputPinText));
            OnPropertyChanged(nameof(TriggerOutputModeText));
            OnPropertyChanged(nameof(TriggerOutputPinText));
            OnPropertyChanged(nameof(SendParameterText));
            OnPropertyChanged(nameof(StartBurnText));
            OnPropertyChanged(nameof(SuspendBurnText));
            OnPropertyChanged(nameof(ContinueBurnText));
            OnPropertyChanged(nameof(StopBurnText));
            OnPropertyChanged(nameof(EnterBurnViewText));
            OnPropertyChanged(nameof(ReSelectIcText));
            OnPropertyChanged(nameof(CloseAlarmText));
            OnPropertyChanged(nameof(OvenControlText));
            OnPropertyChanged(nameof(RealTemperatureText));
            OnPropertyChanged(nameof(CoolingModeText));
            OnPropertyChanged(nameof(TestChamberInTemperatureText));
            OnPropertyChanged(nameof(TestChamberOutTemperatureText));
            OnPropertyChanged(nameof(MachineRoomInTemperatureText));
            OnPropertyChanged(nameof(MachineRoomOutTemperatureText));
            OnPropertyChanged(nameof(DriveRoomTemperatureText));
            OnPropertyChanged(nameof(AmbientTemperatureText));
            OnPropertyChanged(nameof(Commutator1TemperatureText));
            OnPropertyChanged(nameof(Commutator2TemperatureText));
            OnPropertyChanged(nameof(DriveRoomHumidityText));
            OnPropertyChanged(nameof(NitrogenFlowText));
            OnPropertyChanged(nameof(NitrogenPressureText));
            OnPropertyChanged(nameof(InletWaterPressureText));
            OnPropertyChanged(nameof(OutletWaterPressureText));
            OnPropertyChanged(nameof(DriveRoomProportionalValveOpeningText));
            OnPropertyChanged(nameof(TemperatureText));
            OnPropertyChanged(nameof(TemperatureWithUnitText));
            OnPropertyChanged(nameof(TargetTemperatureText));
            OnPropertyChanged(nameof(TargetTemperatureWithUnitText));
            OnPropertyChanged(nameof(CurrentTemperatureText));
            OnPropertyChanged(nameof(CurrentTemperatureWithUnitText));
            OnPropertyChanged(nameof(FloorTemperatureText));
            OnPropertyChanged(nameof(CeilTemperatureText));
            OnPropertyChanged(nameof(OvenDoorText));
            OnPropertyChanged(nameof(OvenLockerText));
            OnPropertyChanged(nameof(AlarmMessageText));
            OnPropertyChanged(nameof(AlarmWindowText));
            OnPropertyChanged(nameof(AlarmWindowTitleText));
            OnPropertyChanged(nameof(CloseOvenAlarmText));
            OnPropertyChanged(nameof(OneClickCoolText));
            OnPropertyChanged(nameof(ClearAlarmText));
            OnPropertyChanged(nameof(AutoDoorLockTempText));
            OnPropertyChanged(nameof(IcLibraryText));
            OnPropertyChanged(nameof(IcSelectText));
            OnPropertyChanged(nameof(RunStrategyText));
            OnPropertyChanged(nameof(RunModeText));
            OnPropertyChanged(nameof(RunSetText));
            OnPropertyChanged(nameof(RunByTimeText));
            OnPropertyChanged(nameof(RunByTimesText));
            OnPropertyChanged(nameof(LoopStrategyText));
            OnPropertyChanged(nameof(InLoopText));
            OnPropertyChanged(nameof(RunInTheLoopText));
            OnPropertyChanged(nameof(EnableLoopText));
            OnPropertyChanged(nameof(CycleToTargetStepText));
            OnPropertyChanged(nameof(LogIntervalText));
            OnPropertyChanged(nameof(ErrorLogIntervalText));
            OnPropertyChanged(nameof(LotText));
            OnPropertyChanged(nameof(OperatorText));
            OnPropertyChanged(nameof(RealDataText));
            OnPropertyChanged(nameof(DpsText));
            OnPropertyChanged(nameof(SetVoltageText));
            OnPropertyChanged(nameof(SetVoltageWithUnitText));
            OnPropertyChanged(nameof(RealVoltageText));
            OnPropertyChanged(nameof(RealVoltageWithUnitText));
            OnPropertyChanged(nameof(InputVoltageText));
            OnPropertyChanged(nameof(InputVoltageWithUnitText));
            OnPropertyChanged(nameof(InputCurrentText));
            OnPropertyChanged(nameof(InputCurrentWithUnitText));
            OnPropertyChanged(nameof(OutputVoltageText));
            OnPropertyChanged(nameof(OutputVoltageWithUnitText));
            OnPropertyChanged(nameof(OutputCurrentText));
            OnPropertyChanged(nameof(OutputCurrentWithUnitText));
            OnPropertyChanged(nameof(RealCurrentText));
            OnPropertyChanged(nameof(RealCurrentWithUnitText));
            OnPropertyChanged(nameof(RealPowerText));
            OnPropertyChanged(nameof(RealPowerWithUnitText));
            OnPropertyChanged(nameof(VoltageFloorText));
            OnPropertyChanged(nameof(VoltageFloorWithUnitText));
            OnPropertyChanged(nameof(VoltageCeilingText));
            OnPropertyChanged(nameof(VoltageCeilingWithUnitText));
            OnPropertyChanged(nameof(CurrentCeilingText));
            OnPropertyChanged(nameof(CurrentCeilingWithUnitText));
            OnPropertyChanged(nameof(RaiseTimeText));
            OnPropertyChanged(nameof(RaiseTimeWithUnitText));
            OnPropertyChanged(nameof(DelayText));
            OnPropertyChanged(nameof(DelayWithUnitText));
            OnPropertyChanged(nameof(DpsUpText));
            OnPropertyChanged(nameof(DpsDownText));
            OnPropertyChanged(nameof(LoopToText));
            OnPropertyChanged(nameof(QuantityText));
            OnPropertyChanged(nameof(PurposeText));
            OnPropertyChanged(nameof(TimeText));
            OnPropertyChanged(nameof(UpdateTimeText));
            OnPropertyChanged(nameof(TimesText));
            OnPropertyChanged(nameof(PatternFileText));
            OnPropertyChanged(nameof(RegisterFileText));
            OnPropertyChanged(nameof(PatternTimeoutText));
            OnPropertyChanged(nameof(IcFileText));
            OnPropertyChanged(nameof(StartTimeText));
            OnPropertyChanged(nameof(TotalTimeBeforeDpsUpText));
            OnPropertyChanged(nameof(CurrentPatternLeftTimeText));
            OnPropertyChanged(nameof(CurrentPatternCostTimeText));
            OnPropertyChanged(nameof(CurrentPatternSetTimeText));
            OnPropertyChanged(nameof(CurrentPatternText));
            OnPropertyChanged(nameof(CurrentRegistersText));
            OnPropertyChanged(nameof(CurrentStepText));
            OnPropertyChanged(nameof(CurrentDpsPlanText));
            OnPropertyChanged(nameof(DutText));
            OnPropertyChanged(nameof(DutEditText));
            OnPropertyChanged(nameof(RowText));
            OnPropertyChanged(nameof(ColumnText));
            OnPropertyChanged(nameof(BelongChipSelectText));
            OnPropertyChanged(nameof(UsedDpsText));
            OnPropertyChanged(nameof(LinkedDrText));
            OnPropertyChanged(nameof(DrText));
            OnPropertyChanged(nameof(SetText));
            OnPropertyChanged(nameof(BurnZoneText));
            OnPropertyChanged(nameof(LVControlBoardFormatText));
            OnPropertyChanged(nameof(OvenText));
            OnPropertyChanged(nameof(VerticalAdapterBoardText));
            OnPropertyChanged(nameof(VerticalAdapterBoardFormatText));
            OnPropertyChanged(nameof(ChooseDataFileText));
            OnPropertyChanged(nameof(ExportAsCsvFileText));
            OnPropertyChanged(nameof(DataFileText));
            OnPropertyChanged(nameof(DateFilterText));
            OnPropertyChanged(nameof(FilterText));
            OnPropertyChanged(nameof(FirstPageText));
            OnPropertyChanged(nameof(PreviousPageText));
            OnPropertyChanged(nameof(NextPageText));
            OnPropertyChanged(nameof(LastPageText));
            OnPropertyChanged(nameof(CurrentPageFormatText));
            OnPropertyChanged(nameof(TotalPageFormatText));
            OnPropertyChanged(nameof(RefreshText));
            OnPropertyChanged(nameof(ControlNodeText));
            OnPropertyChanged(nameof(BurnZoneInfoText));
            OnPropertyChanged(nameof(BorderInfoText));
            OnPropertyChanged(nameof(FirmwareInfoText));
            OnPropertyChanged(nameof(QueryFirmwareInfoText));
            OnPropertyChanged(nameof(ExportFirmwareInfoText));
            OnPropertyChanged(nameof(AllText));
            OnPropertyChanged(nameof(BorderTypeText));
            OnPropertyChanged(nameof(BoardIndexText));
            OnPropertyChanged(nameof(IndexText));
            OnPropertyChanged(nameof(PnText));
            OnPropertyChanged(nameof(SnText));
            OnPropertyChanged(nameof(ParentIdText));
            OnPropertyChanged(nameof(FirmwareTypeText));
            OnPropertyChanged(nameof(FirmwareVersionText));
            OnPropertyChanged(nameof(VersionText));
            OnPropertyChanged(nameof(HardwareVersionText));
            OnPropertyChanged(nameof(SoftwareVersionText));
            OnPropertyChanged(nameof(CompilationTimeText));
            OnPropertyChanged(nameof(UpgradeTimeText));
            OnPropertyChanged(nameof(FirmwareSelfCheckText));
            OnPropertyChanged(nameof(StartSelfCheckText));
            OnPropertyChanged(nameof(SelfCheckInfoText));
            OnPropertyChanged(nameof(ExportSelfCheckInfoText));
            OnPropertyChanged(nameof(IsSelfCheckText));
            OnPropertyChanged(nameof(CheckItemText));
            OnPropertyChanged(nameof(ExpectValueText));
            OnPropertyChanged(nameof(ActualValueText));
            OnPropertyChanged(nameof(ResultText));
            OnPropertyChanged(nameof(RemarkText));
            OnPropertyChanged(nameof(DeviceInfoText));
            OnPropertyChanged(nameof(GetVersionInfoText));
            OnPropertyChanged(nameof(ExportVersionInfoText));
            OnPropertyChanged(nameof(BoardTypeText));
            OnPropertyChanged(nameof(BoardText));
            OnPropertyChanged(nameof(FirmwareText));
            OnPropertyChanged(nameof(FirmwareUpgradeText));
            OnPropertyChanged(nameof(UpgradeStateText));
            OnPropertyChanged(nameof(UpgradeText));
            OnPropertyChanged(nameof(ProgressText));
            OnPropertyChanged(nameof(FirmwareUpgradePromptText));
            OnPropertyChanged(nameof(CheckZoneControllerConnectionText));
            OnPropertyChanged(nameof(ThiLightAlarmText));
            OnPropertyChanged(nameof(SmokeAlarmText));
            OnPropertyChanged(nameof(MainPowerDoorText));
            OnPropertyChanged(nameof(EmergencyButtonText));
            OnPropertyChanged(nameof(ParameterValidateErrorText));
            OnPropertyChanged(nameof(RunScriptText));
            OnPropertyChanged(nameof(BatchExportText));
            OnPropertyChanged(nameof(ExportToText));
            OnPropertyChanged(nameof(AutoExportDataText));
            OnPropertyChanged(nameof(AutoExportDataDirectoryText));
            OnPropertyChanged(nameof(ValueText));
            OnPropertyChanged(nameof(UnitText));
            OnPropertyChanged(nameof(InputNameText));
            OnPropertyChanged(nameof(IcCatalogText));
            OnPropertyChanged(nameof(LogCatalogText));
            OnPropertyChanged(nameof(ReadOnlyModeText));
            OnPropertyChanged(nameof(BaseInfoText));
            OnPropertyChanged(nameof(DutInfoText));
            OnPropertyChanged(nameof(DutConfigFileText));
            OnPropertyChanged(nameof(SelectDutConfigFileText));
            OnPropertyChanged(nameof(SelectPatternFileText));
            OnPropertyChanged(nameof(SelectRegisterFileText));
            OnPropertyChanged(nameof(InputIcLibNameText));
            OnPropertyChanged(nameof(InputIcNameText));
            OnPropertyChanged(nameof(DpsSettingText));
            OnPropertyChanged(nameof(DpsGroupSettingText));
            OnPropertyChanged(nameof(InputDpsGroupNameText));
            OnPropertyChanged(nameof(SequenceSettingText));
            OnPropertyChanged(nameof(StepSettingText));
            OnPropertyChanged(nameof(StepText));
            OnPropertyChanged(nameof(SubstepText));
            OnPropertyChanged(nameof(SubstepSettingText));
            OnPropertyChanged(nameof(RunTimeText));
            OnPropertyChanged(nameof(RunTimesText));
            OnPropertyChanged(nameof(LoopModeText));
            OnPropertyChanged(nameof(LoopTimeText));
            OnPropertyChanged(nameof(LoopTimesText));
            OnPropertyChanged(nameof(TargetStepText));
            OnPropertyChanged(nameof(VectorText));
            OnPropertyChanged(nameof(VectorSettingText));
            OnPropertyChanged(nameof(CurrentVectorText));
            OnPropertyChanged(nameof(InputSubstepNameText));
            OnPropertyChanged(nameof(InputStepNameText));
            OnPropertyChanged(nameof(IcText));
            OnPropertyChanged(nameof(LocalText));
            OnPropertyChanged(nameof(LabelText));
            OnPropertyChanged(nameof(RouteText));
            OnPropertyChanged(nameof(ErrorMessageText));
            OnPropertyChanged(nameof(BaseSettingText));
            OnPropertyChanged(nameof(PowerSettingText));
            OnPropertyChanged(nameof(ProcessSettingText));
            OnPropertyChanged(nameof(DpsPlanText));
            OnPropertyChanged(nameof(StepFormatText));
            OnPropertyChanged(nameof(SubstepFormatText));
            OnPropertyChanged(nameof(DpsFormatText));
            OnPropertyChanged(nameof(DpsPlanFormatText));
            OnPropertyChanged(nameof(DutFormatText));
            OnPropertyChanged(nameof(LogInfosText));
            OnPropertyChanged(nameof(LogLevelText));
            OnPropertyChanged(nameof(ShowAllText));
            OnPropertyChanged(nameof(ShowErrorText));
            OnPropertyChanged(nameof(ShowWarnText));
            OnPropertyChanged(nameof(ShowInfoText));
            OnPropertyChanged(nameof(ShowDebugText));
            OnPropertyChanged(nameof(ClearLogsText));
            OnPropertyChanged(nameof(TraceText));
            OnPropertyChanged(nameof(DebugText));
            OnPropertyChanged(nameof(InfoText));
            OnPropertyChanged(nameof(WarnText));
            OnPropertyChanged(nameof(ErrorText));
            OnPropertyChanged(nameof(FatalText));
            OnPropertyChanged(nameof(FrameFilterText));
            OnPropertyChanged(nameof(LogFileText));
            OnPropertyChanged(nameof(CsvFileText));
            OnPropertyChanged(nameof(SelectCurveDataText));
            OnPropertyChanged(nameof(SystemPowerText));
            OnPropertyChanged(nameof(ThermostatIndexText));
            OnPropertyChanged(nameof(ThermostatMonitorText));
            OnPropertyChanged(nameof(ThermostatEnableText));
            OnPropertyChanged(nameof(ThermostatEnableClosedText));
            OnPropertyChanged(nameof(ProportionalCoefficientText));
            OnPropertyChanged(nameof(IntegralCoefficientText));
            OnPropertyChanged(nameof(DerivativeCoefficientText));
            OnPropertyChanged(nameof(SetThermostatCoefficientText));
            OnPropertyChanged(nameof(HVControlBoardText));
            OnPropertyChanged(nameof(HVControlBoardFormatText));
            OnPropertyChanged(nameof(EMOText));
            OnPropertyChanged(nameof(SequenceText));
            OnPropertyChanged(nameof(PressText));
            OnPropertyChanged(nameof(ReleaseText));
            OnPropertyChanged(nameof(HVStateText));
            OnPropertyChanged(nameof(HighVoltageViewText));
            OnPropertyChanged(nameof(DoorStateText));
            OnPropertyChanged(nameof(ACBoardText));
            OnPropertyChanged(nameof(CSText));
            OnPropertyChanged(nameof(ErrorLineText));
            OnPropertyChanged(nameof(DRText));
            OnPropertyChanged(nameof(ViewListText));
            OnPropertyChanged(nameof(FilePathText));
            OnPropertyChanged(nameof(AppendLogFileText));
            OnPropertyChanged(nameof(AppendAllLogFileText));
            OnPropertyChanged(nameof(RemoveLogFileText));
            OnPropertyChanged(nameof(ClearViewListText));
            OnPropertyChanged(nameof(BurnLogText));
            OnPropertyChanged(nameof(OvenLogText));
            OnPropertyChanged(nameof(DataLogText));
            OnPropertyChanged(nameof(FlowLogText));
            OnPropertyChanged(nameof(SystemLogText));
            OnPropertyChanged(nameof(ZoneLogText));
            OnPropertyChanged(nameof(DeviceLogText));
            OnPropertyChanged(nameof(FirmwareToolLogText));
            OnPropertyChanged(nameof(PatternToolLogText));
            OnPropertyChanged(nameof(HVCtrlBoardLogText));
            OnPropertyChanged(nameof(LVCtrlBoardLogText));
            OnPropertyChanged(nameof(VertAdapterBoardLogText));
            OnPropertyChanged(nameof(TriggerChannelSettingInfoText));
            OnPropertyChanged(nameof(VerticalBoardDirectionText));
            OnPropertyChanged(nameof(VerticalBoardIndexText));
            OnPropertyChanged(nameof(DpsEditText));
        }
    }

    public static class LangKey
    {
        /// <summary>生产模式</summary>
        public const string Enum_DeviceMode_Production = nameof(Enum_DeviceMode_Production);
        /// <summary>工程模式</summary>
        public const string Enum_DeviceMode_Engineering = nameof(Enum_DeviceMode_Engineering);
        /// <summary>管理员模式</summary>
        public const string Enum_DeviceMode_Administrator = nameof(Enum_DeviceMode_Administrator);
        /// <summary>待机中</summary>
        public const string Enum_OvenMode_Off = nameof(Enum_OvenMode_Off);
        /// <summary>待机中</summary>
        public const string Enum_OvenMode_Standby = nameof(Enum_OvenMode_Standby);
        /// <summary>定值运行中</summary>
        public const string Enum_OvenMode_Constant = nameof(Enum_OvenMode_Constant);
        /// <summary>程式运行中</summary>
        public const string Enum_OvenMode_Run = nameof(Enum_OvenMode_Run);
        /// <summary>离线</summary>
        public const string Enum_OvenMode_Offline = nameof(Enum_OvenMode_Offline);
        /// <summary>秒</summary>
        public const string Enum_TimeUnit_Second = nameof(Enum_TimeUnit_Second);
        /// <summary>分钟</summary>
        public const string Enum_TimeUnit_Minute = nameof(Enum_TimeUnit_Minute);
        /// <summary>小时</summary>
        public const string Enum_TimeUnit_Hour = nameof(Enum_TimeUnit_Hour);
        /// <summary>天</summary>
        public const string Enum_TimeUnit_Day = nameof(Enum_TimeUnit_Day);
        /// <summary>时间</summary>
        public const string Enum_RunMode_Time = nameof(Enum_RunMode_Time);
        /// <summary>次数</summary>
        public const string Enum_RunMode_Times = nameof(Enum_RunMode_Times);
        /// <summary>风冷</summary>
        public const string Enum_CoolingMode_Air = nameof(Enum_CoolingMode_Air);
        /// <summary>水冷</summary>
        public const string Enum_CoolingMode_Water = nameof(Enum_CoolingMode_Water);
        /// <summary>查询版本</summary>
        public const string Enum_ZoneFunctionCode_Version = nameof(Enum_ZoneFunctionCode_Version);
        /// <summary>同步时间</summary>
        public const string Enum_ZoneFunctionCode_SyncTime = nameof(Enum_ZoneFunctionCode_SyncTime);
        /// <summary>插板状态</summary>
        public const string Enum_ZoneFunctionCode_PlugedState = nameof(Enum_ZoneFunctionCode_PlugedState);
        /// <summary>老化控制</summary>
        public const string Enum_ZoneFunctionCode_BurnControl = nameof(Enum_ZoneFunctionCode_BurnControl);
        /// <summary>老化流程</summary>
        public const string Enum_ZoneFunctionCode_BurnProcess = nameof(Enum_ZoneFunctionCode_BurnProcess);
        /// <summary>向量错误</summary>
        public const string Enum_ZoneFunctionCode_PatternError = nameof(Enum_ZoneFunctionCode_PatternError);
        /// <summary>电源状态</summary>
        public const string Enum_ZoneFunctionCode_PowerState = nameof(Enum_ZoneFunctionCode_PowerState);
        /// <summary>触发通道</summary>
        public const string Enum_ZoneFunctionCode_TriggerChannel = nameof(Enum_ZoneFunctionCode_TriggerChannel);
        /// <summary>温控系数</summary>
        public const string Enum_ZoneFunctionCode_ThermostatPID = nameof(Enum_ZoneFunctionCode_ThermostatPID);
        /// <summary>器件发送标志</summary>
        public const string Enum_ZoneFunctionCode_ConfigFlag = nameof(Enum_ZoneFunctionCode_ConfigFlag);
        /// <summary>设置电源方案</summary>
        public const string Enum_ZoneFunctionCode_SetDpsPlan = nameof(Enum_ZoneFunctionCode_SetDpsPlan);
        /// <summary>设置向量数据</summary>
        public const string Enum_ZoneFunctionCode_SetPattern = nameof(Enum_ZoneFunctionCode_SetPattern);
        /// <summary>设置地址数据</summary>
        public const string Enum_ZoneFunctionCode_SetRegister = nameof(Enum_ZoneFunctionCode_SetRegister);
        /// <summary>设置老化步次</summary>
        public const string Enum_ZoneFunctionCode_SetStep = nameof(Enum_ZoneFunctionCode_SetStep);
        /// <summary>设置上电方案</summary>
        public const string Enum_ZoneFunctionCode_SetPowerUp = nameof(Enum_ZoneFunctionCode_SetPowerUp);
        /// <summary>设置下电方案</summary>
        public const string Enum_ZoneFunctionCode_SetPowerDown = nameof(Enum_ZoneFunctionCode_SetPowerDown);
        /// <summary>设置独立温控</summary>
        public const string Enum_ZoneFunctionCode_Thermostat = nameof(Enum_ZoneFunctionCode_Thermostat);
        /// <summary>固件自检</summary>
        public const string Enum_ZoneFunctionCode_SelfCheck = nameof(Enum_ZoneFunctionCode_SelfCheck);
        /// <summary>查询版本</summary>
        public const string Enum_LVControlBoardCode_Version = nameof(Enum_LVControlBoardCode_Version);
        /// <summary>同步时间</summary>
        public const string Enum_LVControlBoardCode_SyncTime = nameof(Enum_LVControlBoardCode_SyncTime);
        /// <summary>三色报警灯</summary>
        public const string Enum_LVControlBoardCode_LightAlarm = nameof(Enum_LVControlBoardCode_LightAlarm);
        /// <summary>烟雾探测器</summary>
        public const string Enum_LVControlBoardCode_Smoke = nameof(Enum_LVControlBoardCode_Smoke);
        /// <summary>急停按钮</summary>
        public const string Enum_LVControlBoardCode_Emergency = nameof(Enum_LVControlBoardCode_Emergency);
        /// <summary>烘箱门状态</summary>
        public const string Enum_LVControlBoardCode_OvenDoor = nameof(Enum_LVControlBoardCode_OvenDoor);
        /// <summary>强电门状态</summary>
        public const string Enum_LVControlBoardCode_PowerDoor = nameof(Enum_LVControlBoardCode_PowerDoor);
        /// <summary>系统供电</summary>
        public const string Enum_LVControlBoardCode_SystemPower = nameof(Enum_LVControlBoardCode_SystemPower);
        /// <summary>触发通道</summary>
        public const string Enum_LVControlBoardCode_TriggerChannel = nameof(Enum_LVControlBoardCode_TriggerChannel);
        /// <summary>固件自检</summary>
        public const string Enum_LVControlBoardCode_SelfCheck = nameof(Enum_LVControlBoardCode_SelfCheck);
        /// <summary>查询版本</summary>
        public const string Enum_VerticalAdapterBoardCode_Version = nameof(Enum_VerticalAdapterBoardCode_Version);
        /// <summary>同步时间</summary>
        public const string Enum_VerticalAdapterBoardCode_SyncTime = nameof(Enum_VerticalAdapterBoardCode_SyncTime);
        /// <summary>BMC供电</summary>
        public const string Enum_VerticalAdapterBoardCode_BmcPower = nameof(Enum_VerticalAdapterBoardCode_BmcPower);
        /// <summary>一级电源</summary>
        public const string Enum_VerticalAdapterBoardCode_PrePower = nameof(Enum_VerticalAdapterBoardCode_PrePower);
        /// <summary>触发通道</summary>
        public const string Enum_VerticalAdapterBoardCode_TriggerChannel = nameof(Enum_VerticalAdapterBoardCode_TriggerChannel);
        /// <summary>固件自检</summary>
        public const string Enum_VerticalAdapterBoardCode_SelfCheck = nameof(Enum_VerticalAdapterBoardCode_SelfCheck);
        /// <summary>初始化中</summary>
        public const string Enum_ZoneState_Init = nameof(Enum_ZoneState_Init);
        /// <summary>空闲</summary>
        public const string Enum_ZoneState_Idle = nameof(Enum_ZoneState_Idle);
        /// <summary>就绪</summary>
        public const string Enum_ZoneState_Ready = nameof(Enum_ZoneState_Ready);
        /// <summary>老化中</summary>
        public const string Enum_ZoneState_Burning = nameof(Enum_ZoneState_Burning);
        /// <summary>老化暂停</summary>
        public const string Enum_ZoneState_Suspend = nameof(Enum_ZoneState_Suspend);
        /// <summary>老化停止</summary>
        public const string Enum_ZoneState_Stop = nameof(Enum_ZoneState_Stop);
        /// <summary>老化完成</summary>
        public const string Enum_ZoneState_Finish = nameof(Enum_ZoneState_Finish);
        /// <summary>上电中</summary>
        public const string Enum_ZoneState_PowerUp = nameof(Enum_ZoneState_PowerUp);
        /// <summary>下电中</summary>
        public const string Enum_ZoneState_PowerDown = nameof(Enum_ZoneState_PowerDown);
        /// <summary>自检中</summary>
        public const string Enum_ZoneState_SelfCheck = nameof(Enum_ZoneState_SelfCheck);
        /// <summary>升级中</summary>
        public const string Enum_ZoneState_Upgrading = nameof(Enum_ZoneState_Upgrading);
        /// <summary>离线</summary>
        public const string Enum_ZoneState_Offline = nameof(Enum_ZoneState_Offline);
        /// <summary>传输中</summary>
        public const string Enum_ZoneState_IcSending = nameof(Enum_ZoneState_IcSending);
        /// <summary>错误</summary>
        public const string Enum_ZoneState_Error = nameof(Enum_ZoneState_Error);
        /// <summary>空闲</summary>
        public const string Enum_DutState_Idle = nameof(Enum_DutState_Idle);
        /// <summary>测试中</summary>
        public const string Enum_DutState_Test = nameof(Enum_DutState_Test);
        /// <summary>通过</summary>
        public const string Enum_DutState_Pass = nameof(Enum_DutState_Pass);
        /// <summary>错误</summary>
        public const string Enum_DutState_Fail = nameof(Enum_DutState_Fail);
        /// <summary>忽略</summary>
        public const string Enum_DutState_Ignore = nameof(Enum_DutState_Ignore);
        /// <summary>驱动板</summary>
        public const string Enum_BoardType_DriverBoard = nameof(Enum_BoardType_DriverBoard);
        /// <summary>底板</summary>
        public const string Enum_BoardType_BaseBoard = nameof(Enum_BoardType_BaseBoard);
        /// <summary>数字板</summary>
        public const string Enum_BoardType_DigitalBoard = nameof(Enum_BoardType_DigitalBoard);
        /// <summary>模拟板</summary>
        public const string Enum_BoardType_AnalogBoard = nameof(Enum_BoardType_AnalogBoard);
        /// <summary>二级电源控制板</summary>
        public const string Enum_BoardType_DPSControlBoard = nameof(Enum_BoardType_DPSControlBoard);
        /// <summary>DPS</summary>
        public const string Enum_BoardType_DPS = nameof(Enum_BoardType_DPS);
        /// <summary>VIH</summary>
        public const string Enum_BoardType_VIH = nameof(Enum_BoardType_VIH);
        /// <summary>一级电源控制板</summary>
        public const string Enum_BoardType_PPSControlBoard = nameof(Enum_BoardType_PPSControlBoard);
        /// <summary>温度采集板</summary>
        public const string Enum_BoardType_TempAcquisitionBoard = nameof(Enum_BoardType_TempAcquisitionBoard);
        /// <summary>BMC</summary>
        public const string Enum_BoardType_BMC = nameof(Enum_BoardType_BMC);
        /// <summary>全砖 10V10A</summary>
        public const string Enum_BoardType_FB10V10A = nameof(Enum_BoardType_FB10V10A);
        /// <summary>全砖 06V25A</summary>
        public const string Enum_BoardType_FB06V25A = nameof(Enum_BoardType_FB06V25A);
        /// <summary>全砖 06V50A</summary>
        public const string Enum_BoardType_FB06V50A = nameof(Enum_BoardType_FB06V50A);
        /// <summary>半砖 20V20A</summary>
        public const string Enum_BoardType_HB20V20A = nameof(Enum_BoardType_HB20V20A);
        /// <summary>半砖 45V10A</summary>
        public const string Enum_BoardType_HB45V10A = nameof(Enum_BoardType_HB45V10A);
        /// <summary>竖向转接板</summary>
        public const string Enum_BoardType_VerticalAdapterBoard = nameof(Enum_BoardType_VerticalAdapterBoard);
        /// <summary>老化转接板</summary>
        public const string Enum_BoardType_BIBAdapterBoard = nameof(Enum_BoardType_BIBAdapterBoard);
        /// <summary>I/O控制板</summary>
        public const string Enum_BoardType_IOControllerBoard = nameof(Enum_BoardType_IOControllerBoard);
        /// <summary>辅助板</summary>
        public const string Enum_BoardType_AuxiliaryBoard = nameof(Enum_BoardType_AuxiliaryBoard);
        /// <summary>强电控制板</summary>
        public const string Enum_BoardType_HVControlBoard = nameof(Enum_BoardType_HVControlBoard);
        /// <summary>弱电控制板</summary>
        public const string Enum_BoardType_LVControlBoard = nameof(Enum_BoardType_LVControlBoard);
        /// <summary>老化板</summary>
        public const string Enum_BoardType_BurnInBoard = nameof(Enum_BoardType_BurnInBoard);
        /// <summary>工作中</summary>
        public const string Enum_DpsState_Output = nameof(Enum_DpsState_Output);
        /// <summary>过压</summary>
        public const string Enum_DpsState_Ovp = nameof(Enum_DpsState_Ovp);
        /// <summary>欠压</summary>
        public const string Enum_DpsState_Uvp = nameof(Enum_DpsState_Uvp);
        /// <summary>过流</summary>
        public const string Enum_DpsState_Ocp = nameof(Enum_DpsState_Ocp);
        /// <summary>关闭</summary>
        public const string Enum_DpsState_Idle = nameof(Enum_DpsState_Idle);
        /// <summary>向量加载错误</summary>
        public const string Enum_PatternErrorType_Load = nameof(Enum_PatternErrorType_Load);
        /// <summary>向量解码错误</summary>
        public const string Enum_PatternErrorType_Decode = nameof(Enum_PatternErrorType_Decode);
        /// <summary>向量运行超时</summary>
        public const string Enum_PatternErrorType_Timeout = nameof(Enum_PatternErrorType_Timeout);
        /// <summary>关闭</summary>
        public const string Enum_TriggerOutputType_Close = nameof(Enum_TriggerOutputType_Close);
        /// <summary>信号</summary>
        public const string Enum_TriggerOutputType_Signal = nameof(Enum_TriggerOutputType_Signal);
        /// <summary>Sync</summary>
        public const string Enum_TriggerOutputType_Sync = nameof(Enum_TriggerOutputType_Sync);
        /// <summary>升级中</summary>
        public const string Enum_UpgradeState_Updating = nameof(Enum_UpgradeState_Updating);
        /// <summary>升级完成</summary>
        public const string Enum_UpgradeState_Finish = nameof(Enum_UpgradeState_Finish);
        /// <summary>擦除中</summary>
        public const string Enum_UpgradeState_Erase = nameof(Enum_UpgradeState_Erase);
        /// <summary>切换板卡失败</summary>
        public const string Enum_UpgradeState_SwitchBoardFail = nameof(Enum_UpgradeState_SwitchBoardFail);
        /// <summary>写入更新时间失败</summary>
        public const string Enum_UpgradeState_WriteTimeFail = nameof(Enum_UpgradeState_WriteTimeFail);
        /// <summary>读取FLASH ID失败</summary>
        public const string Enum_UpgradeState_ReadFlashIdFail = nameof(Enum_UpgradeState_ReadFlashIdFail);
        /// <summary>更新超时</summary>
        public const string Enum_UpgradeState_TimeOut = nameof(Enum_UpgradeState_TimeOut);
        /// <summary>其他</summary>
        public const string Enum_UpgradeState_Other = nameof(Enum_UpgradeState_Other);
        /// <summary>通信错误</summary>
        public const string Enum_UpgradeState_CommunicateFail = nameof(Enum_UpgradeState_CommunicateFail);
        /// <summary>数据校验失败</summary>
        public const string Enum_UpgradeState_DataVerifyFail = nameof(Enum_UpgradeState_DataVerifyFail);
        /// <summary>错误</summary>
        public const string Enum_UpgradeState_Error = nameof(Enum_UpgradeState_Error);
        /// <summary>红灯</summary>
        public const string Enum_LightAlarmState_Red = nameof(Enum_LightAlarmState_Red);
        /// <summary>黄灯</summary>
        public const string Enum_LightAlarmState_Yellow = nameof(Enum_LightAlarmState_Yellow);
        /// <summary>绿灯</summary>
        public const string Enum_LightAlarmState_Green = nameof(Enum_LightAlarmState_Green);
        /// <summary>报警</summary>
        public const string Enum_LightAlarmState_Alarm = nameof(Enum_LightAlarmState_Alarm);
        /// <summary>默认</summary>
        public const string Enum_DpsPlanPurpose_Default = nameof(Enum_DpsPlanPurpose_Default);
        /// <summary>上电</summary>
        public const string Enum_DpsPlanPurpose_PowerUp = nameof(Enum_DpsPlanPurpose_PowerUp);
        /// <summary>下电</summary>
        public const string Enum_DpsPlanPurpose_PowerDown = nameof(Enum_DpsPlanPurpose_PowerDown);
        /// <summary>跟踪</summary>
        public const string Enum_LogLevel_Trace = nameof(Enum_LogLevel_Trace);
        /// <summary>调试</summary>
        public const string Enum_LogLevel_Debug = nameof(Enum_LogLevel_Debug);
        /// <summary>消息</summary>
        public const string Enum_LogLevel_Info = nameof(Enum_LogLevel_Info);
        /// <summary>警告</summary>
        public const string Enum_LogLevel_Warn = nameof(Enum_LogLevel_Warn);
        /// <summary>错误</summary>
        public const string Enum_LogLevel_Error = nameof(Enum_LogLevel_Error);
        /// <summary>异常</summary>
        public const string Enum_LogLevel_Fatal = nameof(Enum_LogLevel_Fatal);
        /// <summary>查询版本</summary>
        public const string Enum_HVControlBoardCode_Version = nameof(Enum_HVControlBoardCode_Version);
        /// <summary>同步时间</summary>
        public const string Enum_HVControlBoardCode_SyncTime = nameof(Enum_HVControlBoardCode_SyncTime);
        /// <summary>三色报警灯</summary>
        public const string Enum_HVControlBoardCode_LightAlarm = nameof(Enum_HVControlBoardCode_LightAlarm);
        /// <summary>烟雾报警器</summary>
        public const string Enum_HVControlBoardCode_Smoke = nameof(Enum_HVControlBoardCode_Smoke);
        /// <summary>急停按钮</summary>
        public const string Enum_HVControlBoardCode_Emergency = nameof(Enum_HVControlBoardCode_Emergency);
        /// <summary>门的状态</summary>
        public const string Enum_HVControlBoardCode_Door = nameof(Enum_HVControlBoardCode_Door);
        /// <summary>强电控制</summary>
        public const string Enum_HVControlBoardCode_HighVoltage = nameof(Enum_HVControlBoardCode_HighVoltage);
        /// <summary>固件自检</summary>
        public const string Enum_HVControlBoardCode_SelfCheck = nameof(Enum_HVControlBoardCode_SelfCheck);
        /// <summary>检测板 1</summary>
        public const string Enum_HVStateErrorType_AC1 = nameof(Enum_HVStateErrorType_AC1);
        /// <summary>检测板 2</summary>
        public const string Enum_HVStateErrorType_AC2 = nameof(Enum_HVStateErrorType_AC2);
        /// <summary>检测板 3</summary>
        public const string Enum_HVStateErrorType_AC3 = nameof(Enum_HVStateErrorType_AC3);
        /// <summary>检测板 4</summary>
        public const string Enum_HVStateErrorType_AC4 = nameof(Enum_HVStateErrorType_AC4);
        /// <summary>检测板 5</summary>
        public const string Enum_HVStateErrorType_AC5 = nameof(Enum_HVStateErrorType_AC5);
        /// <summary>检测板 6</summary>
        public const string Enum_HVStateErrorType_AC6 = nameof(Enum_HVStateErrorType_AC6);
        /// <summary>关闭</summary>
        public const string Enum_HVState_Close = nameof(Enum_HVState_Close);
        /// <summary>上电中</summary>
        public const string Enum_HVState_PowerUp = nameof(Enum_HVState_PowerUp);
        /// <summary>下电中</summary>
        public const string Enum_HVState_PowerDown = nameof(Enum_HVState_PowerDown);
        /// <summary>开启</summary>
        public const string Enum_HVState_Open = nameof(Enum_HVState_Open);
        /// <summary>离线</summary>
        public const string Enum_HVState_Offline = nameof(Enum_HVState_Offline);
        /// <summary>错误</summary>
        public const string Enum_HVState_Error = nameof(Enum_HVState_Error);
        /// <summary>设备门</summary>
        public const string Enum_DoorType_Default = nameof(Enum_DoorType_Default);
        /// <summary>烘箱门</summary>
        public const string Enum_DoorType_Oven = nameof(Enum_DoorType_Oven);
        /// <summary>强电门</summary>
        public const string Enum_DoorType_Power = nameof(Enum_DoorType_Power);
        /// <summary>通讯状态</summary>
        public const string Enum_FirmwareSelfCheckItem_ConnectionState = nameof(Enum_FirmwareSelfCheckItem_ConnectionState);
        /// <summary>环境变量</summary>
        public const string Enum_FirmwareSelfCheckItem_EnviormentVariables = nameof(Enum_FirmwareSelfCheckItem_EnviormentVariables);
        /// <summary>配置文件</summary>
        public const string Enum_FirmwareSelfCheckItem_ConfigFiles = nameof(Enum_FirmwareSelfCheckItem_ConfigFiles);
        /// <summary>输出电压</summary>
        public const string Enum_FirmwareSelfCheckItem_OutputVoltage = nameof(Enum_FirmwareSelfCheckItem_OutputVoltage);
        /// <summary>输入电压</summary>
        public const string Enum_FirmwareSelfCheckItem_InputVoltage = nameof(Enum_FirmwareSelfCheckItem_InputVoltage);
        /// <summary>输出电流</summary>
        public const string Enum_FirmwareSelfCheckItem_OutputCurrent = nameof(Enum_FirmwareSelfCheckItem_OutputCurrent);
        /// <summary>输入电流</summary>
        public const string Enum_FirmwareSelfCheckItem_InputCurrent = nameof(Enum_FirmwareSelfCheckItem_InputCurrent);
        /// <summary>输出温度</summary>
        public const string Enum_FirmwareSelfCheckItem_OutputTemperature = nameof(Enum_FirmwareSelfCheckItem_OutputTemperature);
        /// <summary>扩展IO</summary>
        public const string Enum_FirmwareSelfCheckItem_ExpandIO = nameof(Enum_FirmwareSelfCheckItem_ExpandIO);
        /// <summary>数模转换</summary>
        public const string Enum_FirmwareSelfCheckItem_DAC = nameof(Enum_FirmwareSelfCheckItem_DAC);
        /// <summary>FPGA</summary>
        public const string Enum_FirmwareSelfCheckItem_FPGA = nameof(Enum_FirmwareSelfCheckItem_FPGA);
        /// <summary>闪存</summary>
        public const string Enum_FirmwareSelfCheckItem_Flash = nameof(Enum_FirmwareSelfCheckItem_Flash);
        /// <summary>模数转换</summary>
        public const string Enum_FirmwareSelfCheckItem_ADC = nameof(Enum_FirmwareSelfCheckItem_ADC);
        /// <summary>差分信号</summary>
        public const string Enum_FirmwareSelfCheckItem_LVDS = nameof(Enum_FirmwareSelfCheckItem_LVDS);
        /// <summary>EEPROM</summary>
        public const string Enum_FirmwareSelfCheckItem_EEPROM = nameof(Enum_FirmwareSelfCheckItem_EEPROM);
        /// <summary>读寄存器</summary>
        public const string Enum_FirmwareSelfCheckItem_ReadRegister = nameof(Enum_FirmwareSelfCheckItem_ReadRegister);
        /// <summary>写寄存器</summary>
        public const string Enum_FirmwareSelfCheckItem_WriteRegister = nameof(Enum_FirmwareSelfCheckItem_WriteRegister);
        /// <summary>通过</summary>
        public const string Enum_FirmwareSelfCheckResult_Pass = nameof(Enum_FirmwareSelfCheckResult_Pass);
        /// <summary>通讯异常</summary>
        public const string Enum_FirmwareSelfCheckResult_ConnectionError = nameof(Enum_FirmwareSelfCheckResult_ConnectionError);
        /// <summary>环境变量丢失</summary>
        public const string Enum_FirmwareSelfCheckResult_EnviormentVariablesLost = nameof(Enum_FirmwareSelfCheckResult_EnviormentVariablesLost);
        /// <summary>配置文件丢失</summary>
        public const string Enum_FirmwareSelfCheckResult_ConfigFilesLost = nameof(Enum_FirmwareSelfCheckResult_ConfigFilesLost);
        /// <summary>配置文件错误</summary>
        public const string Enum_FirmwareSelfCheckResult_ConfigFilesError = nameof(Enum_FirmwareSelfCheckResult_ConfigFilesError);
        /// <summary>电压过高</summary>
        public const string Enum_FirmwareSelfCheckResult_VoltageTooHigh = nameof(Enum_FirmwareSelfCheckResult_VoltageTooHigh);
        /// <summary>电压过低</summary>
        public const string Enum_FirmwareSelfCheckResult_VoltageTooLow = nameof(Enum_FirmwareSelfCheckResult_VoltageTooLow);
        /// <summary>电流过高</summary>
        public const string Enum_FirmwareSelfCheckResult_CurrentTooHigh = nameof(Enum_FirmwareSelfCheckResult_CurrentTooHigh);
        /// <summary>电流过低</summary>
        public const string Enum_FirmwareSelfCheckResult_CurrentTooLow = nameof(Enum_FirmwareSelfCheckResult_CurrentTooLow);
        /// <summary>温度过高</summary>
        public const string Enum_FirmwareSelfCheckResult_TemperatureTooHigh = nameof(Enum_FirmwareSelfCheckResult_TemperatureTooHigh);
        /// <summary>温度过低</summary>
        public const string Enum_FirmwareSelfCheckResult_TemperatureTooLow = nameof(Enum_FirmwareSelfCheckResult_TemperatureTooLow);
        /// <summary>内存地址错误</summary>
        public const string Enum_FirmwareSelfCheckResult_MemoryAddressError = nameof(Enum_FirmwareSelfCheckResult_MemoryAddressError);
        /// <summary>数据读取错误</summary>
        public const string Enum_FirmwareSelfCheckResult_DataReadError = nameof(Enum_FirmwareSelfCheckResult_DataReadError);
        /// <summary>数据写入错误</summary>
        public const string Enum_FirmwareSelfCheckResult_DataWriteError = nameof(Enum_FirmwareSelfCheckResult_DataWriteError);
        /// <summary>位于左侧</summary>
        public const string Enum_TriggerVerticalDirection_Left = nameof(Enum_TriggerVerticalDirection_Left);
        /// <summary>位于右侧</summary>
        public const string Enum_TriggerVerticalDirection_Right = nameof(Enum_TriggerVerticalDirection_Right);
        /// <summary>未捕获到的异常, {0}</summary>
        public const string UnhandledExceptionMessage = nameof(UnhandledExceptionMessage);
        /// <summary>程序已运行</summary>
        public const string AppHasLaunchedMessage = nameof(AppHasLaunchedMessage);
        /// <summary>程序启动</summary>
        public const string AppStartMessage = nameof(AppStartMessage);
        /// <summary>程序退出</summary>
        public const string AppExitMessage = nameof(AppExitMessage);
        /// <summary>Config.ini 配置文件已发生修改。上次文件修改日期：{0}，当前文件修改日期：{1}</summary>
        public const string ConfigModifiedMessage = nameof(ConfigModifiedMessage);
        /// <summary>加密锁和设备不匹配, 请联系售后人员</summary>
        public const string IllegalDongleMessage = nameof(IllegalDongleMessage);
        /// <summary>设备授权已到期 [ {0:yyyy/MM/dd HH:mm:ss} ], 请联系售后人员</summary>
        public const string DongleExpireMessage = nameof(DongleExpireMessage);
        /// <summary>文件导入成功</summary>
        public const string FileImportSuccessMessage = nameof(FileImportSuccessMessage);
        /// <summary>文件导入失败</summary>
        public const string FileImportFailureMessage = nameof(FileImportFailureMessage);
        /// <summary>文件导出成功</summary>
        public const string FileExportSuccessMessage = nameof(FileExportSuccessMessage);
        /// <summary>文件导出失败</summary>
        public const string FileExportFailureMessage = nameof(FileExportFailureMessage);
        /// <summary>文件创建成功</summary>
        public const string FileCreatedSuccessMessage = nameof(FileCreatedSuccessMessage);
        /// <summary>文件创建失败</summary>
        public const string FileCreatedFailureMessage = nameof(FileCreatedFailureMessage);
        /// <summary>文件删除成功</summary>
        public const string FileDeletedSuccessMessage = nameof(FileDeletedSuccessMessage);
        /// <summary>文件删除失败</summary>
        public const string FileDeletedFailureMessage = nameof(FileDeletedFailureMessage);
        /// <summary>文件不存在</summary>
        public const string FileNotExistMessage = nameof(FileNotExistMessage);
        /// <summary>文件已存在</summary>
        public const string FileExistMessage = nameof(FileExistMessage);
        /// <summary>文件保存成功</summary>
        public const string FileSaveSuccessMessage = nameof(FileSaveSuccessMessage);
        /// <summary>文件保存失败</summary>
        public const string FileSaveFailureMessage = nameof(FileSaveFailureMessage);
        /// <summary>文件另存成功</summary>
        public const string FileSaveAsSuccessMessage = nameof(FileSaveAsSuccessMessage);
        /// <summary>文件另存失败</summary>
        public const string FileSaveAsFailureMessage = nameof(FileSaveAsFailureMessage);
        /// <summary>文件刷新成功</summary>
        public const string FileRefreshSuccessMessage = nameof(FileRefreshSuccessMessage);
        /// <summary>文件刷新失败</summary>
        public const string FileRefreshFailureMessage = nameof(FileRefreshFailureMessage);
        /// <summary>{0} 区初始化失败，{1}</summary>
        public const string ZoneInitFailureMessage = nameof(ZoneInitFailureMessage);
        /// <summary>确定删除所选项目?</summary>
        public const string DeleteAskMessage = nameof(DeleteAskMessage);
        /// <summary>器件库可能包含重要的数据文件, 是否删除?</summary>
        public const string DeleteIcLibraryMessage = nameof(DeleteIcLibraryMessage);
        /// <summary>已存在同名器件库, 是否重命名?</summary>
        public const string ImportIcLibraryExistMessage = nameof(ImportIcLibraryExistMessage);
        /// <summary>已存在同名器件文件, 是否覆盖?</summary>
        public const string ImportIcFileExistMessage = nameof(ImportIcFileExistMessage);
        /// <summary>器件 [ {0} ] 已被修改, 是否保存当前修改</summary>
        public const string IcFileUnsavedMessage = nameof(IcFileUnsavedMessage);
        /// <summary>器件 [ {0} ] 不存在</summary>
        public const string IcFileNotExistMessage = nameof(IcFileNotExistMessage);
        /// <summary>器件 [ {0} ] 正在执行老化, 请停止老化后重试</summary>
        public const string IcFileInUsedMessage = nameof(IcFileInUsedMessage);
        /// <summary>器件库 [ {0} ] 中的器件正在执行老化, 请停止老化后重试</summary>
        public const string IcLibraryInUsedMessage = nameof(IcLibraryInUsedMessage);
        /// <summary>器件库 [ {0} ] 不存在器件 [ {1} ]</summary>
        public const string IcLibraryNotExistIcFileMessage = nameof(IcLibraryNotExistIcFileMessage);
        /// <summary>器件库 [ {0} ] 不存在</summary>
        public const string IcLibraryNotExistMessage = nameof(IcLibraryNotExistMessage);
        /// <summary>器件 [ {0} ] 已被修改，状态将被重置到空闲，请重新选择器件</summary>
        public const string IcFileModifiedReselectMessage = nameof(IcFileModifiedReselectMessage);
        /// <summary>器件 [ {0} ] 已被删除，状态将被重置到空闲，请重新选择器件</summary>
        public const string IcFileDeletedReselectMessage = nameof(IcFileDeletedReselectMessage);
        /// <summary>名称不可以为空或包含空格</summary>
        public const string NameNotNullMessage = nameof(NameNotNullMessage);
        /// <summary>名称不可以超过 {0} 位长度</summary>
        public const string NameNotExceedMessage = nameof(NameNotExceedMessage);
        /// <summary>名称不能包含下列任何字符 \ / : * ? . " > < |</summary>
        public const string NameIllegalMessage = nameof(NameIllegalMessage);
        /// <summary>名称已存在</summary>
        public const string NameExistMessage = nameof(NameExistMessage);
        /// <summary>设备配置文件解析失败, {0}</summary>
        public const string DeviceConfigParseFailureMessage = nameof(DeviceConfigParseFailureMessage);
        /// <summary>初始化中...</summary>
        public const string InitMessage = nameof(InitMessage);
        /// <summary>初始化完成</summary>
        public const string InitEndMessage = nameof(InitEndMessage);
        /// <summary>老化过程中不允许关闭</summary>
        public const string ClosingAppWhenAnyZoneRunMessage = nameof(ClosingAppWhenAnyZoneRunMessage);
        /// <summary>值必须介于 {0}</summary>
        public const string MustBeInRangMessage = nameof(MustBeInRangMessage);
        /// <summary>下电方案的设置电压必须设置为 0</summary>
        public const string PowerDownValueMustZeroMessage = nameof(PowerDownValueMustZeroMessage);
        /// <summary>仅支持英文字母、数字和下划线</summary>
        public const string ValidStringRuleMessage = nameof(ValidStringRuleMessage);
        /// <summary>文件 [ {0} ] 解析失败, 第 {1} 行 {2} 列</summary>
        public const string FileParseErrorMessage = nameof(FileParseErrorMessage);
        /// <summary>文件 [ {0} ] 长度不正确, 预期长度为 {1} 字节</summary>
        public const string FileLengthErrorMessage = nameof(FileLengthErrorMessage);
        /// <summary>ABM 文件 [ {0} ] 不匹配，请使用该上位机软件中的向量转换工具生成的 ABM 文件</summary>
        public const string AbmFileNotMatchMessage = nameof(AbmFileNotMatchMessage);
        /// <summary>检测到循环配置，移动步次顺序会清除所有循环配置, 是否继续?</summary>
        public const string AskMoveStepMessage = nameof(AskMoveStepMessage);
        /// <summary>检测到循环配置，删除任意步次会清除所有循环配置, 是否继续?</summary>
        public const string AskDeleteStepMessage = nameof(AskDeleteStepMessage);
        /// <summary>检测到烟雾，所有老化即将停止，随后强电将关闭，消除烟雾报警状态后才可重新开启强电 !</summary>
        public const string SmokyPromptMessage = nameof(SmokyPromptMessage);
        /// <summary>检测到烘箱离线，请立即检查烘箱通信状态 !</summary>
        public const string OvenOfflinePromptMessage = nameof(OvenOfflinePromptMessage);
        /// <summary>检测到{0} [ {1} ] 已打开，请立即检查{2}状态 !</summary>
        public const string DoorOpenedPromptMessage = nameof(DoorOpenedPromptMessage);
        /// <summary>{0} [ {1} ] 已关闭 !</summary>
        public const string DoorClosedPromptMessage = nameof(DoorClosedPromptMessage);
        /// <summary>检测到烘箱异常报警 [ {0} ]，请立即检查烘箱状态 !</summary>
        public const string OvenErrorPromptMessage = nameof(OvenErrorPromptMessage);
        /// <summary>检测到急停按钮 [ {0} ] 被按下，请确认是否人为操作 !</summary>
        public const string EmoPressedPromptMessage = nameof(EmoPressedPromptMessage);
        /// <summary>是否执行 [ 停止烘箱 ] 操作 ?</summary>
        public const string AskOvenPowerOffMessage = nameof(AskOvenPowerOffMessage);
        /// <summary>是否执行 [ 一键降温 ] 操作 ?</summary>
        public const string AskOvenOneClickCoolMessage = nameof(AskOvenOneClickCoolMessage);
        /// <summary>是否执行 [ 开锁 ] 操作 ?</summary>
        public const string AskOvenDoorUnlockMessage = nameof(AskOvenDoorUnlockMessage);
        /// <summary>自检错误，请联系售后人员</summary>
        public const string DeviceCheckFailMessage = nameof(DeviceCheckFailMessage);
        /// <summary>报文解析错误，{0}</summary>
        public const string AnalysisFrameErrorMessage = nameof(AnalysisFrameErrorMessage);
        /// <summary>CRC32不匹配，{0}</summary>
        public const string Crc32NotMatchMessage = nameof(Crc32NotMatchMessage);
        /// <summary>帧尾不匹配</summary>
        public const string FrameEndingNotMatchMessage = nameof(FrameEndingNotMatchMessage);
        /// <summary>老化板插入</summary>
        public const string PlugedMessage = nameof(PlugedMessage);
        /// <summary>老化板卸下</summary>
        public const string UnplugedMessage = nameof(UnplugedMessage);
        /// <summary>请设置 [ {0} ]</summary>
        public const string PleaseSetMessage = nameof(PleaseSetMessage);
        /// <summary>至少需要配置一个步次</summary>
        public const string AtleastOneStepMessage = nameof(AtleastOneStepMessage);
        /// <summary>至少需要配置一个子步次</summary>
        public const string AtleastOneSubStepMessage = nameof(AtleastOneSubStepMessage);
        /// <summary>至少需要配置一个启用的电源</summary>
        public const string AtleastEnableOneDpsMessage = nameof(AtleastEnableOneDpsMessage);
        /// <summary>当前状态 [ {0} ]</summary>
        public const string StateMessage = nameof(StateMessage);
        /// <summary>向量 [ {0} ] 运行错误, 请检查步次 [ {1} ], 子步次 [ {2} ] 中的向量配置</summary>
        public const string PatternErrorMessage = nameof(PatternErrorMessage);
        /// <summary>设备异常; 请联系售后人员! 错误码 [ {0} ]</summary>
        public const string DeviceErrorMessage = nameof(DeviceErrorMessage);
        /// <summary>器件配置错误, 请检查后重试! {0}</summary>
        public const string IcErrorMessage = nameof(IcErrorMessage);
        /// <summary>发送器件数据 [ {0} ]</summary>
        public const string SendIcDataMessage = nameof(SendIcDataMessage);
        /// <summary>当前步次 [ {0} ] ; 子步次 [ {1} ]</summary>
        public const string CurrentStepMessage = nameof(CurrentStepMessage);
        /// <summary>BMC供电异常, {0} {1}V</summary>
        public const string BmcPowerErrorMessage = nameof(BmcPowerErrorMessage);
        /// <summary>一级电源异常, PPS {0} {1}</summary>
        public const string PrePowerErrorMessage = nameof(PrePowerErrorMessage);
        /// <summary>检测到烟雾，老化已停止</summary>
        public const string StopOnSmokyMeesage = nameof(StopOnSmokyMeesage);
        /// <summary>检测到烘箱异常，老化已停止</summary>
        public const string StopOnOvenErrorMessage = nameof(StopOnOvenErrorMessage);
        /// <summary>检测到急停按钮被按下，老化已停止</summary>
        public const string StopOnEmergencyMessage = nameof(StopOnEmergencyMessage);
        /// <summary>检测到烘箱门 [ {0} ] 被打开，老化已暂停</summary>
        public const string SuspendOnOvenDoorOpenMessage = nameof(SuspendOnOvenDoorOpenMessage);
        /// <summary>检测到烘箱离线，老化已暂停</summary>
        public const string SuspendOnOvenOfflineMessage = nameof(SuspendOnOvenOfflineMessage);
        /// <summary>检测到强电门 [ {0} ] 被打开，老化已暂停</summary>
        public const string SuspendOnPowerDoorOpenMessage = nameof(SuspendOnPowerDoorOpenMessage);
        /// <summary>检测到设备门 [ {0} ] 被打开，老化已暂停</summary>
        public const string SuspendOnDefaultDoorOpenMessage = nameof(SuspendOnDefaultDoorOpenMessage);
        /// <summary>三色报警灯 [ {0} ]</summary>
        public const string LightAlarmMessage = nameof(LightAlarmMessage);
        /// <summary>系统供电异常: {0} {1}V</summary>
        public const string SystemPowerErrorMessage = nameof(SystemPowerErrorMessage);
        /// <summary>确认终止所有区的老化吗?</summary>
        public const string AbortAllZonesBurnAskMessage = nameof(AbortAllZonesBurnAskMessage);
        /// <summary>门 [ {0}] 的类型与配置文件中不匹配，请检查配置文件</summary>
        public const string DoorTypeNotMatchMessage = nameof(DoorTypeNotMatchMessage);
        /// <summary>确认要开启强电吗？</summary>
        public const string OpenHighVoltageAskMessage = nameof(OpenHighVoltageAskMessage);
        /// <summary>确认要关闭强电吗？</summary>
        public const string CloseHighVoltageAskMessage = nameof(CloseHighVoltageAskMessage);
        /// <summary>急停按钮 [ {0} ] 被按下</summary>
        public const string EMOIsPressedMessage = nameof(EMOIsPressedMessage);
        /// <summary>[ {0} ] 已被打开</summary>
        public const string DoorIsOpenedMessage = nameof(DoorIsOpenedMessage);
        /// <summary>有老化区处于老化过程中不允许升级固件</summary>
        public const string UpgradeFirmwareWhenAnyZoneRunMessage = nameof(UpgradeFirmwareWhenAnyZoneRunMessage);
        /// <summary>当前日志文件未找到对应的日志规则，无法查看。请检查配置文件或将该文件放到正确的路径下。</summary>
        public const string LogRuleNotFoundErrorMessage = nameof(LogRuleNotFoundErrorMessage);
        /// <summary>当前日志文件与已经添加的日志文件规则不一致，无法合并查看。可以清除已经添加的日志文件后重试。</summary>
        public const string LogRuleNotSameErrorMessage = nameof(LogRuleNotSameErrorMessage);
        /// <summary>当前日志文件所在文件夹与已经添加的日志文件所在文件夹不一致，无法合并查看。可以清除已经添加的日志文件后重试。</summary>
        public const string LogFolderNotSameErrorMessage = nameof(LogFolderNotSameErrorMessage);
        /// <summary>指令 {0} 请求失败。{1}</summary>
        public const string OvenCommandErrorMessage = nameof(OvenCommandErrorMessage);
        /// <summary>指令 {0} 请求成功。{1}</summary>
        public const string OvenCommandSuccessMessage = nameof(OvenCommandSuccessMessage);
        /// <summary>连接成功</summary>
        public const string ConnectedMessage = nameof(ConnectedMessage);
        /// <summary>断开连接</summary>
        public const string DisconnectedMessage = nameof(DisconnectedMessage);
        /// <summary>自检信息：{0}</summary>
        public const string FirmwareSelfCheckMessage = nameof(FirmwareSelfCheckMessage);
        /// <summary>工作线程已启动!</summary>
        public const string WorkerThreadStartedMessage = nameof(WorkerThreadStartedMessage);
        /// <summary>工作线程已退出!</summary>
        public const string WorkerThreadEndedMessage = nameof(WorkerThreadEndedMessage);
        /// <summary>见微知著·科技赋能·创造非凡价值</summary>
        public const string ConceptText = nameof(ConceptText);
        /// <summary>许可认证</summary>
        public const string LicenseCertificationText = nameof(LicenseCertificationText);
        /// <summary>机器码</summary>
        public const string MachineCodeText = nameof(MachineCodeText);
        /// <summary>管理员密码</summary>
        public const string AdminPasswordText = nameof(AdminPasswordText);
        /// <summary>密码错误</summary>
        public const string InvalidPasswordText = nameof(InvalidPasswordText);
        /// <summary>切换语言</summary>
        public const string SwitchLanguageText = nameof(SwitchLanguageText);
        /// <summary>语言</summary>
        public const string LanguageText = nameof(LanguageText);
        /// <summary>中文</summary>
        public const string ChineseText = nameof(ChineseText);
        /// <summary>English</summary>
        public const string EnglishText = nameof(EnglishText);
        /// <summary>区</summary>
        public const string ZoneText = nameof(ZoneText);
        /// <summary>{0} 区</summary>
        public const string ZoneFormatText = nameof(ZoneFormatText);
        /// <summary>状态</summary>
        public const string StateText = nameof(StateText);
        /// <summary>类型</summary>
        public const string TypeText = nameof(TypeText);
        /// <summary>规格</summary>
        public const string SpecText = nameof(SpecText);
        /// <summary>系统(_S)</summary>
        public const string SystemMenuText = nameof(SystemMenuText);
        /// <summary>模式</summary>
        public const string ModeText = nameof(ModeText);
        /// <summary>设置</summary>
        public const string SettingText = nameof(SettingText);
        /// <summary>数据(_D)</summary>
        public const string DataMenuText = nameof(DataMenuText);
        /// <summary>帮助(_H)</summary>
        public const string HelpMenuText = nameof(HelpMenuText);
        /// <summary>控制(_C)</summary>
        public const string ControlMenuText = nameof(ControlMenuText);
        /// <summary>弱电控制板</summary>
        public const string LVControlBoardText = nameof(LVControlBoardText);
        /// <summary>确认</summary>
        public const string ConfirmText = nameof(ConfirmText);
        /// <summary>取消</summary>
        public const string CancelText = nameof(CancelText);
        /// <summary>启动</summary>
        public const string StartText = nameof(StartText);
        /// <summary>停止</summary>
        public const string StopText = nameof(StopText);
        /// <summary>开启</summary>
        public const string OpenText = nameof(OpenText);
        /// <summary>关闭</summary>
        public const string CloseText = nameof(CloseText);
        /// <summary>已锁闭</summary>
        public const string LockedText = nameof(LockedText);
        /// <summary>未落锁</summary>
        public const string UnlockedText = nameof(UnlockedText);
        /// <summary>锁门</summary>
        public const string LockDoorText = nameof(LockDoorText);
        /// <summary>开锁</summary>
        public const string OpenLockText = nameof(OpenLockText);
        /// <summary>检测到烟雾</summary>
        public const string DetechedSmokeText = nameof(DetechedSmokeText);
        /// <summary>烘箱启动设置</summary>
        public const string OvenStartupSettingText = nameof(OvenStartupSettingText);
        /// <summary>烘箱开门时自动暂停老化</summary>
        public const string SuspendWhenOvenDoorOpenedText = nameof(SuspendWhenOvenDoorOpenedText);
        /// <summary>烘箱断连时自动暂停老化</summary>
        public const string SuspendWhenOvenOfflineText = nameof(SuspendWhenOvenOfflineText);
        /// <summary>正常</summary>
        public const string NormalText = nameof(NormalText);
        /// <summary>下限</summary>
        public const string FloorText = nameof(FloorText);
        /// <summary>上限</summary>
        public const string CeilingText = nameof(CeilingText);
        /// <summary>新建</summary>
        public const string CreateNewText = nameof(CreateNewText);
        /// <summary>重命名</summary>
        public const string RenameText = nameof(RenameText);
        /// <summary>保存</summary>
        public const string SaveText = nameof(SaveText);
        /// <summary>另存为</summary>
        public const string SaveAsText = nameof(SaveAsText);
        /// <summary>删除</summary>
        public const string DeleteText = nameof(DeleteText);
        /// <summary>导入</summary>
        public const string ImportText = nameof(ImportText);
        /// <summary>导出</summary>
        public const string ExportText = nameof(ExportText);
        /// <summary>查看</summary>
        public const string ViewText = nameof(ViewText);
        /// <summary>测试</summary>
        public const string TestText = nameof(TestText);
        /// <summary>编辑</summary>
        public const string EditText = nameof(EditText);
        /// <summary>名称</summary>
        public const string NameText = nameof(NameText);
        /// <summary>上移</summary>
        public const string MoveUpText = nameof(MoveUpText);
        /// <summary>下移</summary>
        public const string MoveDownText = nameof(MoveDownText);
        /// <summary>复制</summary>
        public const string CopyText = nameof(CopyText);
        /// <summary>粘贴</summary>
        public const string PasteText = nameof(PasteText);
        /// <summary>清空</summary>
        public const string ClearText = nameof(ClearText);
        /// <summary>发送</summary>
        public const string SendText = nameof(SendText);
        /// <summary>是</summary>
        public const string YesText = nameof(YesText);
        /// <summary>否</summary>
        public const string NoText = nameof(NoText);
        /// <summary>提示</summary>
        public const string TipsText = nameof(TipsText);
        /// <summary>成功</summary>
        public const string SuccessText = nameof(SuccessText);
        /// <summary>失败</summary>
        public const string FailureText = nameof(FailureText);
        /// <summary>关于</summary>
        public const string AboutText = nameof(AboutText);
        /// <summary>日志</summary>
        public const string LogText = nameof(LogText);
        /// <summary>全选</summary>
        public const string AllSelectText = nameof(AllSelectText);
        /// <summary>在线</summary>
        public const string OnlineText = nameof(OnlineText);
        /// <summary>离线</summary>
        public const string OfflineText = nameof(OfflineText);
        /// <summary>使能</summary>
        public const string EnableText = nameof(EnableText);
        /// <summary>器件名称</summary>
        public const string IcNameText = nameof(IcNameText);
        /// <summary>器件编辑</summary>
        public const string IcEditText = nameof(IcEditText);
        /// <summary>当前器件</summary>
        public const string CurrentIcText = nameof(CurrentIcText);
        /// <summary>数据查看</summary>
        public const string DataViewText = nameof(DataViewText);
        /// <summary>向量格式转换</summary>
        public const string PatternFormatConvertText = nameof(PatternFormatConvertText);
        /// <summary>曲线查看</summary>
        public const string ChartViewText = nameof(ChartViewText);
        /// <summary>触发设置</summary>
        public const string TriggerSetText = nameof(TriggerSetText);
        /// <summary>输出模式</summary>
        public const string OutputModeText = nameof(OutputModeText);
        /// <summary>输出通道</summary>
        public const string OutputPinText = nameof(OutputPinText);
        /// <summary>触发输出模式</summary>
        public const string TriggerOutputModeText = nameof(TriggerOutputModeText);
        /// <summary>触发输出通道</summary>
        public const string TriggerOutputPinText = nameof(TriggerOutputPinText);
        /// <summary>发送</summary>
        public const string SendParameterText = nameof(SendParameterText);
        /// <summary>开始</summary>
        public const string StartBurnText = nameof(StartBurnText);
        /// <summary>暂停</summary>
        public const string SuspendBurnText = nameof(SuspendBurnText);
        /// <summary>继续</summary>
        public const string ContinueBurnText = nameof(ContinueBurnText);
        /// <summary>终止</summary>
        public const string StopBurnText = nameof(StopBurnText);
        /// <summary>进入老化界面</summary>
        public const string EnterBurnViewText = nameof(EnterBurnViewText);
        /// <summary>重置</summary>
        public const string ReSelectIcText = nameof(ReSelectIcText);
        /// <summary>关闭报警</summary>
        public const string CloseAlarmText = nameof(CloseAlarmText);
        /// <summary>烘箱控制</summary>
        public const string OvenControlText = nameof(OvenControlText);
        /// <summary>实时温度</summary>
        public const string RealTemperatureText = nameof(RealTemperatureText);
        /// <summary>降温方式</summary>
        public const string CoolingModeText = nameof(CoolingModeText);
        /// <summary>试验箱进水温度</summary>
        public const string TestChamberInTemperatureText = nameof(TestChamberInTemperatureText);
        /// <summary>试验箱出水温度</summary>
        public const string TestChamberOutTemperatureText = nameof(TestChamberOutTemperatureText);
        /// <summary>机械室进水温度</summary>
        public const string MachineRoomInTemperatureText = nameof(MachineRoomInTemperatureText);
        /// <summary>机械室出水温度</summary>
        public const string MachineRoomOutTemperatureText = nameof(MachineRoomOutTemperatureText);
        /// <summary>驱动室温度</summary>
        public const string DriveRoomTemperatureText = nameof(DriveRoomTemperatureText);
        /// <summary>环温</summary>
        public const string AmbientTemperatureText = nameof(AmbientTemperatureText);
        /// <summary>换向器1温度</summary>
        public const string Commutator1TemperatureText = nameof(Commutator1TemperatureText);
        /// <summary>换向器2温度</summary>
        public const string Commutator2TemperatureText = nameof(Commutator2TemperatureText);
        /// <summary>驱动室湿度</summary>
        public const string DriveRoomHumidityText = nameof(DriveRoomHumidityText);
        /// <summary>氮气流量</summary>
        public const string NitrogenFlowText = nameof(NitrogenFlowText);
        /// <summary>氮气压力</summary>
        public const string NitrogenPressureText = nameof(NitrogenPressureText);
        /// <summary>进水压力</summary>
        public const string InletWaterPressureText = nameof(InletWaterPressureText);
        /// <summary>出水压力</summary>
        public const string OutletWaterPressureText = nameof(OutletWaterPressureText);
        /// <summary>驱动室比例阀开度</summary>
        public const string DriveRoomProportionalValveOpeningText = nameof(DriveRoomProportionalValveOpeningText);
        /// <summary>温度</summary>
        public const string TemperatureText = nameof(TemperatureText);
        /// <summary>温度(℃)</summary>
        public const string TemperatureWithUnitText = nameof(TemperatureWithUnitText);
        /// <summary>目标温度</summary>
        public const string TargetTemperatureText = nameof(TargetTemperatureText);
        /// <summary>目标温度(℃)</summary>
        public const string TargetTemperatureWithUnitText = nameof(TargetTemperatureWithUnitText);
        /// <summary>当前温度</summary>
        public const string CurrentTemperatureText = nameof(CurrentTemperatureText);
        /// <summary>当前温度(℃)</summary>
        public const string CurrentTemperatureWithUnitText = nameof(CurrentTemperatureWithUnitText);
        /// <summary>温度下限</summary>
        public const string FloorTemperatureText = nameof(FloorTemperatureText);
        /// <summary>温度上限</summary>
        public const string CeilTemperatureText = nameof(CeilTemperatureText);
        /// <summary>烘箱门</summary>
        public const string OvenDoorText = nameof(OvenDoorText);
        /// <summary>烘箱锁</summary>
        public const string OvenLockerText = nameof(OvenLockerText);
        /// <summary>报警信息</summary>
        public const string AlarmMessageText = nameof(AlarmMessageText);
        /// <summary>报警窗口</summary>
        public const string AlarmWindowText = nameof(AlarmWindowText);
        /// <summary>检测到设备异常报警，请立即处理</summary>
        public const string AlarmWindowTitleText = nameof(AlarmWindowTitleText);
        /// <summary>关闭烘箱报警</summary>
        public const string CloseOvenAlarmText = nameof(CloseOvenAlarmText);
        /// <summary>一键降温</summary>
        public const string OneClickCoolText = nameof(OneClickCoolText);
        /// <summary>报警解除</summary>
        public const string ClearAlarmText = nameof(ClearAlarmText);
        /// <summary>自动锁门温度</summary>
        public const string AutoDoorLockTempText = nameof(AutoDoorLockTempText);
        /// <summary>器件库</summary>
        public const string IcLibraryText = nameof(IcLibraryText);
        /// <summary>选择老化器件</summary>
        public const string IcSelectText = nameof(IcSelectText);
        /// <summary>运行策略</summary>
        public const string RunStrategyText = nameof(RunStrategyText);
        /// <summary>运行模式</summary>
        public const string RunModeText = nameof(RunModeText);
        /// <summary>运行设置</summary>
        public const string RunSetText = nameof(RunSetText);
        /// <summary>按时间运行</summary>
        public const string RunByTimeText = nameof(RunByTimeText);
        /// <summary>按次数运行</summary>
        public const string RunByTimesText = nameof(RunByTimesText);
        /// <summary>循环策略</summary>
        public const string LoopStrategyText = nameof(LoopStrategyText);
        /// <summary>位于循环中</summary>
        public const string InLoopText = nameof(InLoopText);
        /// <summary>循环运行中</summary>
        public const string RunInTheLoopText = nameof(RunInTheLoopText);
        /// <summary>启用循环</summary>
        public const string EnableLoopText = nameof(EnableLoopText);
        /// <summary>循环到目标步次</summary>
        public const string CycleToTargetStepText = nameof(CycleToTargetStepText);
        /// <summary>数据记录间隔</summary>
        public const string LogIntervalText = nameof(LogIntervalText);
        /// <summary>报错记录间隔</summary>
        public const string ErrorLogIntervalText = nameof(ErrorLogIntervalText);
        /// <summary>批次号</summary>
        public const string LotText = nameof(LotText);
        /// <summary>操作员</summary>
        public const string OperatorText = nameof(OperatorText);
        /// <summary>实时数据</summary>
        public const string RealDataText = nameof(RealDataText);
        /// <summary>电源</summary>
        public const string DpsText = nameof(DpsText);
        /// <summary>设置电压</summary>
        public const string SetVoltageText = nameof(SetVoltageText);
        /// <summary>设置电压(V)</summary>
        public const string SetVoltageWithUnitText = nameof(SetVoltageWithUnitText);
        /// <summary>实时电压</summary>
        public const string RealVoltageText = nameof(RealVoltageText);
        /// <summary>实时电压(V)</summary>
        public const string RealVoltageWithUnitText = nameof(RealVoltageWithUnitText);
        /// <summary>输入电压</summary>
        public const string InputVoltageText = nameof(InputVoltageText);
        /// <summary>输入电压(V)</summary>
        public const string InputVoltageWithUnitText = nameof(InputVoltageWithUnitText);
        /// <summary>输入电流</summary>
        public const string InputCurrentText = nameof(InputCurrentText);
        /// <summary>输入电流(A)</summary>
        public const string InputCurrentWithUnitText = nameof(InputCurrentWithUnitText);
        /// <summary>输出电压</summary>
        public const string OutputVoltageText = nameof(OutputVoltageText);
        /// <summary>输出电压(V)</summary>
        public const string OutputVoltageWithUnitText = nameof(OutputVoltageWithUnitText);
        /// <summary>输出电流</summary>
        public const string OutputCurrentText = nameof(OutputCurrentText);
        /// <summary>输出电流(A)</summary>
        public const string OutputCurrentWithUnitText = nameof(OutputCurrentWithUnitText);
        /// <summary>实时电流</summary>
        public const string RealCurrentText = nameof(RealCurrentText);
        /// <summary>实时电流(A)</summary>
        public const string RealCurrentWithUnitText = nameof(RealCurrentWithUnitText);
        /// <summary>实时功率</summary>
        public const string RealPowerText = nameof(RealPowerText);
        /// <summary>实时功率(W)</summary>
        public const string RealPowerWithUnitText = nameof(RealPowerWithUnitText);
        /// <summary>电压下限</summary>
        public const string VoltageFloorText = nameof(VoltageFloorText);
        /// <summary>电压下限(V)</summary>
        public const string VoltageFloorWithUnitText = nameof(VoltageFloorWithUnitText);
        /// <summary>电压上限</summary>
        public const string VoltageCeilingText = nameof(VoltageCeilingText);
        /// <summary>电压上限(V)</summary>
        public const string VoltageCeilingWithUnitText = nameof(VoltageCeilingWithUnitText);
        /// <summary>电流上限</summary>
        public const string CurrentCeilingText = nameof(CurrentCeilingText);
        /// <summary>电流上限(A)</summary>
        public const string CurrentCeilingWithUnitText = nameof(CurrentCeilingWithUnitText);
        /// <summary>上升时间</summary>
        public const string RaiseTimeText = nameof(RaiseTimeText);
        /// <summary>上升时间(ms)</summary>
        public const string RaiseTimeWithUnitText = nameof(RaiseTimeWithUnitText);
        /// <summary>延时时间</summary>
        public const string DelayText = nameof(DelayText);
        /// <summary>延时时间(ms)</summary>
        public const string DelayWithUnitText = nameof(DelayWithUnitText);
        /// <summary>上电</summary>
        public const string DpsUpText = nameof(DpsUpText);
        /// <summary>下电</summary>
        public const string DpsDownText = nameof(DpsDownText);
        /// <summary>循环到</summary>
        public const string LoopToText = nameof(LoopToText);
        /// <summary>数量</summary>
        public const string QuantityText = nameof(QuantityText);
        /// <summary>用途</summary>
        public const string PurposeText = nameof(PurposeText);
        /// <summary>时间</summary>
        public const string TimeText = nameof(TimeText);
        /// <summary>修改时间</summary>
        public const string UpdateTimeText = nameof(UpdateTimeText);
        /// <summary>次</summary>
        public const string TimesText = nameof(TimesText);
        /// <summary>向量文件</summary>
        public const string PatternFileText = nameof(PatternFileText);
        /// <summary>寄存器文件</summary>
        public const string RegisterFileText = nameof(RegisterFileText);
        /// <summary>向量超时</summary>
        public const string PatternTimeoutText = nameof(PatternTimeoutText);
        /// <summary>器件文件</summary>
        public const string IcFileText = nameof(IcFileText);
        /// <summary>开始时间</summary>
        public const string StartTimeText = nameof(StartTimeText);
        /// <summary>上电后总运行时间</summary>
        public const string TotalTimeBeforeDpsUpText = nameof(TotalTimeBeforeDpsUpText);
        /// <summary>当前向量剩余时间</summary>
        public const string CurrentPatternLeftTimeText = nameof(CurrentPatternLeftTimeText);
        /// <summary>当前向量运行时间</summary>
        public const string CurrentPatternCostTimeText = nameof(CurrentPatternCostTimeText);
        /// <summary>当前向量设定时间</summary>
        public const string CurrentPatternSetTimeText = nameof(CurrentPatternSetTimeText);
        /// <summary>当前向量</summary>
        public const string CurrentPatternText = nameof(CurrentPatternText);
        /// <summary>当前寄存器</summary>
        public const string CurrentRegistersText = nameof(CurrentRegistersText);
        /// <summary>当前步次</summary>
        public const string CurrentStepText = nameof(CurrentStepText);
        /// <summary>当前电源方案</summary>
        public const string CurrentDpsPlanText = nameof(CurrentDpsPlanText);
        /// <summary>工位</summary>
        public const string DutText = nameof(DutText);
        /// <summary>工位编辑</summary>
        public const string DutEditText = nameof(DutEditText);
        /// <summary>行</summary>
        public const string RowText = nameof(RowText);
        /// <summary>列</summary>
        public const string ColumnText = nameof(ColumnText);
        /// <summary>所属片选</summary>
        public const string BelongChipSelectText = nameof(BelongChipSelectText);
        /// <summary>使用的电源</summary>
        public const string UsedDpsText = nameof(UsedDpsText);
        /// <summary>关联的管脚</summary>
        public const string LinkedDrText = nameof(LinkedDrText);
        /// <summary>管脚</summary>
        public const string DrText = nameof(DrText);
        /// <summary>设定</summary>
        public const string SetText = nameof(SetText);
        /// <summary>老化区</summary>
        public const string BurnZoneText = nameof(BurnZoneText);
        /// <summary>弱电控制板 {0}</summary>
        public const string LVControlBoardFormatText = nameof(LVControlBoardFormatText);
        /// <summary>烘箱</summary>
        public const string OvenText = nameof(OvenText);
        /// <summary>竖向转接板</summary>
        public const string VerticalAdapterBoardText = nameof(VerticalAdapterBoardText);
        /// <summary>竖向转接板 {0}</summary>
        public const string VerticalAdapterBoardFormatText = nameof(VerticalAdapterBoardFormatText);
        /// <summary>选择数据文件</summary>
        public const string ChooseDataFileText = nameof(ChooseDataFileText);
        /// <summary>导出为CSV文件</summary>
        public const string ExportAsCsvFileText = nameof(ExportAsCsvFileText);
        /// <summary>数据文件</summary>
        public const string DataFileText = nameof(DataFileText);
        /// <summary>日期筛选</summary>
        public const string DateFilterText = nameof(DateFilterText);
        /// <summary>筛选</summary>
        public const string FilterText = nameof(FilterText);
        /// <summary>首页</summary>
        public const string FirstPageText = nameof(FirstPageText);
        /// <summary>上一页</summary>
        public const string PreviousPageText = nameof(PreviousPageText);
        /// <summary>下一页</summary>
        public const string NextPageText = nameof(NextPageText);
        /// <summary>尾页</summary>
        public const string LastPageText = nameof(LastPageText);
        /// <summary>第 {0} 页</summary>
        public const string CurrentPageFormatText = nameof(CurrentPageFormatText);
        /// <summary>共 {0} 页</summary>
        public const string TotalPageFormatText = nameof(TotalPageFormatText);
        /// <summary>刷新</summary>
        public const string RefreshText = nameof(RefreshText);
        /// <summary>控制节点</summary>
        public const string ControlNodeText = nameof(ControlNodeText);
        /// <summary>老化区信息</summary>
        public const string BurnZoneInfoText = nameof(BurnZoneInfoText);
        /// <summary>板卡信息</summary>
        public const string BorderInfoText = nameof(BorderInfoText);
        /// <summary>固件信息</summary>
        public const string FirmwareInfoText = nameof(FirmwareInfoText);
        /// <summary>查询固件信息</summary>
        public const string QueryFirmwareInfoText = nameof(QueryFirmwareInfoText);
        /// <summary>导出固件信息</summary>
        public const string ExportFirmwareInfoText = nameof(ExportFirmwareInfoText);
        /// <summary>全部</summary>
        public const string AllText = nameof(AllText);
        /// <summary>板卡类型</summary>
        public const string BorderTypeText = nameof(BorderTypeText);
        /// <summary>板卡索引</summary>
        public const string BoardIndexText = nameof(BoardIndexText);
        /// <summary>索引</summary>
        public const string IndexText = nameof(IndexText);
        /// <summary>板卡编号</summary>
        public const string PnText = nameof(PnText);
        /// <summary>生产序号</summary>
        public const string SnText = nameof(SnText);
        /// <summary>父板编号</summary>
        public const string ParentIdText = nameof(ParentIdText);
        /// <summary>固件类型</summary>
        public const string FirmwareTypeText = nameof(FirmwareTypeText);
        /// <summary>固件版本</summary>
        public const string FirmwareVersionText = nameof(FirmwareVersionText);
        /// <summary>版本</summary>
        public const string VersionText = nameof(VersionText);
        /// <summary>硬件版本</summary>
        public const string HardwareVersionText = nameof(HardwareVersionText);
        /// <summary>软件版本</summary>
        public const string SoftwareVersionText = nameof(SoftwareVersionText);
        /// <summary>编译时间</summary>
        public const string CompilationTimeText = nameof(CompilationTimeText);
        /// <summary>升级时间</summary>
        public const string UpgradeTimeText = nameof(UpgradeTimeText);
        /// <summary>固件自检</summary>
        public const string FirmwareSelfCheckText = nameof(FirmwareSelfCheckText);
        /// <summary>开始固件自检</summary>
        public const string StartSelfCheckText = nameof(StartSelfCheckText);
        /// <summary>自检信息</summary>
        public const string SelfCheckInfoText = nameof(SelfCheckInfoText);
        /// <summary>导出自检信息</summary>
        public const string ExportSelfCheckInfoText = nameof(ExportSelfCheckInfoText);
        /// <summary>是否自检</summary>
        public const string IsSelfCheckText = nameof(IsSelfCheckText);
        /// <summary>检查项目</summary>
        public const string CheckItemText = nameof(CheckItemText);
        /// <summary>期望值</summary>
        public const string ExpectValueText = nameof(ExpectValueText);
        /// <summary>实际值</summary>
        public const string ActualValueText = nameof(ActualValueText);
        /// <summary>结果</summary>
        public const string ResultText = nameof(ResultText);
        /// <summary>备注</summary>
        public const string RemarkText = nameof(RemarkText);
        /// <summary>设备信息</summary>
        public const string DeviceInfoText = nameof(DeviceInfoText);
        /// <summary>查询版本信息</summary>
        public const string GetVersionInfoText = nameof(GetVersionInfoText);
        /// <summary>导出版本信息 (所有区)</summary>
        public const string ExportVersionInfoText = nameof(ExportVersionInfoText);
        /// <summary>板卡类型</summary>
        public const string BoardTypeText = nameof(BoardTypeText);
        /// <summary>板卡</summary>
        public const string BoardText = nameof(BoardText);
        /// <summary>固件</summary>
        public const string FirmwareText = nameof(FirmwareText);
        /// <summary>固件升级</summary>
        public const string FirmwareUpgradeText = nameof(FirmwareUpgradeText);
        /// <summary>升级状态</summary>
        public const string UpgradeStateText = nameof(UpgradeStateText);
        /// <summary>升级</summary>
        public const string UpgradeText = nameof(UpgradeText);
        /// <summary>进度</summary>
        public const string ProgressText = nameof(ProgressText);
        /// <summary>注意：升级过程中请勿关闭上位机或设备电源！</summary>
        public const string FirmwareUpgradePromptText = nameof(FirmwareUpgradePromptText);
        /// <summary>请检查当前区和I/O控制板通信</summary>
        public const string CheckZoneControllerConnectionText = nameof(CheckZoneControllerConnectionText);
        /// <summary>三色报警灯</summary>
        public const string ThiLightAlarmText = nameof(ThiLightAlarmText);
        /// <summary>烟雾报警器</summary>
        public const string SmokeAlarmText = nameof(SmokeAlarmText);
        /// <summary>强电箱门</summary>
        public const string MainPowerDoorText = nameof(MainPowerDoorText);
        /// <summary>急停按钮</summary>
        public const string EmergencyButtonText = nameof(EmergencyButtonText);
        /// <summary>参数校验失败</summary>
        public const string ParameterValidateErrorText = nameof(ParameterValidateErrorText);
        /// <summary>运行脚本</summary>
        public const string RunScriptText = nameof(RunScriptText);
        /// <summary>批量导出</summary>
        public const string BatchExportText = nameof(BatchExportText);
        /// <summary>导出到</summary>
        public const string ExportToText = nameof(ExportToText);
        /// <summary>老化结束后自动导出老化数据</summary>
        public const string AutoExportDataText = nameof(AutoExportDataText);
        /// <summary>老化数据导出文件夹</summary>
        public const string AutoExportDataDirectoryText = nameof(AutoExportDataDirectoryText);
        /// <summary>数值</summary>
        public const string ValueText = nameof(ValueText);
        /// <summary>单位</summary>
        public const string UnitText = nameof(UnitText);
        /// <summary>输入名称</summary>
        public const string InputNameText = nameof(InputNameText);
        /// <summary>器件目录</summary>
        public const string IcCatalogText = nameof(IcCatalogText);
        /// <summary>日志目录</summary>
        public const string LogCatalogText = nameof(LogCatalogText);
        /// <summary>只读模式</summary>
        public const string ReadOnlyModeText = nameof(ReadOnlyModeText);
        /// <summary>基础信息</summary>
        public const string BaseInfoText = nameof(BaseInfoText);
        /// <summary>工位信息</summary>
        public const string DutInfoText = nameof(DutInfoText);
        /// <summary>工位配置文件</summary>
        public const string DutConfigFileText = nameof(DutConfigFileText);
        /// <summary>选择工位配置文件</summary>
        public const string SelectDutConfigFileText = nameof(SelectDutConfigFileText);
        /// <summary>选择向量文件</summary>
        public const string SelectPatternFileText = nameof(SelectPatternFileText);
        /// <summary>选择寄存器文件</summary>
        public const string SelectRegisterFileText = nameof(SelectRegisterFileText);
        /// <summary>输入器件库名称</summary>
        public const string InputIcLibNameText = nameof(InputIcLibNameText);
        /// <summary>输入器件名称</summary>
        public const string InputIcNameText = nameof(InputIcNameText);
        /// <summary>电源设置</summary>
        public const string DpsSettingText = nameof(DpsSettingText);
        /// <summary>电源组设置</summary>
        public const string DpsGroupSettingText = nameof(DpsGroupSettingText);
        /// <summary>输入电源组名称</summary>
        public const string InputDpsGroupNameText = nameof(InputDpsGroupNameText);
        /// <summary>排序</summary>
        public const string SequenceSettingText = nameof(SequenceSettingText);
        /// <summary>步次设置</summary>
        public const string StepSettingText = nameof(StepSettingText);
        /// <summary>步次</summary>
        public const string StepText = nameof(StepText);
        /// <summary>子步次</summary>
        public const string SubstepText = nameof(SubstepText);
        /// <summary>子步次设置</summary>
        public const string SubstepSettingText = nameof(SubstepSettingText);
        /// <summary>运行时间</summary>
        public const string RunTimeText = nameof(RunTimeText);
        /// <summary>运行次数</summary>
        public const string RunTimesText = nameof(RunTimesText);
        /// <summary>循环模式</summary>
        public const string LoopModeText = nameof(LoopModeText);
        /// <summary>循环时间</summary>
        public const string LoopTimeText = nameof(LoopTimeText);
        /// <summary>循环次数</summary>
        public const string LoopTimesText = nameof(LoopTimesText);
        /// <summary>目标步次</summary>
        public const string TargetStepText = nameof(TargetStepText);
        /// <summary>向量</summary>
        public const string VectorText = nameof(VectorText);
        /// <summary>向量设置</summary>
        public const string VectorSettingText = nameof(VectorSettingText);
        /// <summary>当前向量</summary>
        public const string CurrentVectorText = nameof(CurrentVectorText);
        /// <summary>输入子步次名称</summary>
        public const string InputSubstepNameText = nameof(InputSubstepNameText);
        /// <summary>输入步次名称</summary>
        public const string InputStepNameText = nameof(InputStepNameText);
        /// <summary>器件</summary>
        public const string IcText = nameof(IcText);
        /// <summary>本地</summary>
        public const string LocalText = nameof(LocalText);
        /// <summary>标签</summary>
        public const string LabelText = nameof(LabelText);
        /// <summary>路由</summary>
        public const string RouteText = nameof(RouteText);
        /// <summary>错误信息</summary>
        public const string ErrorMessageText = nameof(ErrorMessageText);
        /// <summary>基础设置</summary>
        public const string BaseSettingText = nameof(BaseSettingText);
        /// <summary>电源设置</summary>
        public const string PowerSettingText = nameof(PowerSettingText);
        /// <summary>流程设置</summary>
        public const string ProcessSettingText = nameof(ProcessSettingText);
        /// <summary>电源方案</summary>
        public const string DpsPlanText = nameof(DpsPlanText);
        /// <summary>步次[ {0} ]</summary>
        public const string StepFormatText = nameof(StepFormatText);
        /// <summary>子步次[ {0} ]</summary>
        public const string SubstepFormatText = nameof(SubstepFormatText);
        /// <summary>电源[ {0} ]</summary>
        public const string DpsFormatText = nameof(DpsFormatText);
        /// <summary>电源方案[ {0} ]</summary>
        public const string DpsPlanFormatText = nameof(DpsPlanFormatText);
        /// <summary>工位[ {0} ]</summary>
        public const string DutFormatText = nameof(DutFormatText);
        /// <summary>日志信息</summary>
        public const string LogInfosText = nameof(LogInfosText);
        /// <summary>日志级别</summary>
        public const string LogLevelText = nameof(LogLevelText);
        /// <summary>显示所有</summary>
        public const string ShowAllText = nameof(ShowAllText);
        /// <summary>显示错误</summary>
        public const string ShowErrorText = nameof(ShowErrorText);
        /// <summary>显示警告</summary>
        public const string ShowWarnText = nameof(ShowWarnText);
        /// <summary>显示信息</summary>
        public const string ShowInfoText = nameof(ShowInfoText);
        /// <summary>显示调试</summary>
        public const string ShowDebugText = nameof(ShowDebugText);
        /// <summary>清空记录</summary>
        public const string ClearLogsText = nameof(ClearLogsText);
        /// <summary>跟踪</summary>
        public const string TraceText = nameof(TraceText);
        /// <summary>调试</summary>
        public const string DebugText = nameof(DebugText);
        /// <summary>消息</summary>
        public const string InfoText = nameof(InfoText);
        /// <summary>警告</summary>
        public const string WarnText = nameof(WarnText);
        /// <summary>错误</summary>
        public const string ErrorText = nameof(ErrorText);
        /// <summary>异常</summary>
        public const string FatalText = nameof(FatalText);
        /// <summary>帧过滤器</summary>
        public const string FrameFilterText = nameof(FrameFilterText);
        /// <summary>日志文件</summary>
        public const string LogFileText = nameof(LogFileText);
        /// <summary>CSV文件</summary>
        public const string CsvFileText = nameof(CsvFileText);
        /// <summary>选择曲线数据</summary>
        public const string SelectCurveDataText = nameof(SelectCurveDataText);
        /// <summary>系统供电</summary>
        public const string SystemPowerText = nameof(SystemPowerText);
        /// <summary>温控索引</summary>
        public const string ThermostatIndexText = nameof(ThermostatIndexText);
        /// <summary>独立温控监视</summary>
        public const string ThermostatMonitorText = nameof(ThermostatMonitorText);
        /// <summary>独立温控使能</summary>
        public const string ThermostatEnableText = nameof(ThermostatEnableText);
        /// <summary>独立温控使能关闭</summary>
        public const string ThermostatEnableClosedText = nameof(ThermostatEnableClosedText);
        /// <summary>比列系数(Kp)</summary>
        public const string ProportionalCoefficientText = nameof(ProportionalCoefficientText);
        /// <summary>积分系数(Ki)</summary>
        public const string IntegralCoefficientText = nameof(IntegralCoefficientText);
        /// <summary>微分系数(Kd)</summary>
        public const string DerivativeCoefficientText = nameof(DerivativeCoefficientText);
        /// <summary>设置温控系数</summary>
        public const string SetThermostatCoefficientText = nameof(SetThermostatCoefficientText);
        /// <summary>强电控制板</summary>
        public const string HVControlBoardText = nameof(HVControlBoardText);
        /// <summary>强电控制板 {0}</summary>
        public const string HVControlBoardFormatText = nameof(HVControlBoardFormatText);
        /// <summary>急停按钮</summary>
        public const string EMOText = nameof(EMOText);
        /// <summary>序号</summary>
        public const string SequenceText = nameof(SequenceText);
        /// <summary>按下</summary>
        public const string PressText = nameof(PressText);
        /// <summary>松开</summary>
        public const string ReleaseText = nameof(ReleaseText);
        /// <summary>强电状态</summary>
        public const string HVStateText = nameof(HVStateText);
        /// <summary>强电视图</summary>
        public const string HighVoltageViewText = nameof(HighVoltageViewText);
        /// <summary>门的状态</summary>
        public const string DoorStateText = nameof(DoorStateText);
        /// <summary>交流检测板</summary>
        public const string ACBoardText = nameof(ACBoardText);
        /// <summary>片选</summary>
        public const string CSText = nameof(CSText);
        /// <summary>错误行</summary>
        public const string ErrorLineText = nameof(ErrorLineText);
        /// <summary>管脚</summary>
        public const string DRText = nameof(DRText);
        /// <summary>查看列表</summary>
        public const string ViewListText = nameof(ViewListText);
        /// <summary>文件路径</summary>
        public const string FilePathText = nameof(FilePathText);
        /// <summary>添加当前日志</summary>
        public const string AppendLogFileText = nameof(AppendLogFileText);
        /// <summary>添加所有日志</summary>
        public const string AppendAllLogFileText = nameof(AppendAllLogFileText);
        /// <summary>移除当前日志</summary>
        public const string RemoveLogFileText = nameof(RemoveLogFileText);
        /// <summary>清空查看列表</summary>
        public const string ClearViewListText = nameof(ClearViewListText);
        /// <summary>老化日志</summary>
        public const string BurnLogText = nameof(BurnLogText);
        /// <summary>烘箱日志</summary>
        public const string OvenLogText = nameof(OvenLogText);
        /// <summary>数据日志</summary>
        public const string DataLogText = nameof(DataLogText);
        /// <summary>流程日志</summary>
        public const string FlowLogText = nameof(FlowLogText);
        /// <summary>系统日志</summary>
        public const string SystemLogText = nameof(SystemLogText);
        /// <summary>单区日志</summary>
        public const string ZoneLogText = nameof(ZoneLogText);
        /// <summary>设备日志</summary>
        public const string DeviceLogText = nameof(DeviceLogText);
        /// <summary>固件工具日志</summary>
        public const string FirmwareToolLogText = nameof(FirmwareToolLogText);
        /// <summary>向量工具日志</summary>
        public const string PatternToolLogText = nameof(PatternToolLogText);
        /// <summary>强电控制板日志</summary>
        public const string HVCtrlBoardLogText = nameof(HVCtrlBoardLogText);
        /// <summary>弱电控制板日志</summary>
        public const string LVCtrlBoardLogText = nameof(LVCtrlBoardLogText);
        /// <summary>竖向转接板日志</summary>
        public const string VertAdapterBoardLogText = nameof(VertAdapterBoardLogText);
        /// <summary>触发通道设置信息</summary>
        public const string TriggerChannelSettingInfoText = nameof(TriggerChannelSettingInfoText);
        /// <summary>竖向板位置</summary>
        public const string VerticalBoardDirectionText = nameof(VerticalBoardDirectionText);
        /// <summary>竖向板索引</summary>
        public const string VerticalBoardIndexText = nameof(VerticalBoardIndexText);
        /// <summary>电源编辑</summary>
        public const string DpsEditText = nameof(DpsEditText);
    }

    [MarkupExtensionReturnType(typeof(object))]
    public class LangExtension : MarkupExtension
    {
        [ConstructorArgument("PropertyName")]
        public string PropertyName { get; }

        public LangExtension()
        {
        }

        public LangExtension(string propertyName)
        {
            PropertyName = propertyName;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var binding = new System.Windows.Data.Binding(PropertyName)
            {
                Source = LangProvider.Instance,
                Mode = System.Windows.Data.BindingMode.OneWay
            };
            return binding.ProvideValue(serviceProvider);
        }
    }
}