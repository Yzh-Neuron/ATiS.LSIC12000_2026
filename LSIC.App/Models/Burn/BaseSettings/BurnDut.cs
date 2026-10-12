namespace LSIC.App.Models
{
    /// <summary>
    /// 单个工位（运行时）
    /// </summary>
    public class BurnDut : ObservableObject
    {
        public int Id { get; }
        public int Row { get; }
        public int Column { get; }
        public int ChipSelect { get; }
        public int[] Drs { get; }
        public int[] Dpss { get; }
        public BurnThermostat BurnThermostat { get; internal set; }

        public BurnDut(DutSetting setting, BurnThermostat thermostat = null)
        {
            Id = setting.Id;
            Row = setting.Row;
            Column = setting.Column;
            ChipSelect = setting.ChipSelect;
            Drs = setting.Drs;
            Dpss = setting.Dpss;
            BurnThermostat = thermostat;
        }

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
                if (Burning)
                {
                    return DutState.Test;
                }
                return DutState.Idle;
            }
        }

        private bool _masked;
        public bool Masked
        {
            get { return _masked; }
            set { SetFlag(ref _masked, value); }
        }

        private bool _burning;
        public bool Burning
        {
            get { return _burning; }
            set { SetFlag(ref _burning, value); }
        }

        private bool _finished;
        public bool Finished
        {
            get { return _finished; }
            set { SetFlag(ref _finished, value); }
        }

        private bool _error;
        public bool Error
        {
            get { return _error; }
            set { SetFlag(ref _error, value); }
        }

        private void SetFlag(ref bool field, bool value)
        {
            if (SetProperty(ref field, value))
            {
                OnPropertyChanged(nameof(State));
            }
        }
    }
}