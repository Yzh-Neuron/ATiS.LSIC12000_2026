using System.Windows.Controls;

namespace LSIC.App.Views
{
    public partial class PopLogColumnSelectView
    {
        public LogColumn[] Columns { get; private set; }
        public PopLogColumnSelectView(LogRule rule)
        {
            InitializeComponent();
            DataContext = rule;
        }
        private void OnConfirmButtonClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            LogColumn[] columns = ColumnList.GetChildren<CheckBox>()
                                            .Where(i => i.IsChecked.HasValue && i.IsChecked.Value)
                                            .Select(i => i.DataContext as LogColumn)
                                            .ToArray();
            if (columns.Length > 0)
            {
                Columns = columns;
                Close();
            }
        }
        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
