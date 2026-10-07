namespace LSIC.App.Models
{
    /// <summary>
    /// 设备类
    /// </summary>
    public class Device : ObservableObject, IDisposable
    {
        /// <summary>
        /// 获取全局实例
        /// </summary>
        public static Device Instance { get; private set; }

        private Device(SystemConfig systemConfig)
        {
            SystemConfig = systemConfig;
            Zones = SystemConfig.ZoneInfos.Select(info => new Zone(info, CreateDps())).ToArray();
        }

        public SystemConfig SystemConfig { get; }

#if DEBUG
        private DeviceMode _mode = DeviceMode.Administrator;
#else
        private DeviceMode _mode;
#endif
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

        public Zone[] Zones { get; private set; }
        public static void Initialize(SystemConfig systemConfig)
        {
            if (Instance != null)
            {
                Instance.Dispose();
                Instance = null;
            }
            Instance = new Device(systemConfig);
        }

        /// <summary>
        /// 启动所有老化区连接
        /// </summary>
        public void StartupZones()
        {
            foreach (Zone zone in Zones)
            {
                zone.Startup();
            }
        }

        public void Dispose()
        {
            if (Zones == null)
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
    }
}
