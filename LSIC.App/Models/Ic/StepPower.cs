namespace LSIC.App.Models
{
    internal class StepPower : ObservableObject
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


        /// <summary>
        /// 子步次（有序）
        /// </summary>
        public StepVector Vector { get; } = new();

        private int _timeout = 3000;

        /// <summary>
        /// 向量超时时间
        /// </summary>
        public int Timeout
        {
            get => _timeout;
            set => SetProperty(ref _timeout, value);
        }

    }
}
