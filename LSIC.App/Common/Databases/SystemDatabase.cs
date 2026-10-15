using ATiS.Orm;

namespace LSIC.App
{
    public class SystemDatabase
    {
        private SystemDatabase()
        {
            ConnectionConfig config = new ConnectionConfig
            {
                IsAutoCloseConnection = true,
                DataBaseType = DataBaseType.Sqlite,
                ConnectionString = $"datasource={DatabaseName}",
            };
            _db = new OrmClient(config, it =>
            {
                it.Aop.OnLogExecuting = (sql, para) =>
                {
                    Console.WriteLine(MethodUtility.GetNativeSql(sql, para));
                };
            });

            CreateTables();
        }
        public static SystemDatabase Instance = new SystemDatabase();
        private const string DatabaseName = "System.db";
        private readonly OrmClient _db;
        private void CreateTables()
        {
            _db.CodeFirst.InitTables<ConfigItem, BurnInfoItem>();
        }

        #region 配置
        public T GetConfig<T>(string key, T defaultValue = default)
        {
            if (_db.CopyNew().Queryable<ConfigItem>().First(x => x.Key == key) is { } item)
            {
                return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(item.Value);
            }
            return defaultValue;
        }
        public void SetConfig<T>(string key, T value)
        {
            _db.CopyNew().Storageable(new ConfigItem
            {
                Key = key,
                Value = Convert.ToString(value)
            }).ExecuteCommand();
        }
        #endregion

        #region 老化信息
        public BurnInfo GetBurnInfo(int zoneId)
        {
            BurnInfoItem item = _db.CopyNew().Queryable<BurnInfoItem>().First(x => x.ZoneId == zoneId);
            if (item is null)
            {
                return null;
            }

            Ic ic = null;
            if (!string.IsNullOrEmpty(item.Ic))
            {
                ic = IcConfigManager.GetIc(item.Ic);
            }

            return new BurnInfo(ic, item.Operator, item.Operator, item.Start, item.DutMask);
        }
        public BurnInfo CreateBurnInfo(int zoneId, Ic ic, string lot, string op)
        {
            DeleteBurnInfo(zoneId);
            BurnInfoItem item = new BurnInfoItem()
            {
                ZoneId = zoneId,
                Ic = IcConfigManager.GetIcPath(ic),
                Lot = lot,
                Operator = op,
            };
            _db.CopyNew().Storageable(item).ExecuteCommand();
            return new BurnInfo(ic, lot, op);
        }
        public void UpdateBurnInfo(int zoneId, DateTime start, byte[] dutMask)
        {
            _db.CopyNew().Updateable<BurnInfoItem>()
                .SetColumns(it => new BurnInfoItem
                {
                    Start = start,
                    DutMask = dutMask
                })
                .Where(it => it.ZoneId == zoneId)
                .ExecuteCommand();
        }
        public void DeleteBurnInfo(int zoneId)
        {
            _db.CopyNew().Deleteable<BurnInfoItem>().Where(it => it.ZoneId == zoneId).ExecuteCommand();
        }
        #endregion
    }
}
