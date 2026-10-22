using OxyPlot;
using OxyPlot.Wpf;

namespace LSIC.App.Views
{
    public partial class PopPlotView
    {
        private readonly LogPlot _data;
        public PopPlotView(LogPlot data)
        {
            _data = data;
            InitializeComponent();

            xAxis.LabelFormatter = x =>
            {
                DateTime time = OxyPlot.Axes.DateTimeAxis.ToDateTime(x);
                return time.ToString("yyyy-MM-dd\nHH:mm:ss", CultureInfo.InvariantCulture);
            };

            if (_data.Min <= _data.Max)
            {
                double span = _data.Max - _data.Min;
                double zoom;
                if (span < 0.01)
                {
                    zoom = 4;
                }
                else if (span < 0.1)
                {
                    zoom = 2;
                }
                else if (span < 1)
                {
                    zoom = 1;
                }
                else
                {
                    zoom = 0.5;
                }
                double min = _data.Min - span * zoom;
                double max = _data.Max + span * zoom;
                yAxis.Minimum = min;
                yAxis.Maximum = max;
            }

            foreach (KeyValuePair<LogColumn, List<DataPoint>> pair in _data.Columns)
            {
                LineSeries series = new LineSeries
                {
                    Title = pair.Key.Header,
                    ItemsSource = pair.Value,
                    StrokeThickness = 1.5
                };
                PlotView.Series.Add(series);
            }

        }
    }
}
