namespace LSIC.App
{
    public static class LangUtility
    {
        public static string GetString<T>(this T value) where T : Enum
        {
            string obj = $"{"Enum"}_{value.GetType().Name}_{value}";
            return LangProvider.Instance.GetString(obj);
        }
    }
}
