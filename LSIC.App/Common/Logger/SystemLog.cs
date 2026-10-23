namespace LSIC.App
{
    internal static class SystemLog
    {
        /// <summary>
        /// 日志文件保留期（约 6 个月），System / Flow 日志通用；Burn 为生产日志，永久保留不参与清理
        /// </summary>
        internal static readonly TimeSpan RetentionPeriod = TimeSpan.FromDays(182);
        private static readonly RollingFileLogger SystemLogger = new RollingFileLogger(".\\Log\\System\\{date:yyyyMMdd}.log", RollingInterval.Day, 256 * 1024 * 1024, retentionPeriod: RetentionPeriod);
        public static void LogDebug(string message) => SystemLogger.Debug(message);
        public static void LogInfo(string message) => SystemLogger.Info(message);
        public static void LogWarn(string message) => SystemLogger.Warn(message);
        public static void LogError(string message) => SystemLogger.Error(message);
        public static void LogFatal(string message) => SystemLogger.Fatal(message);
        public static void Dispose() => SystemLogger.Dispose();
    }
}