namespace LSIC.App
{
    public class FlowLogger : IDisposable
    {
        private const int MaxCount = 500;
        private readonly RollingFileLogger logger;
        public ConcurrentQueue<Flow> History { get; private set; }
        public event AddFlowHandler OnAddFlow;
        public FlowLogger(RollingFileLogger fileLogger)
        {
            logger = fileLogger;
            History = new ConcurrentQueue<Flow>();
        }
        public void LogDebug(string message)
        {
            AddFlow(DateTime.Now, LogLevel.Debug, message);
        }
        public void LogInfo(string message)
        {
            logger?.Info(message);
            AddFlow(DateTime.Now, LogLevel.Info, message);
        }
        public void LogWarn(string message)
        {
            logger?.Warn(message);
            AddFlow(DateTime.Now, LogLevel.Warn, message);
        }
        public void LogError(string message)
        {
            logger?.Error(message);
            AddFlow(DateTime.Now, LogLevel.Error, message);
        }
        private void AddFlow(DateTime time, LogLevel level, string message)
        {
            while (History.Count >= MaxCount)
            {
                History.TryDequeue(out _);
            }
            Flow flow = new Flow(time, level, message);
            History.Enqueue(flow);

            try
            {
                OnAddFlow?.Invoke(flow);
            }
            catch
            {


            }

        }
        public void Dispose()
        {
            logger?.Dispose();
        }
    }
    public static class FlowLoggerExtension
    {
        public static bool IsValid(this FlowLogger logger)
        {
            return logger != null && Device.Instance.Mode == DeviceMode.Administrator;
        }
        public static bool IsValid<TEnum>(this FlowLogger logger, TEnum code) where TEnum : Enum
        {
            return logger != null && Device.Instance.Mode == DeviceMode.Administrator && FrameFilterUtility.Filter(logger, code);
        }
    }
    public class Flow
    {
        public Flow(DateTime time, LogLevel level, string message)
        {
            Time = time;
            Level = level;
            Message = message;
            Header = $"[ {Time:HH:mm:ss} {Level.GetString()} ]";
        }
        public DateTime Time { get; }
        public LogLevel Level { get; }
        public string Message { get; }
        public string Header { get; }
    }
    public delegate void AddFlowHandler(Flow flow);
}
