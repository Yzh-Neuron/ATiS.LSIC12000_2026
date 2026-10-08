using System;
using ATiS.Orm;

namespace LSIC.App
{
    [OrmTable("CompareError")]
    public class CompareErrorItem
    {
        [OrmColumn(IsPrimaryKey = true)]
        public ulong Id { get; set; }

        [OrmColumn]
        public int Cs { get; set; }

        [OrmColumn]
        public int Dr { get; set; }

        [OrmColumn(Length = 255)]
        public string Step { get; set; }

        [OrmColumn]
        public int Line { get; set; }

        [OrmColumn]
        public DateTime Time { get; set; }
    }
}
