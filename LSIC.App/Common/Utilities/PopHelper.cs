namespace LSIC.App
{
    public static class PopHelper
    {
        public static PopAlarmView AlarmWindow { get; set; }

        public static void Init()
        {
            AlarmWindow = new PopAlarmView();
        }

        public static void SetOwner(Window owner)
        {
            if (AlarmWindow != null)
            {
                AlarmWindow.Owner = owner;
            }
        }

        public static void ShowAlarm(string source, string message)
        {
            if (AlarmWindow is null)
            {
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    PopAlarmView view = new PopAlarmView();
                    view.Add(new Alarm(source, message));
                    view.Show();
                });
                return;
            }
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                AlarmWindow.Add(new Alarm(source, message));
                AlarmWindow.Show();
            });
        }

        public static void Dispose()
        {
            if (AlarmWindow != null)
            {
                AlarmWindow.IsExiting = true;
                AlarmWindow.Close();
            }
        }
    }
}
