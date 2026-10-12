namespace LSIC.App.Models
{
    /// <summary>
    /// DPS电源（运行时）
    /// </summary>
    public class BurnDps
    {
        public int Id { get; }
        public int Vol { get; }
        public int Uvp { get; }
        public int Ovp { get; }
        public int Ocp { get; }
        public int Raise { get; }
        public int Delay { get; }

        public BurnDps(DpsSetting setting)
        {
            Id = setting.Id;
            Vol = setting.Vol;
            Uvp = setting.Uvp;
            Ovp = setting.Ovp;
            Ocp = setting.Ocp;
            Raise = setting.Raise;
            Delay = setting.Delay;
        }
    }
}