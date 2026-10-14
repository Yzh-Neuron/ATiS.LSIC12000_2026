namespace LSIC.App
{
    public static class PatternFileUtility
    {
        public static int GetCommandCount(string file, int pinCount = 64)
        {
            // 单条向量记录的固定字节长度
            int recordSize = 40 + (pinCount / 2);
            // 第一个板子第一条向量的起始偏移量
            long firstVectorOffset = 80;

            using FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read);

            // 文件基本校验
            if (stream.Length < firstVectorOffset + 32)
            {
                return 0;
            }
            byte[] buffer = new byte[8];

            // 定义二分查找的上下界
            long low = 0;
            long high = (stream.Length - firstVectorOffset) / recordSize; // 粗略的上限估算

            while (low <= high)
            {
                long mid = low + (high - low) / 2;
                long pos = firstVectorOffset + mid * recordSize;
                // 防越界
                if (pos + 8 > stream.Length)
                {
                    high = mid - 1;
                    continue;
                }
                // 直接跳转到可能的起始位置
                stream.Position = pos;
                stream.Read(buffer, 0, 8);
                // 将读取的 8 字节转换为 ASCII 字符串
                string magic = Encoding.ASCII.GetString(buffer);
                if (magic == "*SYMBOLS")
                {
                    // 命中标记
                    return (int)mid;
                }
                if (magic == "VECTOR\0\0")
                {
                    // 读到了向量头，说明还没到结尾，往后半区找
                    low = mid + 1;
                }
                else
                {
                    // 读到了不是约定的标识（可能跳进了 Label 区域，或者下一个 Board 区域），需要往前找
                    high = mid - 1;
                }
            }
            // 找不到符合的数据结构格式，返回 -1 或 0
            return -1;
        }
    }
}
