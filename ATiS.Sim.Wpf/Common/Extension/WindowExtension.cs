using ATiS.Sim.Wpf;
using System.Linq;
using System.Windows.Interop;

namespace System.Windows
{
    public static class WindowExtension
    {
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
