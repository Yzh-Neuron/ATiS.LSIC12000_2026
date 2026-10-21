using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ATiS.Sim.Wpf
{
    public class AtisLogo : Control
    {
        public static readonly DependencyProperty LogoBrushProperty =
            DependencyProperty.Register(nameof(LogoBrush), typeof(Brush), typeof(AtisLogo), new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromRgb(0x2A, 0x52, 0xCC)), FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty DotBrushProperty =
            DependencyProperty.Register(nameof(DotBrush), typeof(Brush), typeof(AtisLogo), new FrameworkPropertyMetadata(Brushes.Red, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty RegisteredBrushProperty =
            DependencyProperty.Register(nameof(RegisteredBrush), typeof(Brush), typeof(AtisLogo), new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));
        public Brush LogoBrush
        {
            get => (Brush)GetValue(LogoBrushProperty);
            set => SetValue(LogoBrushProperty, value);
        }
        public Brush DotBrush
        {
            get => (Brush)GetValue(DotBrushProperty);
            set => SetValue(DotBrushProperty, value);
        }
        public Brush RegisteredBrush
        {
            get => (Brush)GetValue(RegisteredBrushProperty);
            set => SetValue(RegisteredBrushProperty, value);
        }
        static AtisLogo()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AtisLogo), new FrameworkPropertyMetadata(typeof(AtisLogo)));
        }
    }
}
