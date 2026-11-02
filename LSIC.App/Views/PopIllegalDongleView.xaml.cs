namespace LSIC.App.Views
{
    public partial class PopIllegalDongleView
    {
        public PopIllegalDongleView(string machine, DateTime? deadline = null)
        {

            string tip = deadline.HasValue
                ? string.Format(LangProvider.Instance.DongleExpireMessage, deadline.Value)
                : LangProvider.Instance.IllegalDongleMessage;

            machine = string.Join("-", machine.Split(4).Select(i => new string(i.ToArray())));

            Machine = machine;
            Prompt = tip;
            DataContext = this;
            InitializeComponent();
        }

        /// <summary>
        /// 机器码
        /// </summary>
        public string Machine { get; set; }

        /// <summary>
        /// 提示信息
        /// </summary>
        public string Prompt { get; set; }

        private void OnCopyButtonClick(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(Machine);
        }
    }
}
