namespace LSIC.App
{
    public class BurnLogger : IDisposable
    {
        private const string Layout = "[{date:yyyyMMddHHmmss}] {msg}";
        private readonly RollingFileLogger _dataLogger;
        private readonly RollingFileLogger _errorLogger;
        public BurnLogger(int zoneId, string ic, string lot, string op, DateTime start)
        {
            string dataLoggerFileName = $@".\Log\Burn\Zone {zoneId}\Data\{{{ic}}}_{{{lot}}}_{{{op}}}_{{{start:yyMMddHHmmss}}}.log";
            _dataLogger = new RollingFileLogger(dataLoggerFileName, messageFormat: Layout);

            string errorLoggerFileName = $@".\Log\Burn\Zone {zoneId}\Error\{{{ic}}}_{{{lot}}}_{{{op}}}_{{{start:yyMMddHHmmss}}}.log";
            _errorLogger = new RollingFileLogger(errorLoggerFileName, messageFormat: Layout);
        }
        public void LogError(CompareError error)
        {
            string text = $"{error.Step}|{error.Pattern}|{error.Register}|{error.Cs}|{error.Line}|{error.Dr}";
            _errorLogger.Info(text);
        }
        public void LogData(BurnStepVector step, BurnVector vectorStep, Dps[] dpss)
        {
            StringBuilder builder = new StringBuilder();
            //builder = builder.Append(temperature);
            //builder = builder.Append('|');
            builder = builder.Append(step == null ? string.Empty : step.Name);
            builder = builder.Append('|');
            builder = builder.Append(vectorStep == null ? string.Empty : vectorStep.PatternFile.FileName());
            builder = builder.Append('|');
            for (int i = 0; i < dpss.Length; i++)
            {
                Dps dps = dpss[i];
                builder = builder.Append(dps.OutputVoltage);
                builder = builder.Append('|');
                builder = builder.Append(dps.OutputCurrent);
                if (i == dpss.Length - 1)
                {
                    break;
                }
                builder = builder.Append('|');
            }
            _dataLogger.Info(builder.ToString());
        }
        public void Dispose()
        {
            _dataLogger?.Dispose();
            _errorLogger?.Dispose();
        }
    }
}