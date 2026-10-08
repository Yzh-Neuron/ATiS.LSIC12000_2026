namespace LSIC.App.Models
{
    public class BurnStepVector
    {
        public byte Id { get; }
        public string Name { get; }
        public ExecuteStrategy Strategy { get; }
        public byte DpsPlanIndex { get; }
        public bool LoopEnable { get; }
        public byte LoopTargetIndex { get; }
        public ExecuteStrategy LoopStrategy { get; }
        public BurnVector[] Vectors { get; }
        public BurnStepVector(byte id, string name, byte dpsPlanIndex, ExecuteStrategy strategy, BurnVector[] vectors, Loop loop, byte loopTargetIndex)
        {
            Id = id;
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
