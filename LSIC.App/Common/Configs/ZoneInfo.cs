namespace LSIC.App
{
    /// <summary>
    /// 驱动板背板信息
    /// </summary>
    public class ZoneInfo
    {
        public int Id { get; private set; }

        public string Ip { get; private set; }

        public ushort Port { get; private set; }

        public int OvenId { get; set; }

        public int LVControlBoardId { get; private set; }
    }
}
