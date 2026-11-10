namespace LSIC.App
{
    /// <summary>
    /// 水冷机状态
    /// </summary>
    public enum WaterCoolerState : byte
    {
        Running = 0x00, // 运行中
        Idle = 0x01, // 空闲
        Fault = 0xFF  // 异常
    }

}
