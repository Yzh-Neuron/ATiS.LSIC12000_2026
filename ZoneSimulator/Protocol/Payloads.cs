using System;
using System.Collections.Generic;

namespace ZoneSimulator
{
    /// <summary>各类上报帧的数据段构造，字节布局严格对照 Zone.AnalysisFrame 的解析顺序</summary>
    public static class Payloads
    {
        /// <summary>[state][loadMode]</summary>
        public static byte[] State(ZoneState state, LoadMode mode)
        {
            ByteWriter w = new();
            w.U8((byte)state);
            w.U8((byte)mode);
            return w.ToArray();
        }

        /// <summary>[0xFF][loadMode][errorType][8 字节错误明细]</summary>
        public static byte[] StateError(ZoneErrorType type, LoadMode mode, byte[] detail8)
        {
            ByteWriter w = new();
            w.U8((byte)ZoneState.Error);
            w.U8((byte)mode);
            w.U8((byte)type);

            byte[] detail = new byte[8];
            if (detail8 is not null)
            {
                Array.Copy(detail8, detail, Math.Min(8, detail8.Length));
            }

            w.Bytes(detail);
            return w.ToArray();
        }

        /// <summary>[dpsState][dpsType][id][0...]</summary>
        public static byte[] DpsErrorDetail(DpsState state, DpsType type, byte id)
        {
            byte[] detail = new byte[8];
            detail[0] = (byte)state;
            detail[1] = (byte)type;
            detail[2] = id;
            return detail;
        }

        /// <summary>[patternErrorType][stepId][vectorId][0...]</summary>
        public static byte[] PatternErrorDetail(PatternErrorType type, byte stepId, byte vectorId)
        {
            byte[] detail = new byte[8];
            detail[0] = (byte)type;
            detail[1] = stepId;
            detail[2] = vectorId;
            return detail;
        }

        /// <summary>[mask][count] + count × 20 字节单路电源</summary>
        public static byte[] DpsStateReport(IReadOnlyList<DpsSim> dps, IReadOnlyList<DpsSim> vih)
        {
            ByteWriter w = new();

            byte mask = 0;
            for (int i = 0; i < dps.Count && i < 8; i++)
            {
                if (dps[i].IsOn)
                {
                    mask |= (byte)(1 << i);
                }
            }

            w.U8(mask);
            w.U8(dps.Count + vih.Count);

            foreach (DpsSim item in dps)
            {
                WriteDps(w, item);
            }

            foreach (DpsSim item in vih)
            {
                WriteDps(w, item);
            }

            return w.ToArray();
        }

        private static void WriteDps(ByteWriter w, DpsSim dps)
        {
            w.U8(dps.Id);
            w.U8(0);
            w.U8((byte)dps.Type);
            w.I32(dps.InputVoltage);
            w.I32(dps.OutputVoltage);
            w.I32(dps.OutputCurrent);
            w.I32(dps.Temperature);
            w.U8((byte)dps.State);
        }

        /// <summary>[id]</summary>
        public static byte[] CurrentDpsPlan(byte id) => new[] { id };

        /// <summary>27 字节老化进程</summary>
        public static byte[] BurnState(
            int totalSeconds,
            byte stepId,
            ExecuteMode mode,
            int runTotal,
            int runCost,
            bool loopEnable,
            byte loopStart,
            byte loopEnd,
            ExecuteMode loopMode,
            int loopTotal,
            int loopCost,
            byte vectorId)
        {
            ByteWriter w = new();
            w.I32(totalSeconds);
            w.U8(stepId);
            w.U8((byte)mode);
            w.I32(runTotal);
            w.I32(runCost);
            w.U8(loopEnable ? 1 : 0);
            w.U8(loopStart);
            w.U8(loopEnd);
            w.U8((byte)loopMode);
            w.I32(loopTotal);
            w.I32(loopCost);
            w.U8(vectorId);
            return w.ToArray();
        }

        /// <summary>[stepId][vectorId][count(2)] + count × [cs][line(4)][mask(32)]</summary>
        public static byte[] CompareErrors(byte stepId, byte vectorId, IReadOnlyList<CompareErrorItem> items)
        {
            ByteWriter w = new();
            w.U8(stepId);
            w.U8(vectorId);
            w.I16(items.Count);

            foreach (CompareErrorItem item in items)
            {
                w.U8(item.Cs);
                w.I32(item.Line);

                byte[] mask = new byte[32];
                if (item.Pin >= 0 && item.Pin < 256)
                {
                    mask[item.Pin / 8] |= (byte)(1 << (item.Pin % 8));
                }

                w.Bytes(mask);
            }

            return w.ToArray();
        }

