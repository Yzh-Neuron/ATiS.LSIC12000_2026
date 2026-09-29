namespace LSIC.App
{
    /// <summary>
    /// IC 器件解析器：读取(Parse)与序列化(Serialize)。
    /// 块名一律使用 <see langword="nameof"/>，文件编码 gb2312。
    /// </summary>
    public static class IcParser
    {
        #region Parse / Serialize

        /// <summary>
        /// 读取 IC 文件并构建 <see cref="Ic"/>。
        /// </summary>
        public static Ic Parse(string path)
        {
            using CharReader reader = new CharReader(path, Encoding.GetEncoding("gb2312"));

            Ic ic = new Ic(Path.GetFileNameWithoutExtension(path));

            // 顶层基础字段：直接平铺，不包裹
            ic.LogInterval = ReadTimeField(reader, nameof(ic.LogInterval));
            ic.ErrorLogEnable = ReadBoolField(reader, nameof(ic.ErrorLogEnable));
            ic.ErrorLogInterval = ReadTimeField(reader, nameof(ic.ErrorLogInterval));
            ic.BurnInBoardFile = ReadStringField(reader, nameof(ic.BurnInBoardFile));

            // 恒温槽设置（可选块，存在则覆盖从老化板同步的值）
            ReadThermostatSettings(reader, ic);

            ReadDpsPlans(reader, ic);
            ReadProcess(reader, ic);

            reader.SkipWhiteSpace();
            reader.ReadEnd();
            return ic;
        }

        /// <summary>
        /// 异步读取 IC 文件。
        /// </summary>
        public static Task<Ic> ParseAsync(string path) => Task.FromResult(Parse(path));

        /// <summary>
        /// 将 <see cref="Ic"/> 序列化为文本。
        /// </summary>
        public static string Serialize(this Ic ic)
        {
            StringBuilder sb = new StringBuilder();

            // 顶层基础字段：直接平铺，不包裹
            AppendTimeField(sb, 0, nameof(ic.LogInterval), ic.LogInterval);
            AppendBoolField(sb, 0, nameof(ic.ErrorLogEnable), ic.ErrorLogEnable);
            AppendTimeField(sb, 0, nameof(ic.ErrorLogInterval), ic.ErrorLogInterval);
            AppendStringField(sb, 0, nameof(ic.BurnInBoardFile), ic.BurnInBoardFile);

            WriteThermostatSettings(sb, ic);
            WriteDpsPlans(sb, ic);
            WriteProcess(sb, ic);

            return sb.ToString();
        }

        #endregion

        #region 恒温槽设置

        private static void ReadThermostatSettings(CharReader reader, Ic ic)
        {
            // 该块可选：文件中不存在时，保留从老化板(BurnInBoardSetting)同步的值。
            reader.SkipWhiteSpace();
            if (!reader.Try(nameof(ic.ThermostatSettings)))
            {
                return;
            }

            reader.ReadExpected(nameof(ic.ThermostatSettings));
            reader.SkipWhiteSpace();
            reader.ReadExpected('{');

            List<ThermostatSetting> settings = new List<ThermostatSetting>();
            reader.SkipWhiteSpace();
            while (!reader.Try('}'))
            {
                settings.Add(ReadThermostatSetting(reader));
                reader.SkipWhiteSpace();
            }
            reader.ReadExpected('}');

            ic.ThermostatSettings = settings.ToArray();
        }

        private static ThermostatSetting ReadThermostatSetting(CharReader reader)
        {
            reader.SkipWhiteSpace();
            reader.ReadExpected(nameof(ThermostatSetting));
            reader.ReadWhiteSpace();

            // 块头带 Id：ThermostatSetting 0 {
            ThermostatSetting setting = new ThermostatSetting(reader.ReadNumber<int>());
            reader.SkipWhiteSpace();
            reader.ReadExpected('{');

            setting.Enable = ReadBoolField(reader, nameof(setting.Enable));
            setting.TargetTemperature = ReadIntField(reader, nameof(setting.TargetTemperature));

            reader.SkipWhiteSpace();
            reader.ReadExpected('}');
            return setting;
        }

        private static void WriteThermostatSettings(StringBuilder sb, Ic ic)
        {
            if (ic.ThermostatSettings is not { Length: > 0 })
            {
                return;
            }

            sb.AppendLine($"{nameof(ic.ThermostatSettings)} {{");
            foreach (ThermostatSetting setting in ic.ThermostatSettings)
            {
                sb.AppendLine($"\t{nameof(ThermostatSetting)} {setting.Id} {{");
                AppendBoolField(sb, 2, nameof(setting.Enable), setting.Enable);
                AppendIntField(sb, 2, nameof(setting.TargetTemperature), setting.TargetTemperature);
                sb.AppendLine("\t}");
            }
            sb.AppendLine("}");
        }

        #endregion

        #region 电源设置

        private static void ReadDpsPlans(CharReader reader, Ic ic)
        {
            ReadBlockOpen(reader, nameof(ic.DpsPlans));

            reader.SkipWhiteSpace();
            while (!reader.Try('}'))
            {
                ic.DpsPlans.Add(ReadDpsPlan(reader));
                reader.SkipWhiteSpace();
            }
            reader.ReadExpected('}');
        }

        private static DpsPlan ReadDpsPlan(CharReader reader)
        {
            reader.SkipWhiteSpace();
            reader.ReadExpected(nameof(DpsPlan));
            reader.ReadWhiteSpace();

            DpsPlan plan = new DpsPlan(reader.ReadStringWrap('"'));
            reader.SkipWhiteSpace();
            reader.ReadExpected('{');

            ReadBlockOpen(reader, nameof(plan.DpssSettings));

            reader.SkipWhiteSpace();
            while (!reader.Try('}'))
            {
                plan.DpssSettings.Add(ReadDpsSetting(reader));
                reader.SkipWhiteSpace();
            }
            reader.ReadExpected('}');

            ReadBlockClose(reader);
            return plan;
        }

        private static DpsSetting ReadDpsSetting(CharReader reader)
        {
            ReadBlockOpen(reader, nameof(DpsSetting));

            int id = ReadIntField(reader, nameof(DpsSetting.Id));
            DpsInfo dpsInfo = Device.Instance.SystemConfig.DpsInfos.FirstOrDefault(i => i.Id == id)
                ?? throw new InvalidOperationException($"找不到 Id 为 {id} 的电源规格(DpsInfo)。");

            DpsSetting setting = new DpsSetting(dpsInfo)
            {
                Vol = ReadIntField(reader, nameof(DpsSetting.Vol)),
                Uvp = ReadIntField(reader, nameof(DpsSetting.Uvp)),
                Ovp = ReadIntField(reader, nameof(DpsSetting.Ovp)),
                Ocp = ReadIntField(reader, nameof(DpsSetting.Ocp)),
                Raise = ReadIntField(reader, nameof(DpsSetting.Raise)),
                Delay = ReadIntField(reader, nameof(DpsSetting.Delay)),
            };

            ReadBlockClose(reader);
            return setting;
        }

        private static void WriteDpsPlans(StringBuilder sb, Ic ic)
        {
            sb.AppendLine($"{nameof(ic.DpsPlans)} {{");
            foreach (DpsPlan plan in ic.DpsPlans)
            {
                sb.AppendLine($"\t{nameof(DpsPlan)} \"{plan.Name}\" {{");
                sb.AppendLine($"\t\t{nameof(plan.DpssSettings)} {{");
                foreach (DpsSetting setting in plan.DpssSettings)
                {
                    sb.AppendLine($"\t\t\t{nameof(DpsSetting)} {{");
                    AppendIntField(sb, 4, nameof(DpsSetting.Id), setting.Id);
                    AppendIntField(sb, 4, nameof(DpsSetting.Vol), setting.Vol);
                    AppendIntField(sb, 4, nameof(DpsSetting.Uvp), setting.Uvp);
                    AppendIntField(sb, 4, nameof(DpsSetting.Ovp), setting.Ovp);
                    AppendIntField(sb, 4, nameof(DpsSetting.Ocp), setting.Ocp);
                    AppendIntField(sb, 4, nameof(DpsSetting.Raise), setting.Raise);
                    AppendIntField(sb, 4, nameof(DpsSetting.Delay), setting.Delay);
                    sb.AppendLine("\t\t\t}");
                }
                sb.AppendLine("\t\t}");
                sb.AppendLine("\t}");
            }
            sb.AppendLine("}");
        }

        #endregion

        #region 流程设置

        private static void ReadProcess(CharReader reader, Ic ic)
        {
            ReadBlockOpen(reader, nameof(ic.Process));

            Models.Process process = ic.Process;
            process.StepItems.Clear();
            process.Loops.Clear();

            // 步骤
            ReadBlockOpen(reader, nameof(process.StepItems));
            reader.SkipWhiteSpace();
            while (!reader.Try('}'))
            {
                process.StepItems.Add(ReadStepItem(reader));
                reader.SkipWhiteSpace();
            }
            reader.ReadExpected('}');

            // 循环
            ReadBlockOpen(reader, nameof(process.Loops));
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
            if (reader.Try(nameof(StepVector)))
            {
                return ReadStepVector(reader);
            }
            if (reader.Try(nameof(StepPower)))
            {
                return ReadStepPower(reader);
            }
            throw new InvalidOperationException("无法识别的流程步骤类型。");
        }

        private static StepVector ReadStepVector(CharReader reader)
        {
            reader.SkipWhiteSpace();
            reader.ReadExpected(nameof(StepVector));
            reader.ReadWhiteSpace();

            StepVector step = new StepVector(reader.ReadStringWrap('"'));
            reader.SkipWhiteSpace();
            reader.ReadExpected('{');

            step.ExecuteStrategy.Copy(ReadExecuteStrategy(reader));
            step.DpsPlan = ReadStringField(reader, nameof(step.DpsPlan));

            ReadBlockOpen(reader, nameof(step.Vectors));
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
            reader.ReadExpected(nameof(StepPower));
            reader.ReadWhiteSpace();

            StepPower step = new StepPower(reader.ReadEnum<StepItemType>());
            reader.SkipWhiteSpace();
            reader.ReadExpected('{');

            step.Timeout = ReadIntField(reader, nameof(step.Timeout));

            ReadBlockClose(reader);
            return step;
        }

        private static Vector ReadVector(CharReader reader)
        {
            ReadBlockOpen(reader, nameof(Vector));

            Vector vector = new Vector
            {
                PatternFile = ReadStringField(reader, nameof(Vector.PatternFile)),
                RegisterFile = ReadStringField(reader, nameof(Vector.RegisterFile)),
            };

            ReadBlockClose(reader);
            return vector;
        }

        private static Loop ReadLoop(CharReader reader, Models.Process process)
        {
            reader.SkipWhiteSpace();
            reader.ReadExpected(nameof(Loop));
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
            ReadBlockOpen(reader, nameof(ExecuteStrategy));

            ExecuteStrategy strategy = new ExecuteStrategy
            {
                Mode = ReadEnumField<ExecuteMode>(reader, nameof(ExecuteStrategy.Mode)),
                Time = ReadTimeField(reader, nameof(ExecuteStrategy.Time)),
                Count = ReadIntField(reader, nameof(ExecuteStrategy.Count)),
            };

            ReadBlockClose(reader);
            return strategy;
        }

        private static void WriteProcess(StringBuilder sb, Ic ic)
        {
            Models.Process process = ic.Process;
            sb.AppendLine($"{nameof(ic.Process)} {{");

            // 步骤
            sb.AppendLine($"\t{nameof(process.StepItems)} {{");
            foreach (StepItem item in process.StepItems)
            {
                WriteStepItem(sb, item);
            }
            sb.AppendLine("\t}");

            // 循环
            sb.AppendLine($"\t{nameof(process.Loops)} {{");
            foreach (Loop loop in process.Loops)
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
                    sb.AppendLine($"\t\t{nameof(StepVector)} \"{step.Name}\" {{");
                    WriteExecuteStrategy(sb, 3, step.ExecuteStrategy);
                    AppendStringField(sb, 3, nameof(step.DpsPlan), step.DpsPlan);

                    sb.AppendLine($"\t\t\t{nameof(step.Vectors)} {{");
                    foreach (Vector vector in step.Vectors)
                    {
                        sb.AppendLine($"\t\t\t\t{nameof(Vector)} {{");
                        AppendStringField(sb, 5, nameof(Vector.PatternFile), vector.PatternFile);
                        AppendStringField(sb, 5, nameof(Vector.RegisterFile), vector.RegisterFile);
                        sb.AppendLine("\t\t\t\t}");
                    }
                    sb.AppendLine("\t\t\t}");

                    sb.AppendLine("\t\t}");
                    break;

                case StepPower step:
                    sb.AppendLine($"\t\t{nameof(StepPower)} {step.ItemType} {{");
                    AppendIntField(sb, 3, nameof(step.Timeout), step.Timeout);
                    sb.AppendLine("\t\t}");
                    break;
            }
        }

        private static void WriteLoop(StringBuilder sb, Loop loop)
        {
            sb.AppendLine($"\t\t{nameof(Loop)} \"{loop.Start.Name}\" \"{loop.End.Name}\" {{");
            WriteExecuteStrategy(sb, 3, loop.ExecuteStrategy);
            sb.AppendLine("\t\t}");
        }

        private static void WriteExecuteStrategy(StringBuilder sb, int indent, ExecuteStrategy strategy)
        {
            sb.AppendLine($"{Indent(indent)}{nameof(ExecuteStrategy)} {{");
            AppendEnumField(sb, indent + 1, nameof(strategy.Mode), strategy.Mode);
            AppendTimeField(sb, indent + 1, nameof(strategy.Time), strategy.Time);
            AppendIntField(sb, indent + 1, nameof(strategy.Count), strategy.Count);
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
