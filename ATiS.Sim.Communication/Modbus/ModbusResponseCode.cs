namespace ATiS.Sim.Communication
{
    internal enum ModbusResponseCode : byte
    {
        Success,
        IllegalFunction,
        IllegalDataAddress,
        IllegalDataValue,
        SlaveDeviceFailure,
        Acknowledge,
        SlaveDeviceBusy,
        NegativeAcknowledge,
        MemoryParityError,
        GatewayPathUnavailable = 0x0A,
        GatewayTargetDeviceFailedToRespond
    }
}
