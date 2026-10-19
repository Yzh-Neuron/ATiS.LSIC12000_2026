using System.Runtime.InteropServices;

namespace LSIC.App
{
    class AssemblyUtility
    {
        public static string GetTitle()
        {
            return Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyProductAttribute>()?.Product;
        }

        public static string GetVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version?.ToString();
        }

        public static string GetGuid()
        {
            return Assembly.GetExecutingAssembly().GetCustomAttribute<GuidAttribute>()?.Value;
        }
    }
}
