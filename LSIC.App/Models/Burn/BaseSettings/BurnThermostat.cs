using LSIC.App.Common;
using OxyPlot;

namespace LSIC.App.Models
{
    /// <summary>
    /// 独立温控（运行时）
    /// </summary>
    public class BurnThermostat : ObservableObject
    {
        public double TimeAxisMin { get; private set; }
        public double TimeAxisMax { get; private set; }
        private readonly CircularBuffer<DataPoint> _pvBuffer = new CircularBuffer<DataPoint>(600);
        public IEnumerable<DataPoint> PvData => _pvBuffer;
        public int Id { get; }
        private bool _enable;
        public bool Enable
        {
            get => _enable;
            set => SetProperty(ref _enable, value);
        }
        private int _targetTemperature;
        public int TargetTemperature
        {
            get => _targetTemperature;
            set => SetProperty(ref _targetTemperature, value);
        }
        private int? _temperature = 0;
        public int? Temperature
        {
            get => _temperature;
            set => SetProperty(ref _temperature, value);
        }
        private double? _kp;
        public double? Kp
        {
            get => _kp;
            set => SetProperty(ref _kp, value);
        }
        private double? _ki;
        public double? Ki
        {
            get => _ki;
            set => SetProperty(ref _ki, value);
        }
        private double? _kd;
        public double? Kd
        {
            get => _kd;
            set => SetProperty(ref _kd, value);
        }
        public BurnThermostat(ThermostatSetting setting)
        {
            Id = setting.Id;
            Enable = setting.Enable;
            TargetTemperature = setting.TargetTemperature;

//#if DEBUG
//            if (Enable)
//            {
//                int temp = 0;
//                Task.Run(async () =>
//                {
//                    while (true)
//                    {
//                        temp = (temp += 100) % 1000;
//                        Update(Enable, temp);
//                        await Task.Delay(3000);
//                    }
//                });
//            }
//#endif
        }
        public void Update(bool enable, int? temperature)
        {
            Enable = enable;
            Temperature = enable ? temperature : null;
            {
                var now = DateTime.Now;
                var nowOa = now.ToOADate();
                var y = (Temperature ?? 0) / 10d;
                _pvBuffer.Add(new DataPoint(nowOa, y));
                TimeAxisMax = nowOa + TimeSpan.FromMinutes(5).TotalDays; ;
                TimeAxisMin = nowOa - TimeSpan.FromMinutes(10).TotalDays;
                OnPropertyChanged(nameof(TimeAxisMin));
                OnPropertyChanged(nameof(TimeAxisMax));
            }
        }
    }
}
