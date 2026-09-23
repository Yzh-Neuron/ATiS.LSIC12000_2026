namespace LSIC.App.Views
{
    public partial class PopIcEditView : IRecipient<MsgArgs<PopIcEditMessage>>
    {
        public PopIcEditViewModel ViewModel { get; }
        public PopIcEditView()
        {
            InitializeComponent();
            ViewModel = new PopIcEditViewModel();
            DataContext = ViewModel;

            MessageBus.Default.Register(this);
        }

        public MessageResult Receive(MsgArgs<PopIcEditMessage> e)
        {
            switch (e.Message)
            {
                case PopIcEditMessage.InputName:
                    {
                        string title = e.Args[0] as string;
                        IEnumerable<string> filters = e.Args[1] as IEnumerable<string>;
                        PopInputNameView pop = new PopInputNameView(title, filters) { Owner = this };
                        pop.ShowDialog();
                        return MessageResult.From(pop.NameResult);
                    }
                case PopIcEditMessage.Message:
                    {
                        string message = e.Args[0] as string;
                        MessageType type = (MessageType)e.Args[1];
                        return MessageResult.From(MessagePop.Show(message, type));
                    }
                case PopIcEditMessage.SelectFile:
                    {
                        string filter = e.Args[0] as string;
                        string current = e.Args[1] as string;
                        string initialDirectory = Path.Exists(current) ? Path.GetDirectoryName(current)! : Path.GetFullPath(".\\Samples");
                        OpenFileDialog dialog = new OpenFileDialog
                        {
                            Filter = filter!,
                            Multiselect = false,
                            InitialDirectory = initialDirectory
                        };
                        if (dialog.ShowDialog() is true)
                        {
                            return MessageResult.From(dialog.FileName);
                        }
                        break;
                    }

                case PopIcEditMessage.CreateDpsSetting:
                case PopIcEditMessage.EditDpsSetting:
                    {
                        IEnumerable<DpsInfo> optional = e.Args[0] as IEnumerable<DpsInfo>;
                        DpsSetting dps = e.Args[1] as DpsSetting;
                        PopDpsSettingEditView pop = new PopDpsSettingEditView(optional, dps)
                        {
                            Owner = this
                        };
                        pop.ShowDialog();
                        return MessageResult.From(pop.DpsSetting);
                    }
            }
            return MessageResult.Null();
        }

        protected override void OnClosed(EventArgs e)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            base.OnClosed(e);
            MessageBus.Default.UnregisterAll(this);
            ViewModel?.Dispose();
        }
    }

    /// <summary>
    /// 包装多个 DutSetting，用于界面批量设置独立温控。
    /// </summary>
    public class DutSettingWrapper : ObservableObject
    {
        private readonly IReadOnlyList<DutSetting> _dutSettings;

        public bool IsValid { get; }

        public DutSetting DutSettingTemplate { get; }

        private bool? _enable;
        public bool? Enable
        {
            get => _enable;
            set
            {
                if (SetProperty(ref _enable, value) && value.HasValue)
                {
                    ApplyThermostat(t => t.Enable = value.Value);
                }
            }
        }

        private int? _targetTemperature = 250;
        public int? TargetTemperature
        {
            get => _targetTemperature;
            set
            {
                if (SetProperty(ref _targetTemperature, value) && value.HasValue)
                {
                    ApplyThermostat(t => t.TargetTemperature = value.Value);
                }
            }
        }

        public DutSettingWrapper(IEnumerable<DutSetting> dutSettings)
        {
            ArgumentNullException.ThrowIfNull(dutSettings);
            _dutSettings = dutSettings.ToList();

            DutSettingTemplate = _dutSettings.FirstOrDefault(t => t.ThermostatSetting != null);
            IsValid = DutSettingTemplate != null;

            if (!IsValid)
            {
                return;
            }

            if (_dutSettings.All(t => t.ThermostatSetting!.Enable))
            {
                _enable = true;
            }
            else if (_dutSettings.All(t => !t.ThermostatSetting!.Enable))
            {
                _enable = false;
            }
            else
            {
                _enable = null;
            }

            var templateTemp = DutSettingTemplate.ThermostatSetting.TargetTemperature;
            _targetTemperature = _dutSettings.All(t => t.ThermostatSetting!.TargetTemperature == templateTemp) ? templateTemp : null;
        }

        private void ApplyThermostat(Action<ThermostatSetting> action)
        {
            foreach (var setting in _dutSettings)
            {
                if (setting.ThermostatSetting != null)
                {
                    action(setting.ThermostatSetting);
                }
            }
        }
    }
}
