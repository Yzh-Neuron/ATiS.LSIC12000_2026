namespace LSIC.App
{
    /// <summary>
    /// 单例窗口管理器：已存在则激活，不存在则创建。
    /// </summary>
    public static class WindowUtility
    {
        /// <summary>
        /// 显示或激活指定类型的窗口，Owner自动设为主窗体。
        /// </summary>
        public static T ShowOrActivate<T>(bool ownerMain = true) where T : Window, new()
        {
            Window owner = ownerMain ? Application.Current.Windows.OfType<WinMainView>().FirstOrDefault() : null;
            return ShowOrActivate<T>(owner);
        }

        /// <summary>
        /// 显示或激活指定类型的窗口。
        /// </summary>
        public static T ShowOrActivate<T>(Window owner) where T : Window, new()
        {
            var window = Application.Current.Windows.OfType<T>().FirstOrDefault();

            if (window is not null && window.IsLoaded && window.Visibility != Visibility.Visible)
            {
                window.Close();
                window = null;
            }

            if (window is not null)
            {
                window.Activate();
                if (window.WindowState == WindowState.Minimized)
                {
                    window.WindowState = WindowState.Normal;
                }
                return window;
            }

            window = new T();

            if (owner is not null)
            {
                window.Owner = owner;
            }

            window.Show();

            // 解除 Owner，避免关闭 Owner 时连带关闭
            window.Owner = null;

            return window;
        }

        /// <summary>
        /// 判断某类型窗口当前是否已打开。
        /// </summary>
        public static bool IsOpened<T>() where T : Window
        {
            return Application.Current.Windows.OfType<T>().Any(w => w.IsLoaded && w.Visibility == Visibility.Visible);
        }

        /// <summary>
        /// 关闭某类型的所有已打开实例。
        /// </summary>
        public static void CloseAll<T>() where T : Window
        {
            var windows = Application.Current.Windows.OfType<T>().ToList();
            foreach (var w in windows)
            {
                try
                {
                    w.Close();
                }
                catch
                {
                }
            }
        }
    }
}