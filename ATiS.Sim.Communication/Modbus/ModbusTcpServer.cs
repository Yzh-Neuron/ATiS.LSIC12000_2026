using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace ATiS.Sim.Communication
{
    public class ModbusTcpServer : TcpServer
    {
        private readonly byte _slaveId;
        private readonly ConcurrentDictionary<string, List<byte>> _buffers = new ConcurrentDictionary<string, List<byte>>();

        public ModbusTcpServer(byte slaveId = 0x01, ushort localPort = 502, int backlog = int.MaxValue, int receiveBufferSize = 4096)
            : base(localPort, backlog, receiveBufferSize)
        {
            _slaveId = slaveId;
        }

        public bool[] Cols { get; set; }

        public bool[] DiscreteInputs { get; set; }

        public short[] InputRegisters { get; set; }

        public short[] HoldingRegisters { get; set; }

        protected override void OnStateChanged(IPEndPoint local, IPEndPoint remote, bool connected)
        {
            base.OnStateChanged(local, remote, connected);
            string key = $"{remote}";
            if (connected)
            {
                List<byte> buffer = new List<byte>(2048);
                while (!_buffers.TryAdd(key, buffer))
                {
                    Thread.Sleep(25);
                }
            }
            else
            {
                while (!_buffers.TryRemove(key, out _))
                {
                    Thread.Sleep(25);
                }
            }
        }

        protected override async void OnReceivedData(IPEndPoint local, IPEndPoint remote, byte[] data)
        {
            try
            {
                string key = $"{remote}";
                if (!_buffers.TryGetValue(key, out List<byte> buffer))
                {
                    return;
                }

                buffer.AddRange(data);
                int index;
                while (buffer.Count > 6)
                {
                    index = 0;

                    ushort sn = (ushort)(buffer[index++] << 8 | buffer[index++]);
                    index += 2;
                    ushort length = (ushort)(buffer[index++] << 8 | buffer[index++]);
                    if (buffer.Count < length + index)
                    {
                        break;
                    }

                    byte station = buffer[index++];
                    ModbusFunction func = (ModbusFunction)buffer[index++];
                    ushort addr = (ushort)(buffer[index++] << 8 | buffer[index++]);

                    byte[] resp;
                    if (station != _slaveId)
                    {
                        resp = BuildExceptionFrame(sn, station, func, ModbusResponseCode.SlaveDeviceFailure);
                        index = 6 + length;
                    }
                    else
                    {
                        ModbusResponseCode code;
                        switch (func)
                        {
                            case ModbusFunction.ReadCoils:
                            case ModbusFunction.ReadDiscreteInputs:
                            case ModbusFunction.ReadHoldingRegisters:
                            case ModbusFunction.ReadInputRegisters:
                                {
                                    ushort quantity = (ushort)(buffer[index++] << 8 | buffer[index++]);
                                    code = Read(func, addr, quantity, out resp);
                                    resp = code == ModbusResponseCode.Success
                                        ? BuildReadFrame(sn, station, func, resp)
                                        : BuildExceptionFrame(sn, station, func, code);
                                }
                                break;
                            case ModbusFunction.WriteSingleCoil:
                            case ModbusFunction.WriteSingleRegister:
                                {
                                    data = new byte[length - 4];
                                    for (int i = 0; i < data.Length; i++)
                                    {
                                        data[i] = buffer[index++];
                                    }

                                    code = Write(func, addr, 1, data, out resp);
                                    resp = code == ModbusResponseCode.Success
                                        ? BuildWriteSingleFrame(sn, station, func, addr, resp)
                                        : BuildExceptionFrame(sn, station, func, code);
                                }
                                break;
                            case ModbusFunction.WriteMultipleCoils:
                            case ModbusFunction.WriteMultipleRegisters:
                                {
                                    ushort quantity = (ushort)(buffer[index++] << 8 | buffer[index++]);
                                    byte thenLength = buffer[index++];
                                    data = new byte[thenLength];
                                    for (int i = 0; i < data.Length; i++)
                                    {
                                        data[i] = buffer[index++];
                                    }

                                    code = Write(func, addr, quantity, data, out _);
                                    resp = code == ModbusResponseCode.Success
                                        ? BuildWriteMultiFrame(sn, station, func, addr, quantity)
                                        : BuildExceptionFrame(sn, station, func, code);
                                }
                                break;
                            default:
                                resp = BuildExceptionFrame(sn, station, func, ModbusResponseCode.IllegalFunction);
                                break;
                        }
                    }

                    buffer.RemoveRange(0, index);
                    await SendToAsync(remote, resp);
                }
            }
            catch (Exception e)
            {
                OnException(local, remote, e);
            }
        }

        private ModbusResponseCode Read(ModbusFunction func, ushort addr, ushort quantity, out byte[] resp)
        {
            resp = null;
            switch (func)
            {
                case ModbusFunction.ReadCoils:
                    {
                        if (Cols == null)
                        {
                            return ModbusResponseCode.IllegalFunction;
                        }
                        if (Cols.Length < addr + quantity)
                        {
                            return ModbusResponseCode.IllegalDataAddress;
                        }
                        resp = new byte[(int)Math.Ceiling(quantity * 1.0 / 8)];
                        for (int i = 0; i < quantity; i++)
                        {
                            byte bit = Convert.ToByte(Cols[addr + i]);
                            int p = i / 8;
                            int bp = i % 8;
                            byte readByte = resp[p];
                            readByte &= (byte)~(1 << bp);
                            readByte |= (byte)(bit << bp);
                            resp[p] = readByte;
                        }
                    }
                    break;
                case ModbusFunction.ReadDiscreteInputs:
                    {
                        if (DiscreteInputs == null)
                        {
                            return ModbusResponseCode.IllegalFunction;
                        }
                        if (DiscreteInputs.Length < addr + quantity)
                        {
                            return ModbusResponseCode.IllegalDataAddress;
                        }
                        resp = new byte[(int)Math.Ceiling(quantity * 1.0 / 8)];
                        for (int i = 0; i < quantity; i++)
                        {
                            byte bit = Convert.ToByte(DiscreteInputs[addr + i]);
                            int p = i / 8;
                            int bp = i % 8;
                            byte readByte = resp[p];
                            readByte &= (byte)~(1 << bp);
                            readByte |= (byte)(bit << bp);
                            resp[p] = readByte;
                        }
                    }
                    break;
                case ModbusFunction.ReadHoldingRegisters:
                    {
                        if (HoldingRegisters == null)
                        {
                            return ModbusResponseCode.IllegalFunction;
                        }
                        if (HoldingRegisters.Length < addr + quantity)
                        {
                            return ModbusResponseCode.IllegalDataAddress;
                        }
                        resp = new byte[quantity * 2];
                        for (int i = 0; i < quantity; i++)
                        {
                            short value = HoldingRegisters[addr + i];
                            resp[2 * i] = (byte)(value >> 8);
                            resp[2 * i + 1] = (byte)(value & 0xFF);
                        }
                    }
                    break;
                case ModbusFunction.ReadInputRegisters:
                    if (InputRegisters == null)
                    {
                        return ModbusResponseCode.IllegalFunction;
                    }
                    if (InputRegisters.Length < addr + quantity)
                    {
                        return ModbusResponseCode.IllegalDataAddress;
                    }
                    resp = new byte[quantity * 2];
                    for (int i = 0; i < quantity; i++)
                    {
                        short value = InputRegisters[addr + i];
                        resp[2 * i] = (byte)(value >> 8);
                        resp[2 * i + 1] = (byte)(value & 0xFF);
                    }
                    break;
                default:
                    throw new InvalidOperationException();
            }
            return ModbusResponseCode.Success;
        }

        private ModbusResponseCode Write(ModbusFunction func, ushort addr, ushort quantity, byte[] data, out byte[] resp)
        {
            resp = null;
            switch (func)
            {
                case ModbusFunction.WriteSingleCoil:
                    {
                        if (Cols == null)
                        {
                            return ModbusResponseCode.IllegalFunction;
                        }
                        if (Cols.Length < addr + quantity)
                        {
                            return ModbusResponseCode.IllegalDataAddress;
                        }
                        if (data.Length != 2)
                        {
                            return ModbusResponseCode.IllegalDataValue;
                        }
                        short value = (short)((data[0] << 8) | data[1]);
                        bool bit = value == -256;
                        Cols[addr] = bit;
                        resp = data;
                    }
                    break;
                case ModbusFunction.WriteMultipleCoils:
                    {
                        if (Cols == null)
                        {
                            return ModbusResponseCode.IllegalFunction;
                        }
                        if (Cols.Length < addr + quantity)
                        {
                            return ModbusResponseCode.IllegalDataAddress;
                        }
                        if (data.Length != (int)Math.Ceiling(quantity * 1.0 / 8))
                        {
                            return ModbusResponseCode.IllegalDataValue;
                        }
                        for (int i = 0; i < quantity; i++)
                        {
                            byte value = data[i / 8];
                            bool bit = (value & (0x01 << (i % 8))) > 0;
                            Cols[addr + i] = bit;
                        }
                        resp = data;
                    }
                    break;
                case ModbusFunction.WriteSingleRegister:
                    {
                        if (HoldingRegisters == null)
                        {
                            return ModbusResponseCode.IllegalFunction;
                        }
                        if (HoldingRegisters.Length < addr + quantity)
                        {
                            return ModbusResponseCode.IllegalDataAddress;
                        }
                        if (data.Length != 2)
                        {
                            return ModbusResponseCode.IllegalDataValue;
                        }
                        short value = (short)((data[0] << 8) | data[1]);
                        HoldingRegisters[addr] = value;
                        resp = data;
                    }
                    break;
                case ModbusFunction.WriteMultipleRegisters:
                    {
                        if (HoldingRegisters == null)
                        {
                            return ModbusResponseCode.IllegalFunction;
                        }
                        if (HoldingRegisters.Length < addr + quantity)
                        {
                            return ModbusResponseCode.IllegalDataAddress;
                        }
                        if (data.Length != 2 * quantity)
                        {
                            return ModbusResponseCode.IllegalDataValue;
                        }
                        for (int i = 0; i < quantity; i++)
                        {
                            short value = (short)((data[2 * i] << 8) | data[2 * i + 1]);
                            HoldingRegisters[addr + i] = value;
                        }
                        resp = data;
                    }
                    break;
                default:
                    throw new InvalidOperationException();
            }
            return default;
        }

        private static byte[] BuildReadFrame(ushort sn, byte station, ModbusFunction func, byte[] data)
        {
            byte[] frame = new byte[9 + data.Length];
            int index = 0;

            // sn
            frame[index++] = (byte)(sn >> 8);
            frame[index++] = (byte)(sn & 0xFF);

            // modbus tcp flag
            frame[index++] = 0x00;
            frame[index++] = 0x00;

            // then length
            int thenLength = 3 + data.Length;
            frame[index++] = (byte)(thenLength >> 8);
            frame[index++] = (byte)(thenLength & 0xFF);

            // station
            frame[index++] = station;

            // function
            frame[index++] = (byte)func;

            // length
            frame[index++] = (byte)data.Length;

            Array.Copy(data, 0, frame, index, data.Length);

            return frame;
        }

        private static byte[] BuildWriteSingleFrame(ushort sn, byte station, ModbusFunction func, ushort addr, byte[] data)
        {
            byte[] frame = new byte[12];
            int index = 0;

            // sn
            frame[index++] = (byte)(sn >> 8);
            frame[index++] = (byte)(sn & 0xFF);

            // modbus tcp flag
            frame[index++] = 0x00;
            frame[index++] = 0x00;

            // then length
            frame[index++] = 0x00;
            frame[index++] = 0x06;

            // station
            frame[index++] = station;

            // func
            frame[index++] = (byte)func;

            // addr
            frame[index++] = (byte)(addr >> 8);
            frame[index++] = (byte)(addr & 0xFF);

            Array.Copy(data, 0, frame, index, data.Length);

            return frame;
        }

        private static byte[] BuildWriteMultiFrame(ushort sn, byte station, ModbusFunction func, ushort addr, ushort quantity)
        {
            byte[] frame = new byte[12];
            int index = 0;

            // sn
            frame[index++] = (byte)(sn >> 8);
            frame[index++] = (byte)(sn & 0xFF);

            // modbus tcp flag
            frame[index++] = 0x00;
            frame[index++] = 0x00;

            // then length
            frame[index++] = 0x00;
            frame[index++] = 0x06;

            // station
            frame[index++] = station;

            // func
            frame[index++] = (byte)func;

            // addr
            frame[index++] = (byte)(addr >> 8);
            frame[index++] = (byte)(addr & 0xFF);

            // quantity
            frame[index++] = (byte)(quantity >> 8);
            frame[index++] = (byte)(quantity & 0xFF);

            return frame;
        }

        private static byte[] BuildExceptionFrame(ushort sn, byte station, ModbusFunction func, ModbusResponseCode code)
        {
            byte[] frame = new byte[9];
            int index = 0;

            // sn
            frame[index++] = (byte)(sn >> 8);
            frame[index++] = (byte)(sn & 0xFF);

            // modbus tcp flag
            frame[index++] = 0x00;
            frame[index++] = 0x00;

            // then length
            frame[index++] = 0x00;
            frame[index++] = 0x04;

            // station
            frame[index++] = station;

            // func + 0x80
            frame[index++] = (byte)(func + 0x80);

            // code
            frame[index++] = (byte)code;

            return frame;
        }
    }
}
