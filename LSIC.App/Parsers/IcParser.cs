using System.IO;
using System.Text;
using ATiS.Base;
using LSIC.App.Models;

namespace LSIC.App
{
    /// <summary>
    /// IC 器件解析器：读取(Parse)与序列化(Serialize)。
    /// 采用块语法，文件编码 gb2312。
    /// </summary>
    public static class IcParser
    {
        #region 块名

        private const string BaseSettingBlock = "BaseSetting";
        private const string PowerSettingBlock = "PowerSetting";
        private const string ProcessSettingBlock = "ProcessSetting";

        private const string DpsPlansBlock = "DpsPlans";
        private const string DpsPlanBlock = "DpsPlan";
        private const string DpssBlock = "Dpss";
        private const string DpsBlock = "Dps";

        private const string StepsBlock = "Steps";
        private const string StepVectorBlock = "StepVector";
        private const string StepPowerBlock = "StepPower";
        private const string VectorsBlock = "Vectors";
        private const string VectorBlock = "Vector";
        private const string LoopsBlock = "Loops";
        private const string LoopBlock = "Loop";
        private const string ExecuteStrategyBlock = "ExecuteStrategy";

        #endregion

        #region Parse

        /// <summary>
        /// 读取 IC 文件并构建 <see cref="Ic"/>。
        /// </summary>
        public static Ic Parse(string path)
        {
            using CharReader reader = new CharReader(path, Encoding.GetEncoding("gb2312"));

            Ic ic = new Ic(Path.GetFileNameWithoutExtension(path));

            ReadBaseSetting(reader, ic);
            ReadPowerSetting(reader, ic);
            ReadProcessSetting(reader, ic);

            reader.SkipWhiteSpace();
            reader.ReadEnd();
            return ic;
        }

        /// <summary>
        /// 异步读取 IC 文件。
        /// </summary>
        public static Task<Ic> ParseAsync(string path) => Task.FromResult(Parse(path));

        #endregion

        #region Serialize

        /// <summary>
        /// 将 <see cref="Ic"/> 序列化为文本。
        /// </summary>
        public static string Serialize(this Ic ic)
        {
            StringBuilder sb = new StringBuilder();

            WriteBaseSetting(sb, ic);
            WritePowerSetting(sb, ic);
            WriteProcessSetting(sb, ic);

            return sb.ToString();
        }

        #endregion

        #region 基础设置

        private static void ReadBaseSetting(CharReader reader, Ic ic)
        {
            ReadBlockOpen(reader, BaseSettingBlock);

            ic.LogInterval = ReadTimeField(reader, nameof(ic.LogInterval));

            ic.ErrorLogEnable = ReadBoolField(reader, nameof(ic.ErrorLogEnable));

            ic.ErrorLogInterval = ReadTimeField(reader, nameof(ic.ErrorLogInterval));

            ic.BurnInBoardFile = ReadStringField(reader, nameof(ic.BurnInBoardFile));

            ReadBlockClose(reader);
        }

        private static void WriteBaseSetting(StringBuilder sb, Ic ic)
        {
            sb.AppendLine($"{BaseSettingBlock} {{");
            AppendTimeField(sb, 1, nameof(ic.LogInterval), ic.LogInterval);
            AppendBoolField(sb, 1, nameof(ic.ErrorLogEnable), ic.ErrorLogEnable);
            AppendTimeField(sb, 1, nameof(ic.ErrorLogInterval), ic.ErrorLogInterval);
            AppendStringField(sb, 1, nameof(ic.BurnInBoardFile), ic.BurnInBoardFile);
            sb.AppendLine("}");
        }

        #endregion

        #region 电源设置

        private static void ReadPowerSetting(CharReader reader, Ic ic)
        {
            ReadBlockOpen(reader, PowerSettingBlock);

            ReadBlockOpen(reader, DpsPlansBlock);
            reader.SkipWhiteSpace();
            while (!reader.Try('}'))
            {
                ic.DpsPlans.Add(ReadDpsPlan(reader));
                reader.SkipWhiteSpace();
            }
            reader.ReadExpected('}');

            ReadBlockClose(reader);
        }

        private static DpsPlan ReadDpsPlan(CharReader reader)
        {
            reader.SkipWhiteSpace();
            reader.ReadExpected(DpsPlanBlock);
            reader.ReadWhiteSpace();

            DpsPlan dpsPlan = new DpsPlan(reader.ReadStringWrap('"'));
            reader.SkipWhiteSpace();
            reader.ReadExpected('{');

            ReadBlockOpen(reader, DpssBlock);
            reader.SkipWhiteSpace();
            while (!reader.Try('}'))
            {
                dpsPlan.DpssSettings.Add(ReadDpsSetting(reader));
                reader.SkipWhiteSpace();
            }
            reader.ReadExpected('}');

            ReadBlockClose(reader);
            return dpsPlan;
        }

