namespace LSIC.App
{
    public static class ObjectExtension
    {
        public static void InvokeOnUI(this object obj, Action action, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            if (action is not null)
            {
                var dispatcher = Application.Current?.Dispatcher;
                if (dispatcher is not null)
                {
                    if (dispatcher.CheckAccess())
                    {
                        action();
                    }
                    else
                    {
                        dispatcher.Invoke(priority, action);
                    }
                }
            }
        }
        public static void BeginInvokeOnUI(this object obj, Action action, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            if (action is not null)
            {
                var dispatcher = Application.Current?.Dispatcher;
                if (dispatcher is not null)
                {
                    if (dispatcher.CheckAccess())
                    {
                        action();
                    }
                    else
                    {
                        dispatcher.BeginInvoke(priority, action);
                    }
                }
            }
        }
    }
}
