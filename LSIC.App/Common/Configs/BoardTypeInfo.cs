namespace LSIC.App
{
    /// <summary>
    /// 板卡类型定义信息
    /// </summary>
    public class BoardTypeInfo
    {
        /// <summary>
        /// 板卡类型字节
        /// </summary>
        public int TypeCode { get; private set; }

        /// <summary>
        /// 显示名 / 多语言键
        /// </summary>
        public string BoardKey { get; private set; }

        /// <summary>
        /// 是否支持固件升级
        /// </summary>
        public bool Upgradable { get; private set; }

        /// <summary>
        /// 固件文件名匹配后缀（如 _MB）
        /// </summary>
        public string FileSuffix { get; private set; }

        /// <summary>
        /// 挂载节点类别：Zone（测试区）/ Aux（辅助控制板）
        /// </summary>
        public string NodeKind { get; private set; }

        /// <summary>
        /// 下位机上报了 ini 未定义的板卡时的兜底定义
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        internal static BoardTypeInfo Unknown(byte code) => new BoardTypeInfo
        {
            TypeCode = code,
            BoardKey = $"0x{code:X2}",
            Upgradable = false,
            FileSuffix = "",
            NodeKind = ""
        };
    }
}
