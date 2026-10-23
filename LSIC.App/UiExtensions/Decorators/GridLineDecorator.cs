using System.Windows.Controls;

namespace LSIC.App.Decorators
{
    [ContentProperty("Target")]
    public class GridLineDecorator : FrameworkElement
    {
        private readonly DrawingVisual _visual = new DrawingVisual();

        public static readonly DependencyProperty GridLineBrushProperty = DependencyProperty.Register("GridLineBrush", typeof(Brush), typeof(GridLineDecorator), new PropertyMetadata(Brushes.LightGray, OnGridLineBrushChanged));

        private Grid _target;

        // 几何签名缓存：LayoutUpdated 触发频率极高，签名没变就不 RenderOpen
        private double _gridWidth;
        private double _gridHeight;
        private Brush _brush;
        private double[] _rowHeights;
        private double[] _columnWidths;

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
                    if (_target is not null)
                    {
                        _target.Loaded -= OnTargetLoaded;
                        _target.Unloaded -= OnTargetUnloaded;
                        _target.LayoutUpdated -= OnTargetLayoutUpdated;
                        _target.IsVisibleChanged -= OnIsVisibleChanged;
                    }

                    RemoveVisualChild(_target);
                    RemoveLogicalChild(_target);
                    _target = value;
                    AddVisualChild(_target);
                    AddLogicalChild(_target);
                    if (_target is not null)
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
                if (Target is not null)
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
            Grid target = Target;
            if (target is null || !target.IsVisible)
            {
                // 不可见时清空旧线条并使签名失效，恢复可见后强制重绘
                InvalidateGeometry();
                using (DrawingContext empty = _visual.RenderOpen())
                {
                }
                return;
            }

            // RenderOpen 会清空已绘制内容，“几何未变”必须在打开 context 之前判断，
            // 否则打开后不画直接 Close 会把已有网格线清空
            if (IsGeometryUnchanged(target))
            {
                return;
            }

            using DrawingContext drawingContext = _visual.RenderOpen();
            Pen pen = new Pen(GridLineBrush, 1.0 * GetDpiFactor());
            double value = pen.Thickness / 2.0;
            GuidelineSet guidelineSet = new GuidelineSet();
            double num = 0.0;
            if (target.RowDefinitions.Count > 0)
            {
                for (int i = 0; i < target.RowDefinitions.Count - 1; i++)
                {
                    num += target.RowDefinitions[i].ActualHeight;
                    double x = 0.0;
                    double num2 = num;
                    double actualWidth = target.ActualWidth;
                    double y = num2;
                    guidelineSet.GuidelinesY.Add(value);
                    drawingContext.PushGuidelineSet(guidelineSet);
                    drawingContext.DrawLine(pen, new Point(x, num2), new Point(actualWidth, y));
                    drawingContext.Pop();
                }
            }

            num = 0.0;
            if (target.ColumnDefinitions.Count > 0)
            {
                for (int j = 0; j < target.ColumnDefinitions.Count - 1; j++)
                {
                    num += target.ColumnDefinitions[j].ActualWidth;
                    double x = num;
                    double num2 = 0.0;
                    double actualWidth = x;
                    double y = target.ActualHeight;
                    guidelineSet.GuidelinesX.Add(value);
                    drawingContext.PushGuidelineSet(guidelineSet);
                    drawingContext.DrawLine(pen, new Point(x, num2), new Point(actualWidth, y));
                    drawingContext.Pop();
                }
            }

            CaptureGeometry(target);
        }

        private bool IsGeometryUnchanged(Grid target)
        {
            int rowCount = target.RowDefinitions.Count;
            int columnCount = target.ColumnDefinitions.Count;

            if (_rowHeights is null
                || _columnWidths is null
                || _rowHeights.Length != rowCount
                || _columnWidths.Length != columnCount
                || _gridWidth != target.ActualWidth
                || _gridHeight != target.ActualHeight
                || !ReferenceEquals(_brush, GridLineBrush))
            {
                return false;
            }

            for (int i = 0; i < rowCount; i++)
            {
                if (_rowHeights[i] != target.RowDefinitions[i].ActualHeight)
                {
                    return false;
                }
            }

            for (int i = 0; i < columnCount; i++)
            {
                if (_columnWidths[i] != target.ColumnDefinitions[i].ActualWidth)
                {
                    return false;
                }
            }

            return true;
        }

        private void CaptureGeometry(Grid target)
        {
            _gridWidth = target.ActualWidth;
            _gridHeight = target.ActualHeight;
            _brush = GridLineBrush;

            _rowHeights = new double[target.RowDefinitions.Count];
            for (int i = 0; i < _rowHeights.Length; i++)
            {
                _rowHeights[i] = target.RowDefinitions[i].ActualHeight;
            }

            _columnWidths = new double[target.ColumnDefinitions.Count];
            for (int i = 0; i < _columnWidths.Length; i++)
            {
                _columnWidths[i] = target.ColumnDefinitions[i].ActualWidth;
            }
        }

        private void InvalidateGeometry()
        {
            _rowHeights = null;
            _columnWidths = null;
        }

        protected override Visual GetVisualChild(int index)
        {
            return index switch
            {
                0 => _target,
                1 => _visual,
                _ => throw new IndexOutOfRangeException(string.Format(LangProvider.Instance.VisualChildIndexOutOfRangeMessage, index)),
            };
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Target is not null)
            {
                Target.Measure(availableSize);
                return Target.DesiredSize;
            }

            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Target is not null)
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
            if (compositionTarget is null)
            {
                return 1.0;
            }

            return 1.0 / compositionTarget.TransformToDevice.M11;
        }
    }
}
