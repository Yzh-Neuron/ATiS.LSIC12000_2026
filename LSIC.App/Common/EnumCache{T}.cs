using System.Runtime.CompilerServices;

namespace LSIC.App
{
    public static class EnumCache<T> where T : struct, Enum
    {
        public static readonly Func<byte, T> ToEnum;
        public static readonly HashSet<byte> Defined = new();
        static EnumCache()
        {
            foreach (var v in Enum.GetValues<T>())
            {
                Defined.Add(Convert.ToByte(v));
            }
            ToEnum = b => Unsafe.As<byte, T>(ref b);
        }
    }
}
