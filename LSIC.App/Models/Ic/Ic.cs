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
        private bool _forceStepLoad;
        public bool ForceStepLoad
        {
            get => _forceStepLoad;
            set => SetProperty(ref _forceStepLoad, value);
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
            set
            {
                if (SetProperty(ref _burnInBoardFile, value, true))
                {
                    if (string.IsNullOrEmpty(value) || !File.Exists(value))
                    {
                        BurnInBoardSetting = null;
                        return;
                    }

                    try
                    {
                        BurnInBoardSetting = BurnInBoardSettingParser.Parse(value);
                    }
                    catch
                    {
                        BurnInBoardSetting = null;
                    }
                }
            }
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
        public ThermostatSetting[] ThermostatSettings { get; set; } = null;
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
                return new ValidationResult(string.Format(LangProvider.Instance.PleaseSetMessage, LangProvider.Instance.DutConfigFileText));
            }
            if (!File.Exists(file))
            {
                return new ValidationResult(LangProvider.Instance.FileNotExistMessage + $"[ {file} ]");
            }

            try
            {
                _ = BurnInBoardSettingParser.Parse(file);
                return ValidationResult.Success;
            }
            catch (CharReaderException e)
            {
                return new ValidationResult(string.Format(LangProvider.Instance.PleaseSetMessage, file, e.Line, e.Column));
            }
            catch (Exception e)
            {
                return new ValidationResult(e.Message);
            }
        }
        #endregion
    }
}
