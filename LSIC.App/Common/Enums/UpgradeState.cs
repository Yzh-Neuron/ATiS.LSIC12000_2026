namespace LSIC.App
{
    /// <summary>
    /// 固件升级状态/错误码
    /// </summary>
    public enum UpgradeState : byte
    {
        Updating,
        Finish,
        Erase,
        SwitchBoardFail = 0xF0,
        WriteTimeFail = 0xF1,
        ReadFlashIdFail = 0xF2,
        TimeOut = 0xF3,
        Other = 0xF4,
        CommunicateFail = 0xF5,
        DataVerifyFail = 0xF6,
        Error = 0xFF
    }
}
