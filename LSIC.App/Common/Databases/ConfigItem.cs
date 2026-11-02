using SqlSugar;

namespace LSIC.App
{
    [SugarTable("Config")]
    public class ConfigItem
    {
        [SugarColumn(IsPrimaryKey = true, Length = 255)]
        public string Key { get; set; }

        [SugarColumn(Length = 255)]
        public string Value { get; set; }
    }
}
