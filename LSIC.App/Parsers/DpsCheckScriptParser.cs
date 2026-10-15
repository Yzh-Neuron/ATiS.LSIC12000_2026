namespace LSIC.App
{
    /// <summary>
    /// 电源自检脚本解析器（.dps）。
    /// 语法（12000 无槽位）：
    /// DPS(电源编号, 设置电压V, 电压下限V, 电压上限V, 电流上限A, 上升时间ms, 采样次数, 采样间隔ms)
    /// '#' 开头为注释行。
    /// </summary>
    public static class DpsCheckScriptParser
    {
        /// <summary>解析脚本文件，返回按顺序执行的电源命令；语法错误抛 <see cref="CharReaderException"/>。</summary>
        public static DpsCommand[] Parse(string file)
        {
            var commands = new List<DpsCommand>();

            using CharReader reader = new CharReader(file);
            reader.SkipWhiteSpace();

            while (!reader.End)
            {
                if (reader.Try('#'))
                {
                    reader.ReadStringUntil('\n');
                }
                else
                {
                    reader.ReadExpected("DPS");
                    reader.SkipWhiteSpace();
                    reader.ReadExpected('(');
                    reader.SkipWhiteSpace();

                    int id = reader.ReadNumber<int>();
                    int vol = ReadElectrical(reader);
                    int uvp = ReadElectrical(reader);
                    int ovp = ReadElectrical(reader);
                    int ocp = ReadElectrical(reader);
                    int raise = ReadInteger(reader);
                    int times = ReadInteger(reader);
                    if (times == 0)
                    {
                        throw new CharReaderException(reader);
                    }
                    int interval = ReadInteger(reader);

                    reader.SkipWhiteSpace();
                    reader.ReadExpected(')');

                    commands.Add(new DpsCommand(reader.Line, id, vol, uvp, ovp, ocp, raise, times, interval));
                }

                reader.SkipWhiteSpace();
            }

            return commands.ToArray();
        }

        /// <summary>
        /// 读取逗号分隔的整型参数。
        /// </summary>
        private static int ReadInteger(CharReader reader)
        {
            reader.SkipWhiteSpace();
            reader.ReadExpected(',');
            reader.SkipWhiteSpace();
            return reader.ReadNumber<int>();
        }

        /// <summary>
        /// 读取逗号分隔的电压/电流浮点参数（V/A），转换为 mV/mA。
        /// </summary>
        private static int ReadElectrical(CharReader reader)
        {
            reader.SkipWhiteSpace();
            reader.ReadExpected(',');
            reader.SkipWhiteSpace();
            float value = reader.ReadNumber<float>();
            return decimal.ToInt32(decimal.Multiply(new decimal(value), 1000m));
        }
    }
}
