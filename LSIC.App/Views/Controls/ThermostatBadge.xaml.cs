namespace LSIC.App.Views
{
    public partial class ThermostatBadge
    {
        public ThermostatBadge()
        {
            InitializeComponent();
            Refresh();
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
            get => (bool)GetValue(TemperatureEnabledProperty);
            set => SetValue(TemperatureEnabledProperty, ValueBoxes.BooleanBox(value));
        }

        public static readonly DependencyProperty IsShowDisableTempProperty =
            DependencyProperty.Register(nameof(IsShowDisableTemp), typeof(bool), typeof(ThermostatBadge), new PropertyMetadata(ValueBoxes.TrueBox, OnDisplayChanged));
        public bool IsShowDisableTemp
        {
            get => (bool)GetValue(IsShowDisableTempProperty);
            set => SetValue(IsShowDisableTempProperty, ValueBoxes.BooleanBox(value));
        }

        public static readonly DependencyProperty EnableBackgroundProperty =
              DependencyProperty.Register(nameof(EnableBackground), typeof(SolidColorBrush), typeof(ThermostatBadge), new PropertyMetadata(Brushes.LightGreen, OnDisplayChanged));
        public SolidColorBrush EnableBackground
        {
            get => (SolidColorBrush)GetValue(EnableBackgroundProperty);
            set => SetValue(EnableBackgroundProperty, value);
        }

        private static void OnDisplayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ThermostatBadge)?.Refresh();
        }
        private void Refresh()
        {
            bool hasSetting = Temperature.HasValue;
            bool showEnabled = hasSetting && TemperatureEnabled;
            bool showDisabled = hasSetting && !TemperatureEnabled && IsShowDisableTemp;
            bool showBadge = showEnabled || showDisabled;

            EnabledTextBlock.Visibility = showEnabled ? Visibility.Visible : Visibility.Collapsed;
            DisabledTextBlock.Visibility = showDisabled ? Visibility.Visible : Visibility.Collapsed;

            Visibility = showBadge ? Visibility.Visible : Visibility.Collapsed;
            if (showEnabled)
            {
                Background = EnableBackground;
            }
            else
            {
                ClearValue(BackgroundProperty);
            }
        }
    }
}