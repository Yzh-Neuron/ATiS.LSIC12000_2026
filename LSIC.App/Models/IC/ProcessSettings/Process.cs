namespace LSIC.App.Models
{
    /// <summary>
    /// 流程
    /// </summary>
    public class Process
    {
        public Process()
        {
            var powerOn = StepPower.PowerOn();
            var powerOff = StepPower.PowerOff();
            StepItems.Add(powerOn);
            StepItems.Add(powerOff);
        }

        /// <summary>
        /// 流程条目（上电 + 向量（有序） + 下电）
        /// </summary>
        public ObservableCollection<StepItem> StepItems { get; } = new();

        /// <summary>
        /// 并列循环区间
        /// </summary>
        public ObservableCollection<Loop> Loops { get; } = new();
    }
}
