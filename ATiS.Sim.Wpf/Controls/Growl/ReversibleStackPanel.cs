using System.Windows;
using System.Windows.Controls;

namespace ATiS.Sim.Wpf
{
    public class ReversibleStackPanel : Panel
    {
        //排列方向
        public static readonly DependencyProperty OrientationProperty =
            StackPanel.OrientationProperty.AddOwner(typeof(ReversibleStackPanel), new FrameworkPropertyMetadata(Orientation.Vertical, FrameworkPropertyMetadataOptions.AffectsMeasure));

        // 反转排列
        public static readonly DependencyProperty ReverseOrderProperty =
            DependencyProperty.Register(nameof(ReverseOrder), typeof(bool), typeof(ReversibleStackPanel), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public bool ReverseOrder
        {
            get => (bool)GetValue(ReverseOrderProperty);
            set => SetValue(ReverseOrderProperty, value);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var totalSize = new Size();
            var childConstraint = Orientation == Orientation.Horizontal
                ? new Size(double.PositiveInfinity, constraint.Height)
                : new Size(constraint.Width, double.PositiveInfinity);

            foreach (UIElement child in InternalChildren)
            {
                if (child.Visibility == Visibility.Collapsed) continue;

                child.Measure(childConstraint);
                var size = child.DesiredSize;

                if (Orientation == Orientation.Horizontal)
                {
                    totalSize.Width += size.Width;
                    totalSize.Height = System.Math.Max(totalSize.Height, size.Height);
                }
                else
                {
                    totalSize.Width = System.Math.Max(totalSize.Width, size.Width);
                    totalSize.Height += size.Height;
                }
            }

            return totalSize;
        }
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            double offset = 0;
            var children = InternalChildren;

            if (Orientation == Orientation.Horizontal)
            {
                // 水平反转：从右向左排列
                if (ReverseOrder) offset = arrangeSize.Width;

                foreach (UIElement child in children)
                {
                    if (child.Visibility == Visibility.Collapsed) continue;
                    var width = child.DesiredSize.Width;

                    if (ReverseOrder) offset -= width;
                    child.Arrange(new Rect(offset, 0, width, arrangeSize.Height));
                    if (!ReverseOrder) offset += width;
                }
            }
            else
            {
                // 垂直反转：从下向上排列
                if (ReverseOrder) offset = arrangeSize.Height;

                foreach (UIElement child in children)
                {
                    if (child.Visibility == Visibility.Collapsed) continue;
                    var height = child.DesiredSize.Height;

                    if (ReverseOrder) offset -= height;
                    child.Arrange(new Rect(0, offset, arrangeSize.Width, height));
                    if (!ReverseOrder) offset += height;
                }
            }

            return arrangeSize;
        }
    }
}
