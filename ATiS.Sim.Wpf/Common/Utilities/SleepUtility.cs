namespace ATiS.Sim.Wpf
{
    /// <summary>
    /// 系统睡眠相关的实用工具类
    /// </summary>
    public static class SleepUtility
    {
        /// <summary>
        /// 设置此线程此时开始一直处于运行状态 此时计算机不应该进入睡眠状态
        /// </summary>
        /// <param name="keepDisplayOn">
        /// 表示是否应该保持屏幕不关闭
        /// 对于游戏、视频或演示相关的任务需要保持屏幕不关闭 而对于后台服务、下载和监控等任务则不需要
        /// </param>
        public static void PreventForCurrentThread(bool keepDisplayOn = true)
        {
            InteropMethods.SetThreadExecutionState(keepDisplayOn
                ? InteropValues.ExecutionState.Continuous | InteropValues.ExecutionState.SystemRequired | InteropValues.ExecutionState.DisplayRequired
                : InteropValues.ExecutionState.Continuous | InteropValues.ExecutionState.SystemRequired);
        }

        /// <summary>
        /// 恢复此线程的运行状态 操作系统现在可以正常进入睡眠状态和关闭屏幕
        /// </summary>
        public static void RestoreForCurrentThread()
        {
            InteropMethods.SetThreadExecutionState(InteropValues.ExecutionState.Continuous);
        }

        /// <summary>
        /// 重置系统睡眠或者关闭屏幕的计时器 这样系统睡眠或者屏幕能够继续持续工作设定的超时时间
        /// </summary>
        /// <param name="keepDisplayOn">
        /// 表示是否应该保持屏幕不关闭
        /// 对于游戏、视频或演示相关的任务需要保持屏幕不关闭 而对于后台服务、下载和监控等任务则不需要
        /// </param>
        public static void ResetIdle(bool keepDisplayOn = true)
        {
            InteropMethods.SetThreadExecutionState(keepDisplayOn
                ? InteropValues.ExecutionState.SystemRequired | InteropValues.ExecutionState.DisplayRequired
                : InteropValues.ExecutionState.SystemRequired);
        }

    }
}
