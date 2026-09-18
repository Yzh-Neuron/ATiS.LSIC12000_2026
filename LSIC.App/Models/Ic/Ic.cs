namespace LSIC.App.Models
{
    public class Ic : ObservableValidator
    {
        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        //记录间隔时间
        private Time _logInterval;
        public Time LogInterval
        {
            get => _logInterval;
            set => SetProperty(ref _logInterval, value);
        }
        private bool _errorLogEnable;
        public bool ErrorLogEnable
        {
            get => _errorLogEnable;
            set => SetProperty(ref _errorLogEnable, value);
        }
        private Time _errorLogInterval;
        public Time ErrorLogInterval
        {
            get => _errorLogInterval;
            set => SetProperty(ref _errorLogInterval, value);
        }


        //老化板配置信息
        private string _burnInBoardFile;
        public string BurnInBoardFile
        {
            get => _burnInBoardFile;
            set => SetProperty(ref _burnInBoardFile, value);
        }
        private DutSetting[] _dutSettings;
        public DutSetting[] DutSettings
        {
            get => _dutSettings;
            set => SetProperty(ref _dutSettings, value);
        }
        private ThermostatSetting[] _thermostatSettings;
        public ThermostatSetting[] ThermostatSettings
        {
            get => _thermostatSettings;
            set => SetProperty(ref _thermostatSettings, value);
        }

        //流程
        public Process Process { get; set; } = new Process();
        public Ic(string name)
        {
            Name = name;
        }
    }
}
