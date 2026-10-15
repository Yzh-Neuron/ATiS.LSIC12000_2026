namespace LSIC.App.Views
{
    public partial class PopTextBrowseView
    {
        public PopTextBrowseView(string title, string file, Encoding encoding)
        {
            InitializeComponent();
            Title = title;
            DataContext = File.ReadAllText(file, encoding);
        }
    }
}
