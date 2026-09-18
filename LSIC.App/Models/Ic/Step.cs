namespace LSIC.App.Models
{
    public class Step : ObservableObject
    {
        private string _name = string.Empty;

        /// <summary>
        /// 步次名称
        /// </summary>
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _dpsPlan = string.Empty;

        /// <summary>
        /// 引用的电源组名称
        /// </summary>
        public string DpsPlan
        {
            get => _dpsPlan;
            set => SetProperty(ref _dpsPlan, value);
        }

        private RunStrategy _runStrategy;
        /// <summary>
        /// 本步运行策略
        /// </summary>
        public RunStrategy RunStrategy
        {
            get => _runStrategy;
            set => SetProperty(ref _runStrategy, value);
        }

        /// <summary>
        /// 子步次（有序）
        /// </summary>
        public ObservableCollection<StepVector> Vectors { get; } = new();

        private bool _isInLoop;

        /// <summary>
        /// 该步次是否落在某个循环区间内（视图用）
        /// </summary>
        public bool IsInLoop
        {
            get => _isInLoop;
            set => SetProperty(ref _isInLoop, value);
        }
    }
}
