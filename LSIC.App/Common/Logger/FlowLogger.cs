namespace LSIC.App
{
    public class FlowLogger : IDisposable
    {
        private const int MaxCount = 500;
        private readonly RollingFileLogger logger;
        public ConcurrentQueue<Flow> History { get; private set; }
        public event AddFlowHandler OnAddFlow;
        public FlowLogger(Zone zone)
        {
            History = new ConcurrentQueue<Flow>();
            logger = new RollingFileLogger($@".\Log\Flow\Zone {zone.Id}\{{date:yyyyMMdd}}.log", RollingInterval.Day, retentionPeriod: SystemLog.RetentionPeriod);
        }
        public void LogDebug(string message)
        {
            AddFlow(DateTime.Now, LogLevel.Info, message);
        }
        public void LogInfo(string message)
        {
            logger.Info(message);
            AddFlow(DateTime.Now, LogLevel.Info, message);
        }
        public void LogWarn(string message)
        {
            logger.Warn(message);
            AddFlow(DateTime.Now, LogLevel.Warn, message);
        }
        public void LogError(string message)
        {
            logger.Error(message);
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
            OnAddFlow?.Invoke(flow);
        }
        public void Dispose()
        {
            logger.Dispose();
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
