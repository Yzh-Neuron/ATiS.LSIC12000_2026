namespace LSIC.App
{
    /// <summary>
    /// 日志规则
    /// </summary>
    public class LogRule
    {
        public int Id { get; private set; }

        /// <summary>
        /// 文件路径匹配模式
        /// </summary>
        public string PathPattern { get; private set; }

        /// <summary>
        /// 日志内容匹配模式
        /// </summary>
        public string LayoutPattern { get; private set; }

        /// <summary>
        /// 每行日志内容的最小字节数
        /// </summary>
        public int MinLogLineBytes { get; private set; }

        /// <summary>
        /// 时间格式
        /// </summary>
        public string TimeFormat { get; private set; }

        /// <summary>
        /// 数据内容分割字符
        /// </summary>
        public SplitCharEx SplitChar { get; private set; }

        private LogColumn[] _columns;
        /// <summary>
        /// 日志列（懒加载）
        /// </summary>
        public LogColumn[] Columns => _columns ??= Device.Instance.SystemConfig.LogColumns.Where(i => i.RuleIds.Contains(Id)).OrderBy(i => i.Index).ToArray();
    }
}