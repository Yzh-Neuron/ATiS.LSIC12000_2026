using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace ATiS.Sim.Wpf
{

    /// <summary>
    /// 拖拽线
    /// </summary>
    public sealed class DragInsertAdorner : Adorner
    {
        private static readonly Pen Pen = CreatePen();
        private double _lineY;

        public DragInsertAdorner(UIElement adornedElement) : base(adornedElement)
        {
            IsHitTestVisible = false;
        }

        public double LineY
        {
            get => _lineY;
            set
            {
                if (Math.Abs(_lineY - value) > 0.01)
                {
                    _lineY = value;
                    InvalidateVisual();
                }
            }
        }

        private static Pen CreatePen()
        {
            //ForegroundBrush PrimaryBrush
            Brush brush = (Application.Current.TryFindResource("ForegroundBrush") as Brush) ?? new SolidColorBrush(Colors.Black);
            var pen = new Pen(brush, 1);
            return pen;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            double width = AdornedElement.RenderSize.Width;
            drawingContext.DrawLine(Pen, new Point(0, _lineY + 0.5), new Point(width, _lineY + 0.5));
        }
    }
}
