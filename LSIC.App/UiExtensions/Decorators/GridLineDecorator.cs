using System.Windows.Controls;

namespace LSIC.App.Decorators
{
    [ContentProperty("Target")]
    public class GridLineDecorator : FrameworkElement
    {
        private readonly DrawingVisual _visual = new DrawingVisual();

        public static readonly DependencyProperty GridLineBrushProperty = DependencyProperty.Register("GridLineBrush", typeof(Brush), typeof(GridLineDecorator), new PropertyMetadata(Brushes.LightGray, OnGridLineBrushChanged));

        private Grid _target;

        public Brush GridLineBrush
        {
            get => (Brush)GetValue(GridLineBrushProperty);
            set => SetValue(GridLineBrushProperty, value);
        }

        public Grid Target
        {
            get => _target;
            set
            {
                if (_target != value)
                {
                    if (_target != null)
                    {
                        _target.Loaded -= OnTargetLoaded;
                        _target.Unloaded -= OnTargetUnloaded;
                        _target.LayoutUpdated -= OnTargetLayoutUpdated;
                    }

                    RemoveVisualChild(_target);
                    RemoveLogicalChild(_target);
                    _target = value;
                    AddVisualChild(_target);
                    AddLogicalChild(_target);
                    if (_target != null)
                    {
                        _target.Loaded += OnTargetLoaded;
                        _target.Unloaded += OnTargetUnloaded;
                        _target.LayoutUpdated += OnTargetLayoutUpdated;
                        _target.IsVisibleChanged += OnIsVisibleChanged;
                    }

                    InvalidateMeasure();
                }
            }
        }

        protected override int VisualChildrenCount
        {
            get
            {
                if (Target != null)
                {
                    return 2;
                }

                return 1;
            }
        }

        protected override IEnumerator LogicalChildren
        {
            get
            {
                yield return Target;
            }
        }

        public GridLineDecorator()
        {
            AddVisualChild(_visual);
        }

        private void DrawGridLines()
        {
            DrawingContext drawingContext = _visual.RenderOpen();
            Grid target = Target;
            if (target == null || !target.IsVisible)
            {
                drawingContext = _visual.RenderOpen();
                drawingContext.Close();
                return;
            }

            Pen pen = new Pen(GridLineBrush, 1.0 * GetDpiFactor());
            double value = pen.Thickness / 2.0;
            GuidelineSet guidelineSet = new GuidelineSet();
            double num = 0.0;
            if (Target.RowDefinitions.Count > 0)
            {
                for (int i = 0; i < Target.RowDefinitions.Count - 1; i++)
                {
                    num += Target.RowDefinitions[i].ActualHeight;
                    double x = 0.0;
                    double num2 = num;
                    double actualWidth = Target.ActualWidth;
                    double y = num2;
                    guidelineSet.GuidelinesY.Add(value);
                    drawingContext.PushGuidelineSet(guidelineSet);
                    drawingContext.DrawLine(pen, new Point(x, num2), new Point(actualWidth, y));
                    drawingContext.Pop();
                }
            }

            num = 0.0;
            if (Target.ColumnDefinitions.Count > 0)
            {
                for (int j = 0; j < Target.ColumnDefinitions.Count - 1; j++)
                {
                    num += Target.ColumnDefinitions[j].ActualWidth;
                    double x = num;
                    double num2 = 0.0;
                    double actualWidth = x;
                    double y = Target.ActualHeight;
                    guidelineSet.GuidelinesX.Add(value);
                    drawingContext.PushGuidelineSet(guidelineSet);
                    drawingContext.DrawLine(pen, new Point(x, num2), new Point(actualWidth, y));
                    drawingContext.Pop();
                }
            }

            drawingContext.Close();
        }

        protected override Visual GetVisualChild(int index)
        {
            return index switch
            {
                0 => _target,
                1 => _visual,
                _ => throw new IndexOutOfRangeException($"Index of visual child '{index}' is out of range"),
            };
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Target != null)
            {
                Target.Measure(availableSize);
                return Target.DesiredSize;
            }

            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Target != null)
            {
                Target.Arrange(new Rect(new Point(0.0, 0.0), finalSize));
            }

            return base.ArrangeOverride(finalSize);
        }

        private static void OnGridLineBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as GridLineDecorator).DrawGridLines();
        }

        private void OnTargetLoaded(object sender, RoutedEventArgs e)
        {
            DrawGridLines();
        }

        private void OnTargetUnloaded(object sender, RoutedEventArgs e)
        {
            DrawGridLines();
        }

        private void OnTargetLayoutUpdated(object sender, EventArgs e)
        {
            DrawGridLines();
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            DrawGridLines();
        }

        private double GetDpiFactor()
        {
            CompositionTarget compositionTarget = PresentationSource.FromVisual(this)?.CompositionTarget;
            if (compositionTarget == null)
            {
                return 1.0;
            }

            return 1.0 / compositionTarget.TransformToDevice.M11;
        }
    }
}
