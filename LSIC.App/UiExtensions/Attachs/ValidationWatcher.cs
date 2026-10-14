using System.Runtime.CompilerServices;

namespace LSIC.App.Attachs
{
    public static class ValidationWatcher
    {
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.RegisterAttached("Source", typeof(object), typeof(ValidationWatcher), new PropertyMetadata(null, OnSourceChanged));

        public static object GetSource(DependencyObject obj) => obj.GetValue(SourceProperty);
        public static void SetSource(DependencyObject obj, object value) => obj.SetValue(SourceProperty, value);

        private static readonly DependencyPropertyKey ErrorsPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("Errors", typeof(string), typeof(ValidationWatcher), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty ErrorsProperty = ErrorsPropertyKey.DependencyProperty;
        public static string GetErrors(DependencyObject obj) => (string)obj.GetValue(ErrorsProperty);

        private static readonly DependencyPropertyKey HasErrorsPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("HasErrors", typeof(bool), typeof(ValidationWatcher), new PropertyMetadata(ValueBoxes.FalseBox));
        public static readonly DependencyProperty HasErrorsProperty = HasErrorsPropertyKey.DependencyProperty;
        public static bool GetHasErrors(DependencyObject obj) => (bool)obj.GetValue(HasErrorsProperty);

        private static readonly ConditionalWeakTable<FrameworkElement, HashSet<INotifyDataErrorInfo>> Subscriptions = new();
        private static readonly Dictionary<Type, PropertyInfo[]> PropertiesCache = new();
        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not FrameworkElement element)
            {
                return;
            }

            Unsubscribe(element);
            element.Loaded -= OnLoaded;
            element.Unloaded -= OnUnloaded;

            if (e.NewValue == null)
            {
                element.SetValue(ErrorsPropertyKey, string.Empty);
                element.SetValue(HasErrorsPropertyKey, ValueBoxes.FalseBox);
                return;
            }

            element.Loaded += OnLoaded;
            element.Unloaded += OnUnloaded;

            if (element.IsLoaded)
            {
                Subscribe(element, e.NewValue);
            }
        }
        private static void OnLoaded(object sender, RoutedEventArgs e)
        {
            var element = (FrameworkElement)sender;
            object source = GetSource(element);
            if (source != null)
            {
                Subscribe(element, source);
            }
        }
        private static void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Unsubscribe((FrameworkElement)sender);
        }
        private static void Subscribe(FrameworkElement element, object source)
        {
            HashSet<INotifyDataErrorInfo> set = Subscriptions.GetOrCreateValue(element);
            Traverse(source, info =>
            {
                if (set.Add(info))
                {
                    info.ErrorsChanged += (_, __) => UpdateValidationState(element);
                }
            });
            UpdateValidationState(element);
        }
        private static void Unsubscribe(FrameworkElement element)
        {
            if (Subscriptions.TryGetValue(element, out HashSet<INotifyDataErrorInfo> set))
            {
                foreach (INotifyDataErrorInfo info in set)
                {
                    info.ErrorsChanged -= (_, __) => UpdateValidationState(element);
                }
                set.Clear();

                element.SetValue(ErrorsPropertyKey, string.Empty);
                element.SetValue(HasErrorsPropertyKey, ValueBoxes.FalseBox);

                Subscriptions.Remove(element);
            }
        }
        private static void Traverse(object obj, Action<INotifyDataErrorInfo> action)
        {
            if (obj == null)
            {
                return;
            }

            if (obj is INotifyDataErrorInfo info)
            {
                action(info);
            }

            foreach (PropertyInfo prop in GetRecursiveProperties(obj.GetType()))
            {
                object value;
                try
                {
                    value = prop.GetValue(obj);
                }
                catch
                {
                    continue;
                }

                if (value is INotifyDataErrorInfo)
                {
                    Traverse(value, action);
                }
                else if (value is IEnumerable enumerable and not string)
                {
                    foreach (object item in enumerable)
                    {
                        Traverse(item, action);
                    }
                }
            }
        }
        private static PropertyInfo[] GetRecursiveProperties(Type type)
        {
            if (PropertiesCache.TryGetValue(type, out PropertyInfo[] cached))
            {
                return cached;
            }

            cached = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetIndexParameters().Length == 0)
                .Where(p => typeof(INotifyDataErrorInfo).IsAssignableFrom(p.PropertyType) || (p.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(p.PropertyType)))
                .ToArray();

            PropertiesCache[type] = cached;
            return cached;
        }
        private static void UpdateValidationState(FrameworkElement element)
        {
            var obj = GetSource(element);
            if (obj is ObservableValidator observableValidator)
            {
                var errors = observableValidator.GetAllExistingErrorsRecursive();
                element.SetValue(ErrorsPropertyKey, errors.ToMessageString());
                element.SetValue(HasErrorsPropertyKey, ValueBoxes.BooleanBox(errors.Count > 0));
            }
            else
            {
                element.SetValue(ErrorsPropertyKey, string.Empty);
                element.SetValue(HasErrorsPropertyKey, ValueBoxes.FalseBox);
            }
        }
    }
}
