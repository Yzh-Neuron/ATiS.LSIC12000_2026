namespace LSIC.App
{
    public static class ObjectExtension
    {
        public static void InvokeOnUI<T>(this object obj, Action<T> action, T arg)
        {
            if (action != null)
            {
                var dispatcher = Application.Current?.Dispatcher;
                if (dispatcher != null)
                {
                    if (dispatcher.CheckAccess())
                    {
                        action(arg);
                    }
                    else
                    {
                        dispatcher.BeginInvoke(DispatcherPriority.Normal, action, arg);
                    }
                }
            }
        }
        public static void InvokeOnUI(this object obj, Action action)
        {
            if (action != null)
            {
                var dispatcher = Application.Current?.Dispatcher;
                if (dispatcher != null)
                {
                    if (dispatcher.CheckAccess())
                    {
                        action();
                    }
                    else
                    {
                        dispatcher.BeginInvoke(DispatcherPriority.Normal, action);
                    }
                }
            }
        }
    }
}
