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
        // 每客户端解析门闩：OnReceivedData 在 await 发送期间可能被下一包数据重入，
        // 串行化同一客户端的解析，防止共享 List<byte> 并发损坏
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _gates = new ConcurrentDictionary<string, SemaphoreSlim>();

        /// <summary>
        /// MBAP length 字段合法下限（最小 PDU：站号+功能码+地址+数量 = 6 字节）
        /// </summary>
        private const ushort MinAduLength = 6;

        /// <summary>
        /// MBAP length 字段合法上限（遵循 Modbus ADU 260 字节上限）
        /// </summary>
        private const ushort MaxAduLength = 260;

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
                // 直接 TryAdd：重复连接同一远端时保留既有缓冲，避免自旋等待拖住接受/接收循环
                _buffers.TryAdd(key, new List<byte>(2048));
            }
            else
            {
                // 直接 TryRemove：不存在时无需自旋；门闩不主动 Dispose（可能仍有在途解析持有），交由 GC 回收
                _buffers.TryRemove(key, out _);
                _gates.TryRemove(key, out _);
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

                SemaphoreSlim gate = _gates.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
                await gate.WaitAsync().ConfigureAwait(false);
                try
                {
                    buffer.AddRange(data);
                    while (buffer.Count > 6)
                    {
                        int index = 0;

                        ushort sn = (ushort)(buffer[index++] << 8 | buffer[index++]);
                        index += 2;
                        ushort length = (ushort)(buffer[index++] << 8 | buffer[index++]);

                        // 长度非法：帧边界已不可信，丢弃整段缓冲重新同步，防止毒帧永久卡死该连接
                        if (length < MinAduLength || length > MaxAduLength)
                        {
                            OnException(local, remote, new Exception($"invalid modbus frame length {length}."));
                            buffer.Clear();
                            break;
                        }
                        if (buffer.Count < 6 + length)
                        {
                            break;
                        }

                        // 先整体取出本帧并推进缓冲区：后续解析异常不会毒化剩余数据
                        byte[] frame = new byte[6 + length];
                        buffer.CopyTo(0, frame, 0, frame.Length);
                        buffer.RemoveRange(0, frame.Length);

                        index = 6;
                        byte station = frame[index++];
                        ModbusFunction func = (ModbusFunction)frame[index++];
                        ushort addr = (ushort)(frame[index++] << 8 | frame[index++]);

                        byte[] resp;
                        if (station != _slaveId)
                        {
                            resp = BuildExceptionFrame(sn, station, func, ModbusResponseCode.SlaveDeviceFailure);
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
                                        ushort quantity = (ushort)(frame[index++] << 8 | frame[index++]);
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
                                            data[i] = frame[index++];
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
                                        ushort quantity = (ushort)(frame[index++] << 8 | frame[index++]);
                                        byte thenLength = frame[index++];
                                        data = new byte[thenLength];
                                        for (int i = 0; i < data.Length; i++)
                                        {
                                            data[i] = frame[index++];
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

                        await SendToAsync(remote, resp);
                    }
                }
                finally
                {
                    gate.Release();
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
