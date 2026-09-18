namespace LSIC.App
{
    public class VerticalAdapterBoardInfo
    {
        public int Id { get; private set; }

        public string Ip { get; private set; }

        public ushort Port { get; private set; }

        public IntegerList ZoneIds { get; private set; }

        public IntegerList PpsIds { get; private set; }
    }
}
