namespace LSIC.App.Models
{
    /// <summary>
    /// 循环视图
    /// </summary>
    public sealed class LoopSlot
    {
        public required Loop Loop { get; init; }
        public required IntClosedRange Range { get; init; }

        /// <summary>
        /// 起始条目索引
        /// </summary>
        public int Row => Range.Start;

        /// <summary>
        /// 连续跨行数
        /// </summary>
        public int RowSpan => Range.Length;

    }
}
