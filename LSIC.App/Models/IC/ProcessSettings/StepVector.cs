namespace LSIC.App.Models
{
    /// <summary>
    /// 向量步
    /// </summary>
    public class StepVector : StepItem
    {
        private RunStrategy _runStrategy;

        public StepVector(string name) : base(name)
        {
        }

        public RunStrategy RunStrategy
        {
            get => _runStrategy;
            set => SetProperty(ref _runStrategy, value);
        }

        /// <summary>
        /// 子步次（有序）
        /// </summary>
        public ObservableCollection<StepItem> Vectors { get; } = new();

        public override StepItemType ItemType => StepItemType.Vector;
    }
}
