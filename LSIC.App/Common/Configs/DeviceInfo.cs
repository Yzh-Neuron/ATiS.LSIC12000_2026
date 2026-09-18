namespace LSIC.App
{
    [Flags]
    public enum DebugFlags
    {
        None = 0x00,
        DisableBuzz = 0x01
    }

    public class DeviceInfo
    {
        public string Title { get; private set; }

        public DebugFlags DebugFlags { get; private set; } = DebugFlags.None;

        public UiLanguage Language { get; private set; }
    }
}