        private static DpsSetting ReadDpsSetting(CharReader reader)
        {
            ReadBlockOpen(reader, DpsBlock);

            int id = ReadIntField(reader, nameof(DpsSetting.Id));
            DpsInfo dpsInfo = Device.Instance.SystemConfig.DpsInfos.FirstOrDefault(i => i.Id == id)
                ?? throw new InvalidOperationException($"找不到 Id 为 {id} 的电源规格(DpsInfo)。");

            DpsSetting dpsSetting = new DpsSetting(dpsInfo)
            {
                Vol = ReadIntField(reader, nameof(DpsSetting.Vol)),
                Uvp = ReadIntField(reader, nameof(DpsSetting.Uvp)),
                Ovp = ReadIntField(reader, nameof(DpsSetting.Ovp)),
                Ocp = ReadIntField(reader, nameof(DpsSetting.Ocp)),
                Raise = ReadIntField(reader, nameof(DpsSetting.Raise)),
                Delay = ReadIntField(reader, nameof(DpsSetting.Delay)),
            };

            ReadBlockClose(reader);
            return dpsSetting;
        }

        private static void WritePowerSetting(StringBuilder sb, Ic ic)
        {
            sb.AppendLine($"{PowerSettingBlock} {{");

            sb.AppendLine($"\t{DpsPlansBlock} {{");
            foreach (DpsPlan dpsPlan in ic.DpsPlans)
            {
                sb.AppendLine($"\t\t{DpsPlanBlock} \"{dpsPlan.Name}\" {{");
                sb.AppendLine($"\t\t\t{DpssBlock} {{");
                foreach (DpsSetting dps in dpsPlan.DpssSettings)
                {
                    sb.AppendLine($"\t\t\t\t{DpsBlock} {{");
                    AppendIntField(sb, 5, nameof(DpsSetting.Id), dps.Id);
                    AppendIntField(sb, 5, nameof(DpsSetting.Vol), dps.Vol);
                    AppendIntField(sb, 5, nameof(DpsSetting.Uvp), dps.Uvp);
                    AppendIntField(sb, 5, nameof(DpsSetting.Ovp), dps.Ovp);
                    AppendIntField(sb, 5, nameof(DpsSetting.Ocp), dps.Ocp);
                    AppendIntField(sb, 5, nameof(DpsSetting.Raise), dps.Raise);
                    AppendIntField(sb, 5, nameof(DpsSetting.Delay), dps.Delay);
                    sb.AppendLine("\t\t\t\t}");
                }
                sb.AppendLine("\t\t\t}");
                sb.AppendLine("\t\t}");
            }
            sb.AppendLine("\t}");

            sb.AppendLine("}");
        }

        #endregion

        #region 流程设置

        private static void ReadProcessSetting(CharReader reader, Ic ic)
        {
            ReadBlockOpen(reader, ProcessSettingBlock);

            Models.Process process = ic.Process;
            process.StepItems.Clear();
            process.Loops.Clear();

            // 步骤
            ReadBlockOpen(reader, StepsBlock);
            reader.SkipWhiteSpace();
            while (!reader.Try('}'))
            {
                process.StepItems.Add(ReadStepItem(reader));
                reader.SkipWhiteSpace();
            }
            reader.ReadExpected('}');

            // 循环
            ReadBlockOpen(reader, LoopsBlock);
            reader.SkipWhiteSpace();
            while (!reader.Try('}'))
            {
                process.Loops.Add(ReadLoop(reader, process));
                reader.SkipWhiteSpace();
            }
            reader.ReadExpected('}');

            ReadBlockClose(reader);

            process.Refresh();
        }

        private static StepItem ReadStepItem(CharReader reader)
        {
            reader.SkipWhiteSpace();
            if (reader.Try(StepVectorBlock))
            {
                return ReadStepVector(reader);
            }
            if (reader.Try(StepPowerBlock))
            {
                return ReadStepPower(reader);
            }
            throw new InvalidOperationException("无法识别的流程步骤类型。");
        }

        private static StepVector ReadStepVector(CharReader reader)
        {
            reader.SkipWhiteSpace();
            reader.ReadExpected(StepVectorBlock);
            reader.ReadWhiteSpace();

            StepVector step = new StepVector(reader.ReadStringWrap('"'));
            reader.SkipWhiteSpace();
            reader.ReadExpected('{');

            // 执行策略
            step.ExecuteStrategy.Copy(ReadExecuteStrategy(reader));

            // 电源方案
            step.DpsPlan = ReadStringField(reader, nameof(StepItem.DpsPlan));

            // 向量
            ReadBlockOpen(reader, VectorsBlock);
            reader.SkipWhiteSpace();
            while (!reader.Try('}'))
            {
                step.Vectors.Add(ReadVector(reader));
                reader.SkipWhiteSpace();
            }
            reader.ReadExpected('}');

            ReadBlockClose(reader);
            return step;
        }

