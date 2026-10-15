using System.Windows.Controls;

namespace LSIC.App.Views
{
    public partial class CtrLogBrowseView : IRecipient<MsgArgs<CtrLogBrowseMessage>>
    {
        public CtrLogBrowseViewModel ViewModel { get; }
        public CtrLogBrowseView()
        {
            InitializeComponent();
            ViewModel = new CtrLogBrowseViewModel();
            DataContext = ViewModel;

            MessageBus.Default.Register(this);
        }
        public MessageResult Receive(MsgArgs<CtrLogBrowseMessage> e)
        {
            switch (e.Message)
            {
                case CtrLogBrowseMessage.LogRuleChanged:
                    {
                        var rule = e.Args[0] as LogRule;

                        DataList.Columns.Clear();

                        // 时间列
                        DataGridTextColumn timeColumn = new DataGridTextColumn
                        {
                            Header = LangProvider.Instance.TimeText,
                            Binding = new Binding("Time") { StringFormat = "yyyy-MM-dd HH:mm:ss" }
                        };
                        DataList.Columns.Add(timeColumn);

                        // ini 配置列
                        for (int i = 0; i < rule.Columns.Length; i++)
                        {
                            LogColumn column = rule.Columns[i];
                            DataGridTemplateColumn templateColumn = new DataGridTemplateColumn
                            {
                                Header = LangProvider.Instance.GetString(column.Header),
                                MaxWidth = 500
                            };

                            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(TextBlock));
                            factory.SetBinding(TextBlock.TextProperty, new Binding($"Columns[{i}]"));
                            factory.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Left);
                            factory.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
                            factory.SetValue(TextBlock.MarginProperty, new Thickness(10d, 0d, 10d, 0d));
                            templateColumn.CellTemplate = new DataTemplate { VisualTree = factory };

                            DataList.Columns.Add(templateColumn);
                        }
                        break;
                    }
                case CtrLogBrowseMessage.OpenFile:
                    {
                        string filter = e.Args[0] as string;
                        string current = e.Args[1] as string;
                        string initialDirectory = Path.Exists(current) ? Path.GetDirectoryName(current)! : Path.GetFullPath(".\\Log");
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
                case CtrLogBrowseMessage.Growl:
                    {
                        Growl.Show(new GrowlInfo
                        {
                            Message = e.Args[0] as string,
                            Type = (MessageType)e.Args[1]
                        });
                        break;
                    }
                case CtrLogBrowseMessage.Export:
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
                case CtrLogBrowseMessage.SelectColumns:
                    {
                        LogRule rule = e.Args[0] as LogRule;
                        var pop = new PopLogColumnSelectView(rule)
                        {
                            Owner = WindowExtension.GetActiveWindow()
                        };
                        pop.ShowDialog();
                        return MessageResult.From(pop.Columns);
                    }
                case CtrLogBrowseMessage.Plot:
                    {
                        LogPlot logPlot = e.Args[0] as LogPlot;
                        var pop = new PopPlotView(logPlot)
                        {
                            Owner = WindowExtension.GetActiveWindow()
                        };
                        pop.ShowDialog();
                        break;
                    }
                default:
                    break;
            }

            return MessageResult.Null();
        }
    }
}