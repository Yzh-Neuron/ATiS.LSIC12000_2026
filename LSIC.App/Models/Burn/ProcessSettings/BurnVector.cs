namespace LSIC.App.Models
{
    /// <summary>
    /// 向量（运行时）
    /// </summary>
    public class BurnVector
    {
        public string PatternFile { get; }
        public string RegisterFile { get; }
        public byte PatternIndex { get; }
        public byte RegisterIndex { get; }
        public BurnVector(Vector vector, Dictionary<string, byte> patternMap, Dictionary<string, byte> registerMap)
        {
            PatternFile = vector.PatternFile;
            RegisterFile = vector.RegisterFile;
            PatternIndex = IndexOf(patternMap, vector.PatternFile);
            RegisterIndex = IndexOf(registerMap, vector.RegisterFile);
        }
        private static byte IndexOf(Dictionary<string, byte> map, string path)
        {
            if (!map.TryGetValue(path, out byte index))
            {
                map[path] = index = (byte)map.Count;
            }
            return index;
        }
    }
}