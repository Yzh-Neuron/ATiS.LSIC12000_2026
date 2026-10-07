namespace LSIC.App.Models
{
    public class BurnStep
    {
        public byte Id { get; }
        public string Name { get; }
        public StepItemType Type { get; }
        public byte DpsPlanIndex { get; }
        public ExecuteStrategy Strategy { get; }
        public BurnVector[] Vectors { get; }

        /// <summary>
        /// 是否在循环区间内
        /// </summary>
        public bool LoopEnable { get; }

        /// <summary>
        /// 循环跳回目标步索引（循环 Start，0xFF 表示无）
        /// </summary>
        public byte LoopTargetIndex { get; }
        public ExecuteStrategy LoopStrategy { get; }

        /// <summary>
        /// 上电/下电超时（仅 StepPower，其他步为 0）
        /// </summary>
        public int Timeout { get; }
        public BurnStep(byte id, string name, StepItemType type, byte dpsPlanIndex, ExecuteStrategy strategy, BurnVector[] vectors, Loop loop, byte loopTargetIndex, int timeout)
        {
            Id = id;
            Name = name;
            Type = type;
            DpsPlanIndex = dpsPlanIndex;
            Strategy = strategy;
            Vectors = vectors;
            LoopEnable = loop != null;
            LoopTargetIndex = loopTargetIndex;
            LoopStrategy = loop != null ? (ExecuteStrategy)loop.ExecuteStrategy.Clone() : null;
            Timeout = timeout;
        }
    }
}