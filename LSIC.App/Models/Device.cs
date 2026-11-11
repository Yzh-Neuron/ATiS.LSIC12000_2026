using System.Data;

namespace LSIC.App.Models
{
    public class Device : ObservableObject, IDisposable
    {
        public static Device Instance { get; private set; }
        private Device(SystemConfig systemConfig)
        {
#if DEBUG
            _mode = DeviceMode.Administrator;
#endif
            SystemConfig = systemConfig;
            Zones = SystemConfig.ZoneInfos.Select(info => new Zone(info, SystemConfig.DpsInfos.Select(i => new Dps(i)).ToArray())).ToArray();
            Controller = SystemConfig.ControllerInfo == null ? null : new Controller(SystemConfig.ControllerInfo);


            BoardTypeCatalog.Initialize(systemConfig.BoardTypeInfos);
            foreach (var rule in SystemConfig.LogRules)
            {
                rule.Columns = SystemConfig.LogColumns.Where(i => i.RuleIds.Contains(rule.Id)).ToArray();
            }
            GlobalLanguageManager.SwitchLanguage(Language);
            GlobalThemeManager.SwitchTheme(ThemeType);
        }
        public SystemConfig SystemConfig { get; }
        private DeviceMode _mode;
        public DeviceMode Mode
        {
            get => _mode;
            set => SetProperty(ref _mode, value);
        }
        private UiLanguage? _language;
        public UiLanguage Language
        {
            get
            {
                try
                {
                    if (_language is not null)
                    {
                        return _language.Value;
                    }
                    _language = SystemDatabase.Instance.GetConfig<UiLanguage?>($"{nameof(Device)}.{nameof(Language)}");
                    _language ??= SystemConfig.DeviceInfo.Language;
                    _language ??= UiLanguage.Chinese;
                    return _language.Value;
                }
                finally
                {
                    DeviceName = _language == UiLanguage.Chinese ? SystemConfig.DeviceInfo.ChineseTitle : SystemConfig.DeviceInfo.EnglishTitle;
                }
            }
            set
            {
                if (_language != value)
                {
                    _language = value;
                    SystemDatabase.Instance.SetConfig($"{nameof(Device)}.{nameof(Language)}", _language);
                    GlobalLanguageManager.SwitchLanguage(value);
                    OnPropertyChanged(nameof(Language));
                }
            }
        }
        private ThemeType? _themeType;
        public ThemeType ThemeType
        {
            get
            {
                if (_themeType is not null)
                {
                    return _themeType.Value;
                }
                _themeType = SystemDatabase.Instance.GetConfig<ThemeType?>($"{nameof(Device)}.{nameof(ThemeType)}");
                _themeType ??= ThemeType.Light;
                return _themeType.Value;
            }
            set
            {
                if (_themeType != value)
                {
                    _themeType = value;
                    SystemDatabase.Instance.SetConfig($"{nameof(Device)}.{nameof(ThemeType)}", value);
                    GlobalThemeManager.SwitchTheme(value);
                    OnPropertyChanged(nameof(ThemeType));
                    ThemeTypeChanged?.Invoke(value);
                }
            }
        }
        private string _deviceName;
        public string DeviceName
        {
            get => _deviceName;
            private set => SetProperty(ref _deviceName, value);
        }
        public Zone[] Zones { get; private set; }
        public Controller Controller { get; private set; }
        public event Action<ThemeType> ThemeTypeChanged;
        public void Dispose()
        {
            if (Zones is null)
            {
                return;
            }

            Zones.AsParallel().ForAll(zone => zone.Dispose());
            Zones = null;

            Controller?.Dispose();
            Controller = null;
        }
        public static void Initialize(SystemConfig systemConfig)
        {
            if (Instance is not null)
            {
                Instance.Dispose();
                Instance = null;
            }
            Instance = new Device(systemConfig);
        }

    }
}
