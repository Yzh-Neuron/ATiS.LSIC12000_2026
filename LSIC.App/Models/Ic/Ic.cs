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

        //老化板配置
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
            set => SetProperty(ref _burnInBoardSetting, value);
        }

        //Dps电源方案
        public ObservableCollection<DpsPlan> DpsPlans { get; } = new();

        //流程
        public Process Process { get; } = new Process();
        public Ic(string name)
        {
            Name = name;
        }

        #region validate methods
        private ValidationResult ValidateBurnInBoardFile(string file)
        {
            if (string.IsNullOrWhiteSpace(file))
            {
                BurnInBoardSetting = null;
                return new ValidationResult(string.Format(LangProvider.Instance.PleaseSetMessage, LangProvider.Instance.DutConfigFileText), [LangProvider.Instance.DutConfigFileText]);
            }
            if (!File.Exists(file))
            {
                BurnInBoardSetting = null;
                return new ValidationResult(LangProvider.Instance.FileNotExistMessage + $"[ {file} ]", [LangProvider.Instance.DutConfigFileText]);
            }

            try
            {
                BurnInBoardSetting = BurnInBoardSettingParser.Parse(file);
                return ValidationResult.Success;
            }
            catch (CharReaderException e)
            {
                BurnInBoardSetting = null;
                return new ValidationResult(string.Format(LangProvider.Instance.PleaseSetMessage, file, e.Line, e.Column), [LangProvider.Instance.DutConfigFileText]);
            }
            catch (Exception e)
            {
                BurnInBoardSetting = null;
                return new ValidationResult(e.Message, [LangProvider.Instance.DutConfigFileText]);
            }
        }
        #endregion
    }
}
