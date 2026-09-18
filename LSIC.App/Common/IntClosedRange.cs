namespace LSIC.App
{
    /// <summary>
    /// 整数闭区间 [Start, End]
    /// </summary>
    public readonly record struct IntClosedRange(int Start, int End)
    {
        public int Length => End - Start + 1;

        /// <summary>
        /// 判断数值是否包含在区间内（包含两端边界）
        /// </summary>
        /// <param name="value">待判断数值</param>
        /// <returns></returns>
        public bool Contains(int value)
        {
            return value >= Start && value <= End;
        }

        /// <summary>
        /// 判断当前区间与目标区间是否存在重叠
        /// </summary>
        /// <param name="other">目标区间</param>
        /// <returns></returns>
        public bool IsOverlapWith(IntClosedRange other)
        {
            return !(End < other.Start || Start > other.End);
        }

        /// <summary>
        /// 获取区间内所有整数序列
        /// </summary>
        /// <returns></returns>
        public IEnumerable<int> GetAllValues()
        {
            for (int i = Start; i <= End; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// 校验区间合法性
        /// </summary>
        /// <param name="maxExclusive">上限边界（不包含）</param>
        /// <returns></returns>
        public bool IsValid(int maxExclusive)
        {
            return Start >= 0 && End < maxExclusive && Start <= End;
        }
    }
}
