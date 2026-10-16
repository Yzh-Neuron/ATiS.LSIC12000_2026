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
            Zones = SystemConfig.ZoneInfos.Select(info => new Zone(info, CreateDps())).ToArray();
            BoardTypeCatalog.Initialize(systemConfig.BoardTypeInfos);
            foreach (var rule in SystemConfig.LogRules)
            {
                rule.Columns = SystemConfig.LogColumns.Where(i => i.RuleIds.Contains(rule.Id)).ToArray();
            }
            GlobalLanguageManager.SwitchLanguage(Language);
        }
        public SystemConfig SystemConfig { get; }
        private DeviceMode _mode;
        public DeviceMode Mode
        {
            get => _mode;
            set
            {
                if (SetProperty(ref _mode, value))
                {
                    ModeChanged?.Invoke();
                }
            }
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
                if (_language == value)
                {
                    return;
                }
                if (SetProperty(ref _language, value))
                {
                    SystemDatabase.Instance.SetConfig($"{nameof(Device)}.{nameof(Language)}", _language);
                    GlobalLanguageManager.SwitchLanguage(value);
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
        public void StartupZones()
        {
            foreach (Zone zone in Zones)
            {
                zone.Startup();
            }
        }
        public void Dispose()
        {
            if (Zones is null)
            {
                return;
            }

            foreach (Zone zone in Zones)
            {
                zone.Dispose();
            }
            Zones = null;
        }
        public event Action ModeChanged;
        private Dps[] CreateDps()
        {
            return SystemConfig.DpsInfos.Select(i => new Dps(i)).ToArray();
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
