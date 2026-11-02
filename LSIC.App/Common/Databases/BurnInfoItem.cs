using SqlSugar;

namespace LSIC.App
{
    [SugarTable("BurnInfo")]
    public class BurnInfoItem
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int ZoneId { get; set; }

        [SugarColumn(Length = 32)]
        public string Ic { get; set; }

        [SugarColumn(Length = 32, IsNullable = true)]
        public string Lot { get; set; }

        [SugarColumn(Length = 32, IsNullable = true)]
        public string Operator { get; set; }

        [SugarColumn(IsNullable = true)]
        public DateTime? Start { get; set; }

        [SugarColumn(ColumnDataType = "BLOB", IsNullable = true)]
        public byte[] DutMask { get; set; }
    }
}
