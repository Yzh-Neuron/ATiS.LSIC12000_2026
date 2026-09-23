using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;

namespace LSIC.App.Behaviors
{
    public class DutSelectionBehavior : Behavior<ListBox>
    {
        public static readonly DependencyProperty SelectedWrapperProperty =
            DependencyProperty.Register(nameof(SelectedWrapper), typeof(DutSettingWrapper), typeof(DutSelectionBehavior), new PropertyMetadata(null));

        public DutSettingWrapper SelectedWrapper
        {
            get => (DutSettingWrapper)GetValue(SelectedWrapperProperty);
            set => SetValue(SelectedWrapperProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += OnSelectionChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AssociatedObject.SelectedItems.Count > 0)
            {
                var dutSettings = AssociatedObject.SelectedItems.Cast<DutSetting>();

                if (dutSettings.Count() > 0)
                {
                    SelectedWrapper = new DutSettingWrapper(dutSettings);
                    return;
                }
            }
            SelectedWrapper = null;
        }
    }
}
