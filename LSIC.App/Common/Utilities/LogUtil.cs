namespace LSIC.App
{
    internal static class LogUtil
    {
        private static readonly RollingFileLogger SystemLogger = new RollingFileLogger(".\\Log\\System\\Log_.log", RollingInterval.Day, 256 * 1024 * 1024);

        public static void Debug(string message) => SystemLogger.Debug(message);

        public static void Info(string message) => SystemLogger.Info(message);

        public static void Warn(string message) => SystemLogger.Warn(message);

        public static void Error(string message) => SystemLogger.Error(message);

        public static void Fatal(string message) => SystemLogger.Fatal(message);
    }
}
