using System.Windows.Controls;
using System.Windows.Shapes;

namespace LSIC.App.Controls
{
    [TemplatePart(Name = PartTrack, Type = typeof(ClipBorder))]
    [TemplatePart(Name = PartTick, Type = typeof(Line))]
    [TemplatePart(Name = PartFill, Type = typeof(Rectangle))]
    public class PowerBar : Control
    {
        private const string PartTrack = "PART_Track";
        private const string PartTick = "PART_MaxTick";
        private const string PartFill = "PART_Fill";
        private Line _tick;
        private Rectangle _fill;
        private ClipBorder _track;
        private TranslateTransform _tickOffset;
        private LinearGradientBrush _trackBrush;
        private Color _cachedLowColor;
        private Color _cachedMidColor;
        private Color _cachedHighColor;
        private bool _updatePending;
        static PowerBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PowerBar), new FrameworkPropertyMetadata(typeof(PowerBar)));
        }

        #region 依赖属性
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(double), typeof(PowerBar), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsArrange, OnLayoutPropertyChanged));
        public double MaxPower
        {
            get { return (double)GetValue(MaxPowerProperty); }
            set { SetValue(MaxPowerProperty, value); }
        }
        public static readonly DependencyProperty MaxPowerProperty =
            DependencyProperty.Register(nameof(MaxPower), typeof(double), typeof(PowerBar), new FrameworkPropertyMetadata(100d, FrameworkPropertyMetadataOptions.AffectsArrange, OnLayoutPropertyChanged));
        public double TickRatio
        {
            get { return (double)GetValue(TickRatioProperty); }
            set { SetValue(TickRatioProperty, value); }
        }
        public static readonly DependencyProperty TickRatioProperty =
            DependencyProperty.Register(nameof(TickRatio), typeof(double), typeof(PowerBar), new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsArrange, OnLayoutPropertyChanged));
        public Color LowColor
        {
            get { return (Color)GetValue(LowColorProperty); }
            set { SetValue(LowColorProperty, value); }
        }
        public static readonly DependencyProperty LowColorProperty =
            DependencyProperty.Register(nameof(LowColor), typeof(Color), typeof(PowerBar), new FrameworkPropertyMetadata(Colors.Green, OnBrushPropertyChanged));
        public Color MidColor
        {
            get { return (Color)GetValue(MidColorProperty); }
            set { SetValue(MidColorProperty, value); }
        }
        public static readonly DependencyProperty MidColorProperty =
            DependencyProperty.Register(nameof(MidColor), typeof(Color), typeof(PowerBar), new FrameworkPropertyMetadata(Colors.Yellow, OnBrushPropertyChanged));
        public Color HighColor
        {
            get { return (Color)GetValue(HighColorProperty); }
            set { SetValue(HighColorProperty, value); }
        }
        public static readonly DependencyProperty HighColorProperty =
            DependencyProperty.Register(nameof(HighColor), typeof(Color), typeof(PowerBar), new FrameworkPropertyMetadata(Colors.OrangeRed, OnBrushPropertyChanged));
        public Brush TickBrush
        {
            get { return (Brush)GetValue(TickBrushProperty); }
            set { SetValue(TickBrushProperty, value); }
        }
        public static readonly DependencyProperty TickBrushProperty =
            DependencyProperty.Register(nameof(TickBrush), typeof(Brush), typeof(PowerBar), new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));
        public double Ratio
        {
            get
            {
                var max = MaxPower <= 0 ? 1 : MaxPower;
                return Math.Clamp(Value / max, 0d, 1d);
            }
        }
        #endregion
        private static void OnLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PowerBar)d).InvalidateUpdate();
        }
        private static void OnBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PowerBar)d).UpdateTrackBrush();
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _track = GetTemplateChild(PartTrack) as ClipBorder;
            _fill = GetTemplateChild(PartFill) as Rectangle;
            _tick = GetTemplateChild(PartTick) as Line;

            if (_tick != null)
            {
                _tickOffset = new TranslateTransform();
                _tick.RenderTransform = _tickOffset;
            }
            else
            {
                _tickOffset = null;
            }

            UpdateTrackBrush();
            InvalidateUpdate();
        }
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            InvalidateUpdate();
        }
        private void UpdateTrackBrush()
        {
            if (_track == null)
            {
                return;
            }
            if (_trackBrush != null
                && _cachedLowColor == LowColor
                && _cachedMidColor == MidColor
                && _cachedHighColor == HighColor)
            {
                return;
            }

            _cachedLowColor = LowColor;
            _cachedMidColor = MidColor;
            _cachedHighColor = HighColor;

            var brush = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 0)
            };

            brush.GradientStops.Add(new GradientStop(LowColor, 0.0));
            brush.GradientStops.Add(new GradientStop(MidColor, 0.2));
            brush.GradientStops.Add(new GradientStop(HighColor, 1.0));
            brush.Freeze();

            _trackBrush = brush;
            _track.Background = brush;
        }
        private void InvalidateUpdate()
        {
            if (_updatePending)
            {
                return;
            }
            _updatePending = true;
            Dispatcher.BeginInvoke(new Action(FlushUpdate), DispatcherPriority.Render);
        }
        private void FlushUpdate()
        {
            _updatePending = false;

            if (_fill == null || _tick == null || _tickOffset == null)
            {
                return;
            }

            var w = ActualWidth;
            var h = ActualHeight;
            if (w <= 0 || h <= 0)
            {
                return;
            }

            _fill.Width = Math.Max(w * (1 - Ratio), 0);

            if (_tick.Y2 != h)
            {
                _tick.Y1 = 0;
                _tick.Y2 = h;
            }
            _tickOffset.X = w * Math.Clamp(TickRatio, 0d, 1d);
        }
    }
}