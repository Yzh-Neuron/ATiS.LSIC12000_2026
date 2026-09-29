namespace LSIC.App.Models
{
    public class DutSetting : ObservableObject
    {
        public int Id { get; private set; }

        public int ChipSelect { get; private set; }

        public int[] Drs { get; private set; }

        public int[] Dpss { get; private set; }

        public ThermostatSetting ThermostatSetting { get; private set; }

        public int Row { get; private set; }

        public int Column { get; private set; }

        public DutSetting(int id, int row, int column, int chipSelect, int[] drs, int[] dpss, int? thermostatId)
        {
            Id = id;
            ChipSelect = chipSelect;
            Drs = drs;
            Dpss = dpss;
            Row = row;
            Column = column;

            if (thermostatId.HasValue)
            {
                ThermostatSetting = new ThermostatSetting(thermostatId.Value);
            }
        }
    }
}