        private static StepPower ReadStepPower(CharReader reader)
        {
            reader.SkipWhiteSpace();
            reader.ReadExpected(StepPowerBlock);
            reader.ReadWhiteSpace();

            StepPower step = new StepPower(reader.ReadEnum<StepItemType>());
            reader.SkipWhiteSpace();
            reader.ReadExpected('{');

            step.Timeout = ReadIntField(reader, nameof(StepPower.Timeout));

            ReadBlockClose(reader);
            return step;
        }

        private static Vector ReadVector(CharReader reader)
        {
            ReadBlockOpen(reader, VectorBlock);

            string patternFile = ReadStringField(reader, nameof(Vector.PatternFile));
            string registerFile = ReadStringField(reader, nameof(Vector.RegisterFile));

            ReadBlockClose(reader);

            return new Vector
            {
                PatternFile = patternFile,
                RegisterFile = registerFile,
            };
        }

        private static Loop ReadLoop(CharReader reader, Models.Process process)
        {
            reader.SkipWhiteSpace();
            reader.ReadExpected(LoopBlock);
            reader.ReadWhiteSpace();

            string startName = reader.ReadStringWrap('"');
            reader.ReadWhiteSpace();
            string endName = reader.ReadStringWrap('"');

            StepItem start = process.StepItems.FirstOrDefault(i => i.Name == startName)
                ?? throw new InvalidOperationException($"循环起始步骤 \"{startName}\" 不存在。");
            StepItem end = process.StepItems.FirstOrDefault(i => i.Name == endName)
                ?? throw new InvalidOperationException($"循环结束步骤 \"{endName}\" 不存在。");

            Loop loop = new Loop(start) { End = end };
            reader.SkipWhiteSpace();
            reader.ReadExpected('{');

            loop.ExecuteStrategy.Copy(ReadExecuteStrategy(reader));

            ReadBlockClose(reader);
            return loop;
        }

        private static ExecuteStrategy ReadExecuteStrategy(CharReader reader)
        {
            ReadBlockOpen(reader, ExecuteStrategyBlock);

            ExecuteStrategy strategy = new ExecuteStrategy
            {
                Mode = ReadEnumField<ExecuteMode>(reader, nameof(ExecuteStrategy.Mode)),
                Count = ReadIntField(reader, nameof(ExecuteStrategy.Count)),
            };

            // Time：值 + 单位
            reader.SkipWhiteSpace();
            reader.ReadExpected(nameof(ExecuteStrategy.Time));
            reader.ReadWhiteSpace();
            int value = reader.ReadNumber<int>();
            reader.ReadWhiteSpace();
            TimeUnit unit = reader.ReadEnum<TimeUnit>();
            reader.SkipWhiteSpace();
            reader.ReadExpected(';');
            strategy.Time = new Time(value, unit);

            ReadBlockClose(reader);
            return strategy;
        }

        private static void WriteProcessSetting(StringBuilder sb, Ic ic)
        {
            sb.AppendLine($"{ProcessSettingBlock} {{");

            // 步骤
            sb.AppendLine($"\t{StepsBlock} {{");
            foreach (StepItem item in ic.Process.StepItems)
            {
                WriteStepItem(sb, item);
            }
            sb.AppendLine("\t}");

            // 循环
            sb.AppendLine($"\t{LoopsBlock} {{");
            foreach (Loop loop in ic.Process.Loops)
            {
                WriteLoop(sb, loop);
            }
            sb.AppendLine("\t}");

            sb.AppendLine("}");
        }

        private static void WriteStepItem(StringBuilder sb, StepItem item)
        {
            switch (item)
            {
                case StepVector step:
                    sb.AppendLine($"\t\t{StepVectorBlock} \"{step.Name}\" {{");
                    WriteExecuteStrategy(sb, 3, step.ExecuteStrategy);
                    AppendStringField(sb, 3, nameof(StepItem.DpsPlan), step.DpsPlan);

                    sb.AppendLine($"\t\t\t{VectorsBlock} {{");
                    foreach (Vector vector in step.Vectors)
                    {
                        sb.AppendLine($"\t\t\t\t{VectorBlock} {{");
                        AppendStringField(sb, 5, nameof(Vector.PatternFile), vector.PatternFile);
                        AppendStringField(sb, 5, nameof(Vector.RegisterFile), vector.RegisterFile);
                        sb.AppendLine("\t\t\t\t}");
                    }
                    sb.AppendLine("\t\t\t}");

                    sb.AppendLine("\t\t}");
                    break;

                case StepPower step:
                    sb.AppendLine($"\t\t{StepPowerBlock} {step.ItemType} {{");
                    AppendIntField(sb, 3, nameof(StepPower.Timeout), step.Timeout);
                    sb.AppendLine("\t\t}");
                    break;
            }
        }

