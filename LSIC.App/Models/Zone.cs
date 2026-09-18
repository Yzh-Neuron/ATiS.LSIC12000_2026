namespace LSIC.App.Models
{
    public class Zone
    {
        public Zone(ZoneInfo info)
        {
            ArgumentNullException.ThrowIfNull(info);
            Info = info;
        }

        public ZoneInfo Info { get; }
        public int Id => Info.Id;
        public string Ip => Info.Ip;
        public ushort Port => Info.Port;
    }
}
