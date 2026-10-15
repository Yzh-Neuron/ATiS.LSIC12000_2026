namespace LSIC.App.Views
{
    public partial class PopExecuteStrategyView : IRecipient<MsgArgs<PopExecuteStrategyMessage>>
    {
        public ExecuteStrategy ExecuteStrategy { get; private set; }
        public PopExecuteStrategyViewModel ViewModel { get; }

        public PopExecuteStrategyView(ExecuteStrategy executeStrategy, bool isLoop = false)
        {
            InitializeComponent();
            ViewModel = new PopExecuteStrategyViewModel(executeStrategy);
            DataContext = ViewModel;

            MessageBus.Default.Register(this);

            if (isLoop)
            {
                Title = LangProvider.Instance.LoopStrategyText;
                modeText.Text = LangProvider.Instance.LoopModeText;
                timeText.Text = LangProvider.Instance.LoopTimeText;
                timesText.Text = LangProvider.Instance.LoopTimesText;
            }
            else
            {
                Title = LangProvider.Instance.RunStrategyText;
                modeText.Text = LangProvider.Instance.RunModeText;
                timeText.Text = LangProvider.Instance.RunTimeText;
                timesText.Text = LangProvider.Instance.RunTimesText;
            }
        }
        public MessageResult Receive(MsgArgs<PopExecuteStrategyMessage> e)
        {
            switch (e.Message)
            {
                case PopExecuteStrategyMessage.Confirm:
                    {
                        ExecuteStrategy = e.Args[0] as ExecuteStrategy;
                        Close();
                        break;
                    }
                case PopExecuteStrategyMessage.Cancel:
                    {
                        ExecuteStrategy = null;
                        Close();
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            return MessageResult.Null();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            MessageBus.Default.UnregisterAll(this);
        }
    }
}
