namespace ZoneSimulator
{
    /// <summary>老化区功能码，与 LSIC.App/Common/Enums/ZoneFunctionCode.cs 一致</summary>
    public enum ZoneFunctionCode : byte
    {
        Version = 0x00,
        ControlAndState = 0x01,
        BurnInBoardInsertState = 0x02,
        DpsState = 0x03,
        CurrentDpsPlan = 0x04,
        BurnState = 0x05,
        CompareError = 0x06,
        Trigger = 0x07,

        DpsPlanCount = 0x50,
        DpsPlan = 0x51,
        PatternCount = 0x52,
        Pattern = 0x53,
        RegisterCount = 0x54,
        Register = 0x55,
        UpSet = 0x56,
        StepSet = 0x57,
        DownSet = 0x58,
        ConfigFinish = 0x5F,

        Upgrade = 0xA0,
        UpgradeFinish = 0xA1,

        DeviceCheck = 0xF0,
        DpsCheck = 0xF1,
        Thermostat = 0xF2,
        ThermostatPID = 0xF3,
    }

    /// <summary>老化区状态，与 LSIC.App/Common/Enums/ZoneState.cs 一致</summary>
    public enum ZoneState : byte
    {
        Init = 0,
        Idle = 1,
        Ready = 2,
        PowerUp = 3,
        Burning = 4,
        PowerDown = 5,
        Stop = 6,
        Transition = 7,

        DeviceChecking = 0xA0,
        DpsChecking = 0xA1,
        Upgrading = 0xA2,

        DeviceCheckFail = 0xF0,
        Error = 0xFF,
    }

    /// <summary>错误大类，与 LSIC.App/Common/Enums/ZoneErrorType.cs 一致</summary>
    public enum ZoneErrorType : byte
    {
        DPS = 0,
        Pattern = 1,
        Device = 2,
    }

    public enum LoadMode : byte
    {
        Default = 0,
        Step = 1,
    }

    public enum ExecuteMode : byte
    {
        Time = 0,
        Times = 1,
    }

    public enum DpsType : byte
    {
        DPS = 0,
        VIH = 1,
    }

    /// <summary>电源状态，与 LSIC.App/Common/Enums/DpsState.cs 一致</summary>
    public enum DpsState : byte
    {
        Off = 0,
        OutputOv = 1,
        OutputOc = 2,
        InputOvUv = 3,
        ChipOt = 4,
        CommLogicFault = 5,
        VendorCustomFault = 6,
        PgAbnormal = 7,
        FaultSummary = 8,
        OutputUv = 9,
        OutputCurrentWarning = 10,
        TemperatureWarning = 11,
        MosPowerLoopFault = 12,
        OtherFault = 13,

        Runing = 0xFF,
    }

    public enum TriggerOutputType
    {
        Close = 0,
        Signal = 1,
        Sync = 2,
    }

    public enum UpgradeState : byte
    {
        Updating = 0,
        Finish = 1,
        Erase = 2,

        SwitchBoardFail = 0xF0,
        WriteTimeFail = 0xF1,
        ReadFlashIdFail = 0xF2,
        TimeOut = 0xF3,
        Other = 0xF4,
        CommunicateFail = 0xF5,
        DataVerifyFail = 0xF6,
        Error = 0xFF,
    }

    public enum PatternErrorType : byte
    {
        Load = 0x01,
        Decode = 0x02,
        Timeout = 0x03,
    }

    public enum FirmwareType
    {
        FPGA = 0,
        ARM = 1,
    }
}
