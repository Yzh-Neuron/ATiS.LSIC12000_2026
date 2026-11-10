namespace LSIC.App.Models
{
    /// <summary>
    /// 水冷管
    /// </summary>
    public class WaterCoolingPipe : ObservableObject
    {
        private double _flowRate;

        /// <summary>
        /// 水流量 L/min。
        /// </summary>
        public double FlowRate
        {
            get => _flowRate;
            set => SetProperty(ref _flowRate, value);
        }

        private double _valveFeedback;

        /// <summary>
        /// 阀位反馈 %
        /// </summary>
        public double ValveFeedback
        {
            get => _valveFeedback;
            set => SetProperty(ref _valveFeedback, value);
        }
    }
}
