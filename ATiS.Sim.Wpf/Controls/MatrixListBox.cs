using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ATiS.Sim.Wpf
{
    /// <summary>
    /// 矩阵布局
    /// </summary>
    public class MatrixListBox : ListBox
    {
        static MatrixListBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MatrixListBox), new FrameworkPropertyMetadata(typeof(MatrixListBox)));
        }

        public MatrixListBox()
        {
            SelectionMode = SelectionMode.Extended;
        }

        #region 行列数

        public static readonly DependencyProperty RowsProperty = DependencyProperty.Register(
            nameof(Rows), typeof(int), typeof(MatrixListBox), new FrameworkPropertyMetadata(0, OnRowsChanged, CoerceNonNegative));
        public int Rows
        {
            get => (int)GetValue(RowsProperty);
            set => SetValue(RowsProperty, value);
        }

        public static readonly DependencyProperty ColsProperty = DependencyProperty.Register(
            nameof(Cols), typeof(int), typeof(MatrixListBox), new FrameworkPropertyMetadata(0, OnColsChanged, CoerceNonNegative));
        public int Cols
        {
            get => (int)GetValue(ColsProperty);
            set => SetValue(ColsProperty, value);
        }

        public static readonly DependencyProperty RowHeadersProperty = DependencyProperty.Register(
            nameof(RowHeaders), typeof(IEnumerable), typeof(MatrixListBox), new PropertyMetadata(null));
        public IEnumerable? RowHeaders
        {
            get => (IEnumerable?)GetValue(RowHeadersProperty);
            private set => SetValue(RowHeadersProperty, value);
        }

        public static readonly DependencyProperty ColumnHeadersProperty = DependencyProperty.Register(
            nameof(ColumnHeaders), typeof(IEnumerable), typeof(MatrixListBox), new PropertyMetadata(null));

        public IEnumerable? ColumnHeaders
        {
            get => (IEnumerable?)GetValue(ColumnHeadersProperty);
            private set => SetValue(ColumnHeadersProperty, value);
        }

        private static void OnRowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MatrixListBox)d).RowHeaders = BuildRange((int)e.NewValue);
        }
        private static void OnColsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MatrixListBox)d).ColumnHeaders = BuildRange((int)e.NewValue);
        }
        private static object CoerceNonNegative(DependencyObject d, object baseValue)
        {
            return baseValue is int n && n < 0 ? 0 : baseValue;
        }
        private static int[] BuildRange(int count)
        {
            return Enumerable.Range(0, count).ToArray();
        }

        #endregion 行列数

        #region 尺寸

        public static readonly DependencyProperty CellWidthProperty = DependencyProperty.Register(
            nameof(CellWidth), typeof(double), typeof(MatrixListBox), new FrameworkPropertyMetadata(130d, FrameworkPropertyMetadataOptions.AffectsMeasure));
        public double CellWidth
        {
            get => (double)GetValue(CellWidthProperty);
            set => SetValue(CellWidthProperty, value);
        }

        public static readonly DependencyProperty CellHeightProperty = DependencyProperty.Register(
            nameof(CellHeight), typeof(double), typeof(MatrixListBox), new FrameworkPropertyMetadata(42d, FrameworkPropertyMetadataOptions.AffectsMeasure));
        public double CellHeight
        {
            get => (double)GetValue(CellHeightProperty);
            set => SetValue(CellHeightProperty, value);
        }

        public static readonly DependencyProperty HeaderWidthProperty = DependencyProperty.Register(
            nameof(HeaderWidth), typeof(double), typeof(MatrixListBox), new FrameworkPropertyMetadata(30d, OnHeaderSizeChanged));
        public double HeaderWidth
        {
            get => (double)GetValue(HeaderWidthProperty);
            set => SetValue(HeaderWidthProperty, value);
        }

        public static readonly DependencyProperty HeaderHeightProperty = DependencyProperty.Register(
            nameof(HeaderHeight), typeof(double), typeof(MatrixListBox), new FrameworkPropertyMetadata(30d, OnHeaderSizeChanged));
        public double HeaderHeight
        {
            get => (double)GetValue(HeaderHeightProperty);
            set => SetValue(HeaderHeightProperty, value);
        }

        private static void OnHeaderSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MatrixListBox)d).UpdateHeaderGridLength();
        }

        #endregion

        #region 行列头模板

        public static readonly DependencyProperty ColumnHeaderTemplateProperty = DependencyProperty.Register(
            nameof(ColumnHeaderTemplate), typeof(DataTemplate), typeof(MatrixListBox), new PropertyMetadata(null));
        public DataTemplate? ColumnHeaderTemplate
        {
            get => (DataTemplate?)GetValue(ColumnHeaderTemplateProperty);
            set => SetValue(ColumnHeaderTemplateProperty, value);
        }

        public static readonly DependencyProperty RowHeaderTemplateProperty = DependencyProperty.Register(
            nameof(RowHeaderTemplate), typeof(DataTemplate), typeof(MatrixListBox), new PropertyMetadata(null));

        public DataTemplate? RowHeaderTemplate
        {
            get => (DataTemplate?)GetValue(RowHeaderTemplateProperty);
            set => SetValue(RowHeaderTemplateProperty, value);
        }

        #endregion

        private ScrollViewer? _scrollViewer;
        private TranslateTransform? _columnHeaderTranslate;
        private TranslateTransform? _rowHeaderTranslate;
        private ColumnDefinition? _rowHeaderColumn;
        private RowDefinition? _columnHeaderRow;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_scrollViewer != null)
            {
                _scrollViewer.ScrollChanged -= OnScrollChanged;
            }

            _scrollViewer = GetTemplateChild("PART_ScrollViewer") as ScrollViewer;

            _columnHeaderTranslate = null;
            _rowHeaderTranslate = null;
            _rowHeaderColumn = null;
            _columnHeaderRow = null;

            if (_scrollViewer != null)
            {
                _scrollViewer.DataContext = this;
                _scrollViewer.ApplyTemplate();

                _columnHeaderTranslate = _scrollViewer.FindTemplatePart<TranslateTransform>("PART_ColumnHeaderTranslate");
                _rowHeaderTranslate = _scrollViewer.FindTemplatePart<TranslateTransform>("PART_RowHeaderTranslate");
                _rowHeaderColumn = _scrollViewer.FindTemplatePart<ColumnDefinition>("PART_RowHeaderColumn");
                _columnHeaderRow = _scrollViewer.FindTemplatePart<RowDefinition>("PART_ColumnHeaderRow");

                _scrollViewer.ScrollChanged += OnScrollChanged;
            }

            UpdateHeaderGridLength();
            UpdateHeaderOffsets();
        }
        private void UpdateHeaderGridLength()
        {
            if (_rowHeaderColumn != null)
            {
                _rowHeaderColumn.Width = new GridLength(HeaderWidth);
            }
            if (_columnHeaderRow != null)
            {
                _columnHeaderRow.Height = new GridLength(HeaderHeight);
            }
        }
        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            UpdateHeaderOffsets(e.HorizontalOffset, e.VerticalOffset);
        }
        private void UpdateHeaderOffsets(double horizontalOffset, double verticalOffset)
        {
            if (_columnHeaderTranslate != null)
            {
                _columnHeaderTranslate.X = -horizontalOffset;
            }
            if (_rowHeaderTranslate != null)
            {
                _rowHeaderTranslate.Y = -verticalOffset;
            }
        }
        private void UpdateHeaderOffsets()
        {
            if (_scrollViewer != null)
            {
                UpdateHeaderOffsets(_scrollViewer.HorizontalOffset, _scrollViewer.VerticalOffset);
            }
        }
    }
}
