namespace ATiS.Sim.Wpf
{
    /// <summary>
    ///  拖放参数
    /// </summary>
    public sealed class DragDropArgs
    {
        /// <summary>
        /// 被拖动的数据项
        /// </summary>
        public object Item { get; }

        /// <summary>
        /// 原索引
        /// </summary>
        public int FromIndex { get; }

        /// <summary>
        /// 目标索引
        /// </summary>
        public int ToIndex { get; }

        internal DragDropArgs(object item, int fromIndex, int toIndex)
        {
            Item = item;
            FromIndex = fromIndex;
            ToIndex = toIndex;
        }

        public override string ToString() => $"{Item}: {FromIndex} -> {ToIndex}";
    }
}
