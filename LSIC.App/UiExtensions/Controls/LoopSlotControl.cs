using System.Windows.Controls;

namespace LSIC.App.Controls
{
    /// <summary>
    /// 循环槽
    /// </summary>
    public class LoopSlotControl : ItemsControl
    {
        public static readonly DependencyProperty RowHeightProperty =
            DependencyProperty.Register(nameof(RowHeight), typeof(double), typeof(LoopSlotControl), new FrameworkPropertyMetadata(56d, FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty RowCountProperty =
                DependencyProperty.Register(nameof(RowCount), typeof(int), typeof(LoopSlotControl), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure));
        public double RowHeight
        {
            get => (double)GetValue(RowHeightProperty);
            set => SetValue(RowHeightProperty, value);
        }
        public int RowCount
        {
            get => (int)GetValue(RowCountProperty);
            set => SetValue(RowCountProperty, value);
        }
        public LoopSlotControl()
        {
            Focusable = false;
        }
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            InvalidateMeasure();
        }
    }

}
