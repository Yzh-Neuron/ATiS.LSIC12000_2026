using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace ATiS.Sim.Wpf
{
    /// <summary>
    /// 语言切换辅助类：在语言切换时强制刷新控件上的语言绑定。
    /// 用法：XAML 中设置 local:LanguageHelper.Auto="True"
    /// </summary>
    public static class LanguageHelper
    {
        private static readonly ConcurrentDictionary<Type, Func<FrameworkElement, LanguageEventHandler>> _handlerRegistrations = new();

        private static readonly ConditionalWeakTable<FrameworkElement, LanguageEventHandler> _elementHandlers = new();

        public static readonly DependencyProperty AutoProperty = DependencyProperty.RegisterAttached(
            "Auto", typeof(bool), typeof(LanguageHelper), new PropertyMetadata(false, OnAutoPropertyChanged));

        public static bool GetAuto(DependencyObject obj) => (bool)obj.GetValue(AutoProperty);
        public static void SetAuto(DependencyObject obj, bool value) => obj.SetValue(AutoProperty, value);

        private static void OnAutoPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is not bool isAutoEnable || d is not FrameworkElement frameworkElement)
            {
                return;
            }

            frameworkElement.Loaded -= OnFrameworkElementLoaded;
            frameworkElement.Unloaded -= OnFrameworkElementUnloaded;

            if (isAutoEnable)
            {
                frameworkElement.Loaded += OnFrameworkElementLoaded;
                frameworkElement.Unloaded += OnFrameworkElementUnloaded;
            }
            else
            {
                // Auto 关闭时，取消已存在 handler 的语言事件订阅，防止泄漏
                if (_elementHandlers.TryGetValue(frameworkElement, out LanguageEventHandler? handler))
                {
                    handler.Unsubscribe();
                }
            }
        }

        #region 控件事件处理

        private static void OnFrameworkElementLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement frameworkElement)
            {
                return;
            }

            // 已存在处理器（如 Tab 切换导致多次 Loaded），直接订阅并刷新
            if (_elementHandlers.TryGetValue(frameworkElement, out LanguageEventHandler? handler))
            {
                handler.Subscribe();
                handler.UpdateElementLanguage(GlobalLanguageManager.CurrentLanguage);
                return;
            }

            handler = CreateLanguageHandler(frameworkElement);
            if (handler == null)
            {
                return;
            }

            try
            {
                _elementHandlers.Add(frameworkElement, handler);
            }
            catch (ArgumentException)
            {
                // 竞态保护：另一线程已添加，取回已有实例
                _elementHandlers.TryGetValue(frameworkElement, out handler);
            }

            handler?.Subscribe();
        }

        private static void OnFrameworkElementUnloaded(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement frameworkElement ||
                !_elementHandlers.TryGetValue(frameworkElement, out LanguageEventHandler? handler))
            {
                return;
            }

            handler.Unsubscribe();
        }

        #endregion

        #region 语言处理器抽象基类及实现

        public abstract class LanguageEventHandler
        {
            protected LanguageEventHandler(FrameworkElement targetElement)
            {
                _targetElementRef = new WeakReference<FrameworkElement>(targetElement);
            }

            private bool _isSubscribed;
            private readonly WeakReference<FrameworkElement> _targetElementRef;

            protected bool TryGetTargetElement(out FrameworkElement? targetElement)
            {
                return _targetElementRef.TryGetTarget(out targetElement);
            }

            /// <summary>
            /// 使用 PropertyChangedEventManager（WPF 弱事件管理器）订阅语言变化。
            /// 关键：弱事件不会强引用 handler，从根源上避免静态单例事件导致的内存泄漏。
            /// </summary>
            public virtual void Subscribe()
            {
                if (_isSubscribed)
                {
                    return;
                }

                PropertyChangedEventManager.AddHandler(
                    FrameworkLanguageProvider.Instance,
                    OnLanguageChanged,nameof(FrameworkLanguageProvider.CurrentLanguage));

                _isSubscribed = true;
            }

            public virtual void Unsubscribe()
            {
                if (!_isSubscribed)
                {
                    return;
                }

                PropertyChangedEventManager.RemoveHandler(
                    FrameworkLanguageProvider.Instance,
                    OnLanguageChanged,
                    nameof(FrameworkLanguageProvider.CurrentLanguage));

                _isSubscribed = false;
            }

            private void OnLanguageChanged(object? sender, PropertyChangedEventArgs e)
            {
                // 目标元素已被 GC 回收：自动退订，避免后续空转调用
                if (!TryGetTargetElement(out _))
                {
                    Unsubscribe();
                    return;
                }

                UpdateElementLanguage(GlobalLanguageManager.CurrentLanguage);
            }

            public abstract void UpdateElementLanguage(UiLanguage languageType);
        }

        private class TextBlockHandler : LanguageEventHandler
        {
            public TextBlockHandler(TextBlock targetElement) : base(targetElement) { }

            public override void UpdateElementLanguage(UiLanguage languageType)
            {
                if (TryGetTargetElement(out FrameworkElement? element) && element is TextBlock textBlock)
                {
                    GetBindingExpression(textBlock, TextBlock.TextProperty)?.UpdateTarget();
                }
            }
        }

        private class TextBoxHandler : LanguageEventHandler
        {
            public TextBoxHandler(TextBox targetElement) : base(targetElement) { }

            public override void UpdateElementLanguage(UiLanguage languageType)
            {
                if (TryGetTargetElement(out FrameworkElement? element) && element is TextBox textBox)
                {
                    GetBindingExpression(textBox, TextBox.TextProperty)?.UpdateTarget();
                }
            }
        }

        private class ButtonHandler : LanguageEventHandler
        {
            public ButtonHandler(Button targetElement) : base(targetElement) { }

            public override void UpdateElementLanguage(UiLanguage languageType)
            {
                if (TryGetTargetElement(out FrameworkElement? element) && element is Button button)
                {
                    GetBindingExpression(button, ContentControl.ContentProperty)?.UpdateTarget();
                }
            }
        }

        private class HeaderedItemsControlHandler : LanguageEventHandler
        {
            public HeaderedItemsControlHandler(HeaderedItemsControl targetElement) : base(targetElement) { }

            public override void UpdateElementLanguage(UiLanguage languageType)
            {
                if (TryGetTargetElement(out FrameworkElement? element) && element is HeaderedItemsControl control)
                {
                    GetBindingExpression(control, ItemsControl.ItemsSourceProperty)?.UpdateTarget();
                    GetBindingExpression(control, HeaderedItemsControl.HeaderProperty)?.UpdateTarget();
                    // 递归更新所有子菜单项
                    UpdateChildHeaderedItems(control);
                }
            }

            private void UpdateChildHeaderedItems(HeaderedItemsControl headeredItems)
            {
                for (int i = 0; i < headeredItems.Items.Count; i++)
                {
                    if (headeredItems.ItemContainerGenerator.ContainerFromIndex(i) is HeaderedItemsControl child)
                    {
                        child.GetBindingExpression(HeaderedItemsControl.HeaderProperty)?.UpdateTarget();
                        UpdateChildHeaderedItems(child);
                    }
                }
            }
        }

        private class SelectorHandler : LanguageEventHandler
        {
            public SelectorHandler(Selector targetElement) : base(targetElement) { }

            public override void UpdateElementLanguage(UiLanguage languageType)
            {
                if (TryGetTargetElement(out FrameworkElement? element) && element is Selector selector)
                {
                    GetBindingExpression(selector, ItemsControl.ItemsSourceProperty)?.UpdateTarget();
                    GetBindingExpression(selector, Selector.SelectedValueProperty)?.UpdateTarget();
                    GetBindingExpression(selector, Selector.SelectedItemProperty)?.UpdateTarget();
                }
            }
        }

        private static BindingExpression? GetBindingExpression<T>(T target, DependencyProperty dp) where T : FrameworkElement
        {
            return target.GetBindingExpression(dp);
        }

        #endregion

        #region 控件语言更新切换处理器的注册

        static LanguageHelper()
        {
            RegisterDefaultHandlers();
        }

        private static void RegisterDefaultHandlers()
        {
            RegisterHandler<TextBlock>(e => new TextBlockHandler(e));
            RegisterHandler<TextBox>(e => new TextBoxHandler(e));
            RegisterHandler<Button>(e => new ButtonHandler(e));
            RegisterHandler<HeaderedItemsControl>(e => new HeaderedItemsControlHandler(e));
            RegisterHandler<Selector>(e => new SelectorHandler(e));
        }

        public static void RegisterHandler<T>(Func<T, LanguageEventHandler> factory) where T : FrameworkElement
        {
            _handlerRegistrations[typeof(T)] = element => factory((T)element);
        }

        private static LanguageEventHandler? CreateLanguageHandler(FrameworkElement element)
        {
            if (element == null)
            {
                return null;
            }

            Type currentType = element.GetType();

            if (_handlerRegistrations.TryGetValue(currentType, out var factory))
            {
                return factory(element);
            }

            // 按照继承树查找，例如自定义 Button 子类匹配到 Button 处理器
            for (Type? type = currentType.BaseType; type != null; type = type.BaseType)
            {
                if (_handlerRegistrations.TryGetValue(type, out var baseFactory))
                {
                    // 缓存派生类型 → 基类处理器的映射，避免每次遍历继承树
                    _handlerRegistrations.TryAdd(currentType, baseFactory);
                    return baseFactory(element);
                }
            }

            return null;
        }

        #endregion
    }
}
