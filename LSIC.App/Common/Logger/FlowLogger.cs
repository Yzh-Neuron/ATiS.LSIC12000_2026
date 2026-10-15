namespace LSIC.App
{
    public class FlowLogger : IDisposable
    {
        private const int MaxCount = 500;
        private readonly RollingFileLogger logger;
        public ConcurrentQueue<Flow> History { get; private set; }
        public event AddFlowHandler OnAddFlow;
        public event RemoveFlowHandler OnRemoveFlow;
        public FlowLogger(Zone zone)
        {
            History = new ConcurrentQueue<Flow>();
            logger = new RollingFileLogger($@".\Log\Flow\Zone {zone.Id}\{{date:yyyyMMdd}}.log", RollingInterval.Day);
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
                bool success = History.TryDequeue(out _);
                if (success &&
                    OnRemoveFlow is not null)
                {
                    OnRemoveFlow.Invoke();
                }
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
        }
        public DateTime Time { get; private set; }
        public LogLevel Level { get; private set; }
        public string Message { get; private set; }
        public override string ToString()
        {
            return $"[{Time:yyyy-MM-dd HH:mm:ss}] {Message}";
        }
    }
    public delegate void AddFlowHandler(Flow flow);
    public delegate void RemoveFlowHandler();
}
