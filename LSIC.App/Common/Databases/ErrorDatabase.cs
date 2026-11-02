using SqlSugar;

namespace LSIC.App
{
    public class ErrorDatabase
    {
        static ErrorDatabase()
        {
            _directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp", "Error");
            if (!Directory.Exists(_directory))
            {
                _ = Directory.CreateDirectory(_directory);
            }
        }
        private static readonly string _directory;
        public ErrorDatabase(Zone zone)
        {
            ConnectionConfig config = new ConnectionConfig
            {
                IsAutoCloseConnection = true,
                DbType = DbType.Sqlite,
                ConnectionString = $"datasource={_directory}/{nameof(Zone)}{zone.Id:D2}.db",
            };
            _db = new SqlSugarClient(config, it =>
            {
                it.Aop.OnLogExecuting = (sql, para) =>
                {
                    Console.WriteLine(UtilMethods.GetNativeSql(sql, para));
                };
            });

            CreateTables();
        }
        private readonly SqlSugarClient _db;
        private void CreateTables()
        {
            _db.CodeFirst.InitTables<CompareErrorItem>();
        }
        public bool TryAddError(CompareError error, TimeSpan interval)
        {
            DateTime now = DateTime.Now;

            SqlSugarClient db = _db.CopyNew();
            CompareErrorItem item = db.CopyNew().Queryable<CompareErrorItem>().First(o => o.Id == error.Id);
            if (item is null)
            {
                item = new CompareErrorItem()
                {
                    Id = error.Id,
                    Cs = error.Cs,
                    Dr = error.Dr,
                    Step = error.Step,
                    Line = error.Line,
                    Time = now,
                };
                db.Storageable(item).ExecuteCommand();
                return true;
            }
            if (now - item.Time > interval)
            {
                db.Updateable(item)
                    .SetColumns(it => new CompareErrorItem
                    {
                        Time = now
                    })
                    .Where(it => it.Id == item.Id)
                    .ExecuteCommand();
                return true;
            }
            return false;
        }
        public KeyValuePair<int, int>[] GetErrors()
        {
            CompareErrorItem[] items = _db.CopyNew().Queryable<CompareErrorItem>().ToArray();
            KeyValuePair<int, int>[] pairs = new KeyValuePair<int, int>[items.Length];
            for (int i = 0; i < items.Length; i++)
            {
                pairs[i] = new KeyValuePair<int, int>(items[i].Cs, items[i].Dr);
            }
            return pairs;
        }
        public void Clear()
        {
            _db.CopyNew().DbMaintenance.TruncateTable<CompareErrorItem>();
        }
    }
}
