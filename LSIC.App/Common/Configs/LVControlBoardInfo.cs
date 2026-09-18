namespace LSIC.App
{
    /// <summary>
    /// 系统控制板信息
    /// </summary>
    public class LVControlBoardInfo
    {
        public int Id { get; private set; }

        public string Ip { get; private set; }

        public ushort Port { get; private set; }
    }
}
