namespace LSIC.App
{
    /// <summary>
    /// 控制卡功能码
    /// </summary>
    internal enum ControllerFunctionCode : byte
    {
        /// <summary>
        /// 预留控制板状态
        /// </summary>
        ReservedBoardStatus = 0x00,

        /// <summary>
        /// 水冷机状态反馈
        /// </summary>
        ChillerFeedback = 0x01,

        /// <summary>
        /// 水阀控制 &amp; 阀位反馈
        /// </summary>
        WaterValveControlFeedback = 0x02,

        /// <summary>
        /// 老化门控制 &amp; 状态反馈 
        /// </summary>
        AgingDoorControlFeedback = 0x03,

        /// <summary>
        /// 报警灯控制 &amp; 状态反馈
        /// </summary>
        AlarmLightControlFeedback = 0x04
    }
}