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
        private Time _logInterval = new();
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
        private Time _errorLogInterval = new();
        public Time ErrorLogInterval
        {
            get => _errorLogInterval;
            set => SetProperty(ref _errorLogInterval, value);
        }
        private string _burnInBoardFile;
        [ValidationMetadata(Method = nameof(ValidateBurnInBoardFile))]
        public string BurnInBoardFile
        {
            get => _burnInBoardFile;
            set => SetProperty(ref _burnInBoardFile, value, true);
        }
        private BurnInBoardSetting _burnInBoardSetting;
        public BurnInBoardSetting BurnInBoardSetting
        {
            get => _burnInBoardSetting;
            set
            {
                if (SetProperty(ref _burnInBoardSetting, value))
                {
                    ThermostatSettings = value?.ThermostatSettings;
                }
            }
        }
        public ThermostatSetting[] ThermostatSettings = null;
        public ObservableCollection<DpsPlan> DpsPlans { get; } = new();
        public Process Process { get; } = new Process();
        public Ic(string name)
        {
            Name = name;
            ValidateAllProperties();
        }

        #region validate methods
        private ValidationResult ValidateBurnInBoardFile(string file)
        {
            if (string.IsNullOrWhiteSpace(file))
            {
                BurnInBoardSetting = null;
                return new ValidationResult(string.Format(LangProvider.Instance.PleaseSetMessage, LangProvider.Instance.DutConfigFileText));
            }
            if (!File.Exists(file))
            {
                BurnInBoardSetting = null;
                return new ValidationResult(LangProvider.Instance.FileNotExistMessage + $"[ {file} ]");
            }

            try
            {
                BurnInBoardSetting = BurnInBoardSettingParser.Parse(file);
                return ValidationResult.Success;
            }
            catch (CharReaderException e)
            {
                BurnInBoardSetting = null;
                return new ValidationResult(string.Format(LangProvider.Instance.PleaseSetMessage, file, e.Line, e.Column));
            }
            catch (Exception e)
            {
                BurnInBoardSetting = null;
                return new ValidationResult(e.Message);
            }
        }
        #endregion
    }
}
