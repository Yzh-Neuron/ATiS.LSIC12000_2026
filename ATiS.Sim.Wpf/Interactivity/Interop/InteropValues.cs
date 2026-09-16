using System;
using System.Runtime.InteropServices;

namespace ATiS.Sim.Wpf
{
    /// <summary>
    /// 提供与 Windows API 的互操作值。
    /// </summary>
    internal class InteropValues
    {
        /// <summary>
        /// 提供与 Windows API 的外部 DLL。
        /// </summary>
        internal static class ExternDll
        {
            /// <summary>
            /// 涉及用户界面的 Windows API。
            /// </summary>
            public const string User32 = "user32.dll";

            /// <summary>
            /// 涉及图形设备接口的 Windows API。
            /// </summary>
            public const string Gdi32 = "gdi32.dll";

            /// <summary>
            /// 涉及 GDI+ 的 Windows API。
            /// </summary>
            public const string GdiPlus = "gdiplus.dll";

            /// <summary>
            /// 涉及内核的 Windows API。
            /// </summary>
            public const string Kernel32 = "kernel32.dll";

            /// <summary>
            /// 涉及 Shell 的 Windows API。Shell 主要包括 Windows Explorer 和任务栏。
            /// </summary>
            public const string Shell32 = "shell32.dll";

            /// <summary>
            /// 涉及 COM 的 Windows API。COM 是 Windows 的组件对象模型。
            /// </summary>
            public const string MsImg = "msimg32.dll";

            /// <summary>
            /// 涉及 NTDLL 的 Windows API。NTDLL 是 Windows NT 内核的用户模式接口。
            /// </summary>
            public const string NTdll = "ntdll.dll";

            /// <summary>
            /// 涉及 DWM 的 Windows API。DWM 是 Windows Vista 以及更高版本的桌面窗口管理器。
            /// </summary>
            public const string DwmApi = "dwmapi.dll";
        }

        public enum SystemMetrics
        {
            /// <summary>
            /// 带标题的窗口的边框填充量（以像素为单位）。
            /// 返回带标题的窗口周围的额外边框填充量
            /// Windows XP/2000：不支持此值。
            /// </summary>
            CXPADDEDBORDER = 92,
        }

        /// <summary>
        /// 窗口位置正在改变。
        /// </summary>
        internal const int WindowPositionChanging = 0x0046;

        /// <summary>
        /// 窗口的大小不能改变。
        /// </summary>
        internal const int WindowCannotResize = 0x0001;

        /// <summary>
        /// 按下 Alt 键。
        /// </summary>
        internal const int AltKeyDown = 0x0104;

        /// <summary>
        /// 按下 F4 键。
        /// </summary>
        internal const int F4KeyDown = 0x0073;

        /// <summary>
        /// 窗口位置。
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct WindowPosition
        {
            /// <summary>
            /// 窗口句柄。
            /// </summary>
            public IntPtr hwnd;

            /// <summary>
            /// 在 Z 顺序中位于指定窗口之后的窗口句柄。
            /// </summary>
            public IntPtr hwndInsertAfter;

            /// <summary>
            /// 窗口的左边界的 x 坐标。
            /// </summary>
            public int x;

            /// <summary>
            /// 窗口的上边界的 y 坐标。
            /// </summary>
            public int y;

            /// <summary>
            /// 窗口的宽度。
            /// </summary>
            public int cx;

            /// <summary>
            /// 窗口的高度。
            /// </summary>
            public int cy;

            /// <summary>
            /// 窗口位置标志。
            /// </summary>
            public uint flags;
        }

        [Flags]
        internal enum ExecutionState : uint
        {
            SystemRequired = 0x01,

            DisplayRequired = 0x02,

            [Obsolete("This value is not supported.")]
            UserPresent = 0x04,

            AwaymodeRequired = 0x40,

            Continuous = 0x80000000
        }

    }
}
