namespace LSIC.App.Models
{
    /// <summary>
    /// 向量步
    /// </summary>
    public class StepVector : StepItem
    {
        private RunStrategy _runStrategy;
        public RunStrategy RunStrategy
        {
            get => _runStrategy;
            set => SetProperty(ref _runStrategy, value);
        }
        public override StepItemType ItemType => StepItemType.Vector;
        public StepVector(string name) : base(name)
        {
        }

    }
}
