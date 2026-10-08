using ATiS.Orm;

namespace LSIC.App
{
    [OrmTable("Config")]
    public class ConfigItem
    {
        [OrmColumn(IsPrimaryKey = true, Length = 255)]
        public string Key { get; set; }

        [OrmColumn(Length = 255)]
        public string Value { get; set; }
    }
}
