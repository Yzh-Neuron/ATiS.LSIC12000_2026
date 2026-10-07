namespace LSIC.App
{
    public static class IcParser
    {
        // 反序列化
        public static Ic Parse(string path)
        {
            using CharReader reader = new CharReader(path, Encoding.GetEncoding("gb2312"));
            Ic ic = new Ic(Path.GetFileNameWithoutExtension(path));

            ic.LogInterval = ReadTimeField(reader, nameof(ic.LogInterval));
            ic.ErrorLogEnable = ReadBoolField(reader, nameof(ic.ErrorLogEnable));
            ic.ErrorLogInterval = ReadTimeField(reader, nameof(ic.ErrorLogInterval));
            ic.ForceStepLoad = ReadBoolField(reader, nameof(ic.ForceStepLoad));
            ic.BurnInBoardFile = ReadStringField(reader, nameof(ic.BurnInBoardFile));

            ReadThermostatSettings(reader, ic);
            ReadDpsPlans(reader, ic);
            ReadProcess(reader, ic);

            reader.SkipWhiteSpace();
            reader.ReadEnd();
            return ic;
        }
        private static void ReadThermostatSettings(CharReader reader, Ic ic)
        {
            reader.SkipWhiteSpace();
            if (!reader.Try(nameof(ic.ThermostatSettings)))
            {
                return;
            }

            reader.ReadExpected(nameof(ic.ThermostatSettings));
            reader.ReadWhiteSpace();
            reader.ReadExpected('{');

            reader.SkipWhiteSpace();
            while (!reader.Try('}'))
            {
                reader.ReadExpected(nameof(ThermostatSetting));
                reader.ReadWhiteSpace();
                ThermostatSetting setting = new ThermostatSetting(reader.ReadNumber<int>());
                reader.SkipWhiteSpace();
                reader.ReadExpected('{');

                setting.Enable = ReadBoolField(reader, nameof(setting.Enable));
                setting.TargetTemperature = ReadIntField(reader, nameof(setting.TargetTemperature));

                reader.SkipWhiteSpace();
                reader.ReadExpected('}');

                ic.ThermostatSettings?.FirstOrDefault(t => t.Id == setting.Id)?.Copy(setting);
                reader.SkipWhiteSpace();
            }

            reader.SkipWhiteSpace();
            reader.ReadExpected('}');
        }
        private static void ReadDpsPlans(CharReader reader, Ic ic)
        {
            reader.SkipWhiteSpace();
            reader.ReadExpected(nameof(ic.DpsPlans));
            reader.ReadWhiteSpace();
            reader.ReadExpected('{');

            reader.SkipWhiteSpace();
            while (!reader.Try('}'))
            {
                ic.DpsPlans.Add(ReadDpsPlan(reader));
                reader.SkipWhiteSpace();
            }

            reader.SkipWhiteSpace();
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

            reader.SkipWhiteSpace();
            reader.ReadExpected(nameof(plan.DpssSettings));
            reader.ReadWhiteSpace();
            reader.ReadExpected('{');

            reader.SkipWhiteSpace();
            while (!reader.Try('}'))
            {
                DpsSetting setting = ReadDpsSetting(reader);
                if (setting != null)
                {
                    plan.DpssSettings.Add(setting);
                }
                reader.SkipWhiteSpace();
            }

            reader.SkipWhiteSpace();
            reader.ReadExpected('}');

            reader.SkipWhiteSpace();
            reader.ReadExpected('}');
            return plan;
        }
        private static DpsSetting ReadDpsSetting(CharReader reader)
        {
            reader.SkipWhiteSpace();
            reader.ReadExpected(nameof(DpsSetting));
            reader.ReadWhiteSpace();
            reader.ReadExpected('{');

            int id = ReadIntField(reader, nameof(DpsSetting.Id));
            int vol = ReadIntField(reader, nameof(DpsSetting.Vol));
            int uvp = ReadIntField(reader, nameof(DpsSetting.Uvp));
            int ovp = ReadIntField(reader, nameof(DpsSetting.Ovp));
            int ocp = ReadIntField(reader, nameof(DpsSetting.Ocp));
            int raise = ReadIntField(reader, nameof(DpsSetting.Raise));
            int delay = ReadIntField(reader, nameof(DpsSetting.Delay));

            reader.SkipWhiteSpace();
            reader.ReadExpected('}');

            DpsInfo dpsInfo = Device.Instance.SystemConfig.DpsInfos.FirstOrDefault(i => i.Id == id);
            return dpsInfo == null ? null
                : new DpsSetting(dpsInfo)
                {
                    Vol = vol,
                    Uvp = uvp,
                    Ovp = ovp,
                    Ocp = ocp,
                    Raise = raise,
                    Delay = delay,
                };
        }
        private static void ReadProcess(CharReader reader, Ic ic)
        {
            reader.SkipWhiteSpace();
            reader.ReadExpected(nameof(ic.Process));
            reader.ReadWhiteSpace();
            reader.ReadExpected('{');

            Models.Process process = ic.Process;

            reader.SkipWhiteSpace();
            reader.ReadExpected(nameof(process.StepItems));
            reader.ReadWhiteSpace();
            reader.ReadExpected('{');

            reader.SkipWhiteSpace();
            while (!reader.Try('}'))
            {
                if (reader.Try(nameof(StepVector)))
                {
                    process.AddVector(ReadStepVector(reader));
                }
                else if (reader.Try(nameof(StepPower)))
                {
                    ReadStepPowerInto(reader, process);
                }
                else
                {
                    throw new InvalidOperationException("无法识别的流程步骤类型。");
                }
                reader.SkipWhiteSpace();
            }

            reader.SkipWhiteSpace();
            reader.ReadExpected('}');

            reader.SkipWhiteSpace();
            reader.ReadExpected(nameof(process.Loops));
            reader.ReadWhiteSpace();
            reader.ReadExpected('{');

            reader.SkipWhiteSpace();
            while (!reader.Try('}'))
            {
                process.Loops.Add(ReadLoop(reader, process));
                reader.SkipWhiteSpace();
            }

            reader.SkipWhiteSpace();
            reader.ReadExpected('}');

            reader.SkipWhiteSpace();
            reader.ReadExpected('}');

            process.Refresh();
        }
        private static void ReadStepPowerInto(CharReader reader, Models.Process process)
        {
            reader.ReadExpected(nameof(StepPower));
            reader.ReadWhiteSpace();
            StepItemType type = reader.ReadEnum<StepItemType>();
            reader.SkipWhiteSpace();
            reader.ReadExpected('{');

            StepPower power = process.StepItems.OfType<StepPower>().First(p => p.ItemType == type);
            power.DpsPlan = ReadStringField(reader, nameof(power.DpsPlan));
            power.Timeout = ReadIntField(reader, nameof(power.Timeout));
            ReadVectorsInto(reader, power, onlyFirst: true);

            reader.SkipWhiteSpace();
            reader.ReadExpected('}');
        }
        private static StepVector ReadStepVector(CharReader reader)
        {
            reader.SkipWhiteSpace();
            reader.ReadExpected(nameof(StepVector));
            reader.ReadWhiteSpace();
            StepVector step = new StepVector(reader.ReadStringWrap('"'));
            reader.SkipWhiteSpace();
            reader.ReadExpected('{');

            step.DpsPlan = ReadStringField(reader, nameof(step.DpsPlan));
            step.ExecuteStrategy.Copy(ReadExecuteStrategy(reader));
            ReadVectorsInto(reader, step);

            reader.SkipWhiteSpace();
            reader.ReadExpected('}');
            return step;
        }
        private static void ReadVectorsInto(CharReader reader, StepItem step, bool onlyFirst = false)
        {
            reader.SkipWhiteSpace();
            if (!reader.Try(nameof(step.Vectors)))
            {
                return;
            }

            reader.ReadExpected(nameof(step.Vectors));
            reader.ReadWhiteSpace();
            reader.ReadExpected('{');

            reader.SkipWhiteSpace();
            while (!reader.Try('}'))
            {
                Vector vector = ReadVector(reader);
                if (!onlyFirst || step.Vectors.Count == 0)
                {
                    step.Vectors.Add(vector);
                }
                reader.SkipWhiteSpace();
            }

            reader.SkipWhiteSpace();
            reader.ReadExpected('}');
        }
        private static Vector ReadVector(CharReader reader)
        {
            reader.SkipWhiteSpace();
            reader.ReadExpected(nameof(Vector));
            reader.ReadWhiteSpace();
            reader.ReadExpected('{');

            Vector vector = new Vector
            {
                PatternFile = ReadStringField(reader, nameof(Vector.PatternFile)),
                RegisterFile = ReadStringField(reader, nameof(Vector.RegisterFile)),
            };

            reader.SkipWhiteSpace();
            reader.ReadExpected('}');
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

            reader.SkipWhiteSpace();
            reader.ReadExpected('}');
            return loop;
        }
        private static ExecuteStrategy ReadExecuteStrategy(CharReader reader)
        {
            reader.SkipWhiteSpace();
            reader.ReadExpected(nameof(ExecuteStrategy));
            reader.ReadWhiteSpace();
            reader.ReadExpected('{');

            ExecuteStrategy strategy = new ExecuteStrategy
            {
                Mode = ReadEnumField<ExecuteMode>(reader, nameof(ExecuteStrategy.Mode)),
                Time = ReadTimeField(reader, nameof(ExecuteStrategy.Time)),
                Count = ReadIntField(reader, nameof(ExecuteStrategy.Count)),
            };

            reader.SkipWhiteSpace();
            reader.ReadExpected('}');
            return strategy;
        }
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


        // 序列化
        public static string Serialize(this Ic ic)
        {
            StringBuilder sb = new StringBuilder();
            AppendField(sb, 0, nameof(ic.LogInterval), ic.LogInterval);
            AppendField(sb, 0, nameof(ic.ErrorLogEnable), ic.ErrorLogEnable);
            AppendField(sb, 0, nameof(ic.ErrorLogInterval), ic.ErrorLogInterval);
            AppendField(sb, 0, nameof(ic.ForceStepLoad), ic.ForceStepLoad);
            AppendField(sb, 0, nameof(ic.BurnInBoardFile), ic.BurnInBoardFile);

            WriteThermostatSettings(sb, ic);
            WriteDpsPlans(sb, ic);
            WriteProcess(sb, ic);
            return sb.ToString();
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
                AppendField(sb, 2, nameof(setting.Enable), setting.Enable);
                AppendField(sb, 2, nameof(setting.TargetTemperature), setting.TargetTemperature);
                sb.AppendLine("\t}");
            }
            sb.AppendLine("}");
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
                    AppendField(sb, 4, nameof(setting.Id), setting.Id);
                    AppendField(sb, 4, nameof(setting.Vol), setting.Vol);
                    AppendField(sb, 4, nameof(setting.Uvp), setting.Uvp);
                    AppendField(sb, 4, nameof(setting.Ovp), setting.Ovp);
                    AppendField(sb, 4, nameof(setting.Ocp), setting.Ocp);
                    AppendField(sb, 4, nameof(setting.Raise), setting.Raise);
                    AppendField(sb, 4, nameof(setting.Delay), setting.Delay);
                    sb.AppendLine("\t\t\t}");
                }
                sb.AppendLine("\t\t}");
                sb.AppendLine("\t}");
            }
            sb.AppendLine("}");
        }
        private static void WriteProcess(StringBuilder sb, Ic ic)
        {
            Models.Process process = ic.Process;
            sb.AppendLine($"{nameof(ic.Process)} {{");

            sb.AppendLine($"\t{nameof(process.StepItems)} {{");
            foreach (StepItem item in process.StepItems)
            {
                WriteStepItem(sb, item);
            }
            sb.AppendLine("\t}");

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
                    AppendField(sb, 3, nameof(step.DpsPlan), step.DpsPlan);
                    WriteExecuteStrategy(sb, 3, step.ExecuteStrategy);
                    WriteVectors(sb, 3, step.Vectors);
                    sb.AppendLine("\t\t}");
                    break;

                case StepPower step:
                    sb.AppendLine($"\t\t{nameof(StepPower)} {step.ItemType} {{");
                    AppendField(sb, 3, nameof(step.DpsPlan), step.DpsPlan);
                    AppendField(sb, 3, nameof(step.Timeout), step.Timeout);
                    if (step.Vectors.Count > 0)
                    {
                        WriteVectors(sb, 3, step.Vectors);
                    }
                    sb.AppendLine("\t\t}");
                    break;
            }
        }
        private static void WriteVectors(StringBuilder sb, int indent, IList<Vector> vectors)
        {
            sb.AppendLine($"{Indent(indent)}{nameof(StepItem.Vectors)} {{");
            foreach (Vector vector in vectors)
            {
                sb.AppendLine($"{Indent(indent + 1)}{nameof(Vector)} {{");
                AppendField(sb, indent + 2, nameof(vector.PatternFile), vector.PatternFile);
                AppendField(sb, indent + 2, nameof(vector.RegisterFile), vector.RegisterFile);
                sb.AppendLine($"{Indent(indent + 1)}}}");
            }
            sb.AppendLine($"{Indent(indent)}}}");
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
            AppendField(sb, indent + 1, nameof(strategy.Mode), strategy.Mode);
            AppendField(sb, indent + 1, nameof(strategy.Time), strategy.Time);
            AppendField(sb, indent + 1, nameof(strategy.Count), strategy.Count);
            sb.AppendLine($"{Indent(indent)}}}");
        }
        private static void AppendField(StringBuilder sb, int indent, string field, object value)
        {
            sb.Append(Indent(indent)).Append(field).Append(' ');
            switch (value)
            {
                case Time t:
                    sb.Append(t.Value).Append(' ').Append(t.Unit);
                    break;
                case string s:
                    sb.Append('"').Append(s).Append('"');
                    break;
                default:
                    sb.Append(value);
                    break;
            }
            sb.AppendLine(";");
        }
        private static string Indent(int indent) => new string('\t', indent);
    }
}
