namespace LSIC.App.Models
{
    /// <summary>
    /// 单个老化工位信息
    /// </summary>
    public class Dut : ObservableObject, ICloneable
    {
        public Dut(int? zoneId, int id, int row, int column, int cs, int[] drs, int[] dpss, int? thermostatId)
        {
            ZoneId = zoneId;
            Id = id;
            Row = row;
            Column = column;
            ChipSelect = cs;
            Drs = drs;
            Dpss = dpss;

            ThermostatId = thermostatId;
        }

        public int? ZoneId { get; }

        public int Id { get; }

        public int Row { get; }

        public int Column { get; }

        public int ChipSelect { get; }

        public int[] Drs { get; }

        public int[] Dpss { get; }

        public int? ThermostatId { get; }

        public Thermostat Thermostat { get; }

        public DutState State
        {
            get
            {
                if (Masked)
                {
                    return DutState.Ignore;
                }
                if (Error)
                {
                    return DutState.Fail;
                }
                if (Finished)
                {
                    return DutState.Pass;
                }

                return Burning ? DutState.Test : DutState.Idle;
            }
        }

        private bool _masked;
        public bool Masked
        {
            get => _masked;
            set
            {
                if (_masked != value)
                {
                    _masked = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(State));
                }
            }
        }

        private bool _burning;
        public bool Burning
        {
            get => _burning;
            set
            {
                if (_burning != value)
                {
                    _burning = value;
                    _finished = false;

                    if (_burning) // 开始老化，清除历史数据
                    {
                        Thermostat.PvData.Clear();
                    }

                    OnPropertyChanged(nameof(State));
                }
            }
        }

        private bool _finished;
        public bool Finished
        {
            get => _finished;
            set
            {
                if (_finished != value)
                {
                    _finished = value;
                    _burning = false;
                    OnPropertyChanged(nameof(State));
                }
            }
        }

        private bool _error;
        public bool Error
        {
            get => _error;
            set
            {
                if (_error != value)
                {
                    _error = value;
                    OnPropertyChanged(nameof(State));
                }
            }
        }

        public object Clone()
        {
            Dut dut = new Dut(ZoneId, Id, Row, Column, ChipSelect, Drs, Dpss, Thermostat?.Id)
            {
                Masked = Masked
            };
            return dut;
        }
    }
}