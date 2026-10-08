using ATiS.Orm;

namespace LSIC.App
{
    [OrmTable("BurnInfo")]
    public class BurnInfoItem
    {
        [OrmColumn(IsPrimaryKey = true)]
        public int ZoneId { get; set; }

        [OrmColumn(Length = 32)]
        public string Ic { get; set; }

        [OrmColumn(Length = 32, IsNullable = true)]
        public string Lot { get; set; }

        [OrmColumn(Length = 32, IsNullable = true)]
        public string Operator { get; set; }

        [OrmColumn(IsNullable = true)]
        public DateTime? Start { get; set; }

        [OrmColumn(ColumnDataType = "BLOB", IsNullable = true)]
        public byte[] DutMask { get; set; }
    }
}
