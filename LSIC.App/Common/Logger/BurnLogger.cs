namespace LSIC.App
{
    public class BurnLogger : IDisposable
    {
        private const string Layout = "[{date:yyyyMMddHHmmss}] {msg}";
        private readonly RollingFileLogger _dataLogger;
        private readonly RollingFileLogger _errorLogger;
        private readonly RollingFileLogger _spiLogger;

        public BurnLogger(int zoneId, string ic, string lot, string op, DateTime start)
        {
            string dataLoggerFileName = $@".\Log\Burn\Zone {zoneId}\Data\{{{ic}}}_{{{lot}}}_{{{op}}}_{{{start:yyMMddHHmmss}}}.log";
            _dataLogger = new RollingFileLogger(dataLoggerFileName, messageFormat: Layout);

            string errorLoggerFileName = $@".\Log\Burn\Zone {zoneId}\Error\{{{ic}}}_{{{lot}}}_{{{op}}}_{{{start:yyMMddHHmmss}}}.log";
            _errorLogger = new RollingFileLogger(errorLoggerFileName, messageFormat: Layout);

            string spiLoggerFileName = $@".\Log\Burn\Zone {zoneId}\SPI\{{{ic}}}_{{{lot}}}_{{{op}}}_{{{start:yyMMddHHmmss}}}.log";
            _spiLogger = new RollingFileLogger(spiLoggerFileName, messageFormat: Layout);
        }

        public void LogError(CompareError error)
        {
            string text = $"{error.Step}|{error.Pattern}|{error.Register}|{error.Cs}|{error.Line}|{error.Dr}";
            _errorLogger.Info(text);
        }

        /// <summary>
        /// 释放老化区 Data/Error/SPI 日志文件句柄
        /// </summary>
        public void Dispose()
        {
            _dataLogger?.Dispose();
            _errorLogger?.Dispose();
            _spiLogger?.Dispose();
        }
    }
}