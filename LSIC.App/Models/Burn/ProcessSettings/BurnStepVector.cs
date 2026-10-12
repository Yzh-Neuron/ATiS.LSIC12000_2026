namespace LSIC.App.Models
{
    /// <summary>
    /// 向量步骤（运行时）
    /// </summary>
    public class BurnStepVector
    {
        public string Name { get; }
        public ExecuteStrategy Strategy { get; }
        public byte DpsPlanIndex { get; }
        public bool LoopEnable { get; }
        public byte LoopTargetIndex { get; }
        public ExecuteStrategy LoopStrategy { get; }
        public BurnVector[] Vectors { get; }

        /// <summary>
        /// 步次身份即数组位置；LoopTargetIndex 为循环终点 End 的位置
        /// </summary>
        public BurnStepVector(string name, byte dpsPlanIndex, ExecuteStrategy strategy, BurnVector[] vectors, Loop loop, byte loopTargetIndex)
        {
            Name = name;
            DpsPlanIndex = dpsPlanIndex;
            Strategy = strategy;
            Vectors = vectors;
            LoopEnable = loop != null;
            LoopTargetIndex = loopTargetIndex;
            LoopStrategy = loop != null ? (ExecuteStrategy)loop.ExecuteStrategy.Clone() : null;
        }
    }
}
