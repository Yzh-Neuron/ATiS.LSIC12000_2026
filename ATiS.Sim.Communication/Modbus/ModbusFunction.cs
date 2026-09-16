namespace ATiS.Sim.Communication
{
    public enum ModbusFunction : byte
    {
        ReadCoils = 0x01,
        ReadDiscreteInputs,
        ReadHoldingRegisters,
        ReadInputRegisters,
        WriteSingleCoil,
        WriteSingleRegister,
        WriteMultipleCoils = 0x0F,
        WriteMultipleRegisters = 0x10
    }
}
