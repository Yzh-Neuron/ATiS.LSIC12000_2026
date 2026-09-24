using System.Windows;

namespace ATiS.Sim.Wpf
{
    public class BindingProxy<T> : Freezable
    {
        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy<T>();
        }

        public T Data
        {
            get => (T)GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register(nameof(Data), typeof(T), typeof(BindingProxy<T>), new FrameworkPropertyMetadata(default(T)));
    }
}
