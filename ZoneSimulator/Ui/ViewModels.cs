using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ZoneSimulator
{
    /// <summary>界面绑定用枚举列表</summary>
    public static class EnumLists
    {
        public static readonly ZoneState[] ZoneStates =
        {
            ZoneState.Init, ZoneState.Idle, ZoneState.Ready, ZoneState.PowerUp, ZoneState.Burning,
            ZoneState.PowerDown, ZoneState.Stop, ZoneState.Transition,
            ZoneState.DeviceChecking, ZoneState.DpsChecking, ZoneState.Upgrading, ZoneState.DeviceCheckFail,
        };

        public static readonly DpsState[] DpsStates =
        {
            DpsState.Off, DpsState.Runing, DpsState.OutputOv, DpsState.OutputOc, DpsState.InputOvUv,
            DpsState.ChipOt, DpsState.CommLogicFault, DpsState.VendorCustomFault, DpsState.PgAbnormal,
            DpsState.OutputUv, DpsState.OutputCurrentWarning, DpsState.TemperatureWarning,
            DpsState.MosPowerLoopFault, DpsState.OtherFault,
        };

        public static readonly DpsState[] DpsFaultStates =
        {
            DpsState.OutputOv, DpsState.OutputOc, DpsState.InputOvUv, DpsState.ChipOt,
            DpsState.CommLogicFault, DpsState.VendorCustomFault, DpsState.PgAbnormal, DpsState.OutputUv,
            DpsState.OutputCurrentWarning, DpsState.TemperatureWarning, DpsState.MosPowerLoopFault,
            DpsState.OtherFault,
        };

        public static readonly TriggerOutputType[] TriggerTypes =
        {
            TriggerOutputType.Close, TriggerOutputType.Signal, TriggerOutputType.Sync,
        };

        public static readonly PatternErrorType[] PatternErrorTypes =
        {
            PatternErrorType.Load, PatternErrorType.Decode, PatternErrorType.Timeout,
        };
    }

    /// <summary>老化区列表项</summary>
    public sealed class ZoneVm : INotifyPropertyChanged
    {
        public ZoneVm(ZoneSim sim)
        {
            Sim = sim;
        }

        public ZoneSim Sim { get; }

        public string Header => $"Z{Sim.Id}  {Sim.Remote}";

        public string Detail =>
            $"{StateText}  {(Sim.Plugged ? "已插板" : "未插板")}  {Sim.Cost}/{Sim.Total}s";

        public string StateText => Sim.State == ZoneState.Error ? "Error" : Sim.State.ToString();

        public void Refresh()
        {
            OnPropertyChanged(nameof(Header));
            OnPropertyChanged(nameof(Detail));
            OnPropertyChanged(nameof(StateText));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    /// <summary>电源行：编辑写回 DpsSim，定时刷新显示</summary>
    public sealed class DpsRowVm : INotifyPropertyChanged
    {
        private readonly DpsSim _dps;

        public DpsRowVm(DpsSim dps)
        {
            _dps = dps;
        }

        public string Name => _dps.Name;

        public int InputVoltage
        {
            get => _dps.InputVoltage;
            set => _dps.InputVoltage = value;
        }

        public int OutputVoltage
        {
            get => _dps.OutputVoltage;
            set
            {
                _dps.OutputVoltage = value;
                _dps.TargetVoltage = value;
            }
        }

        public int OutputCurrent
        {
            get => _dps.OutputCurrent;
            set
            {
                _dps.OutputCurrent = value;
                _dps.TargetCurrent = value;
            }
        }

        public int Temperature
        {
            get => _dps.Temperature;
            set => _dps.Temperature = value;
        }

        public DpsState State
        {
            get => _dps.State;
            set => _dps.State = value;
        }

        public void RefreshDisplay()
        {
            OnPropertyChanged(nameof(InputVoltage));
            OnPropertyChanged(nameof(OutputVoltage));
            OnPropertyChanged(nameof(OutputCurrent));
            OnPropertyChanged(nameof(Temperature));
            OnPropertyChanged(nameof(State));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
