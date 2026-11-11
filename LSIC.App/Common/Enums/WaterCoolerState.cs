namespace LSIC.App
{
    /// <summary>
    /// 水冷机状态
    /// </summary>
    public enum WaterCoolerState : byte
    {

        Idle = 0x00, // 空闲
        Running = 0x01, // 运行中
        Fault = 0xFF  // 异常
    }

}
