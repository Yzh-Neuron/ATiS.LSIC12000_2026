using OxyPlot;

namespace LSIC.App
{
    public class LogPlot
    {
        public LogPlot(double min, double max, KeyValuePair<LogColumn, List<DataPoint>>[] columns)
        {
            Min = min;
            Max = max;
            Columns = columns;
        }

        public double Min { get; private set; }

        public double Max { get; private set; }

        public KeyValuePair<LogColumn, List<DataPoint>>[] Columns { get; private set; }
    }
}