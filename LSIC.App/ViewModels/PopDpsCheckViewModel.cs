namespace LSIC.App.ViewModels
{
    public class PopDpsCheckViewModel : ObservableValidator
    {
        public Zone[] Zones { get; }
        private Zone _selectedZone;
        public Zone SelectedZone
        {
            get => _selectedZone;
            set => SetProperty(ref _selectedZone, value);
        }
        private string _selectedScript;
        [ValidationMetadata(Method = nameof(ValidateSelectedScript))]
        public string SelectedScript
        {
            get => _selectedScript;
            set => SetProperty(ref _selectedScript, value, true);
        }
        public ICommand BrowseCommand { get; }
        public ICommand RunCommand { get; }
        public PopDpsCheckViewModel()
        {
            ValidateAllProperties();
            Zones = Device.Instance.Zones;
            _selectedZone = Zones.FirstOrDefault();

            BrowseCommand = new RelayCommand(Browse);
            RunCommand = new AsyncRelayCommand(RunAsync, CanRun);
        }
        private void Browse()
        {
            string filter = $"{LangProvider.Instance.DpsCheckScriptText}|*.dps";
            var result = MessageBus.Default.Send(MsgArgs.From(PopDpsCheckMessage.OpenFile, [filter, SelectedScript])).First();
            if (result.Result is string file)
            {
                SelectedScript = file;
            }
        }
        private bool CanRun()
        {
            return SelectedZone != null && !string.IsNullOrWhiteSpace(SelectedScript) && SelectedZone.State.CanCheck();
        }
        private async Task RunAsync()
        {
            List<ValidationResult> errors = this.GetAllExistingErrorsRecursive();
            if (errors.Count > 0)
            {
                MessageBus.Default.Send(MsgArgs.From(PopDpsCheckMessage.Growl, [errors.ToMessageString(), MessageType.Error]));
                return;
            }

            DpsCommand[] commands = DpsCheckScriptParser.Parse(SelectedScript);
            foreach (DpsCommand command in commands)
            {
                DpsInfo info = Device.Instance.SystemConfig.DpsInfos.FirstOrDefault(i => i.Id == command.Id);
                if (info == null)
                {
                    MessageBus.Default.Send(MsgArgs.From(PopDpsCheckMessage.Growl, [string.Format(LangProvider.Instance.DpsCheckScriptDpsNotFoundMessage, command.Id, command.Line), MessageType.Error]));
                    return;
                }

                DpsSetting setting = new DpsSetting(info)
                {
                    Vol = command.Vol,
                    Ovp = command.Ovp,
                    Uvp = command.Uvp,
                    Ocp = command.Ocp,
                    Raise = command.Raise,
                    Delay = 0
                };

                errors = setting.GetAllExistingErrorsRecursive();
                if (errors.Count > 0)
                {
                    MessageBus.Default.Send(MsgArgs.From(PopDpsCheckMessage.Growl, [string.Format(LangProvider.Instance.DpsCheckScriptParameterErrordMessage, command.Line), MessageType.Error]));
                    return;
                }
            }

            if (!SelectedZone.State.CanCheck())
            {
                MessageBus.Default.Send(MsgArgs.From(PopDpsCheckMessage.Growl, [LangProvider.Instance.DpsCheckIsNotPermiitedMessage, MessageType.Error]));
                return;
            }

            if (SelectedZone.Pluged)
            {
                var msgResult = MessageBus.Default.Send(MsgArgs.From(PopDpsCheckMessage.Message, [LangProvider.Instance.DpsCheckAskMessage, MessageType.Question])).First();
                if (msgResult.Result is not true)
                {
                    return;
                }
            }

            await SelectedZone.StartDpsCheckAsync(SelectedScript, commands);
        }

        #region validate methods
        private ValidationResult ValidateSelectedScript(string file)
        {
            if (string.IsNullOrWhiteSpace(file))
            {
                return new ValidationResult(string.Format(LangProvider.Instance.PleaseSetMessage, LangProvider.Instance.RegisterFileText));
            }

            if (!File.Exists(file))
            {
                return new ValidationResult(LangProvider.Instance.FileNotExistMessage + $"[ {file} ]");
            }

            try
            {
                _ = DpsCheckScriptParser.Parse(file);
                return ValidationResult.Success;
            }
            catch (CharReaderException e)
            {
                return new ValidationResult(string.Format(LangProvider.Instance.FileParseErrorMessage, file, e.Line, e.Column));
            }
            catch (Exception e)
            {
                return new ValidationResult(e.Message);
            }
        }
        #endregion
    }
}
