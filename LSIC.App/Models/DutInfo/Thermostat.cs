using OxyPlot;
using OxyPlot.Axes;

namespace LSIC.App.Models
{
    /// <summary>
    /// 恒温器
    /// </summary>
    public class Thermostat : ObservableObject, ICloneable
    {
        public Thermostat(int zoneId, int id)
        {
            ZoneId = zoneId;
            Zone = Device.Instance.Zones.FirstOrDefault(o => o.Id == zoneId);
            Id = id;
        }

        public int ZoneId { get; }

        public Zone Zone { get; }

        public int Id { get; }

        public Dut Dut { get; set; }

        private bool _enable;
        public bool Enable
        {
            get => _enable;
            set => SetProperty(ref _enable, value);
        }

        private int? _targetTemperature;
        public int? TargetTemperature
        {
            get => _targetTemperature;
            set => SetProperty(ref _targetTemperature, value);
        }

        private int? _temperature;
        public int? Temperature
        {
            get => _temperature;
            set => SetProperty(ref _temperature, value);
        }

        private int? _kp;
        public int? Kp
        {
            get => _kp;
            set => SetProperty(ref _kp, value);
        }

        private int? _ki;
        public int? Ki
        {
            get => _ki;
            set => SetProperty(ref _ki, value);
        }

        private int? _kd;
        public int? Kd
        {
            get => _kd;
            set => SetProperty(ref _kd, value);
        }

        public ObservableCollection<DataPoint> PvData { get; } = new ObservableCollection<DataPoint>();

        public TimeRange ChartDataTimeRange { get; } = new TimeRange();

        public void Update(bool enable, int? temperature)
        {
            Enable = enable;
            Temperature = enable ? temperature : null;

            if (Enable)
            {
                while (PvData.Count >= 600) // 3s 一个点，最多保存 30 分钟数据
                {
                    PvData.RemoveAt(0);
                }

                DateTime now = DateTime.Now;
                PvData.Add(DateTimeAxis.CreateDataPoint(now, (Temperature ?? 0) / 10d));

                DateTime start = DateTimeAxis.ToDateTime(PvData.First().X);
                ChartDataTimeRange.Start = start;
                DateTime end = DateTimeAxis.ToDateTime(PvData.Last().X);
                ChartDataTimeRange.End = end - start < TimeSpan.FromMinutes(30d) ? start.AddMinutes(30d) : end;
            }
        }

        public object Clone()
        {
            return new Thermostat(ZoneId, Id);
        }
    }
}