namespace LSIC.App.Models
{
    /// <summary>
    /// 向量步
    /// </summary>
    public class StepVector : StepItem
    {
        public ExecuteStrategy ExecuteStrategy { get; } = new();
        public override StepItemType ItemType => StepItemType.Vector;
        public StepVector(string name) : base(name)
        {
        }

    }
}
