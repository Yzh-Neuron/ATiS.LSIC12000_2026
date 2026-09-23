using System.Windows.Media;

namespace LSIC.App.Views
{
    public partial class ThermostatBadge
    {
        public ThermostatBadge()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TemperatureProperty =
            DependencyProperty.Register(nameof(Temperature), typeof(int?), typeof(ThermostatBadge), new PropertyMetadata(null, OnDisplayChanged));

        public int? Temperature
        {
            get => (int?)GetValue(TemperatureProperty);
            set => SetValue(TemperatureProperty, value);
        }

        public static readonly DependencyProperty TemperatureEnabledProperty =
             DependencyProperty.Register(nameof(TemperatureEnabled), typeof(bool), typeof(ThermostatBadge), new PropertyMetadata(ValueBoxes.FalseBox, OnDisplayChanged));

        public bool TemperatureEnabled
        {
            get { return (bool)GetValue(TemperatureEnabledProperty); }
            set { SetValue(TemperatureEnabledProperty, ValueBoxes.BooleanBox(value)); }
        }


        public static readonly DependencyProperty IsShowDisableTempProperty =
            DependencyProperty.Register(nameof(IsShowDisableTemp), typeof(bool), typeof(ThermostatBadge), new PropertyMetadata(ValueBoxes.TrueBox, OnDisplayChanged));


        public bool IsShowDisableTemp
        {
            get { return (bool)GetValue(IsShowDisableTempProperty); }
            set { SetValue(IsShowDisableTempProperty, ValueBoxes.BooleanBox(value)); }
        }


        public static readonly DependencyProperty EnableBackgroundProperty =
              DependencyProperty.Register(nameof(EnableBackground), typeof(SolidColorBrush), typeof(ThermostatBadge), new PropertyMetadata(Brushes.LightGreen));

        public SolidColorBrush EnableBackground
        {
            get { return (SolidColorBrush)GetValue(EnableBackgroundProperty); }
            set { SetValue(EnableBackgroundProperty, value); }
        }


        private static void OnDisplayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ThermostatBadge)d;
            var hasSetting = control.Temperature.HasValue;

            control.DisabledPanel.Visibility = hasSetting && !control.TemperatureEnabled == true && control.IsShowDisableTemp ? Visibility.Visible : Visibility.Collapsed;
            control.EnabledPanel.Visibility = hasSetting && control.TemperatureEnabled == true ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}