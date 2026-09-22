namespace LSIC.App.Models
{
    public class DutSetting : ObservableObject
    {
        public int Id { get; private set; }

        public int ChipSelect { get; private set; }

        public int[] Drs { get; private set; }

        public int[] Dpss { get; private set; }

        public int? ThermostatId { get; set; }

        public int Row { get; private set; }

        public int Column { get; private set; }

        public static DutSetting Create(int id, int row, int column, int chipSelect, int[] drs, int[] dpss, int? thermostatId)
        {
            DutSetting dutSetting = new DutSetting
            {
                Id = id,
                ChipSelect = chipSelect,
                Drs = drs,
                Dpss = dpss,
                ThermostatId = thermostatId,
                Row = row,
                Column = column
            };

            return dutSetting;
        }
    }
}
