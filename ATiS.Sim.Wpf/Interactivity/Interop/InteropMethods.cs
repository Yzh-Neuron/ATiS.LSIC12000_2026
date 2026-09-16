using System;
using System.Runtime.InteropServices;

namespace ATiS.Sim.Wpf
{
    /// <summary>
    /// 提供与 Windows API 的互操作方法。
    /// </summary>
    internal static class InteropMethods
    {
        /// <summary>
        /// 获取前台窗口句柄。
        /// </summary>
        [DllImport(InteropValues.ExternDll.User32, SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// 设置前台窗口。
        /// </summary>
        [DllImport(InteropValues.ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// 获取活动窗口。
        /// </summary>
        [DllImport(InteropValues.ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        internal static extern IntPtr GetActiveWindow();

        /// <summary>
        /// 获取系统参数。
        /// </summary>
        [DllImport(InteropValues.ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        internal static extern int GetSystemMetrics(InteropValues.SystemMetrics para);

        [DllImport(InteropValues.ExternDll.Kernel32)]
        internal static extern InteropValues.ExecutionState SetThreadExecutionState(InteropValues.ExecutionState state);
    }
}
