namespace LSIC.App.Models
{
    public class BurnInfo : ObservableObject
    {
        private string _lot;
        public string Lot
        {
            get => _lot;
            set => SetProperty(ref _lot, value);
        }
        private string _operator;
        public string Operator
        {
            get => _operator;
            set => SetProperty(ref _operator, value);
        }

        /// <summary>
        /// 老化板（进入老化界面时由 IC 配置解析而来）
        /// </summary>
        public BurnInBoard BurnInBoard { get; }

        /// <summary>
        /// 进入老化界面时封装老化信息
        /// </summary>
        public BurnInfo(Ic ic, string lot, string op)
        {
            Lot = lot;
            Operator = op;
            BurnInBoard = new BurnInBoard(ic.BurnInBoardSetting ?? throw new InvalidOperationException("老化板配置未解析，无法进入老化界面"));
        }
    }
}