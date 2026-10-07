namespace LSIC.App.Models
{
    /// <summary>
    /// 电源配置快照（运行时，下发用）
    /// </summary>
    public class Dps
    {
        public int Id { get; }
        public int Vol { get; }
        public int Uvp { get; }
        public int Ovp { get; }
        public int Ocp { get; }
        public int Raise { get; }
        public int Delay { get; }

        public Dps(DpsSetting setting)
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