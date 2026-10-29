using ATIS.Pattern;

namespace LSIC.App.Views
{
    public partial class PopAbmViewerView : IRecipient<MsgArgs<PopAbmViewerMessage>>
    {
        public PopAbmViewerViewModel ViewModel { get; }
        public PopAbmViewerView()
        {
            InitializeComponent();
            ViewModel = new PopAbmViewerViewModel();
            DataContext = ViewModel;

            AbmText.English = GlobalLanguageManager.CurrentLanguage == UiLanguage.English;
            MessageBus.Default.Register(this);
        }
        public MessageResult Receive(MsgArgs<PopAbmViewerMessage> e)
        {
            switch (e.Message)
            {
                case PopAbmViewerMessage.OpenFile:
                    {
                        string filter = e.Args[0] as string;
                        string current = e.Args[1] as string;
                        string initialDirectory = Path.Exists(current) ? Path.GetDirectoryName(current) : Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                        OpenFileDialog dialog = new OpenFileDialog
                        {
                            Title = LangProvider.Instance.SelectFileText,
                            Filter = filter,
                            Multiselect = false,
                            InitialDirectory = initialDirectory
                        };
                        if (dialog.ShowDialog() is true)
                        {
                            return MessageResult.From(dialog.FileName);
                        }
                        break;
                    }
                case PopAbmViewerMessage.Growl:
                    {
                        Growl.Show(new GrowlInfo
                        {
                            Message = e.Args[0] as string,
                            Type = (MessageType)e.Args[1]
                        });
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