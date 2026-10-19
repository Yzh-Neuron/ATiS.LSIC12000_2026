namespace LSIC.App
{
    internal static class SystemLog
    {
        private static readonly RollingFileLogger SystemLogger = new RollingFileLogger(".\\Log\\System\\{date:yyyyMMdd}.log", RollingInterval.Day, 256 * 1024 * 1024);
        public static void LogDebug(string message) => SystemLogger.Debug(message);
        public static void LogInfo(string message) => SystemLogger.Info(message);
        public static void LogWarn(string message) => SystemLogger.Warn(message);
        public static void LogError(string message) => SystemLogger.Error(message);
        public static void LogFatal(string message) => SystemLogger.Fatal(message);
        public static void Dispose() => SystemLogger.Dispose();
    }
}