        private static void WriteLoop(StringBuilder sb, Loop loop)
        {
            sb.AppendLine($"\t\t{LoopBlock} \"{loop.Start.Name}\" \"{loop.End.Name}\" {{");
            WriteExecuteStrategy(sb, 3, loop.ExecuteStrategy);
            sb.AppendLine("\t\t}");
        }

        private static void WriteExecuteStrategy(StringBuilder sb, int indent, ExecuteStrategy strategy)
        {
            sb.AppendLine($"{Indent(indent)}{ExecuteStrategyBlock} {{");
            AppendEnumField(sb, indent + 1, nameof(ExecuteStrategy.Mode), strategy.Mode);
            AppendLine(sb, indent + 1, $"{nameof(ExecuteStrategy.Time)} {strategy.Time.Value} {strategy.Time.Unit};");
            AppendIntField(sb, indent + 1, nameof(ExecuteStrategy.Count), strategy.Count);
            sb.AppendLine($"{Indent(indent)}}}");
        }

        #endregion

        #region 字段读写辅助

        private static int ReadIntField(CharReader reader, string field)
        {
            reader.SkipWhiteSpace();
            reader.ReadExpected(field);
            reader.ReadWhiteSpace();
            int value = reader.ReadNumber<int>();
            reader.SkipWhiteSpace();
            reader.ReadExpected(';');
            return value;
        }

        private static bool ReadBoolField(CharReader reader, string field)
        {
            reader.SkipWhiteSpace();
            reader.ReadExpected(field);
            reader.ReadWhiteSpace();
            bool value = reader.ReadBool();
            reader.SkipWhiteSpace();
            reader.ReadExpected(';');
            return value;
        }

        private static string ReadStringField(CharReader reader, string field)
        {
            reader.SkipWhiteSpace();
            reader.ReadExpected(field);
            reader.ReadWhiteSpace();
            string value = reader.ReadStringWrap('"');
            reader.SkipWhiteSpace();
            reader.ReadExpected(';');
            return value;
        }

        private static T ReadEnumField<T>(CharReader reader, string field) where T : Enum
        {
            reader.SkipWhiteSpace();
            reader.ReadExpected(field);
            reader.ReadWhiteSpace();
            T value = reader.ReadEnum<T>();
            reader.SkipWhiteSpace();
            reader.ReadExpected(';');
            return value;
        }

        private static Time ReadTimeField(CharReader reader, string field)
        {
            reader.SkipWhiteSpace();
            reader.ReadExpected(field);
            reader.ReadWhiteSpace();
            int value = reader.ReadNumber<int>();
            reader.ReadWhiteSpace();
            TimeUnit unit = reader.ReadEnum<TimeUnit>();
            reader.SkipWhiteSpace();
            reader.ReadExpected(';');
            return new Time(value, unit);
        }

        private static void ReadBlockOpen(CharReader reader, string block)
        {
            reader.SkipWhiteSpace();
            reader.ReadExpected(block);
            reader.SkipWhiteSpace();
            reader.ReadExpected('{');
        }

        private static void ReadBlockClose(CharReader reader)
        {
            reader.SkipWhiteSpace();
            reader.ReadExpected('}');
        }

        private static void AppendIntField(StringBuilder sb, int indent, string field, int value)
        {
            AppendLine(sb, indent, $"{field} {value};");
        }

        private static void AppendBoolField(StringBuilder sb, int indent, string field, bool value)
        {
            AppendLine(sb, indent, $"{field} {value};");
        }

        private static void AppendStringField(StringBuilder sb, int indent, string field, string value)
        {
            AppendLine(sb, indent, $"{field} \"{value}\";");
        }

        private static void AppendEnumField<T>(StringBuilder sb, int indent, string field, T value) where T : Enum
        {
            AppendLine(sb, indent, $"{field} {value};");
        }

        private static void AppendTimeField(StringBuilder sb, int indent, string field, Time time)
        {
            AppendLine(sb, indent, $"{field} {time.Value} {time.Unit};");
        }

        private static void AppendLine(StringBuilder sb, int indent, string content)
        {
            sb.Append(Indent(indent)).AppendLine(content);
        }

        private static string Indent(int indent) => new string('\t', indent);

        #endregion
    }
}
