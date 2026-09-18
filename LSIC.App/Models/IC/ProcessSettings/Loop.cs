namespace LSIC.App.Models
{
    /// <summary>
    /// 循环
    /// </summary>
    public class Loop
    {
        public Loop(StepItem start)
        {
            Start = start;
            End = start;
        }
        public StepItem Start { get; }
        public StepItem End { get; init; }
        public RunStrategy RunStrategy { get; } = new();
    }
}
