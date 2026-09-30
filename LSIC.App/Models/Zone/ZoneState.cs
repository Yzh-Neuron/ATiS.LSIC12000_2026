namespace LSIC.App.Models
{
    /// <summary>
    /// 老化区状态
    /// </summary>
    public enum ZoneState : byte
    {
        Init,
        Idle,
        Ready,
        Burning,
        Suspend,
        Stop,
        Finish,
        PowerUp,
        PowerDown,

        SelfCheck = 0x80,
        Upgrading = 0x81,

        // 非协议状态放 E 段
        Offline = 0xE0,
        IcSending = 0xE1,

        Error = 0xFF
    }
}
