using ATiS.Sim.Wpf;
using System.Linq;
using System.Windows.Input;
using System.Windows.Interop;

namespace System.Windows
{
    internal static class WindowExtension
    {
        public static void BindingSystemCommands(this Window window)
        {
            window.CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, CloseWindow));
            window.CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, MaximizeWindow, CanResizeWindow));
            window.CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, MinimizeWindow, CanMinimizeWindow));
            window.CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, RestoreWindow, CanResizeWindow));
        }

        private static void CanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            Window window = (Window)sender;
            e.CanExecute = window.ResizeMode is ResizeMode.CanResize or ResizeMode.CanResizeWithGrip;
        }

        private static void CanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            Window window = (Window)sender;
            e.CanExecute = window.ResizeMode != ResizeMode.NoResize;
        }

        private static void CloseWindow(object sender, ExecutedRoutedEventArgs e)
        {
            Window window = (Window)sender;
            window.Close();
        }

        private static void MaximizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            Window window = (Window)sender;
            SystemCommands.MaximizeWindow(window);
            e.Handled = true;
        }

        private static void MinimizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            Window window = (Window)sender;
            SystemCommands.MinimizeWindow(window);
            e.Handled = true;
        }

        private static void RestoreWindow(object sender, ExecutedRoutedEventArgs e)
        {
            Window window = (Window)sender;
            SystemCommands.RestoreWindow(window);
            e.Handled = true;
        }

        #region 扩展方法

        /// <summary>
        /// 获取窗体的指针
        /// </summary>
        public static IntPtr GetHandle(this Window window) => new WindowInteropHelper(window).EnsureHandle();

        /// <summary>
        /// 获取当前应用中处于激活的一个窗口
        /// </summary>
        public static Window? GetActiveWindow()
        {
            IntPtr activeWindow = InteropMethods.GetActiveWindow();
            if (activeWindow == IntPtr.Zero)
            {
                return null;
            }
            return Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.GetHandle() == activeWindow);
        }

        #endregion
    }
}
