using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ATiS.Sim.Wpf
{
    public static class DataGridDoubleClick
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(DataGridDoubleClick), new PropertyMetadata(null, OnCommandChanged));

        public static void SetCommand(DependencyObject o, ICommand value) => o.SetValue(CommandProperty, value);
        public static ICommand GetCommand(DependencyObject o) => (ICommand)o.GetValue(CommandProperty);

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not DataGrid dataGrid)
            {
                return;
            }

            dataGrid.MouseDoubleClick -= OnMouseDoubleClick;
            if (e.NewValue is ICommand)
            {
                dataGrid.MouseDoubleClick += OnMouseDoubleClick;
            }
        }

        private static void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is not DataGrid dataGrid || e.OriginalSource is not DependencyObject source)
            {
                return;
            }

            if (source.GetParent<DataGridRow>() is not { } row)
            {
                return;
            }

            if (GetCommand(dataGrid) is ICommand command && command.CanExecute(row.Item))
            {
                command.Execute(row.Item);
            }
        }
    }
}
