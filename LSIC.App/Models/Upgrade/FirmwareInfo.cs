namespace LSIC.App.Models
{
    public class FirmwareInfo
    {
        public FirmwareInfo(FirmwareType type, int version, DateTime? compileTime, DateTime? upgradeTime)
        {
            Type = type;
            Version = version;
            CompileTime = compileTime;
            UpgradeTime = upgradeTime;
        }
        public FirmwareType Type { get; }
        public int Version { get; }
        public DateTime? CompileTime { get; }
        public DateTime? UpgradeTime { get; }
        public override string ToString() => $"{Type} {Version}";
    }
}