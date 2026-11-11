namespace LSIC.App
{
    public static class PopHelper
    {
        public static PopAlarmView AlarmWindow { get; private set; }

        public static void Init()
        {
            AlarmWindow = new PopAlarmView();
        }

        public static void SetOwner(Window owner)
        {
            if (AlarmWindow is not null)
            {
                AlarmWindow.Owner = owner;
                AlarmWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
        }

        public static void ShowAlarm(string source, string message)
        {
            ObjectExtension.BeginInvokeOnUI(null, () =>
            {
                // 如果窗口已被关闭，重新创建
                if (AlarmWindow == null || !AlarmWindow.IsLoaded)
                {
                    AlarmWindow = new PopAlarmView();
                    AlarmWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    AlarmWindow.Topmost = true;
                    AlarmWindow.Add(new Alarm(source, message));
                    AlarmWindow.Show();
                    return;
                }

                // 窗口已存在且已加载：添加报警并置顶
                AlarmWindow.Add(new Alarm(source, message));
                AlarmWindow.Topmost = true;

                if (!AlarmWindow.IsVisible)
                {
                    AlarmWindow.Show();
                }
                else
                {
                    AlarmWindow.Activate();
                }
            });
        }

        public static void Dispose()
        {
            if (AlarmWindow is not null)
            {
                AlarmWindow.IsExiting = true;
                AlarmWindow.Close();
                AlarmWindow = null; // 重要：置空以便下次重建
            }
        }
    }
}