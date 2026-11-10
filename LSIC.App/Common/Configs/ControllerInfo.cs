namespace LSIC.App
{

    /// <summary>
    /// 辅助控制板信息
    /// </summary>
    public class ControllerInfo
    {

        /// <summary>
        /// 控制板Id
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
