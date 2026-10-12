namespace LSIC.App
{

    /// <summary>
    /// 老化区信息
    /// </summary>
    public class ZoneInfo
    {

        /// <summary>
        /// 老化区Id
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string Ip { get; private set; }

        /// <summary>
        /// 端口号
        /// </summary>
        public ushort Port { get; private set; }
    }
}
