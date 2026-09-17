namespace LSIC.App.Views
{
    public partial class PopAboutView : IRecipient<MsgArgs<PopTemplateMessage>>
    {
        public PopAboutViewModel ViewModel { get; }
        public PopAboutView()
        {
            InitializeComponent();
            ViewModel = new PopAboutViewModel();
            DataContext = ViewModel;

            MessageBus.Default.Register(this);
        }

        public MessageResult Receive(MsgArgs<PopTemplateMessage> e)
        {
            throw new NotImplementedException();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            MessageBus.Default.UnregisterAll(this);
        }
    }


}

namespace LSIC.App.ViewModels
{
    public class PopAboutViewModel : ObservableObject
    {
        public PopAboutViewModel()
        {
            //Title = Device.Instance.SystemConfig.DeviceInfo.Title;

            Assembly assembly = Assembly.GetExecutingAssembly();
            Version = assembly.GetName().Version?.ToString();
            Company = assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company;
            Copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright;
        }

        public string Title { get; private set; }

        public string Version { get; private set; }

        public string Company { get; private set; }

        public string Copyright { get; private set; }

    }
}

namespace LSIC.App.ViewMessages
{
    public class PopAboutViewMessage
    {


    }
}
