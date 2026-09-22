using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ATiS.Sim.Wpf
{
    /// <summary>
    /// 带行/列头、滚动联动与 <see cref="UniformGrid"/> 矩阵布局的可复用 ListBox。
    /// <para>控件只负责：矩阵布局（<see cref="Rows"/>/<see cref="Cols"/>）、行列头生成、
    /// 行列头与内容区的滚动同步，以及 ListBox 原生的选择/多选/数据绑定能力。</para>
    /// <para>每个单元格的视觉内容一律由使用方通过 <see cref="ItemsControl.ItemTemplate"/> 提供，
    /// 控件本身不内置任何业务外观（图标、文本、选中框等）。</para>
    /// </summary>
    public class MatrixListBox : ListBox
    {
        static MatrixListBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(MatrixListBox),
                new FrameworkPropertyMetadata(typeof(MatrixListBox)));
        }

        public MatrixListBox()
        {
            // 矩阵场景默认多选；使用方仍可在 XAML/代码中改为 Single/Multiple
            SelectionMode = SelectionMode.Extended;
        }

        #region 行列数

        public static readonly DependencyProperty RowsProperty = DependencyProperty.Register(
            nameof(Rows), typeof(int), typeof(MatrixListBox),
            new FrameworkPropertyMetadata(0, OnRowsChanged, CoerceNonNegative));

        /// <summary>矩阵行数（同时生成行头数量）。</summary>
        public int Rows
        {
            get => (int)GetValue(RowsProperty);
            set => SetValue(RowsProperty, value);
        }

        public static readonly DependencyProperty ColsProperty = DependencyProperty.Register(
            nameof(Cols), typeof(int), typeof(MatrixListBox),
            new FrameworkPropertyMetadata(0, OnColsChanged, CoerceNonNegative));

        /// <summary>矩阵列数（同时生成列头数量）。</summary>
        public int Cols
        {
            get => (int)GetValue(ColsProperty);
            set => SetValue(ColsProperty, value);
        }

        /// <summary>行头数据源（0..Rows-1），供模板内的 ItemsControl 绑定。</summary>
        public static readonly DependencyProperty RowHeadersProperty = DependencyProperty.Register(
            nameof(RowHeaders), typeof(IEnumerable), typeof(MatrixListBox),
            new PropertyMetadata(null));

        public IEnumerable? RowHeaders
        {
            get => (IEnumerable?)GetValue(RowHeadersProperty);
            private set => SetValue(RowHeadersProperty, value);
        }

        /// <summary>列头数据源（0..Cols-1），供模板内的 ItemsControl 绑定。</summary>
        public static readonly DependencyProperty ColumnHeadersProperty = DependencyProperty.Register(
            nameof(ColumnHeaders), typeof(IEnumerable), typeof(MatrixListBox),
            new PropertyMetadata(null));

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
            nameof(CellWidth), typeof(double), typeof(MatrixListBox),
            new FrameworkPropertyMetadata(130d, FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>每个单元格（及列头项）的宽度。</summary>
        public double CellWidth
        {
            get => (double)GetValue(CellWidthProperty);
            set => SetValue(CellWidthProperty, value);
        }

        public static readonly DependencyProperty CellHeightProperty = DependencyProperty.Register(
            nameof(CellHeight), typeof(double), typeof(MatrixListBox),
            new FrameworkPropertyMetadata(42d, FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>每个单元格（及行头项）的高度。</summary>
        public double CellHeight
        {
            get => (double)GetValue(CellHeightProperty);
            set => SetValue(CellHeightProperty, value);
        }

        public static readonly DependencyProperty HeaderWidthProperty = DependencyProperty.Register(
            nameof(HeaderWidth), typeof(double), typeof(MatrixListBox),
            new FrameworkPropertyMetadata(30d, OnHeaderSizeChanged));

        /// <summary>左侧行头列的宽度。</summary>
        public double HeaderWidth
        {
            get => (double)GetValue(HeaderWidthProperty);
            set => SetValue(HeaderWidthProperty, value);
        }

        public static readonly DependencyProperty HeaderHeightProperty = DependencyProperty.Register(
            nameof(HeaderHeight), typeof(double), typeof(MatrixListBox),
            new FrameworkPropertyMetadata(30d, OnHeaderSizeChanged));

        /// <summary>顶部列头行的高度。</summary>
        public double HeaderHeight
        {
            get => (double)GetValue(HeaderHeightProperty);
            set => SetValue(HeaderHeightProperty, value);
        }

        private static void OnHeaderSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MatrixListBox)d).UpdateHeaderGridLength();
        }

        #endregion 尺寸

        #region 行列头模板

        public static readonly DependencyProperty ColumnHeaderTemplateProperty = DependencyProperty.Register(
            nameof(ColumnHeaderTemplate), typeof(DataTemplate), typeof(MatrixListBox),
            new PropertyMetadata(null));

        /// <summary>列头数据模板（数据上下文为列索引 int）；样式内置默认 “C{索引}” 模板。</summary>
        public DataTemplate? ColumnHeaderTemplate
        {
            get => (DataTemplate?)GetValue(ColumnHeaderTemplateProperty);
            set => SetValue(ColumnHeaderTemplateProperty, value);
        }

        public static readonly DependencyProperty RowHeaderTemplateProperty = DependencyProperty.Register(
            nameof(RowHeaderTemplate), typeof(DataTemplate), typeof(MatrixListBox),
            new PropertyMetadata(null));

        /// <summary>行头数据模板（数据上下文为行索引 int）；样式内置默认 “R{索引}” 模板。</summary>
        public DataTemplate? RowHeaderTemplate
        {
            get => (DataTemplate?)GetValue(RowHeaderTemplateProperty);
            set => SetValue(RowHeaderTemplateProperty, value);
        }

        #endregion 行列头模板

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

            // PART_ScrollViewer 位于本控件模板；其余部件位于 ScrollViewer 的内嵌模板，
            // 需通过 ScrollViewer.Template.FindName 在其模板 NameScope 中查找。
            _scrollViewer = GetTemplateChild("PART_ScrollViewer") as ScrollViewer;
            _columnHeaderTranslate = null;
            _rowHeaderTranslate = null;
            _rowHeaderColumn = null;
            _columnHeaderRow = null;

            if (_scrollViewer != null)
            {
                _scrollViewer.ApplyTemplate();

                _columnHeaderTranslate = FindTemplatePart<TranslateTransform>(_scrollViewer, "PART_ColumnHeaderTranslate");
                _rowHeaderTranslate = FindTemplatePart<TranslateTransform>(_scrollViewer, "PART_RowHeaderTranslate");
                _rowHeaderColumn = FindTemplatePart<ColumnDefinition>(_scrollViewer, "PART_RowHeaderColumn");
                _columnHeaderRow = FindTemplatePart<RowDefinition>(_scrollViewer, "PART_ColumnHeaderRow");

                _scrollViewer.ScrollChanged += OnScrollChanged;
            }

            UpdateHeaderGridLength();
            UpdateHeaderOffsets();
        }

        private static T? FindTemplatePart<T>(Control templatedParent, string name)
            where T : DependencyObject
        {
            return templatedParent.Template.FindName(name, templatedParent) as T;
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
