using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace LSIC.App.Views
{
    public partial class PopPlotView
    {
        private readonly LogPlot _data;
        public PopPlotView(LogPlot data)
        {
            _data = data;
            InitializeComponent();
            Loaded += OnLoaded;
        }
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            PlotModel model = new PlotModel();

            DateTimeAxis xAxis = new DateTimeAxis
            {
                LabelFormatter = x =>
                {
                    DateTime time = DateTimeAxis.ToDateTime(x);
                    return time.ToString("yyyy-MM-dd\n   HH:mm:ss");
                },
                MajorGridlineColor = OxyColors.LightGray,
                MajorGridlineThickness = 1,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineColor = OxyColors.LightGray,
                MinorGridlineThickness = 1,
                MinorGridlineStyle = LineStyle.Solid,
            };
            model.Axes.Add(xAxis);

            LinearAxis yAxis = new LinearAxis
            {
                MajorGridlineColor = OxyColors.LightGray,
                MajorGridlineThickness = 1,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineColor = OxyColors.LightGray,
                MinorGridlineThickness = 1,
                MinorGridlineStyle = LineStyle.Solid,
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
            model.Axes.Add(yAxis);

            foreach (KeyValuePair<LogColumn, List<DataPoint>> pair in _data.Columns)
            {
                LineSeries series = new LineSeries
                {
                    Title = pair.Key.Header,
                    ItemsSource = pair.Value,
                    StrokeThickness = 1.5
                };
                model.Series.Add(series);
            }

            PlotView.Model = model;
        }
    }
}
