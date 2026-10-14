using System.Windows.Controls;

namespace LSIC.App.Controls
{
    /// <summary>
    /// 循环槽布局面板
    /// </summary>
    public class LoopSlotPanel : Panel
    {
        public static readonly DependencyProperty RowHeightProperty =
            DependencyProperty.Register(nameof(RowHeight), typeof(double), typeof(LoopSlotPanel), new FrameworkPropertyMetadata(56d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

        public double RowHeight
        {
            get => (double)GetValue(RowHeightProperty);
            set => SetValue(RowHeightProperty, value);
        }

        public static readonly DependencyProperty RowCountProperty =
            DependencyProperty.Register(nameof(RowCount), typeof(int), typeof(LoopSlotPanel), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

        public int RowCount
        {
            get => (int)GetValue(RowCountProperty);
            set => SetValue(RowCountProperty, value);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double width = double.IsInfinity(availableSize.Width) ? 56 : availableSize.Width;

            foreach (UIElement child in InternalChildren)
            {
                child.Measure(new Size(width, double.PositiveInfinity));
            }

            return new Size(width, RowCount * RowHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double width = finalSize.Width;

            foreach (UIElement child in InternalChildren)
            {
                if (child is FrameworkElement element && element.DataContext is LoopSlot slot)
                {
                    child.Arrange(new Rect(0, slot.Row * RowHeight, width, slot.RowSpan * RowHeight));
                }
                else
                {
                    child.Arrange(new Rect(0, 0, 0, 0));
                }
            }

            return new Size(width, RowCount * RowHeight);
        }
    }
}

