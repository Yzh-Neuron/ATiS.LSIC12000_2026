namespace ATiS.Sim.Log
{
    public enum RollingInterval
    {
        /// <summary>不按时间滚动，始终写静态文件名。</summary>
        Infinite = 0,

        /// <summary>按年滚动（文件名日期格式建议 yyyy）。</summary>
        Year = 1,

        /// <summary>按月滚动（建议 yyyyMM）。</summary>
        Month = 2,

        /// <summary>按天滚动（建议 yyyyMMdd）。</summary>
        Day = 3,

        /// <summary>按小时滚动（建议 yyyyMMddHH）。</summary>
        Hour = 4,

        /// <summary>按分钟滚动（建议 yyyyMMddHHmm）。</summary>
        Minute = 5
    }
}
