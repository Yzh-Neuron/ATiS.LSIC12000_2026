namespace LSIC.App.Models
{
    public class BurnDpsPlan
    {
        public byte Id { get; }
        public string Name { get; }
        public Dps[] Dpss { get; }

        public BurnDpsPlan(byte id, DpsPlan plan)
        {
            Id = id;
            Name = plan.Name;

            List<Dps> dpss = new List<Dps>();
            foreach (DpsSetting setting in plan.DpssSettings)
            {
                dpss.Add(new Dps(setting));
            }
            Dpss = dpss.ToArray();
        }
    }
}