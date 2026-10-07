namespace LSIC.App
{
    public class BurnLogger : IDisposable
    {
        private const string Layout = "[{date:yyyyMMddHHmmss}] {message}";
        private readonly RollingFileLogger _dataLogger;
        private readonly RollingFileLogger _errorLogger;
        private readonly RollingFileLogger _spiLogger;
        public BurnLogger(Zone zone, string ic, string lot, string op, DateTime start)
        {
            string dataLoggerFileName = $@".\Log\Burn\Zone {zone.Id}\Data\{{{ic}}}_{{{lot}}}_{{{op}}}_{{{start:yyMMddHHmmss}}}.log";
            _dataLogger = new RollingFileLogger(dataLoggerFileName, messageFormat: Layout);

            string errorLoggerFileName = $@".\Log\Burn\Zone {zone.Id}\Error\{{{ic}}}_{{{lot}}}_{{{op}}}_{{{start:yyMMddHHmmss}}}.log";
            _errorLogger = new RollingFileLogger(dataLoggerFileName, messageFormat: Layout);

            string spiLoggerFileName = $@".\Log\Burn\Zone {zone.Id}\SPI\{{{ic}}}_{{{lot}}}_{{{op}}}_{{{start:yyMMddHHmmss}}}.log";
            _spiLogger = new RollingFileLogger(spiLoggerFileName, messageFormat: Layout);
        }

        public void Dispose()
        {
            _dataLogger?.Dispose();
            _errorLogger?.Dispose();
            _spiLogger?.Dispose();
        }
    }
}
