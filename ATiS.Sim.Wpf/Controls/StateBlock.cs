using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ATiS.Sim.Wpf
{
    public class StateBlock : Control
    {
        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(nameof(IsChecked),
            typeof(bool?), typeof(StateBlock),
            new FrameworkPropertyMetadata(ValueBoxes.FalseBox,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));

        public static readonly DependencyProperty AllowMouseToggleProperty = DependencyProperty.Register(
            nameof(AllowMouseToggle), typeof(bool), typeof(StateBlock),new PropertyMetadata(ValueBoxes.FalseBox));

        public static readonly DependencyProperty CheckedContentProperty =
            DependencyProperty.Register(nameof(CheckedContent), typeof(object), typeof(StateBlock),
                new PropertyMetadata(default(object)));

        public static readonly DependencyProperty UnCheckedContentProperty =
            DependencyProperty.Register(nameof(UnCheckedContent), typeof(object), typeof(StateBlock),
                new PropertyMetadata(default(object)));

        public static readonly DependencyProperty IndeterminateContentProperty =
            DependencyProperty.Register(nameof(IndeterminateContent), typeof(object), typeof(StateBlock),
                new PropertyMetadata(default(object)));


        [Category("Appearance")]
        [TypeConverter(typeof(NullableBoolConverter))]
        [Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
        public bool? IsChecked
        {
            get
            {
                // Because Nullable<bool> unboxing is very slow (uses reflection) first we cast to bool
                var value = GetValue(IsCheckedProperty);
                // ReSharper disable once RedundantExplicitNullableCreation
                return value == null ? new bool?() : new bool?((bool)value);
            }
            set => SetValue(IsCheckedProperty, value.HasValue ? ValueBoxes.BooleanBox(value.Value) : null);
        }

        /// <summary>
        /// 是否允许鼠标左键点击切换IsChecked状态
        /// </summary>
        public bool AllowMouseToggle
        {
            get => (bool)GetValue(AllowMouseToggleProperty);
            set => SetValue(AllowMouseToggleProperty, ValueBoxes.BooleanBox(value));
        }

        public object CheckedContent
        {
            get => GetValue(CheckedContentProperty);
            set => SetValue(CheckedContentProperty, value);
        }

        public object UnCheckedContent
        {
            get => GetValue(UnCheckedContentProperty);
            set => SetValue(UnCheckedContentProperty, value);
        }

        public object IndeterminateContent
        {
            get => GetValue(IndeterminateContentProperty);
            set => SetValue(IndeterminateContentProperty, value);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (AllowMouseToggle && e.ChangedButton is MouseButton.Left)
            {
                SetCurrentValue(IsCheckedProperty, IsChecked == true ? ValueBoxes.FalseBox : ValueBoxes.TrueBox);
            }
        }
    }
}
