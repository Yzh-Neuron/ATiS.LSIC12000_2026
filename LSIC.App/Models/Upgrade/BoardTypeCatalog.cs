namespace LSIC.App.Models
{
    public class BoardTypeCatalog
    {
        public static void Initialize(IEnumerable<BoardTypeInfo> infos)
        {
            All = infos?.ToArray() ?? [];
            ByCode = All.ToDictionary(i => (byte)i.TypeCode);
        }
        public static IReadOnlyList<BoardTypeInfo> All { get; private set; }
        public static IReadOnlyDictionary<byte, BoardTypeInfo> ByCode { get; private set; }
        public static BoardTypeInfo[] ForNode(string nodeKind)
        {
            return All.Where(i => i.NodeKind == nodeKind).ToArray();
        }
    }
}
