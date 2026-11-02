using SqlSugar;

namespace LSIC.App
{
    [SugarTable("CompareError")]
    public class CompareErrorItem
    {
        [SugarColumn(IsPrimaryKey = true)]
        public ulong Id { get; set; }

        [SugarColumn]
        public int Cs { get; set; }

        [SugarColumn]
        public int Dr { get; set; }

        [SugarColumn(Length = 255)]
        public string Step { get; set; }

        [SugarColumn]
        public int Line { get; set; }

        [SugarColumn]
        public DateTime Time { get; set; }
    }
}
