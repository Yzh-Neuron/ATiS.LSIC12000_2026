namespace LSIC.App
{
    public sealed partial class LangProvider : LanguageProviderBase
    {
        public static LangProvider Instance { get; } = new();
        public UiLanguage CurrentLanguage => GlobalLanguageManager.CurrentLanguage;

        /// <summary>
        /// 生产模式
        /// </summary>
        public string Enum_DeviceMode_Production => GetString("Enum.DeviceMode.Production");
        /// <summary>
        /// 工程模式
        /// </summary>
        public string Enum_DeviceMode_Engineering => GetString("Enum.DeviceMode.Engineering");
        /// <summary>
        /// 管理员模式
        /// </summary>
        public string Enum_DeviceMode_Administrator => GetString("Enum.DeviceMode.Administrator");
        /// <summary>
        /// 秒
        /// </summary>
        public string Enum_TimeUnit_Second => GetString("Enum.TimeUnit.Second");
        /// <summary>
        /// 分钟
        /// </summary>
        public string Enum_TimeUnit_Minute => GetString("Enum.TimeUnit.Minute");
        /// <summary>
        /// 小时
        /// </summary>
        public string Enum_TimeUnit_Hour => GetString("Enum.TimeUnit.Hour");
        /// <summary>
        /// 天
        /// </summary>
        public string Enum_TimeUnit_Day => GetString("Enum.TimeUnit.Day");
        /// <summary>
        /// 时间
        /// </summary>
        public string Enum_ExecuteMode_Time => GetString("Enum.ExecuteMode.Time");
        /// <summary>
        /// 次数
        /// </summary>
        public string Enum_ExecuteMode_Times => GetString("Enum.ExecuteMode.Times");
        /// <summary>
        /// 初始化中
        /// </summary>
        public string Enum_ZoneState_Init => GetString("Enum.ZoneState.Init");
        /// <summary>
        /// 空闲
        /// </summary>
        public string Enum_ZoneState_Idle => GetString("Enum.ZoneState.Idle");
        /// <summary>
        /// 就绪
        /// </summary>
        public string Enum_ZoneState_Ready => GetString("Enum.ZoneState.Ready");
        /// <summary>
        /// 上电中
        /// </summary>
        public string Enum_ZoneState_PowerUp => GetString("Enum.ZoneState.PowerUp");
        /// <summary>
        /// 老化中
        /// </summary>
        public string Enum_ZoneState_Burning => GetString("Enum.ZoneState.Burning");
        /// <summary>
        /// 下电中
        /// </summary>
        public string Enum_ZoneState_PowerDown => GetString("Enum.ZoneState.PowerDown");
        /// <summary>
        /// 老化停止
        /// </summary>
        public string Enum_ZoneState_Stop => GetString("Enum.ZoneState.Stop");
        /// <summary>
        /// 传输中
        /// </summary>
        public string Enum_ZoneState_Transition => GetString("Enum.ZoneState.Transition");
        /// <summary>
        /// 设备自检中
        /// </summary>
        public string Enum_ZoneState_DeviceChecking => GetString("Enum.ZoneState.DeviceChecking");
        /// <summary>
        /// 电源自检中
        /// </summary>
        public string Enum_ZoneState_DpsChecking => GetString("Enum.ZoneState.DpsChecking");
        /// <summary>
        /// 固件升级中
        /// </summary>
        public string Enum_ZoneState_Upgrading => GetString("Enum.ZoneState.Upgrading");
        /// <summary>
        /// 离线
        /// </summary>
        public string Enum_ZoneState_Offline => GetString("Enum.ZoneState.Offline");
        /// <summary>
        /// 器件发送中
        /// </summary>
        public string Enum_ZoneState_Sending => GetString("Enum.ZoneState.Sending");
        /// <summary>
        /// 自检错误
        /// </summary>
        public string Enum_ZoneState_DeviceCheckFail => GetString("Enum.ZoneState.DeviceCheckFail");
        /// <summary>
        /// 器件配置错误
        /// </summary>
        public string Enum_ZoneState_IcError => GetString("Enum.ZoneState.IcError");
        /// <summary>
        /// 错误
        /// </summary>
        public string Enum_ZoneState_Error => GetString("Enum.ZoneState.Error");
        /// <summary>
        /// 全部载入
        /// </summary>
        public string Enum_LoadMode_Default => GetString("Enum.LoadMode.Default");
        /// <summary>
        /// 单步载入
        /// </summary>
        public string Enum_LoadMode_Step => GetString("Enum.LoadMode.Step");
        /// <summary>
        /// 空闲
        /// </summary>
        public string Enum_DutState_Idle => GetString("Enum.DutState.Idle");
        /// <summary>
        /// 测试中
        /// </summary>
        public string Enum_DutState_Test => GetString("Enum.DutState.Test");
        /// <summary>
        /// 通过
        /// </summary>
        public string Enum_DutState_Pass => GetString("Enum.DutState.Pass");
        /// <summary>
        /// 错误
        /// </summary>
        public string Enum_DutState_Fail => GetString("Enum.DutState.Fail");
        /// <summary>
        /// 忽略
        /// </summary>
        public string Enum_DutState_Ignore => GetString("Enum.DutState.Ignore");
        /// <summary>
        /// 老化板
        /// </summary>
        public string Enum_BoardType_BurnInBoard => GetString("Enum.BoardType.BurnInBoard");
        /// <summary>
        /// 工作中
        /// </summary>
        public string Enum_DpsState_Output => GetString("Enum.DpsState.Output");
        /// <summary>
        /// 过压
        /// </summary>
        public string Enum_DpsState_Ovp => GetString("Enum.DpsState.Ovp");
        /// <summary>
        /// 欠压
        /// </summary>
        public string Enum_DpsState_Uvp => GetString("Enum.DpsState.Uvp");
        /// <summary>
        /// 过流
        /// </summary>
        public string Enum_DpsState_Ocp => GetString("Enum.DpsState.Ocp");
        /// <summary>
        /// 关闭
        /// </summary>
        public string Enum_DpsState_Idle => GetString("Enum.DpsState.Idle");
        /// <summary>
        /// 向量加载错误
        /// </summary>
        public string Enum_PatternErrorType_Load => GetString("Enum.PatternErrorType.Load");
        /// <summary>
        /// 向量解码错误
        /// </summary>
        public string Enum_PatternErrorType_Decode => GetString("Enum.PatternErrorType.Decode");
        /// <summary>
        /// 向量运行超时
        /// </summary>
        public string Enum_PatternErrorType_Timeout => GetString("Enum.PatternErrorType.Timeout");
        /// <summary>
        /// 关闭
        /// </summary>
        public string Enum_TriggerOutputType_Close => GetString("Enum.TriggerOutputType.Close");
        /// <summary>
        /// 信号
        /// </summary>
        public string Enum_TriggerOutputType_Signal => GetString("Enum.TriggerOutputType.Signal");
        /// <summary>
        /// Sync
        /// </summary>
        public string Enum_TriggerOutputType_Sync => GetString("Enum.TriggerOutputType.Sync");
        /// <summary>
        /// 升级中
        /// </summary>
        public string Enum_UpgradeState_Updating => GetString("Enum.UpgradeState.Updating");
        /// <summary>
        /// 升级完成
        /// </summary>
        public string Enum_UpgradeState_Finish => GetString("Enum.UpgradeState.Finish");
        /// <summary>
        /// 擦除中
        /// </summary>
        public string Enum_UpgradeState_Erase => GetString("Enum.UpgradeState.Erase");
        /// <summary>
        /// 切换板卡失败
        /// </summary>
        public string Enum_UpgradeState_SwitchBoardFail => GetString("Enum.UpgradeState.SwitchBoardFail");
        /// <summary>
        /// 写入更新时间失败
        /// </summary>
        public string Enum_UpgradeState_WriteTimeFail => GetString("Enum.UpgradeState.WriteTimeFail");
        /// <summary>
        /// 读取FLASH ID失败
        /// </summary>
        public string Enum_UpgradeState_ReadFlashIdFail => GetString("Enum.UpgradeState.ReadFlashIdFail");
        /// <summary>
        /// 更新超时
        /// </summary>
        public string Enum_UpgradeState_TimeOut => GetString("Enum.UpgradeState.TimeOut");
        /// <summary>
        /// 其他
        /// </summary>
        public string Enum_UpgradeState_Other => GetString("Enum.UpgradeState.Other");
        /// <summary>
        /// 通信错误
        /// </summary>
        public string Enum_UpgradeState_CommunicateFail => GetString("Enum.UpgradeState.CommunicateFail");
        /// <summary>
        /// 数据校验失败
        /// </summary>
        public string Enum_UpgradeState_DataVerifyFail => GetString("Enum.UpgradeState.DataVerifyFail");
        /// <summary>
        /// 错误
        /// </summary>
        public string Enum_UpgradeState_Error => GetString("Enum.UpgradeState.Error");
        /// <summary>
        /// 主控板
        /// </summary>
        public string DeviceCheck_Type_00 => GetString("DeviceCheck.Type[00]");
        /// <summary>
        /// 底板
        /// </summary>
        public string DeviceCheck_Type_10 => GetString("DeviceCheck.Type[10]");
        /// <summary>
        /// 数字板
        /// </summary>
        public string DeviceCheck_Type_20 => GetString("DeviceCheck.Type[20]");
        /// <summary>
        /// 电源板
        /// </summary>
        public string DeviceCheck_Type_30 => GetString("DeviceCheck.Type[30]");
        /// <summary>
        /// 竖向板
        /// </summary>
        public string DeviceCheck_Type_E0 => GetString("DeviceCheck.Type[E0]");
        /// <summary>
        /// 老化板
        /// </summary>
        public string DeviceCheck_Type_EA => GetString("DeviceCheck.Type[EA]");
        /// <summary>
        /// 环境变量
        /// </summary>
        public string DeviceCheck_Item_01 => GetString("DeviceCheck.Item[01]");
        /// <summary>
        /// 配置文件
        /// </summary>
        public string DeviceCheck_Item_02 => GetString("DeviceCheck.Item[02]");
        /// <summary>
        /// EEPROM
        /// </summary>
        public string DeviceCheck_Item_03 => GetString("DeviceCheck.Item[03]");
        /// <summary>
        /// 扩展I/O
        /// </summary>
        public string DeviceCheck_Item_04 => GetString("DeviceCheck.Item[04]");
        /// <summary>
        /// 电源模块
        /// </summary>
        public string DeviceCheck_Item_05 => GetString("DeviceCheck.Item[05]");
        /// <summary>
        /// DAC自检
        /// </summary>
        public string DeviceCheck_Item_06 => GetString("DeviceCheck.Item[06]");
        /// <summary>
        /// FPGA自检
        /// </summary>
        public string DeviceCheck_Item_07 => GetString("DeviceCheck.Item[07]");
        /// <summary>
        /// 电源自检(XADC)
        /// </summary>
        public string DeviceCheck_Item_08 => GetString("DeviceCheck.Item[08]");
        /// <summary>
        /// FLASH自检
        /// </summary>
        public string DeviceCheck_Item_09 => GetString("DeviceCheck.Item[09]");
        /// <summary>
        /// PL机型自检
        /// </summary>
        public string DeviceCheck_Item_0A => GetString("DeviceCheck.Item[0A]");
        /// <summary>
        /// 温控自检
        /// </summary>
        public string DeviceCheck_Item_0B => GetString("DeviceCheck.Item[0B]");
        /// <summary>
        /// COMPARE VREF自检
        /// </summary>
        public string DeviceCheck_Item_0C => GetString("DeviceCheck.Item[0C]");
        /// <summary>
        /// Dps VIN自检
        /// </summary>
        public string DeviceCheck_Item_0D => GetString("DeviceCheck.Item[0D]");
        /// <summary>
        /// 通过
        /// </summary>
        public string DeviceCheck_Result_00 => GetString("DeviceCheck.Result[00]");
        /// <summary>
        /// 环境变量丢失
        /// </summary>
        public string DeviceCheck_Result_01 => GetString("DeviceCheck.Result[01]");
        /// <summary>
        /// 配置文件不存在
        /// </summary>
        public string DeviceCheck_Result_02 => GetString("DeviceCheck.Result[02]");
        /// <summary>
        /// 配置文件格式错误
        /// </summary>
        public string DeviceCheck_Result_03 => GetString("DeviceCheck.Result[03]");
        /// <summary>
        /// EEPROM通信错误
        /// </summary>
        public string DeviceCheck_Result_04 => GetString("DeviceCheck.Result[04]");
        /// <summary>
        /// I/O扩展芯片通信错误
        /// </summary>
        public string DeviceCheck_Result_05 => GetString("DeviceCheck.Result[05]");
        /// <summary>
        /// ADC通信错误
        /// </summary>
        public string DeviceCheck_Result_06 => GetString("DeviceCheck.Result[06]");
        /// <summary>
        /// 电源模块通信错误
        /// </summary>
        public string DeviceCheck_Result_07 => GetString("DeviceCheck.Result[07]");
        /// <summary>
        /// 电源模块混插
        /// </summary>
        public string DeviceCheck_Result_08 => GetString("DeviceCheck.Result[08]");
        /// <summary>
        /// 电源模块固件错误
        /// </summary>
        public string DeviceCheck_Result_09 => GetString("DeviceCheck.Result[09]");
        /// <summary>
        /// LVDS寄存器通信错误
        /// </summary>
        public string DeviceCheck_Result_0A => GetString("DeviceCheck.Result[0A]");
        /// <summary>
        /// LVDS数据流通信错误
        /// </summary>
        public string DeviceCheck_Result_0B => GetString("DeviceCheck.Result[0B]");
        /// <summary>
        /// 电源通信错误
        /// </summary>
        public string DeviceCheck_Result_0C => GetString("DeviceCheck.Result[0C]");
        /// <summary>
        /// 电源电压错误
        /// </summary>
        public string DeviceCheck_Result_0D => GetString("DeviceCheck.Result[0D]");
        /// <summary>
        /// FLASH选择错误(打开)
        /// </summary>
        public string DeviceCheck_Result_0E => GetString("DeviceCheck.Result[0E]");
        /// <summary>
        /// FLASH通信错误
        /// </summary>
        public string DeviceCheck_Result_0F => GetString("DeviceCheck.Result[0F]");
        /// <summary>
        /// FLASH选择错误(关闭)
        /// </summary>
        public string DeviceCheck_Result_10 => GetString("DeviceCheck.Result[10]");
        /// <summary>
        /// PL机型读取错误(AXIBUS)
        /// </summary>
        public string DeviceCheck_Result_11 => GetString("DeviceCheck.Result[11]");
        /// <summary>
        /// ADC通信错误(SPI)
        /// </summary>
        public string DeviceCheck_Result_12 => GetString("DeviceCheck.Result[12]");
        /// <summary>
        /// COMPARE VREF 打开错误
        /// </summary>
        public string DeviceCheck_Result_13 => GetString("DeviceCheck.Result[13]");
        /// <summary>
        /// Dps VIN 打开错误
        /// </summary>
        public string DeviceCheck_Result_14 => GetString("DeviceCheck.Result[14]");
        /// <summary>
        /// 未捕获到的异常, {0}
        /// </summary>
        public string UnhandledExceptionMessage => GetString("UnhandledExceptionMessage");
        /// <summary>
        /// 文件导入成功
        /// </summary>
        public string FileImportSuccessMessage => GetString("FileImportSuccessMessage");
        /// <summary>
        /// 文件导入失败
        /// </summary>
        public string FileImportFailureMessage => GetString("FileImportFailureMessage");
        /// <summary>
        /// 文件导出成功
        /// </summary>
        public string FileExportSuccessMessage => GetString("FileExportSuccessMessage");
        /// <summary>
        /// 文件导出失败
        /// </summary>
        public string FileExportFailureMessage => GetString("FileExportFailureMessage");
        /// <summary>
        /// 文件不存在
        /// </summary>
        public string FileNotExistMessage => GetString("FileNotExistMessage");
        /// <summary>
        /// 确定删除所选项目?
        /// </summary>
        public string DeleteAskMessage => GetString("DeleteAskMessage");
        /// <summary>
        /// 已存在同名器件文件, 是否覆盖?
        /// </summary>
        public string ImportIcFileExistMessage => GetString("ImportIcFileExistMessage");
        /// <summary>
        /// 存在与导入器件同名的器件 是否对导入器件重命名?
        /// </summary>
        public string ImportIcNameExistsMessage => GetString("ImportIcNameExistsMessage");
        /// <summary>
        /// 名称不可以为空或包含空格
        /// </summary>
        public string NameNotNullMessage => GetString("NameNotNullMessage");
        /// <summary>
        /// 名称不可以超过 {0} 位长度
        /// </summary>
        public string NameNotExceedMessage => GetString("NameNotExceedMessage");
        /// <summary>
        /// 名称已存在
        /// </summary>
        public string NameExistMessage => GetString("NameExistMessage");
        /// <summary>
        /// 设备配置文件解析失败, {0}
        /// </summary>
        public string DeviceConfigParseFailureMessage => GetString("DeviceConfigParseFailureMessage");
        /// <summary>
        /// 值必须介于 {0}
        /// </summary>
        public string MustBeInRangeMessage => GetString("MustBeInRangeMessage");
        /// <summary>
        /// 仅支持英文字母、数字和下划线
        /// </summary>
        public string ValidStringRuleMessage => GetString("ValidStringRuleMessage");
        /// <summary>
        /// 文件 [ {0} ] 解析失败, 第 {1} 行 {2} 列
        /// </summary>
        public string FileParseErrorMessage => GetString("FileParseErrorMessage");
        /// <summary>
        /// 文件 [ {0} ] 长度不正确, 预期长度为 {1} 字节
        /// </summary>
        public string FileLengthErrorMessage => GetString("FileLengthErrorMessage");
        /// <summary>
        /// ABM 文件 [ {0} ] 不匹配，请使用该上位机软件中的向量转换工具生成的 ABM 文件
        /// </summary>
        public string AbmFileNotMatchMessage => GetString("AbmFileNotMatchMessage");
        /// <summary>
        /// 报文解析错误，{0}
        /// </summary>
        public string AnalysisFrameErrorMessage => GetString("AnalysisFrameErrorMessage");
        /// <summary>
        /// CRC32不匹配，{0}
        /// </summary>
        public string Crc32NotMatchMessage => GetString("Crc32NotMatchMessage");
        /// <summary>
        /// 帧尾不匹配
        /// </summary>
        public string FrameEndingNotMatchMessage => GetString("FrameEndingNotMatchMessage");
        /// <summary>
        /// 老化板插入
        /// </summary>
        public string PluggedMessage => GetString("PluggedMessage");
        /// <summary>
        /// 老化板卸下
        /// </summary>
        public string UnpluggedMessage => GetString("UnpluggedMessage");
        /// <summary>
        /// 请设置 [ {0} ]
        /// </summary>
        public string PleaseSetMessage => GetString("PleaseSetMessage");
        /// <summary>
        /// 至少需要配置一个步次
        /// </summary>
        public string AtLeastOneStepMessage => GetString("AtLeastOneStepMessage");
        /// <summary>
        /// 至少需要配置一个子步次
        /// </summary>
        public string AtLeastOneSubStepMessage => GetString("AtLeastOneSubStepMessage");
        /// <summary>
        /// 当前状态 [ {0} ]
        /// </summary>
        public string StateMessage => GetString("StateMessage");
        /// <summary>
        /// 器件配置错误, 请检查后重试! {0}
        /// </summary>
        public string IcErrorMessage => GetString("IcErrorMessage");
        /// <summary>
        /// 发送器件数据 [ {0} ]
        /// </summary>
        public string SendIcDataMessage => GetString("SendIcDataMessage");
        /// <summary>
        /// 不支持该数据文件
        /// </summary>
        public string UnsupportedDataFileMessage => GetString("UnsupportedDataFileMessage");
        /// <summary>
        /// 请先勾选要升级的老化区
        /// </summary>
        public string UpgradeSelectZoneMessage => GetString("UpgradeSelectZoneMessage");
        /// <summary>
        /// 请先为要升级的板卡类型选择固件文件
        /// </summary>
        public string UpgradeSelectFirmwareMessage => GetString("UpgradeSelectFirmwareMessage");
        /// <summary>
        /// 固件文件名必须以 {0} 开头，请确认选择的是该类型固件
        /// </summary>
        public string UpgradeFilePrefixMismatchMessage => GetString("UpgradeFilePrefixMismatchMessage");
        /// <summary>
        /// 电源自检脚本: 找不到目标电源，编号{0}，位于{1}行
        /// </summary>
        public string DpsCheckScriptDpsNotFoundMessage => GetString("DpsCheckScriptDpsNotFoundMessage");
        /// <summary>
        /// 电源自检脚本: 参数错误或数值超出范围，位于{0}行
        /// </summary>
        public string DpsCheckScriptParameterErrorMessage => GetString("DpsCheckScriptParameterErrorMessage");
        /// <summary>
        /// 当前状态不允许运行电源自检
        /// </summary>
        public string DpsCheckIsNotPermittedMessage => GetString("DpsCheckIsNotPermittedMessage");
        /// <summary>
        /// 检测到当前区已插老化板，运行电源自检可能会烧毁器件 是否继续 ?
        /// </summary>
        public string DpsCheckAskMessage => GetString("DpsCheckAskMessage");
        /// <summary>
        /// 电源错误:{0} -> [{1}]
        /// </summary>
        public string ZoneDpsErrorMessage => GetString("ZoneDpsErrorMessage");
        /// <summary>
        /// 电源错误: VIH{0} -> [{1}]
        /// </summary>
        public string ZoneVIHErrorMessage => GetString("ZoneVIHErrorMessage");
        /// <summary>
        /// 向量错误: [{0}] 请检查第{1}步 第{2}组向量配置
        /// </summary>
        public string ZonePatternErrorMessage => GetString("ZonePatternErrorMessage");
        /// <summary>
        /// 设备异常: 请联系售后人员! 错误码[{0}]
        /// </summary>
        public string ZoneDeviceErrorMessage => GetString("ZoneDeviceErrorMessage");
        /// <summary>
        /// 当前步次: [{0}]	向量: [{1}]
        /// </summary>
        public string StepRunInfoMessage => GetString("StepRunInfoMessage");
        /// <summary>
        /// 运行电源自检脚本: {0}
        /// </summary>
        public string RunDpsCheckScriptMessage => GetString("RunDpsCheckScriptMessage");
        /// <summary>
        /// 电源自检[{0}/{1}] ->{2}    {3}V    {4}A    ({5}/{6})
        /// </summary>
        public string DpsCheckResultMessage => GetString("DpsCheckResultMessage");
        /// <summary>
        /// 触发通道{0}设置成功
        /// </summary>
        public string TriggerDrSetSuccessMessage => GetString("TriggerDrSetSuccessMessage");
        /// <summary>
        /// 见微知著·科技赋能·创造非凡价值
        /// </summary>
        public string ConceptText => GetString("ConceptText");
        /// <summary>
        /// 确认
        /// </summary>
        public string ConfirmText => GetString("ConfirmText");
        /// <summary>
        /// 取消
        /// </summary>
        public string CancelText => GetString("CancelText");
        /// <summary>
        /// 新建
        /// </summary>
        public string CreateNewText => GetString("CreateNewText");
        /// <summary>
        /// 重命名
        /// </summary>
        public string RenameText => GetString("RenameText");
        /// <summary>
        /// 保存
        /// </summary>
        public string SaveText => GetString("SaveText");
        /// <summary>
        /// 另存为
        /// </summary>
        public string SaveAsText => GetString("SaveAsText");
        /// <summary>
        /// 删除
        /// </summary>
        public string DeleteText => GetString("DeleteText");
        /// <summary>
        /// 导入
        /// </summary>
        public string ImportText => GetString("ImportText");
        /// <summary>
        /// 导出
        /// </summary>
        public string ExportText => GetString("ExportText");
        /// <summary>
        /// 查看
        /// </summary>
        public string ViewText => GetString("ViewText");
        /// <summary>
        /// 编辑
        /// </summary>
        public string EditText => GetString("EditText");
        /// <summary>
        /// 名称
        /// </summary>
        public string NameText => GetString("NameText");
        /// <summary>
        /// 关于
        /// </summary>
        public string AboutText => GetString("AboutText");
        /// <summary>
        /// 全选
        /// </summary>
        public string AllSelectText => GetString("AllSelectText");
        /// <summary>
        /// 帮助(_H)
        /// </summary>
        public string HelpMenuText => GetString("HelpMenuText");
        /// <summary>
        /// 刷新
        /// </summary>
        public string RefreshText => GetString("RefreshText");
        /// <summary>
        /// 区
        /// </summary>
        public string ZoneText => GetString("ZoneText");
        /// <summary>
        /// {0} 区
        /// </summary>
        public string ZoneFormatText => GetString("ZoneFormatText");
        /// <summary>
        /// 状态
        /// </summary>
        public string StateText => GetString("StateText");
        /// <summary>
        /// 类型
        /// </summary>
        public string TypeText => GetString("TypeText");
        /// <summary>
        /// 规格
        /// </summary>
        public string SpecText => GetString("SpecText");
        /// <summary>
        /// 工位
        /// </summary>
        public string DutText => GetString("DutText");
        /// <summary>
        /// 行
        /// </summary>
        public string RowText => GetString("RowText");
        /// <summary>
        /// 列
        /// </summary>
        public string ColumnText => GetString("ColumnText");
        /// <summary>
        /// 所属片选
        /// </summary>
        public string BelongChipSelectText => GetString("BelongChipSelectText");
        /// <summary>
        /// 使用的电源
        /// </summary>
        public string UsedDpsText => GetString("UsedDpsText");
        /// <summary>
        /// 关联的管脚
        /// </summary>
        public string LinkedDrText => GetString("LinkedDrText");
        /// <summary>
        /// 管脚
        /// </summary>
        public string DrText => GetString("DrText");
        /// <summary>
        /// 片选
        /// </summary>
        public string CSText => GetString("CSText");
        /// <summary>
        /// 器件名称
        /// </summary>
        public string IcNameText => GetString("IcNameText");
        /// <summary>
        /// 器件编辑
        /// </summary>
        public string IcEditText => GetString("IcEditText");
        /// <summary>
        /// 当前器件
        /// </summary>
        public string CurrentIcText => GetString("CurrentIcText");
        /// <summary>
        /// 向量格式转换
        /// </summary>
        public string PatternFormatConvertText => GetString("PatternFormatConvertText");
        /// <summary>
        /// 曲线查看
        /// </summary>
        public string ChartViewText => GetString("ChartViewText");
        /// <summary>
        /// 触发设置
        /// </summary>
        public string TriggerSetText => GetString("TriggerSetText");
        /// <summary>
        /// 发送
        /// </summary>
        public string SendParameterText => GetString("SendParameterText");
        /// <summary>
        /// 开始
        /// </summary>
        public string StartBurnText => GetString("StartBurnText");
        /// <summary>
        /// 终止
        /// </summary>
        public string StopBurnText => GetString("StopBurnText");
        /// <summary>
        /// 进入老化界面
        /// </summary>
        public string EnterBurnViewText => GetString("EnterBurnViewText");
        /// <summary>
        /// 重置
        /// </summary>
        public string ReSelectIcText => GetString("ReSelectIcText");
        /// <summary>
        /// 关闭报警
        /// </summary>
        public string CloseAlarmText => GetString("CloseAlarmText");
        /// <summary>
        /// 器件库
        /// </summary>
        public string IcLibraryText => GetString("IcLibraryText");
        /// <summary>
        /// 运行策略
        /// </summary>
        public string RunStrategyText => GetString("RunStrategyText");
        /// <summary>
        /// 运行模式
        /// </summary>
        public string RunModeText => GetString("RunModeText");
        /// <summary>
        /// 循环策略
        /// </summary>
        public string LoopStrategyText => GetString("LoopStrategyText");
        /// <summary>
        /// 循环运行中
        /// </summary>
        public string RunInTheLoopText => GetString("RunInTheLoopText");
        /// <summary>
        /// 温度(℃)
        /// </summary>
        public string TemperatureWithUnitText => GetString("TemperatureWithUnitText");
        /// <summary>
        /// 报警信息
        /// </summary>
        public string AlarmMessageText => GetString("AlarmMessageText");
        /// <summary>
        /// 报警窗口
        /// </summary>
        public string AlarmWindowText => GetString("AlarmWindowText");
        /// <summary>
        /// 关闭烘箱报警
        /// </summary>
        public string CloseOvenAlarmText => GetString("CloseOvenAlarmText");
        /// <summary>
        /// 一键降温
        /// </summary>
        public string OneClickCoolText => GetString("OneClickCoolText");
        /// <summary>
        /// 温控索引
        /// </summary>
        public string ThermostatIndexText => GetString("ThermostatIndexText");
        /// <summary>
        /// 独立温控监视
        /// </summary>
        public string ThermostatMonitorText => GetString("ThermostatMonitorText");
        /// <summary>
        /// 温控使能
        /// </summary>
        public string ThermostatEnableText => GetString("ThermostatEnableText");
        /// <summary>
        /// 电源
        /// </summary>
        public string DpsText => GetString("DpsText");
        /// <summary>
        /// 设置电压
        /// </summary>
        public string SetVoltageText => GetString("SetVoltageText");
        /// <summary>
        /// 设置电压(V)
        /// </summary>
        public string SetVoltageWithUnitText => GetString("SetVoltageWithUnitText");
        /// <summary>
        /// 实时电压(V)
        /// </summary>
        public string RealVoltageWithUnitText => GetString("RealVoltageWithUnitText");
        /// <summary>
        /// 实时电流(A)
        /// </summary>
        public string RealCurrentWithUnitText => GetString("RealCurrentWithUnitText");
        /// <summary>
        /// 实时功率(W)
        /// </summary>
        public string RealPowerWithUnitText => GetString("RealPowerWithUnitText");
        /// <summary>
        /// 电压下限
        /// </summary>
        public string VoltageFloorText => GetString("VoltageFloorText");
        /// <summary>
        /// 电压下限(V)
        /// </summary>
        public string VoltageFloorWithUnitText => GetString("VoltageFloorWithUnitText");
        /// <summary>
        /// 电压上限
        /// </summary>
        public string VoltageCeilingText => GetString("VoltageCeilingText");
        /// <summary>
        /// 电压上限(V)
        /// </summary>
        public string VoltageCeilingWithUnitText => GetString("VoltageCeilingWithUnitText");
        /// <summary>
        /// 电流上限
        /// </summary>
        public string CurrentCeilingText => GetString("CurrentCeilingText");
        /// <summary>
        /// 电流上限(A)
        /// </summary>
        public string CurrentCeilingWithUnitText => GetString("CurrentCeilingWithUnitText");
        /// <summary>
        /// 上升时间
        /// </summary>
        public string RaiseTimeText => GetString("RaiseTimeText");
        /// <summary>
        /// 上升时间(ms)
        /// </summary>
        public string RaiseTimeWithUnitText => GetString("RaiseTimeWithUnitText");
        /// <summary>
        /// 延时时间
        /// </summary>
        public string DelayText => GetString("DelayText");
        /// <summary>
        /// 延时时间(ms)
        /// </summary>
        public string DelayWithUnitText => GetString("DelayWithUnitText");
        /// <summary>
        /// 电源设置
        /// </summary>
        public string DpsSettingText => GetString("DpsSettingText");
        /// <summary>
        /// 电源方案
        /// </summary>
        public string DpsPlanText => GetString("DpsPlanText");
        /// <summary>
        /// 电源编辑
        /// </summary>
        public string DpsEditText => GetString("DpsEditText");
        /// <summary>
        /// 基础信息
        /// </summary>
        public string BaseInfoText => GetString("BaseInfoText");
        /// <summary>
        /// 基础设置
        /// </summary>
        public string BaseSettingText => GetString("BaseSettingText");
        /// <summary>
        /// 流程设置
        /// </summary>
        public string ProcessSettingText => GetString("ProcessSettingText");
        /// <summary>
        /// 步次
        /// </summary>
        public string StepText => GetString("StepText");
        /// <summary>
        /// 步次[{0}]
        /// </summary>
        public string StepFormatText => GetString("StepFormatText");
        /// <summary>
        /// 运行时间
        /// </summary>
        public string RunTimeText => GetString("RunTimeText");
        /// <summary>
        /// 运行次数
        /// </summary>
        public string RunTimesText => GetString("RunTimesText");
        /// <summary>
        /// 循环模式
        /// </summary>
        public string LoopModeText => GetString("LoopModeText");
        /// <summary>
        /// 循环时间
        /// </summary>
        public string LoopTimeText => GetString("LoopTimeText");
        /// <summary>
        /// 循环次数
        /// </summary>
        public string LoopTimesText => GetString("LoopTimesText");
        /// <summary>
        /// 向量设置
        /// </summary>
        public string VectorSettingText => GetString("VectorSettingText");
        /// <summary>
        /// 向量文件
        /// </summary>
        public string PatternFileText => GetString("PatternFileText");
        /// <summary>
        /// 寄存器文件
        /// </summary>
        public string RegisterFileText => GetString("RegisterFileText");
        /// <summary>
        /// 器件文件
        /// </summary>
        public string IcFileText => GetString("IcFileText");
        /// <summary>
        /// 向量超时
        /// </summary>
        public string PatternTimeoutText => GetString("PatternTimeoutText");
        /// <summary>
        /// 当前向量
        /// </summary>
        public string CurrentPatternText => GetString("CurrentPatternText");
        /// <summary>
        /// 当前向量剩余时间
        /// </summary>
        public string CurrentPatternLeftTimeText => GetString("CurrentPatternLeftTimeText");
        /// <summary>
        /// 当前向量运行时间
        /// </summary>
        public string CurrentPatternCostTimeText => GetString("CurrentPatternCostTimeText");
        /// <summary>
        /// 当前向量设定时间
        /// </summary>
        public string CurrentPatternSetTimeText => GetString("CurrentPatternSetTimeText");
        /// <summary>
        /// 当前步次
        /// </summary>
        public string CurrentStepText => GetString("CurrentStepText");
        /// <summary>
        /// 当前寄存器
        /// </summary>
        public string CurrentRegistersText => GetString("CurrentRegistersText");
        /// <summary>
        /// 当前电源方案
        /// </summary>
        public string CurrentDpsPlanText => GetString("CurrentDpsPlanText");
        /// <summary>
        /// 上电后总运行时间
        /// </summary>
        public string TotalTimeBeforeDpsUpText => GetString("TotalTimeBeforeDpsUpText");
        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartTimeText => GetString("StartTimeText");
        /// <summary>
        /// 数量
        /// </summary>
        public string QuantityText => GetString("QuantityText");
        /// <summary>
        /// 时间
        /// </summary>
        public string TimeText => GetString("TimeText");
        /// <summary>
        /// 修改时间
        /// </summary>
        public string UpdateTimeText => GetString("UpdateTimeText");
        /// <summary>
        /// 次
        /// </summary>
        public string TimesText => GetString("TimesText");
        /// <summary>
        /// 批次号
        /// </summary>
        public string LotText => GetString("LotText");
        /// <summary>
        /// 操作员
        /// </summary>
        public string OperatorText => GetString("OperatorText");
        /// <summary>
        /// 载入模式
        /// </summary>
        public string LoadModeText => GetString("LoadModeText");
        /// <summary>
        /// 强制单步载入
        /// </summary>
        public string ForceStepLoadText => GetString("ForceStepLoadText");
        /// <summary>
        /// 单位
        /// </summary>
        public string UnitText => GetString("UnitText");
        /// <summary>
        /// 工位配置文件
        /// </summary>
        public string DutConfigFileText => GetString("DutConfigFileText");
        /// <summary>
        /// 日志
        /// </summary>
        public string LogText => GetString("LogText");
        /// <summary>
        /// 数据日志
        /// </summary>
        public string DataLogText => GetString("DataLogText");
        /// <summary>
        /// 数据记录间隔
        /// </summary>
        public string LogIntervalText => GetString("LogIntervalText");
        /// <summary>
        /// 报错记录间隔
        /// </summary>
        public string ErrorLogIntervalText => GetString("ErrorLogIntervalText");
        /// <summary>
        /// 清空记录
        /// </summary>
        public string ClearLogsText => GetString("ClearLogsText");
        /// <summary>
        /// 帧过滤器
        /// </summary>
        public string FrameFilterText => GetString("FrameFilterText");
        /// <summary>
        /// 查看列表
        /// </summary>
        public string ViewListText => GetString("ViewListText");
        /// <summary>
        /// CSV文件
        /// </summary>
        public string CsvFileText => GetString("CsvFileText");
        /// <summary>
        /// 数据文件
        /// </summary>
        public string DataFileText => GetString("DataFileText");
        /// <summary>
        /// 日期筛选
        /// </summary>
        public string DateFilterText => GetString("DateFilterText");
        /// <summary>
        /// 筛选
        /// </summary>
        public string FilterText => GetString("FilterText");
        /// <summary>
        /// 首页
        /// </summary>
        public string FirstPageText => GetString("FirstPageText");
        /// <summary>
        /// 上一页
        /// </summary>
        public string PreviousPageText => GetString("PreviousPageText");
        /// <summary>
        /// 下一页
        /// </summary>
        public string NextPageText => GetString("NextPageText");
        /// <summary>
        /// 尾页
        /// </summary>
        public string LastPageText => GetString("LastPageText");
        /// <summary>
        /// 导出为CSV文件
        /// </summary>
        public string ExportAsCsvFileText => GetString("ExportAsCsvFileText");
        /// <summary>
        /// 批量导出
        /// </summary>
        public string BatchExportText => GetString("BatchExportText");
        /// <summary>
        /// 错误行
        /// </summary>
        public string ErrorLineText => GetString("ErrorLineText");
        /// <summary>
        /// 板卡索引
        /// </summary>
        public string BoardIndexText => GetString("BoardIndexText");
        /// <summary>
        /// 固件版本
        /// </summary>
        public string FirmwareVersionText => GetString("FirmwareVersionText");
        /// <summary>
        /// 硬件版本
        /// </summary>
        public string HardwareVersionText => GetString("HardwareVersionText");
        /// <summary>
        /// 设备信息
        /// </summary>
        public string DeviceInfoText => GetString("DeviceInfoText");
        /// <summary>
        /// 查询版本信息
        /// </summary>
        public string GetVersionInfoText => GetString("GetVersionInfoText");
        /// <summary>
        /// 导出版本信息 (所有区)
        /// </summary>
        public string ExportVersionInfoText => GetString("ExportVersionInfoText");
        /// <summary>
        /// 板卡类型
        /// </summary>
        public string BoardTypeText => GetString("BoardTypeText");
        /// <summary>
        /// 固件
        /// </summary>
        public string FirmwareText => GetString("FirmwareText");
        /// <summary>
        /// 固件升级
        /// </summary>
        public string FirmwareUpgradeText => GetString("FirmwareUpgradeText");
        /// <summary>
        /// 升级
        /// </summary>
        public string UpgradeText => GetString("UpgradeText");
        /// <summary>
        /// 注意：升级过程中请勿关闭上位机或设备电源！
        /// </summary>
        public string FirmwareUpgradePromptText => GetString("FirmwareUpgradePromptText");
        /// <summary>
        /// 结果
        /// </summary>
        public string ResultText => GetString("ResultText");
        /// <summary>
        /// 设备自检
        /// </summary>
        public string DeviceCheckText => GetString("DeviceCheckText");
        /// <summary>
        /// 开始自检
        /// </summary>
        public string StartDeviceCheckText => GetString("StartDeviceCheckText");
        /// <summary>
        /// 自检结果
        /// </summary>
        public string DeviceCheckResultText => GetString("DeviceCheckResultText");
        /// <summary>
        /// 电源自检
        /// </summary>
        public string DpsCheckText => GetString("DpsCheckText");
        /// <summary>
        /// 电源自检脚本
        /// </summary>
        public string DpsCheckScriptText => GetString("DpsCheckScriptText");
        /// <summary>
        /// 运行脚本
        /// </summary>
        public string RunScriptText => GetString("RunScriptText");
        /// <summary>
        /// 选择文件
        /// </summary>
        public string SelectFileText => GetString("SelectFileText");
        /// <summary>
        /// 工位屏蔽
        /// </summary>
        public string DutMaskText => GetString("DutMaskText");

        protected override void OnLanguageChanged()
        {
            OnPropertyChanged(nameof(CurrentLanguage));
            OnPropertyChanged(nameof(Enum_DeviceMode_Production));
            OnPropertyChanged(nameof(Enum_DeviceMode_Engineering));
            OnPropertyChanged(nameof(Enum_DeviceMode_Administrator));
            OnPropertyChanged(nameof(Enum_TimeUnit_Second));
            OnPropertyChanged(nameof(Enum_TimeUnit_Minute));
            OnPropertyChanged(nameof(Enum_TimeUnit_Hour));
            OnPropertyChanged(nameof(Enum_TimeUnit_Day));
            OnPropertyChanged(nameof(Enum_ExecuteMode_Time));
            OnPropertyChanged(nameof(Enum_ExecuteMode_Times));
            OnPropertyChanged(nameof(Enum_ZoneState_Init));
            OnPropertyChanged(nameof(Enum_ZoneState_Idle));
            OnPropertyChanged(nameof(Enum_ZoneState_Ready));
            OnPropertyChanged(nameof(Enum_ZoneState_PowerUp));
            OnPropertyChanged(nameof(Enum_ZoneState_Burning));
            OnPropertyChanged(nameof(Enum_ZoneState_PowerDown));
            OnPropertyChanged(nameof(Enum_ZoneState_Stop));
            OnPropertyChanged(nameof(Enum_ZoneState_Transition));
            OnPropertyChanged(nameof(Enum_ZoneState_DeviceChecking));
            OnPropertyChanged(nameof(Enum_ZoneState_DpsChecking));
            OnPropertyChanged(nameof(Enum_ZoneState_Upgrading));
            OnPropertyChanged(nameof(Enum_ZoneState_Offline));
            OnPropertyChanged(nameof(Enum_ZoneState_Sending));
            OnPropertyChanged(nameof(Enum_ZoneState_DeviceCheckFail));
            OnPropertyChanged(nameof(Enum_ZoneState_IcError));
            OnPropertyChanged(nameof(Enum_ZoneState_Error));
            OnPropertyChanged(nameof(Enum_LoadMode_Default));
            OnPropertyChanged(nameof(Enum_LoadMode_Step));
            OnPropertyChanged(nameof(Enum_DutState_Idle));
            OnPropertyChanged(nameof(Enum_DutState_Test));
            OnPropertyChanged(nameof(Enum_DutState_Pass));
            OnPropertyChanged(nameof(Enum_DutState_Fail));
            OnPropertyChanged(nameof(Enum_DutState_Ignore));
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
            OnPropertyChanged(nameof(DeviceCheck_Type_00));
            OnPropertyChanged(nameof(DeviceCheck_Type_10));
            OnPropertyChanged(nameof(DeviceCheck_Type_20));
            OnPropertyChanged(nameof(DeviceCheck_Type_30));
            OnPropertyChanged(nameof(DeviceCheck_Type_E0));
            OnPropertyChanged(nameof(DeviceCheck_Type_EA));
            OnPropertyChanged(nameof(DeviceCheck_Item_01));
            OnPropertyChanged(nameof(DeviceCheck_Item_02));
            OnPropertyChanged(nameof(DeviceCheck_Item_03));
            OnPropertyChanged(nameof(DeviceCheck_Item_04));
            OnPropertyChanged(nameof(DeviceCheck_Item_05));
            OnPropertyChanged(nameof(DeviceCheck_Item_06));
            OnPropertyChanged(nameof(DeviceCheck_Item_07));
            OnPropertyChanged(nameof(DeviceCheck_Item_08));
            OnPropertyChanged(nameof(DeviceCheck_Item_09));
            OnPropertyChanged(nameof(DeviceCheck_Item_0A));
            OnPropertyChanged(nameof(DeviceCheck_Item_0B));
            OnPropertyChanged(nameof(DeviceCheck_Item_0C));
            OnPropertyChanged(nameof(DeviceCheck_Item_0D));
            OnPropertyChanged(nameof(DeviceCheck_Result_00));
            OnPropertyChanged(nameof(DeviceCheck_Result_01));
            OnPropertyChanged(nameof(DeviceCheck_Result_02));
            OnPropertyChanged(nameof(DeviceCheck_Result_03));
            OnPropertyChanged(nameof(DeviceCheck_Result_04));
            OnPropertyChanged(nameof(DeviceCheck_Result_05));
            OnPropertyChanged(nameof(DeviceCheck_Result_06));
            OnPropertyChanged(nameof(DeviceCheck_Result_07));
            OnPropertyChanged(nameof(DeviceCheck_Result_08));
            OnPropertyChanged(nameof(DeviceCheck_Result_09));
            OnPropertyChanged(nameof(DeviceCheck_Result_0A));
            OnPropertyChanged(nameof(DeviceCheck_Result_0B));
            OnPropertyChanged(nameof(DeviceCheck_Result_0C));
            OnPropertyChanged(nameof(DeviceCheck_Result_0D));
            OnPropertyChanged(nameof(DeviceCheck_Result_0E));
            OnPropertyChanged(nameof(DeviceCheck_Result_0F));
            OnPropertyChanged(nameof(DeviceCheck_Result_10));
            OnPropertyChanged(nameof(DeviceCheck_Result_11));
            OnPropertyChanged(nameof(DeviceCheck_Result_12));
            OnPropertyChanged(nameof(DeviceCheck_Result_13));
            OnPropertyChanged(nameof(DeviceCheck_Result_14));
            OnPropertyChanged(nameof(UnhandledExceptionMessage));
            OnPropertyChanged(nameof(FileImportSuccessMessage));
            OnPropertyChanged(nameof(FileImportFailureMessage));
            OnPropertyChanged(nameof(FileExportSuccessMessage));
            OnPropertyChanged(nameof(FileExportFailureMessage));
            OnPropertyChanged(nameof(FileNotExistMessage));
            OnPropertyChanged(nameof(DeleteAskMessage));
            OnPropertyChanged(nameof(ImportIcFileExistMessage));
            OnPropertyChanged(nameof(ImportIcNameExistsMessage));
            OnPropertyChanged(nameof(NameNotNullMessage));
            OnPropertyChanged(nameof(NameNotExceedMessage));
            OnPropertyChanged(nameof(NameExistMessage));
            OnPropertyChanged(nameof(DeviceConfigParseFailureMessage));
            OnPropertyChanged(nameof(MustBeInRangeMessage));
            OnPropertyChanged(nameof(ValidStringRuleMessage));
            OnPropertyChanged(nameof(FileParseErrorMessage));
            OnPropertyChanged(nameof(FileLengthErrorMessage));
            OnPropertyChanged(nameof(AbmFileNotMatchMessage));
            OnPropertyChanged(nameof(AnalysisFrameErrorMessage));
            OnPropertyChanged(nameof(Crc32NotMatchMessage));
            OnPropertyChanged(nameof(FrameEndingNotMatchMessage));
            OnPropertyChanged(nameof(PluggedMessage));
            OnPropertyChanged(nameof(UnpluggedMessage));
            OnPropertyChanged(nameof(PleaseSetMessage));
            OnPropertyChanged(nameof(AtLeastOneStepMessage));
            OnPropertyChanged(nameof(AtLeastOneSubStepMessage));
            OnPropertyChanged(nameof(StateMessage));
            OnPropertyChanged(nameof(IcErrorMessage));
            OnPropertyChanged(nameof(SendIcDataMessage));
            OnPropertyChanged(nameof(UnsupportedDataFileMessage));
            OnPropertyChanged(nameof(UpgradeSelectZoneMessage));
            OnPropertyChanged(nameof(UpgradeSelectFirmwareMessage));
            OnPropertyChanged(nameof(UpgradeFilePrefixMismatchMessage));
            OnPropertyChanged(nameof(DpsCheckScriptDpsNotFoundMessage));
            OnPropertyChanged(nameof(DpsCheckScriptParameterErrorMessage));
            OnPropertyChanged(nameof(DpsCheckIsNotPermittedMessage));
            OnPropertyChanged(nameof(DpsCheckAskMessage));
            OnPropertyChanged(nameof(ZoneDpsErrorMessage));
            OnPropertyChanged(nameof(ZoneVIHErrorMessage));
            OnPropertyChanged(nameof(ZonePatternErrorMessage));
            OnPropertyChanged(nameof(ZoneDeviceErrorMessage));
            OnPropertyChanged(nameof(StepRunInfoMessage));
            OnPropertyChanged(nameof(RunDpsCheckScriptMessage));
            OnPropertyChanged(nameof(DpsCheckResultMessage));
            OnPropertyChanged(nameof(TriggerDrSetSuccessMessage));
            OnPropertyChanged(nameof(ConceptText));
            OnPropertyChanged(nameof(ConfirmText));
            OnPropertyChanged(nameof(CancelText));
            OnPropertyChanged(nameof(CreateNewText));
            OnPropertyChanged(nameof(RenameText));
            OnPropertyChanged(nameof(SaveText));
            OnPropertyChanged(nameof(SaveAsText));
            OnPropertyChanged(nameof(DeleteText));
            OnPropertyChanged(nameof(ImportText));
            OnPropertyChanged(nameof(ExportText));
            OnPropertyChanged(nameof(ViewText));
            OnPropertyChanged(nameof(EditText));
            OnPropertyChanged(nameof(NameText));
            OnPropertyChanged(nameof(AboutText));
            OnPropertyChanged(nameof(AllSelectText));
            OnPropertyChanged(nameof(HelpMenuText));
            OnPropertyChanged(nameof(RefreshText));
            OnPropertyChanged(nameof(ZoneText));
            OnPropertyChanged(nameof(ZoneFormatText));
            OnPropertyChanged(nameof(StateText));
            OnPropertyChanged(nameof(TypeText));
            OnPropertyChanged(nameof(SpecText));
            OnPropertyChanged(nameof(DutText));
            OnPropertyChanged(nameof(RowText));
            OnPropertyChanged(nameof(ColumnText));
            OnPropertyChanged(nameof(BelongChipSelectText));
            OnPropertyChanged(nameof(UsedDpsText));
            OnPropertyChanged(nameof(LinkedDrText));
            OnPropertyChanged(nameof(DrText));
            OnPropertyChanged(nameof(CSText));
            OnPropertyChanged(nameof(IcNameText));
            OnPropertyChanged(nameof(IcEditText));
            OnPropertyChanged(nameof(CurrentIcText));
            OnPropertyChanged(nameof(PatternFormatConvertText));
            OnPropertyChanged(nameof(ChartViewText));
            OnPropertyChanged(nameof(TriggerSetText));
            OnPropertyChanged(nameof(SendParameterText));
            OnPropertyChanged(nameof(StartBurnText));
            OnPropertyChanged(nameof(StopBurnText));
            OnPropertyChanged(nameof(EnterBurnViewText));
            OnPropertyChanged(nameof(ReSelectIcText));
            OnPropertyChanged(nameof(CloseAlarmText));
            OnPropertyChanged(nameof(IcLibraryText));
            OnPropertyChanged(nameof(RunStrategyText));
            OnPropertyChanged(nameof(RunModeText));
            OnPropertyChanged(nameof(LoopStrategyText));
            OnPropertyChanged(nameof(RunInTheLoopText));
            OnPropertyChanged(nameof(TemperatureWithUnitText));
            OnPropertyChanged(nameof(AlarmMessageText));
            OnPropertyChanged(nameof(AlarmWindowText));
            OnPropertyChanged(nameof(CloseOvenAlarmText));
            OnPropertyChanged(nameof(OneClickCoolText));
            OnPropertyChanged(nameof(ThermostatIndexText));
            OnPropertyChanged(nameof(ThermostatMonitorText));
            OnPropertyChanged(nameof(ThermostatEnableText));
            OnPropertyChanged(nameof(DpsText));
            OnPropertyChanged(nameof(SetVoltageText));
            OnPropertyChanged(nameof(SetVoltageWithUnitText));
            OnPropertyChanged(nameof(RealVoltageWithUnitText));
            OnPropertyChanged(nameof(RealCurrentWithUnitText));
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
            OnPropertyChanged(nameof(DpsSettingText));
            OnPropertyChanged(nameof(DpsPlanText));
            OnPropertyChanged(nameof(DpsEditText));
            OnPropertyChanged(nameof(BaseInfoText));
            OnPropertyChanged(nameof(BaseSettingText));
            OnPropertyChanged(nameof(ProcessSettingText));
            OnPropertyChanged(nameof(StepText));
            OnPropertyChanged(nameof(StepFormatText));
            OnPropertyChanged(nameof(RunTimeText));
            OnPropertyChanged(nameof(RunTimesText));
            OnPropertyChanged(nameof(LoopModeText));
            OnPropertyChanged(nameof(LoopTimeText));
            OnPropertyChanged(nameof(LoopTimesText));
            OnPropertyChanged(nameof(VectorSettingText));
            OnPropertyChanged(nameof(PatternFileText));
            OnPropertyChanged(nameof(RegisterFileText));
            OnPropertyChanged(nameof(IcFileText));
            OnPropertyChanged(nameof(PatternTimeoutText));
            OnPropertyChanged(nameof(CurrentPatternText));
            OnPropertyChanged(nameof(CurrentPatternLeftTimeText));
            OnPropertyChanged(nameof(CurrentPatternCostTimeText));
            OnPropertyChanged(nameof(CurrentPatternSetTimeText));
            OnPropertyChanged(nameof(CurrentStepText));
            OnPropertyChanged(nameof(CurrentRegistersText));
            OnPropertyChanged(nameof(CurrentDpsPlanText));
            OnPropertyChanged(nameof(TotalTimeBeforeDpsUpText));
            OnPropertyChanged(nameof(StartTimeText));
            OnPropertyChanged(nameof(QuantityText));
            OnPropertyChanged(nameof(TimeText));
            OnPropertyChanged(nameof(UpdateTimeText));
            OnPropertyChanged(nameof(TimesText));
            OnPropertyChanged(nameof(LotText));
            OnPropertyChanged(nameof(OperatorText));
            OnPropertyChanged(nameof(LoadModeText));
            OnPropertyChanged(nameof(ForceStepLoadText));
            OnPropertyChanged(nameof(UnitText));
            OnPropertyChanged(nameof(DutConfigFileText));
            OnPropertyChanged(nameof(LogText));
            OnPropertyChanged(nameof(DataLogText));
            OnPropertyChanged(nameof(LogIntervalText));
            OnPropertyChanged(nameof(ErrorLogIntervalText));
            OnPropertyChanged(nameof(ClearLogsText));
            OnPropertyChanged(nameof(FrameFilterText));
            OnPropertyChanged(nameof(ViewListText));
            OnPropertyChanged(nameof(CsvFileText));
            OnPropertyChanged(nameof(DataFileText));
            OnPropertyChanged(nameof(DateFilterText));
            OnPropertyChanged(nameof(FilterText));
            OnPropertyChanged(nameof(FirstPageText));
            OnPropertyChanged(nameof(PreviousPageText));
            OnPropertyChanged(nameof(NextPageText));
            OnPropertyChanged(nameof(LastPageText));
            OnPropertyChanged(nameof(ExportAsCsvFileText));
            OnPropertyChanged(nameof(BatchExportText));
            OnPropertyChanged(nameof(ErrorLineText));
            OnPropertyChanged(nameof(BoardIndexText));
            OnPropertyChanged(nameof(FirmwareVersionText));
            OnPropertyChanged(nameof(HardwareVersionText));
            OnPropertyChanged(nameof(DeviceInfoText));
            OnPropertyChanged(nameof(GetVersionInfoText));
            OnPropertyChanged(nameof(ExportVersionInfoText));
            OnPropertyChanged(nameof(BoardTypeText));
            OnPropertyChanged(nameof(FirmwareText));
            OnPropertyChanged(nameof(FirmwareUpgradeText));
            OnPropertyChanged(nameof(UpgradeText));
            OnPropertyChanged(nameof(FirmwareUpgradePromptText));
            OnPropertyChanged(nameof(ResultText));
            OnPropertyChanged(nameof(DeviceCheckText));
            OnPropertyChanged(nameof(StartDeviceCheckText));
            OnPropertyChanged(nameof(DeviceCheckResultText));
            OnPropertyChanged(nameof(DpsCheckText));
            OnPropertyChanged(nameof(DpsCheckScriptText));
            OnPropertyChanged(nameof(RunScriptText));
            OnPropertyChanged(nameof(SelectFileText));
            OnPropertyChanged(nameof(DutMaskText));
        }
    }

    public static class LangKey
    {
         /// <summary>
        /// 生产模式
        /// </summary>
        public const string Enum_DeviceMode_Production = nameof(Enum_DeviceMode_Production);
         /// <summary>
        /// 工程模式
        /// </summary>
        public const string Enum_DeviceMode_Engineering = nameof(Enum_DeviceMode_Engineering);
         /// <summary>
        /// 管理员模式
        /// </summary>
        public const string Enum_DeviceMode_Administrator = nameof(Enum_DeviceMode_Administrator);
         /// <summary>
        /// 秒
        /// </summary>
        public const string Enum_TimeUnit_Second = nameof(Enum_TimeUnit_Second);
         /// <summary>
        /// 分钟
        /// </summary>
        public const string Enum_TimeUnit_Minute = nameof(Enum_TimeUnit_Minute);
         /// <summary>
        /// 小时
        /// </summary>
        public const string Enum_TimeUnit_Hour = nameof(Enum_TimeUnit_Hour);
         /// <summary>
        /// 天
        /// </summary>
        public const string Enum_TimeUnit_Day = nameof(Enum_TimeUnit_Day);
         /// <summary>
        /// 时间
        /// </summary>
        public const string Enum_ExecuteMode_Time = nameof(Enum_ExecuteMode_Time);
         /// <summary>
        /// 次数
        /// </summary>
        public const string Enum_ExecuteMode_Times = nameof(Enum_ExecuteMode_Times);
         /// <summary>
        /// 初始化中
        /// </summary>
        public const string Enum_ZoneState_Init = nameof(Enum_ZoneState_Init);
         /// <summary>
        /// 空闲
        /// </summary>
        public const string Enum_ZoneState_Idle = nameof(Enum_ZoneState_Idle);
         /// <summary>
        /// 就绪
        /// </summary>
        public const string Enum_ZoneState_Ready = nameof(Enum_ZoneState_Ready);
         /// <summary>
        /// 上电中
        /// </summary>
        public const string Enum_ZoneState_PowerUp = nameof(Enum_ZoneState_PowerUp);
         /// <summary>
        /// 老化中
        /// </summary>
        public const string Enum_ZoneState_Burning = nameof(Enum_ZoneState_Burning);
         /// <summary>
        /// 下电中
        /// </summary>
        public const string Enum_ZoneState_PowerDown = nameof(Enum_ZoneState_PowerDown);
         /// <summary>
        /// 老化停止
        /// </summary>
        public const string Enum_ZoneState_Stop = nameof(Enum_ZoneState_Stop);
         /// <summary>
        /// 传输中
        /// </summary>
        public const string Enum_ZoneState_Transition = nameof(Enum_ZoneState_Transition);
         /// <summary>
        /// 设备自检中
        /// </summary>
        public const string Enum_ZoneState_DeviceChecking = nameof(Enum_ZoneState_DeviceChecking);
         /// <summary>
        /// 电源自检中
        /// </summary>
        public const string Enum_ZoneState_DpsChecking = nameof(Enum_ZoneState_DpsChecking);
         /// <summary>
        /// 固件升级中
        /// </summary>
        public const string Enum_ZoneState_Upgrading = nameof(Enum_ZoneState_Upgrading);
         /// <summary>
        /// 离线
        /// </summary>
        public const string Enum_ZoneState_Offline = nameof(Enum_ZoneState_Offline);
         /// <summary>
        /// 器件发送中
        /// </summary>
        public const string Enum_ZoneState_Sending = nameof(Enum_ZoneState_Sending);
         /// <summary>
        /// 自检错误
        /// </summary>
        public const string Enum_ZoneState_DeviceCheckFail = nameof(Enum_ZoneState_DeviceCheckFail);
         /// <summary>
        /// 器件配置错误
        /// </summary>
        public const string Enum_ZoneState_IcError = nameof(Enum_ZoneState_IcError);
         /// <summary>
        /// 错误
        /// </summary>
        public const string Enum_ZoneState_Error = nameof(Enum_ZoneState_Error);
         /// <summary>
        /// 全部载入
        /// </summary>
        public const string Enum_LoadMode_Default = nameof(Enum_LoadMode_Default);
         /// <summary>
        /// 单步载入
        /// </summary>
        public const string Enum_LoadMode_Step = nameof(Enum_LoadMode_Step);
         /// <summary>
        /// 空闲
        /// </summary>
        public const string Enum_DutState_Idle = nameof(Enum_DutState_Idle);
         /// <summary>
        /// 测试中
        /// </summary>
        public const string Enum_DutState_Test = nameof(Enum_DutState_Test);
         /// <summary>
        /// 通过
        /// </summary>
        public const string Enum_DutState_Pass = nameof(Enum_DutState_Pass);
         /// <summary>
        /// 错误
        /// </summary>
        public const string Enum_DutState_Fail = nameof(Enum_DutState_Fail);
         /// <summary>
        /// 忽略
        /// </summary>
        public const string Enum_DutState_Ignore = nameof(Enum_DutState_Ignore);
         /// <summary>
        /// 老化板
        /// </summary>
        public const string Enum_BoardType_BurnInBoard = nameof(Enum_BoardType_BurnInBoard);
         /// <summary>
        /// 工作中
        /// </summary>
        public const string Enum_DpsState_Output = nameof(Enum_DpsState_Output);
         /// <summary>
        /// 过压
        /// </summary>
        public const string Enum_DpsState_Ovp = nameof(Enum_DpsState_Ovp);
         /// <summary>
        /// 欠压
        /// </summary>
        public const string Enum_DpsState_Uvp = nameof(Enum_DpsState_Uvp);
         /// <summary>
        /// 过流
        /// </summary>
        public const string Enum_DpsState_Ocp = nameof(Enum_DpsState_Ocp);
         /// <summary>
        /// 关闭
        /// </summary>
        public const string Enum_DpsState_Idle = nameof(Enum_DpsState_Idle);
         /// <summary>
        /// 向量加载错误
        /// </summary>
        public const string Enum_PatternErrorType_Load = nameof(Enum_PatternErrorType_Load);
         /// <summary>
        /// 向量解码错误
        /// </summary>
        public const string Enum_PatternErrorType_Decode = nameof(Enum_PatternErrorType_Decode);
         /// <summary>
        /// 向量运行超时
        /// </summary>
        public const string Enum_PatternErrorType_Timeout = nameof(Enum_PatternErrorType_Timeout);
         /// <summary>
        /// 关闭
        /// </summary>
        public const string Enum_TriggerOutputType_Close = nameof(Enum_TriggerOutputType_Close);
         /// <summary>
        /// 信号
        /// </summary>
        public const string Enum_TriggerOutputType_Signal = nameof(Enum_TriggerOutputType_Signal);
         /// <summary>
        /// Sync
        /// </summary>
        public const string Enum_TriggerOutputType_Sync = nameof(Enum_TriggerOutputType_Sync);
         /// <summary>
        /// 升级中
        /// </summary>
        public const string Enum_UpgradeState_Updating = nameof(Enum_UpgradeState_Updating);
         /// <summary>
        /// 升级完成
        /// </summary>
        public const string Enum_UpgradeState_Finish = nameof(Enum_UpgradeState_Finish);
         /// <summary>
        /// 擦除中
        /// </summary>
        public const string Enum_UpgradeState_Erase = nameof(Enum_UpgradeState_Erase);
         /// <summary>
        /// 切换板卡失败
        /// </summary>
        public const string Enum_UpgradeState_SwitchBoardFail = nameof(Enum_UpgradeState_SwitchBoardFail);
         /// <summary>
        /// 写入更新时间失败
        /// </summary>
        public const string Enum_UpgradeState_WriteTimeFail = nameof(Enum_UpgradeState_WriteTimeFail);
         /// <summary>
        /// 读取FLASH ID失败
        /// </summary>
        public const string Enum_UpgradeState_ReadFlashIdFail = nameof(Enum_UpgradeState_ReadFlashIdFail);
         /// <summary>
        /// 更新超时
        /// </summary>
        public const string Enum_UpgradeState_TimeOut = nameof(Enum_UpgradeState_TimeOut);
         /// <summary>
        /// 其他
        /// </summary>
        public const string Enum_UpgradeState_Other = nameof(Enum_UpgradeState_Other);
         /// <summary>
        /// 通信错误
        /// </summary>
        public const string Enum_UpgradeState_CommunicateFail = nameof(Enum_UpgradeState_CommunicateFail);
         /// <summary>
        /// 数据校验失败
        /// </summary>
        public const string Enum_UpgradeState_DataVerifyFail = nameof(Enum_UpgradeState_DataVerifyFail);
         /// <summary>
        /// 错误
        /// </summary>
        public const string Enum_UpgradeState_Error = nameof(Enum_UpgradeState_Error);
         /// <summary>
        /// 主控板
        /// </summary>
        public const string DeviceCheck_Type_00 = nameof(DeviceCheck_Type_00);
         /// <summary>
        /// 底板
        /// </summary>
        public const string DeviceCheck_Type_10 = nameof(DeviceCheck_Type_10);
         /// <summary>
        /// 数字板
        /// </summary>
        public const string DeviceCheck_Type_20 = nameof(DeviceCheck_Type_20);
         /// <summary>
        /// 电源板
        /// </summary>
        public const string DeviceCheck_Type_30 = nameof(DeviceCheck_Type_30);
         /// <summary>
        /// 竖向板
        /// </summary>
        public const string DeviceCheck_Type_E0 = nameof(DeviceCheck_Type_E0);
         /// <summary>
        /// 老化板
        /// </summary>
        public const string DeviceCheck_Type_EA = nameof(DeviceCheck_Type_EA);
         /// <summary>
        /// 环境变量
        /// </summary>
        public const string DeviceCheck_Item_01 = nameof(DeviceCheck_Item_01);
         /// <summary>
        /// 配置文件
        /// </summary>
        public const string DeviceCheck_Item_02 = nameof(DeviceCheck_Item_02);
         /// <summary>
        /// EEPROM
        /// </summary>
        public const string DeviceCheck_Item_03 = nameof(DeviceCheck_Item_03);
         /// <summary>
        /// 扩展I/O
        /// </summary>
        public const string DeviceCheck_Item_04 = nameof(DeviceCheck_Item_04);
         /// <summary>
        /// 电源模块
        /// </summary>
        public const string DeviceCheck_Item_05 = nameof(DeviceCheck_Item_05);
         /// <summary>
        /// DAC自检
        /// </summary>
        public const string DeviceCheck_Item_06 = nameof(DeviceCheck_Item_06);
         /// <summary>
        /// FPGA自检
        /// </summary>
        public const string DeviceCheck_Item_07 = nameof(DeviceCheck_Item_07);
         /// <summary>
        /// 电源自检(XADC)
        /// </summary>
        public const string DeviceCheck_Item_08 = nameof(DeviceCheck_Item_08);
         /// <summary>
        /// FLASH自检
        /// </summary>
        public const string DeviceCheck_Item_09 = nameof(DeviceCheck_Item_09);
         /// <summary>
        /// PL机型自检
        /// </summary>
        public const string DeviceCheck_Item_0A = nameof(DeviceCheck_Item_0A);
         /// <summary>
        /// 温控自检
        /// </summary>
        public const string DeviceCheck_Item_0B = nameof(DeviceCheck_Item_0B);
         /// <summary>
        /// COMPARE VREF自检
        /// </summary>
        public const string DeviceCheck_Item_0C = nameof(DeviceCheck_Item_0C);
         /// <summary>
        /// Dps VIN自检
        /// </summary>
        public const string DeviceCheck_Item_0D = nameof(DeviceCheck_Item_0D);
         /// <summary>
        /// 通过
        /// </summary>
        public const string DeviceCheck_Result_00 = nameof(DeviceCheck_Result_00);
         /// <summary>
        /// 环境变量丢失
        /// </summary>
        public const string DeviceCheck_Result_01 = nameof(DeviceCheck_Result_01);
         /// <summary>
        /// 配置文件不存在
        /// </summary>
        public const string DeviceCheck_Result_02 = nameof(DeviceCheck_Result_02);
         /// <summary>
        /// 配置文件格式错误
        /// </summary>
        public const string DeviceCheck_Result_03 = nameof(DeviceCheck_Result_03);
         /// <summary>
        /// EEPROM通信错误
        /// </summary>
        public const string DeviceCheck_Result_04 = nameof(DeviceCheck_Result_04);
         /// <summary>
        /// I/O扩展芯片通信错误
        /// </summary>
        public const string DeviceCheck_Result_05 = nameof(DeviceCheck_Result_05);
         /// <summary>
        /// ADC通信错误
        /// </summary>
        public const string DeviceCheck_Result_06 = nameof(DeviceCheck_Result_06);
         /// <summary>
        /// 电源模块通信错误
        /// </summary>
        public const string DeviceCheck_Result_07 = nameof(DeviceCheck_Result_07);
         /// <summary>
        /// 电源模块混插
        /// </summary>
        public const string DeviceCheck_Result_08 = nameof(DeviceCheck_Result_08);
         /// <summary>
        /// 电源模块固件错误
        /// </summary>
        public const string DeviceCheck_Result_09 = nameof(DeviceCheck_Result_09);
         /// <summary>
        /// LVDS寄存器通信错误
        /// </summary>
        public const string DeviceCheck_Result_0A = nameof(DeviceCheck_Result_0A);
         /// <summary>
        /// LVDS数据流通信错误
        /// </summary>
        public const string DeviceCheck_Result_0B = nameof(DeviceCheck_Result_0B);
         /// <summary>
        /// 电源通信错误
        /// </summary>
        public const string DeviceCheck_Result_0C = nameof(DeviceCheck_Result_0C);
         /// <summary>
        /// 电源电压错误
        /// </summary>
        public const string DeviceCheck_Result_0D = nameof(DeviceCheck_Result_0D);
         /// <summary>
        /// FLASH选择错误(打开)
        /// </summary>
        public const string DeviceCheck_Result_0E = nameof(DeviceCheck_Result_0E);
         /// <summary>
        /// FLASH通信错误
        /// </summary>
        public const string DeviceCheck_Result_0F = nameof(DeviceCheck_Result_0F);
         /// <summary>
        /// FLASH选择错误(关闭)
        /// </summary>
        public const string DeviceCheck_Result_10 = nameof(DeviceCheck_Result_10);
         /// <summary>
        /// PL机型读取错误(AXIBUS)
        /// </summary>
        public const string DeviceCheck_Result_11 = nameof(DeviceCheck_Result_11);
         /// <summary>
        /// ADC通信错误(SPI)
        /// </summary>
        public const string DeviceCheck_Result_12 = nameof(DeviceCheck_Result_12);
         /// <summary>
        /// COMPARE VREF 打开错误
        /// </summary>
        public const string DeviceCheck_Result_13 = nameof(DeviceCheck_Result_13);
         /// <summary>
        /// Dps VIN 打开错误
        /// </summary>
        public const string DeviceCheck_Result_14 = nameof(DeviceCheck_Result_14);
         /// <summary>
        /// 未捕获到的异常, {0}
        /// </summary>
        public const string UnhandledExceptionMessage = nameof(UnhandledExceptionMessage);
         /// <summary>
        /// 文件导入成功
        /// </summary>
        public const string FileImportSuccessMessage = nameof(FileImportSuccessMessage);
         /// <summary>
        /// 文件导入失败
        /// </summary>
        public const string FileImportFailureMessage = nameof(FileImportFailureMessage);
         /// <summary>
        /// 文件导出成功
        /// </summary>
        public const string FileExportSuccessMessage = nameof(FileExportSuccessMessage);
         /// <summary>
        /// 文件导出失败
        /// </summary>
        public const string FileExportFailureMessage = nameof(FileExportFailureMessage);
         /// <summary>
        /// 文件不存在
        /// </summary>
        public const string FileNotExistMessage = nameof(FileNotExistMessage);
         /// <summary>
        /// 确定删除所选项目?
        /// </summary>
        public const string DeleteAskMessage = nameof(DeleteAskMessage);
         /// <summary>
        /// 已存在同名器件文件, 是否覆盖?
        /// </summary>
        public const string ImportIcFileExistMessage = nameof(ImportIcFileExistMessage);
         /// <summary>
        /// 存在与导入器件同名的器件 是否对导入器件重命名?
        /// </summary>
        public const string ImportIcNameExistsMessage = nameof(ImportIcNameExistsMessage);
         /// <summary>
        /// 名称不可以为空或包含空格
        /// </summary>
        public const string NameNotNullMessage = nameof(NameNotNullMessage);
         /// <summary>
        /// 名称不可以超过 {0} 位长度
        /// </summary>
        public const string NameNotExceedMessage = nameof(NameNotExceedMessage);
         /// <summary>
        /// 名称已存在
        /// </summary>
        public const string NameExistMessage = nameof(NameExistMessage);
         /// <summary>
        /// 设备配置文件解析失败, {0}
        /// </summary>
        public const string DeviceConfigParseFailureMessage = nameof(DeviceConfigParseFailureMessage);
         /// <summary>
        /// 值必须介于 {0}
        /// </summary>
        public const string MustBeInRangeMessage = nameof(MustBeInRangeMessage);
         /// <summary>
        /// 仅支持英文字母、数字和下划线
        /// </summary>
        public const string ValidStringRuleMessage = nameof(ValidStringRuleMessage);
         /// <summary>
        /// 文件 [ {0} ] 解析失败, 第 {1} 行 {2} 列
        /// </summary>
        public const string FileParseErrorMessage = nameof(FileParseErrorMessage);
         /// <summary>
        /// 文件 [ {0} ] 长度不正确, 预期长度为 {1} 字节
        /// </summary>
        public const string FileLengthErrorMessage = nameof(FileLengthErrorMessage);
         /// <summary>
        /// ABM 文件 [ {0} ] 不匹配，请使用该上位机软件中的向量转换工具生成的 ABM 文件
        /// </summary>
        public const string AbmFileNotMatchMessage = nameof(AbmFileNotMatchMessage);
         /// <summary>
        /// 报文解析错误，{0}
        /// </summary>
        public const string AnalysisFrameErrorMessage = nameof(AnalysisFrameErrorMessage);
         /// <summary>
        /// CRC32不匹配，{0}
        /// </summary>
        public const string Crc32NotMatchMessage = nameof(Crc32NotMatchMessage);
         /// <summary>
        /// 帧尾不匹配
        /// </summary>
        public const string FrameEndingNotMatchMessage = nameof(FrameEndingNotMatchMessage);
         /// <summary>
        /// 老化板插入
        /// </summary>
        public const string PluggedMessage = nameof(PluggedMessage);
         /// <summary>
        /// 老化板卸下
        /// </summary>
        public const string UnpluggedMessage = nameof(UnpluggedMessage);
         /// <summary>
        /// 请设置 [ {0} ]
        /// </summary>
        public const string PleaseSetMessage = nameof(PleaseSetMessage);
         /// <summary>
        /// 至少需要配置一个步次
        /// </summary>
        public const string AtLeastOneStepMessage = nameof(AtLeastOneStepMessage);
         /// <summary>
        /// 至少需要配置一个子步次
        /// </summary>
        public const string AtLeastOneSubStepMessage = nameof(AtLeastOneSubStepMessage);
         /// <summary>
        /// 当前状态 [ {0} ]
        /// </summary>
        public const string StateMessage = nameof(StateMessage);
         /// <summary>
        /// 器件配置错误, 请检查后重试! {0}
        /// </summary>
        public const string IcErrorMessage = nameof(IcErrorMessage);
         /// <summary>
        /// 发送器件数据 [ {0} ]
        /// </summary>
        public const string SendIcDataMessage = nameof(SendIcDataMessage);
         /// <summary>
        /// 不支持该数据文件
        /// </summary>
        public const string UnsupportedDataFileMessage = nameof(UnsupportedDataFileMessage);
         /// <summary>
        /// 请先勾选要升级的老化区
        /// </summary>
        public const string UpgradeSelectZoneMessage = nameof(UpgradeSelectZoneMessage);
         /// <summary>
        /// 请先为要升级的板卡类型选择固件文件
        /// </summary>
        public const string UpgradeSelectFirmwareMessage = nameof(UpgradeSelectFirmwareMessage);
         /// <summary>
        /// 固件文件名必须以 {0} 开头，请确认选择的是该类型固件
        /// </summary>
        public const string UpgradeFilePrefixMismatchMessage = nameof(UpgradeFilePrefixMismatchMessage);
         /// <summary>
        /// 电源自检脚本: 找不到目标电源，编号{0}，位于{1}行
        /// </summary>
        public const string DpsCheckScriptDpsNotFoundMessage = nameof(DpsCheckScriptDpsNotFoundMessage);
         /// <summary>
        /// 电源自检脚本: 参数错误或数值超出范围，位于{0}行
        /// </summary>
        public const string DpsCheckScriptParameterErrorMessage = nameof(DpsCheckScriptParameterErrorMessage);
         /// <summary>
        /// 当前状态不允许运行电源自检
        /// </summary>
        public const string DpsCheckIsNotPermittedMessage = nameof(DpsCheckIsNotPermittedMessage);
         /// <summary>
        /// 检测到当前区已插老化板，运行电源自检可能会烧毁器件 是否继续 ?
        /// </summary>
        public const string DpsCheckAskMessage = nameof(DpsCheckAskMessage);
         /// <summary>
        /// 电源错误:{0} -> [{1}]
        /// </summary>
        public const string ZoneDpsErrorMessage = nameof(ZoneDpsErrorMessage);
         /// <summary>
        /// 电源错误: VIH{0} -> [{1}]
        /// </summary>
        public const string ZoneVIHErrorMessage = nameof(ZoneVIHErrorMessage);
         /// <summary>
        /// 向量错误: [{0}] 请检查第{1}步 第{2}组向量配置
        /// </summary>
        public const string ZonePatternErrorMessage = nameof(ZonePatternErrorMessage);
         /// <summary>
        /// 设备异常: 请联系售后人员! 错误码[{0}]
        /// </summary>
        public const string ZoneDeviceErrorMessage = nameof(ZoneDeviceErrorMessage);
         /// <summary>
        /// 当前步次: [{0}]	向量: [{1}]
        /// </summary>
        public const string StepRunInfoMessage = nameof(StepRunInfoMessage);
         /// <summary>
        /// 运行电源自检脚本: {0}
        /// </summary>
        public const string RunDpsCheckScriptMessage = nameof(RunDpsCheckScriptMessage);
         /// <summary>
        /// 电源自检[{0}/{1}] ->{2}    {3}V    {4}A    ({5}/{6})
        /// </summary>
        public const string DpsCheckResultMessage = nameof(DpsCheckResultMessage);
         /// <summary>
        /// 触发通道{0}设置成功
        /// </summary>
        public const string TriggerDrSetSuccessMessage = nameof(TriggerDrSetSuccessMessage);
         /// <summary>
        /// 见微知著·科技赋能·创造非凡价值
        /// </summary>
        public const string ConceptText = nameof(ConceptText);
         /// <summary>
        /// 确认
        /// </summary>
        public const string ConfirmText = nameof(ConfirmText);
         /// <summary>
        /// 取消
        /// </summary>
        public const string CancelText = nameof(CancelText);
         /// <summary>
        /// 新建
        /// </summary>
        public const string CreateNewText = nameof(CreateNewText);
         /// <summary>
        /// 重命名
        /// </summary>
        public const string RenameText = nameof(RenameText);
         /// <summary>
        /// 保存
        /// </summary>
        public const string SaveText = nameof(SaveText);
         /// <summary>
        /// 另存为
        /// </summary>
        public const string SaveAsText = nameof(SaveAsText);
         /// <summary>
        /// 删除
        /// </summary>
        public const string DeleteText = nameof(DeleteText);
         /// <summary>
        /// 导入
        /// </summary>
        public const string ImportText = nameof(ImportText);
         /// <summary>
        /// 导出
        /// </summary>
        public const string ExportText = nameof(ExportText);
         /// <summary>
        /// 查看
        /// </summary>
        public const string ViewText = nameof(ViewText);
         /// <summary>
        /// 编辑
        /// </summary>
        public const string EditText = nameof(EditText);
         /// <summary>
        /// 名称
        /// </summary>
        public const string NameText = nameof(NameText);
         /// <summary>
        /// 关于
        /// </summary>
        public const string AboutText = nameof(AboutText);
         /// <summary>
        /// 全选
        /// </summary>
        public const string AllSelectText = nameof(AllSelectText);
         /// <summary>
        /// 帮助(_H)
        /// </summary>
        public const string HelpMenuText = nameof(HelpMenuText);
         /// <summary>
        /// 刷新
        /// </summary>
        public const string RefreshText = nameof(RefreshText);
         /// <summary>
        /// 区
        /// </summary>
        public const string ZoneText = nameof(ZoneText);
         /// <summary>
        /// {0} 区
        /// </summary>
        public const string ZoneFormatText = nameof(ZoneFormatText);
         /// <summary>
        /// 状态
        /// </summary>
        public const string StateText = nameof(StateText);
         /// <summary>
        /// 类型
        /// </summary>
        public const string TypeText = nameof(TypeText);
         /// <summary>
        /// 规格
        /// </summary>
        public const string SpecText = nameof(SpecText);
         /// <summary>
        /// 工位
        /// </summary>
        public const string DutText = nameof(DutText);
         /// <summary>
        /// 行
        /// </summary>
        public const string RowText = nameof(RowText);
         /// <summary>
        /// 列
        /// </summary>
        public const string ColumnText = nameof(ColumnText);
         /// <summary>
        /// 所属片选
        /// </summary>
        public const string BelongChipSelectText = nameof(BelongChipSelectText);
         /// <summary>
        /// 使用的电源
        /// </summary>
        public const string UsedDpsText = nameof(UsedDpsText);
         /// <summary>
        /// 关联的管脚
        /// </summary>
        public const string LinkedDrText = nameof(LinkedDrText);
         /// <summary>
        /// 管脚
        /// </summary>
        public const string DrText = nameof(DrText);
         /// <summary>
        /// 片选
        /// </summary>
        public const string CSText = nameof(CSText);
         /// <summary>
        /// 器件名称
        /// </summary>
        public const string IcNameText = nameof(IcNameText);
         /// <summary>
        /// 器件编辑
        /// </summary>
        public const string IcEditText = nameof(IcEditText);
         /// <summary>
        /// 当前器件
        /// </summary>
        public const string CurrentIcText = nameof(CurrentIcText);
         /// <summary>
        /// 向量格式转换
        /// </summary>
        public const string PatternFormatConvertText = nameof(PatternFormatConvertText);
         /// <summary>
        /// 曲线查看
        /// </summary>
        public const string ChartViewText = nameof(ChartViewText);
         /// <summary>
        /// 触发设置
        /// </summary>
        public const string TriggerSetText = nameof(TriggerSetText);
         /// <summary>
        /// 发送
        /// </summary>
        public const string SendParameterText = nameof(SendParameterText);
         /// <summary>
        /// 开始
        /// </summary>
        public const string StartBurnText = nameof(StartBurnText);
         /// <summary>
        /// 终止
        /// </summary>
        public const string StopBurnText = nameof(StopBurnText);
         /// <summary>
        /// 进入老化界面
        /// </summary>
        public const string EnterBurnViewText = nameof(EnterBurnViewText);
         /// <summary>
        /// 重置
        /// </summary>
        public const string ReSelectIcText = nameof(ReSelectIcText);
         /// <summary>
        /// 关闭报警
        /// </summary>
        public const string CloseAlarmText = nameof(CloseAlarmText);
         /// <summary>
        /// 器件库
        /// </summary>
        public const string IcLibraryText = nameof(IcLibraryText);
         /// <summary>
        /// 运行策略
        /// </summary>
        public const string RunStrategyText = nameof(RunStrategyText);
         /// <summary>
        /// 运行模式
        /// </summary>
        public const string RunModeText = nameof(RunModeText);
         /// <summary>
        /// 循环策略
        /// </summary>
        public const string LoopStrategyText = nameof(LoopStrategyText);
         /// <summary>
        /// 循环运行中
        /// </summary>
        public const string RunInTheLoopText = nameof(RunInTheLoopText);
         /// <summary>
        /// 温度(℃)
        /// </summary>
        public const string TemperatureWithUnitText = nameof(TemperatureWithUnitText);
         /// <summary>
        /// 报警信息
        /// </summary>
        public const string AlarmMessageText = nameof(AlarmMessageText);
         /// <summary>
        /// 报警窗口
        /// </summary>
        public const string AlarmWindowText = nameof(AlarmWindowText);
         /// <summary>
        /// 关闭烘箱报警
        /// </summary>
        public const string CloseOvenAlarmText = nameof(CloseOvenAlarmText);
         /// <summary>
        /// 一键降温
        /// </summary>
        public const string OneClickCoolText = nameof(OneClickCoolText);
         /// <summary>
        /// 温控索引
        /// </summary>
        public const string ThermostatIndexText = nameof(ThermostatIndexText);
         /// <summary>
        /// 独立温控监视
        /// </summary>
        public const string ThermostatMonitorText = nameof(ThermostatMonitorText);
         /// <summary>
        /// 温控使能
        /// </summary>
        public const string ThermostatEnableText = nameof(ThermostatEnableText);
         /// <summary>
        /// 电源
        /// </summary>
        public const string DpsText = nameof(DpsText);
         /// <summary>
        /// 设置电压
        /// </summary>
        public const string SetVoltageText = nameof(SetVoltageText);
         /// <summary>
        /// 设置电压(V)
        /// </summary>
        public const string SetVoltageWithUnitText = nameof(SetVoltageWithUnitText);
         /// <summary>
        /// 实时电压(V)
        /// </summary>
        public const string RealVoltageWithUnitText = nameof(RealVoltageWithUnitText);
         /// <summary>
        /// 实时电流(A)
        /// </summary>
        public const string RealCurrentWithUnitText = nameof(RealCurrentWithUnitText);
         /// <summary>
        /// 实时功率(W)
        /// </summary>
        public const string RealPowerWithUnitText = nameof(RealPowerWithUnitText);
         /// <summary>
        /// 电压下限
        /// </summary>
        public const string VoltageFloorText = nameof(VoltageFloorText);
         /// <summary>
        /// 电压下限(V)
        /// </summary>
        public const string VoltageFloorWithUnitText = nameof(VoltageFloorWithUnitText);
         /// <summary>
        /// 电压上限
        /// </summary>
        public const string VoltageCeilingText = nameof(VoltageCeilingText);
         /// <summary>
        /// 电压上限(V)
        /// </summary>
        public const string VoltageCeilingWithUnitText = nameof(VoltageCeilingWithUnitText);
         /// <summary>
        /// 电流上限
        /// </summary>
        public const string CurrentCeilingText = nameof(CurrentCeilingText);
         /// <summary>
        /// 电流上限(A)
        /// </summary>
        public const string CurrentCeilingWithUnitText = nameof(CurrentCeilingWithUnitText);
         /// <summary>
        /// 上升时间
        /// </summary>
        public const string RaiseTimeText = nameof(RaiseTimeText);
         /// <summary>
        /// 上升时间(ms)
        /// </summary>
        public const string RaiseTimeWithUnitText = nameof(RaiseTimeWithUnitText);
         /// <summary>
        /// 延时时间
        /// </summary>
        public const string DelayText = nameof(DelayText);
         /// <summary>
        /// 延时时间(ms)
        /// </summary>
        public const string DelayWithUnitText = nameof(DelayWithUnitText);
         /// <summary>
        /// 电源设置
        /// </summary>
        public const string DpsSettingText = nameof(DpsSettingText);
         /// <summary>
        /// 电源方案
        /// </summary>
        public const string DpsPlanText = nameof(DpsPlanText);
         /// <summary>
        /// 电源编辑
        /// </summary>
        public const string DpsEditText = nameof(DpsEditText);
         /// <summary>
        /// 基础信息
        /// </summary>
        public const string BaseInfoText = nameof(BaseInfoText);
         /// <summary>
        /// 基础设置
        /// </summary>
        public const string BaseSettingText = nameof(BaseSettingText);
         /// <summary>
        /// 流程设置
        /// </summary>
        public const string ProcessSettingText = nameof(ProcessSettingText);
         /// <summary>
        /// 步次
        /// </summary>
        public const string StepText = nameof(StepText);
         /// <summary>
        /// 步次[{0}]
        /// </summary>
        public const string StepFormatText = nameof(StepFormatText);
         /// <summary>
        /// 运行时间
        /// </summary>
        public const string RunTimeText = nameof(RunTimeText);
         /// <summary>
        /// 运行次数
        /// </summary>
        public const string RunTimesText = nameof(RunTimesText);
         /// <summary>
        /// 循环模式
        /// </summary>
        public const string LoopModeText = nameof(LoopModeText);
         /// <summary>
        /// 循环时间
        /// </summary>
        public const string LoopTimeText = nameof(LoopTimeText);
         /// <summary>
        /// 循环次数
        /// </summary>
        public const string LoopTimesText = nameof(LoopTimesText);
         /// <summary>
        /// 向量设置
        /// </summary>
        public const string VectorSettingText = nameof(VectorSettingText);
         /// <summary>
        /// 向量文件
        /// </summary>
        public const string PatternFileText = nameof(PatternFileText);
         /// <summary>
        /// 寄存器文件
        /// </summary>
        public const string RegisterFileText = nameof(RegisterFileText);
         /// <summary>
        /// 器件文件
        /// </summary>
        public const string IcFileText = nameof(IcFileText);
         /// <summary>
        /// 向量超时
        /// </summary>
        public const string PatternTimeoutText = nameof(PatternTimeoutText);
         /// <summary>
        /// 当前向量
        /// </summary>
        public const string CurrentPatternText = nameof(CurrentPatternText);
         /// <summary>
        /// 当前向量剩余时间
        /// </summary>
        public const string CurrentPatternLeftTimeText = nameof(CurrentPatternLeftTimeText);
         /// <summary>
        /// 当前向量运行时间
        /// </summary>
        public const string CurrentPatternCostTimeText = nameof(CurrentPatternCostTimeText);
         /// <summary>
        /// 当前向量设定时间
        /// </summary>
        public const string CurrentPatternSetTimeText = nameof(CurrentPatternSetTimeText);
         /// <summary>
        /// 当前步次
        /// </summary>
        public const string CurrentStepText = nameof(CurrentStepText);
         /// <summary>
        /// 当前寄存器
        /// </summary>
        public const string CurrentRegistersText = nameof(CurrentRegistersText);
         /// <summary>
        /// 当前电源方案
        /// </summary>
        public const string CurrentDpsPlanText = nameof(CurrentDpsPlanText);
         /// <summary>
        /// 上电后总运行时间
        /// </summary>
        public const string TotalTimeBeforeDpsUpText = nameof(TotalTimeBeforeDpsUpText);
         /// <summary>
        /// 开始时间
        /// </summary>
        public const string StartTimeText = nameof(StartTimeText);
         /// <summary>
        /// 数量
        /// </summary>
        public const string QuantityText = nameof(QuantityText);
         /// <summary>
        /// 时间
        /// </summary>
        public const string TimeText = nameof(TimeText);
         /// <summary>
        /// 修改时间
        /// </summary>
        public const string UpdateTimeText = nameof(UpdateTimeText);
         /// <summary>
        /// 次
        /// </summary>
        public const string TimesText = nameof(TimesText);
         /// <summary>
        /// 批次号
        /// </summary>
        public const string LotText = nameof(LotText);
         /// <summary>
        /// 操作员
        /// </summary>
        public const string OperatorText = nameof(OperatorText);
         /// <summary>
        /// 载入模式
        /// </summary>
        public const string LoadModeText = nameof(LoadModeText);
         /// <summary>
        /// 强制单步载入
        /// </summary>
        public const string ForceStepLoadText = nameof(ForceStepLoadText);
         /// <summary>
        /// 单位
        /// </summary>
        public const string UnitText = nameof(UnitText);
         /// <summary>
        /// 工位配置文件
        /// </summary>
        public const string DutConfigFileText = nameof(DutConfigFileText);
         /// <summary>
        /// 日志
        /// </summary>
        public const string LogText = nameof(LogText);
         /// <summary>
        /// 数据日志
        /// </summary>
        public const string DataLogText = nameof(DataLogText);
         /// <summary>
        /// 数据记录间隔
        /// </summary>
        public const string LogIntervalText = nameof(LogIntervalText);
         /// <summary>
        /// 报错记录间隔
        /// </summary>
        public const string ErrorLogIntervalText = nameof(ErrorLogIntervalText);
         /// <summary>
        /// 清空记录
        /// </summary>
        public const string ClearLogsText = nameof(ClearLogsText);
         /// <summary>
        /// 帧过滤器
        /// </summary>
        public const string FrameFilterText = nameof(FrameFilterText);
         /// <summary>
        /// 查看列表
        /// </summary>
        public const string ViewListText = nameof(ViewListText);
         /// <summary>
        /// CSV文件
        /// </summary>
        public const string CsvFileText = nameof(CsvFileText);
         /// <summary>
        /// 数据文件
        /// </summary>
        public const string DataFileText = nameof(DataFileText);
         /// <summary>
        /// 日期筛选
        /// </summary>
        public const string DateFilterText = nameof(DateFilterText);
         /// <summary>
        /// 筛选
        /// </summary>
        public const string FilterText = nameof(FilterText);
         /// <summary>
        /// 首页
        /// </summary>
        public const string FirstPageText = nameof(FirstPageText);
         /// <summary>
        /// 上一页
        /// </summary>
        public const string PreviousPageText = nameof(PreviousPageText);
         /// <summary>
        /// 下一页
        /// </summary>
        public const string NextPageText = nameof(NextPageText);
         /// <summary>
        /// 尾页
        /// </summary>
        public const string LastPageText = nameof(LastPageText);
         /// <summary>
        /// 导出为CSV文件
        /// </summary>
        public const string ExportAsCsvFileText = nameof(ExportAsCsvFileText);
         /// <summary>
        /// 批量导出
        /// </summary>
        public const string BatchExportText = nameof(BatchExportText);
         /// <summary>
        /// 错误行
        /// </summary>
        public const string ErrorLineText = nameof(ErrorLineText);
         /// <summary>
        /// 板卡索引
        /// </summary>
        public const string BoardIndexText = nameof(BoardIndexText);
         /// <summary>
        /// 固件版本
        /// </summary>
        public const string FirmwareVersionText = nameof(FirmwareVersionText);
         /// <summary>
        /// 硬件版本
        /// </summary>
        public const string HardwareVersionText = nameof(HardwareVersionText);
         /// <summary>
        /// 设备信息
        /// </summary>
        public const string DeviceInfoText = nameof(DeviceInfoText);
         /// <summary>
        /// 查询版本信息
        /// </summary>
        public const string GetVersionInfoText = nameof(GetVersionInfoText);
         /// <summary>
        /// 导出版本信息 (所有区)
        /// </summary>
        public const string ExportVersionInfoText = nameof(ExportVersionInfoText);
         /// <summary>
        /// 板卡类型
        /// </summary>
        public const string BoardTypeText = nameof(BoardTypeText);
         /// <summary>
        /// 固件
        /// </summary>
        public const string FirmwareText = nameof(FirmwareText);
         /// <summary>
        /// 固件升级
        /// </summary>
        public const string FirmwareUpgradeText = nameof(FirmwareUpgradeText);
         /// <summary>
        /// 升级
        /// </summary>
        public const string UpgradeText = nameof(UpgradeText);
         /// <summary>
        /// 注意：升级过程中请勿关闭上位机或设备电源！
        /// </summary>
        public const string FirmwareUpgradePromptText = nameof(FirmwareUpgradePromptText);
         /// <summary>
        /// 结果
        /// </summary>
        public const string ResultText = nameof(ResultText);
         /// <summary>
        /// 设备自检
        /// </summary>
        public const string DeviceCheckText = nameof(DeviceCheckText);
         /// <summary>
        /// 开始自检
        /// </summary>
        public const string StartDeviceCheckText = nameof(StartDeviceCheckText);
         /// <summary>
        /// 自检结果
        /// </summary>
        public const string DeviceCheckResultText = nameof(DeviceCheckResultText);
         /// <summary>
        /// 电源自检
        /// </summary>
        public const string DpsCheckText = nameof(DpsCheckText);
         /// <summary>
        /// 电源自检脚本
        /// </summary>
        public const string DpsCheckScriptText = nameof(DpsCheckScriptText);
         /// <summary>
        /// 运行脚本
        /// </summary>
        public const string RunScriptText = nameof(RunScriptText);
         /// <summary>
        /// 选择文件
        /// </summary>
        public const string SelectFileText = nameof(SelectFileText);
         /// <summary>
        /// 工位屏蔽
        /// </summary>
        public const string DutMaskText = nameof(DutMaskText);
    }

    [MarkupExtensionReturnType(typeof(object))]
    public class LangExtension : MarkupExtension
    {
        public string PropertyName { get; }

        public LangExtension(string propertyName)
        {
            PropertyName = propertyName;
        }

        public LangExtension()
        {
            
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