        /// <summary>[type][pin?]（仅 Signal 带 pin）</summary>
        public static byte[] TriggerReport(TriggerOutputType type, byte pin)
        {
            ByteWriter w = new();
            w.U8((byte)type);
            if (type == TriggerOutputType.Signal)
            {
                w.U8(pin);
            }

            return w.ToArray();
        }

        /// <summary>[code][id][parent][PN(16)][SN(16)][hardware][firmwareCount] + count × 21 字节固件</summary>
        public static byte[] Board(
            byte code,
            byte id,
            byte parent,
            string pn,
            string sn,
            byte hardware,
            params FirmwareEntry[] firmwares)
        {
            ByteWriter w = new();
            w.U8(code);
            w.U8(id);
            w.U8(parent);
            w.FixedString(pn, 16);
            w.FixedString(sn, 16);
            w.U8(hardware & 0x0F);
            w.U8(firmwares.Length);

            foreach (FirmwareEntry firmware in firmwares)
            {
                w.U8((byte)firmware.Type);
                w.I32(firmware.Version);
                w.I64(-1);
                w.I64(-1);
            }

            return w.ToArray();
        }

        /// <summary>[type][item][result][location]</summary>
        public static byte[] DeviceCheckItem(byte type, byte item, byte result, byte location)
        {
            return new[] { type, item, result, location };
        }

        /// <summary>[current-1(4)][total(4)][readTimes-1(4)][setTimes(4)][id] + 电源状态 17 字节</summary>
        public static byte[] DpsCheckResult(int current, int total, int readTimes, int setTimes, DpsSim dps, DpsState state)
        {
            ByteWriter w = new();
            w.I32(current - 1);
            w.I32(total);
            w.I32(readTimes - 1);
            w.I32(setTimes);
            w.U8(dps.Id);
            w.I32(dps.InputVoltage);
            w.I32(dps.OutputVoltage);
            w.I32(dps.OutputCurrent);
            w.I32(dps.Temperature);
            w.U8((byte)state);
            return w.ToArray();
        }

        /// <summary>[count] + count × [id][enable][temperature(4)]</summary>
        public static byte[] Thermostats(IReadOnlyList<ThermostatReport> items)
        {
            ByteWriter w = new();
            w.U8(items.Count);
            foreach (ThermostatReport item in items)
            {
                w.U8(item.Id);
                w.U8(item.Enable ? 1 : 0);
                w.I32(item.Temperature);
            }

            return w.ToArray();
        }

        /// <summary>[count] + count × [id][kp(4)][ki(4)][kd(4)]</summary>
        public static byte[] ThermostatPids(IReadOnlyList<ThermostatPid> items)
        {
            ByteWriter w = new();
            w.U8(items.Count);
            foreach (ThermostatPid item in items)
            {
                w.U8(item.Id);
                w.I32(item.Kp);
                w.I32(item.Ki);
                w.I32(item.Kd);
            }

            return w.ToArray();
        }

        /// <summary>[boardCode][id][progress][state]</summary>
        public static byte[] UpgradeProgress(byte boardCode, byte boardId, byte progress, UpgradeState state)
        {
            return new[] { boardCode, boardId, progress, (byte)state };
        }
    }

    public readonly struct CompareErrorItem
    {
        public CompareErrorItem(byte cs, int line, int pin)
        {
            Cs = cs;
            Line = line;
            Pin = pin;
        }

        public byte Cs { get; }
        public int Line { get; }
        public int Pin { get; }
    }

    public readonly struct FirmwareEntry
    {
        public FirmwareEntry(FirmwareType type, int version)
        {
            Type = type;
            Version = version;
        }

        public FirmwareType Type { get; }
        public int Version { get; }
    }

    public readonly struct ThermostatReport
    {
        public ThermostatReport(byte id, bool enable, int temperature)
        {
            Id = id;
            Enable = enable;
            Temperature = temperature;
        }

        public byte Id { get; }
        public bool Enable { get; }
        public int Temperature { get; }
    }

    public readonly struct ThermostatPid
    {
        public ThermostatPid(byte id, int kp, int ki, int kd)
        {
            Id = id;
            Kp = kp;
            Ki = ki;
            Kd = kd;
        }

        public byte Id { get; }
        public int Kp { get; }
        public int Ki { get; }
        public int Kd { get; }
    }
}
