namespace LSIC.App.Models
{
    public class BurnDpsPlan
    {
        public byte Id { get; }
        public string Name { get; }
        public BurnDps[] Dpss { get; }

        public BurnDpsPlan(byte id, DpsPlan plan)
        {
            Id = id;
            Name = plan.Name;

            List<BurnDps> dpss = new List<BurnDps>();
            foreach (DpsSetting setting in plan.DpssSettings)
            {
                dpss.Add(new BurnDps(setting));
            }
            Dpss = dpss.ToArray();
        }
    }
}