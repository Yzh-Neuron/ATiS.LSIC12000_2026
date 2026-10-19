namespace LSIC.App.Views
{
    public partial class PopIcEditView : IRecipient<MsgArgs<PopIcEditMessage>>
    {
        public PopIcEditViewModel ViewModel { get; }
        public PopIcEditView()
        {
            InitializeComponent();
            ViewModel = new PopIcEditViewModel();
            DataContext = ViewModel;

            MessageBus.Default.Register(this);
        }
        public MessageResult Receive(MsgArgs<PopIcEditMessage> e)
        {
            switch (e.Message)
            {
                case PopIcEditMessage.InputName:
                    {
                        string title = e.Args[0] as string;
                        IEnumerable<string> filters = e.Args[1] as IEnumerable<string>;
                        PopInputNameView pop = new PopInputNameView(title, filters)
                        {
                            Owner = this
                        };
                        pop.ShowDialog();
                        return MessageResult.From(pop.NameResult);
                    }
                case PopIcEditMessage.Export:
                    {
                        string filter = e.Args[0] as string;
                        string current = e.Args[1] as string;
                        SaveFileDialog dialog = new SaveFileDialog
                        {
                            Title = LangProvider.Instance.ExportText,
                            Filter = filter,
                            FileName = current,
                            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
                        };
                        if (dialog.ShowDialog() is true)
                        {
                            return MessageResult.From(dialog.FileName);
                        }
                        break;
                    }
                case PopIcEditMessage.OpenFile:
                    {
                        string filter = e.Args[0] as string;
                        string current = e.Args[1] as string;
                        string initialDirectory = Path.Exists(current) ? Path.GetDirectoryName(current)! : Path.GetFullPath(".\\Samples");
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
                case PopIcEditMessage.CreateDpsSetting:
                case PopIcEditMessage.EditDpsSetting:
                    {
                        IEnumerable<DpsInfo> optional = e.Args[0] as IEnumerable<DpsInfo>;
                        DpsSetting dps = e.Args[1] as DpsSetting;
                        PopDpsSettingEditView pop = new PopDpsSettingEditView(optional, dps)
                        {
                            Owner = this
                        };
                        pop.ShowDialog();
                        return MessageResult.From(pop.DpsSetting);
                    }
                case PopIcEditMessage.EditVector:
                case PopIcEditMessage.CreateVector:
                    {
                        Vector vector = e.Args[0] as Vector;
                        PopVectorEditView pop = new PopVectorEditView(vector)
                        {
                            Owner = this
                        };
                        pop.ShowDialog();
                        return MessageResult.From(pop.Vector);
                    }
                case PopIcEditMessage.EditExecuteStrategy:
                    {
                        ExecuteStrategy executeStrategy = e.Args[0] as ExecuteStrategy;
                        bool isLoop = e.Args[1] is true;
                        PopExecuteStrategyView pop = new PopExecuteStrategyView(executeStrategy, isLoop)
                        {
                            Owner = this
                        };
                        pop.ShowDialog();
                        return MessageResult.From(pop.ExecuteStrategy);
                    }
                case PopIcEditMessage.Growl:
                    {
                        Growl.Show(new GrowlInfo
                        {
                            Message = e.Args[0] as string,
                            Type = (MessageType)e.Args[1]
                        });
                        break;
                    }
                case PopIcEditMessage.Message:
                    {
                        string message = e.Args[0] as string;
                        MessageType type = (MessageType)e.Args[1];
                        return MessageResult.From(MessagePop.Show(message, type));
                    }
            }
            return MessageResult.Null();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            MessageBus.Default.UnregisterAll(this);
            ViewModel?.Dispose();
        }
    }
    public class PopPopIcEditViewModelProxy : BindingProxy<PopIcEditViewModel>
    {
        protected override Freezable CreateInstanceCore()
        {
            return new PopPopIcEditViewModelProxy();
        }
    }
}
