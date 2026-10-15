namespace LSIC.App
{
    internal class IcConfigManager
    {
        public const string IcSuffix = "Ic";
        private const string Ics = nameof(Ics);
        private const int DeleteRetryCount = 10;
        private const int DeleteRetryDelayMs = 10;
        private static readonly Encoding _encoding = Encoding.GetEncoding("gb2312");
        private static readonly string _directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Ics);
        static IcConfigManager()
        {
            if (!Directory.Exists(_directory))
            {
                Directory.CreateDirectory(_directory);
            }
        }
        public static T[] GetIcMetas<T>(string filter = null) where T : IcEntryBase, new()
        {
            string pattern = filter is null ? $"*.{IcSuffix}" : $"*{filter}*.{IcSuffix}";

            return Directory.EnumerateFiles(_directory, pattern)
                .Select(path => new T()
                {
                    Name = Path.GetFileNameWithoutExtension(path),
                    CreateTime = File.GetCreationTime(path),
                    UpdateTime = File.GetLastWriteTime(path),
                })
                .OrderBy(ic => ic.CreateTime)
                .ToArray();
        }
        public static Ic GetIc(string name)
        {
            return IcParser.Parse(GetIcPath(name));
        }
        public static void SaveIc(Ic ic)
        {
            File.WriteAllText(GetIcPath(ic), ic.Serialize(), _encoding);
        }
        public static Task SaveIcAsync(Ic ic)
        {
            return File.WriteAllTextAsync(GetIcPath(ic), ic.Serialize(), _encoding);
        }
        public static Ic SaveAsIc(Ic ic, string name)
        {
            File.WriteAllText(GetIcPath(name), ic.Serialize(), _encoding);
            return GetIc(name);
        }
        public static void ExportIc(Ic ic, string path)
        {
            File.WriteAllText(path, ic.Serialize(), _encoding);
        }
        public static async Task<Ic> SaveAsIcAsync(Ic ic, string name)
        {
            await File.WriteAllTextAsync(GetIcPath(name), ic.Serialize(), _encoding);
            return GetIc(name);
        }
        public static void RenameIc(Ic ic, string name)
        {
            File.WriteAllText(GetIcPath(name), ic.Serialize(), _encoding);
            DeleteIc(ic);
        }
        public static async Task RenameIcAsync(Ic ic, string name)
        {
            await File.WriteAllTextAsync(GetIcPath(name), ic.Serialize(), _encoding);
            await DeleteIcAsync(ic);
        }
        public static bool DeleteIc(Ic ic)
        {
            string path = GetIcPath(ic);

            for (int i = 0; i < DeleteRetryCount; i++)
            {
                TryDelete(path);
                if (!File.Exists(path))
                {
                    return true;
                }

                Thread.Sleep(DeleteRetryDelayMs);
            }

            return false;
        }
        public static async Task<bool> DeleteIcAsync(Ic ic)
        {
            string path = GetIcPath(ic);

            for (int i = 0; i < DeleteRetryCount; i++)
            {
                TryDelete(path);
                if (!File.Exists(path))
                {
                    return true;
                }

                await Task.Delay(DeleteRetryDelayMs);
            }

            return false;
        }

        #region 辅助方法
        public static string GetIcPath(Ic ic)
        {
            return GetIcPath(ic.Name);
        }
        public static string GetIcPath(string name)
        {
            return Path.Combine(_directory, $"{name}.{IcSuffix}");
        }
        public static string GetIcFileName(Ic ic)
        {
            return $"{ic.Name}.{IcSuffix}";
        }
        private static void TryDelete(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (IOException)
            {

            }
            catch (UnauthorizedAccessException)
            {

            }
        }
        #endregion
    }
}
