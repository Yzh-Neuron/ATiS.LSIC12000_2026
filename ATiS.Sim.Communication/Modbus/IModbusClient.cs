using System.Threading.Tasks;

namespace ATiS.Sim.Communication
{
    internal interface IModbusClient
    {
        Task<bool?> ReadCoilAsync(ushort address);

        Task<bool[]> ReadCoilsAsync(ushort address, ushort quantity);

        Task<bool?> ReadDiscreteInputAsync(ushort address);

        Task<bool[]> ReadDiscreteInputsAsync(ushort address, ushort quantity);

        Task<short?> ReadHoldingRegisterAsync(ushort address);

        Task<short[]> ReadHoldingRegistersAsync(ushort address, ushort quantity);

        Task<short?> ReadInputRegisterAsync(ushort address);

        Task<short[]> ReadInputRegistersAsync(ushort address, ushort quantity);

        Task<bool> WriteSingleCoilAsync(ushort address, bool value);

        Task<bool> WriteMultipleCoilsAsync(ushort address, bool[] values);

        Task<bool> WriteSingleRegisterAsync(ushort address, short value);

        Task<bool> WriteMultipleRegistersAsync(ushort address, short[] values);
    }
}
