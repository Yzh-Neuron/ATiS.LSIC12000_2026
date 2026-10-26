namespace LSIC.App.Views
{
    public partial class PopPermissionVerifyView : IRecipient<MsgArgs<PopPermissionVerifyMessage>>
    {
        private const string ExpectedPassword = "admin123321";
        public PopPermissionVerifyViewModel ViewModel { get; }
        public PopPermissionVerifyView()
        {
            InitializeComponent();
            ViewModel = new PopPermissionVerifyViewModel();
            DataContext = ViewModel;

            MessageBus.Default.Register(this);

#if DEBUG
            AdminPassword.Password = ExpectedPassword;
#endif
        }
        public MessageResult Receive(MsgArgs<PopPermissionVerifyMessage> e)
        {
            switch (e.Message)
            {
                case PopPermissionVerifyMessage.Confirm:
                    {
                        if (ExpectedPassword == AdminPassword.Password)
                        {
                            DialogResult = true;
                        }
                        else
                        {
                            ViewModel.InvalidPassword = true;
                        }
                        break;
                    }
                case PopPermissionVerifyMessage.Cancel:
                    {
                        Close();
                        break;
                    }
            }
            return null;
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            MessageBus.Default.UnregisterAll(this);
        }
        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            ViewModel.InvalidPassword = false;
        }
    }
}