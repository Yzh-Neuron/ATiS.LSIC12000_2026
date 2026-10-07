namespace LSIC.App
{
    /// <summary>
    /// 老化区状态
    /// </summary>
    public enum ZoneState : byte
    {
        Init,
        Idle,
        Ready,
        PowerUp,
        Burning,
        PowerDown,
        Stop,
        Transition, // 传输中

        DeviceChecking = 0xA0,
        DpsChecking = 0xA1,
        Upgrading = 0xA2,

        DeviceCheckFail = 0xF0,
        Error = 0xFF,

        //非协议段
        Offline = 0xE0,
        Sending = 0xE1,
        IcError = 0xFE,

    }
}
