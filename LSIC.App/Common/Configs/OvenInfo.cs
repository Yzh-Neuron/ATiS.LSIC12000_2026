namespace LSIC.App
{
    public class OvenInfo
    {
        public int Id { get; private set; }

        public string Ip { get; private set; }

        public ushort Port { get; private set; }

        public OvenType OvenType { get; private set; } = OvenType.GWS;

        public int DoorId { get; private set; }

        public int LogInterval { get; private set; }
    }
}
