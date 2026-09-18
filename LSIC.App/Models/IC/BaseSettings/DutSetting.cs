namespace LSIC.App.Models
{
    public class DutSetting : ObservableObject
    {
        public int Id { get; private set; }

        public int ChipSelect { get; private set; }

        public int[] Drs { get; private set; }

        public int[] Dpss { get; private set; }

        public int ThermostatId { get; set; }

        public int Row { get; private set; }

        public int Column { get; private set; }
    }
}
