//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using ATiS.Base;
//using LSIC.App.Models;

//namespace LSIC.App
//{
//    public static class IcParser
//    {
//        public static void Parse(ref IcFile ic)
//        {
//            using CharReader reader = new CharReader(ic.Path, Encoding.GetEncoding("gb2312"));

//            ic.BaseSetting = ReadBaseSetting(reader, nameof(ic.BaseSetting));
//            ic.PowerSetting = ReadPowerSetting(reader, nameof(ic.PowerSetting));
//            ic.ProcessSetting = ReadProcessSetting(reader, nameof(ic.ProcessSetting));

//            reader.SkipWhiteSpace();
//            reader.ReadEnd();
//        }

//        public static async Task ParseAsync(IcFile ic)
//        {
//            using CharReader reader = new CharReader(ic.Path, Encoding.GetEncoding("gb2312"));

//            ic.BaseSetting = ReadBaseSetting(reader, nameof(ic.BaseSetting));
//            ic.PowerSetting = ReadPowerSetting(reader, nameof(ic.PowerSetting));
//            ic.ProcessSetting = ReadProcessSetting(reader, nameof(ic.ProcessSetting));

//            reader.SkipWhiteSpace();
//            reader.ReadEnd();

//            await Task.CompletedTask;
//        }

//        public static string Serialize(this IcFile ic)
//        {
//            StringBuilder sb = new StringBuilder();

//            基础设置
//           BaseSetting baseSet = ic.BaseSetting;
//            sb.AppendLine($"{nameof(ic.BaseSetting)} {{");
//            sb.AppendLine($"\t{nameof(baseSet.LogInterval)} {baseSet.LogInterval.Value} {baseSet.LogInterval.Unit};");
//            sb.AppendLine($"\t{nameof(baseSet.ErrorLogEnable)} {baseSet.ErrorLogEnable.ToString()};");
//            sb.AppendLine($"\t{nameof(baseSet.ErrorLogInterval)} {baseSet.ErrorLogInterval.Value} {baseSet.ErrorLogInterval.Unit};");
//            sb.AppendLine($"\t{nameof(baseSet.DutConfigFile)} \"{baseSet.DutConfigFile}\";");
//            sb.AppendLine($"\t{nameof(baseSet.ThermostatSettings)} {{");
//            foreach (ThermostatSetting thermostat in baseSet.ThermostatSettings)
//            {
//                sb.AppendLine($"\t\t{nameof(ThermostatSetting)} \"{thermostat.Id}\" {{");
//                sb.AppendLine($"\t\t\t{nameof(thermostat.Enable)} {thermostat.Enable};");
//                sb.AppendLine($"\t\t\t{nameof(thermostat.TargetTemperature)} {thermostat.TargetTemperature};");
//                sb.AppendLine("\t\t}");
//            }
//            sb.AppendLine("\t}");
//            sb.AppendLine($"}}");

//            电源设置
//           PowerSetting poweSet = ic.PowerSetting;
//            sb.AppendLine($"{nameof(ic.PowerSetting)} {{");
//            sb.AppendLine($"\t{nameof(poweSet.DpsPlans)} {{");
//            foreach (DpsPlan dpsPlan in poweSet.DpsPlans)
//            {
//                sb.AppendLine($"\t\t{nameof(DpsPlan)} \"{dpsPlan.Name}\" {{");
//                sb.AppendLine($"\t\t\t{nameof(dpsPlan.Purpose)} {dpsPlan.Purpose};");
//                sb.AppendLine($"\t\t\t{nameof(dpsPlan.PatternFile)} \"{dpsPlan.PatternFile}\";");
//                sb.AppendLine($"\t\t\t{nameof(dpsPlan.RegisterFile)} \"{dpsPlan.RegisterFile}\";");
//                sb.AppendLine($"\t\t\t{nameof(dpsPlan.PatternTimeout)} {dpsPlan.PatternTimeout};");
//                sb.AppendLine($"\t\t\t{nameof(dpsPlan.Dpss)} {{");
//                foreach (Dps dps in dpsPlan.Dpss)
//                {
//                    sb.AppendLine($"\t\t\t\t{nameof(Dps)} {{");
//                    sb.AppendLine($"\t\t\t\t\t{nameof(dps.Enable)} {dps.Enable};");
//                    sb.AppendLine($"\t\t\t\t\t{nameof(dps.Id)} {dps.Id};");
//                    sb.AppendLine($"\t\t\t\t\t{nameof(dps.Vol)} {dps.Vol};");
//                    sb.AppendLine($"\t\t\t\t\t{nameof(dps.Uvp)} {dps.Uvp};");
//                    sb.AppendLine($"\t\t\t\t\t{nameof(dps.Ovp)} {dps.Ovp};");
//                    sb.AppendLine($"\t\t\t\t\t{nameof(dps.Ocp)} {dps.Ocp};");
//                    sb.AppendLine($"\t\t\t\t\t{nameof(dps.Raise)} {dps.Raise};");
//                    sb.AppendLine($"\t\t\t\t\t{nameof(dps.Delay)} {dps.Delay};");
//                    sb.AppendLine("\t\t\t\t}");
//                }
//                sb.AppendLine("\t\t\t}");
//                sb.AppendLine("\t\t}");
//            }
//            sb.AppendLine("\t}");
//            sb.AppendLine("}");

//            流程设置
//           ProcessSetting procSet = ic.ProcessSetting;
//            sb.AppendLine($"{nameof(ic.ProcessSetting)} {{");
//            sb.AppendLine($"\t{nameof(procSet.Steps)} {{");
//            foreach (Step step in procSet.Steps)
//            {
//                sb.AppendLine($"\t\t{nameof(Step)} \"{step.Name}\" {{");
//                sb.AppendLine($"\t\t\t{nameof(step.ExecuteStrategy)} {{");
//                sb.AppendLine($"\t\t\t\t{nameof(step.ExecuteStrategy.Mode)} {step.ExecuteStrategy.Mode};");
//                sb.AppendLine($"\t\t\t\t{nameof(step.ExecuteStrategy.Time)} {step.ExecuteStrategy.Time.Value} {step.ExecuteStrategy.Time.Unit};");
//                sb.AppendLine($"\t\t\t\t{nameof(step.ExecuteStrategy.Count)} {step.ExecuteStrategy.Count};");
//                sb.AppendLine("\t\t\t}");
//                sb.AppendLine($"\t\t\t{nameof(step.LoopStrategy)} {{");
//                sb.AppendLine($"\t\t\t\t{nameof(step.LoopStrategy.Enable)} {step.LoopStrategy.Enable.ToString()};");
//                sb.AppendLine($"\t\t\t\t{nameof(step.LoopStrategy.Target)} \"{step.LoopStrategy.Target}\";");
//                sb.AppendLine($"\t\t\t\t{nameof(step.LoopStrategy.Mode)} {step.LoopStrategy.Mode};");
//                sb.AppendLine($"\t\t\t\t{nameof(step.LoopStrategy.Time)} {step.LoopStrategy.Time.Value} {step.LoopStrategy.Time.Unit};");
//                sb.AppendLine($"\t\t\t\t{nameof(step.LoopStrategy.Count)} {step.LoopStrategy.Count};");
//                sb.AppendLine("\t\t\t}");
//                sb.AppendLine($"\t\t\t{nameof(step.DpsPlan)} \"{step.DpsPlan}\";");
//                sb.AppendLine($"\t\t\t{nameof(step.SubSteps)} {{");
//                foreach (SubStep vectorStep in step.SubSteps)
//                {
//                    sb.AppendLine($"\t\t\t\t{nameof(SubStep)} \"{vectorStep.Name}\" {{");
//                    sb.AppendLine($"\t\t\t\t\t{nameof(vectorStep.PatternFile)} \"{vectorStep.PatternFile}\";");
//                    sb.AppendLine($"\t\t\t\t\t{nameof(vectorStep.RegisterFile)} \"{vectorStep.RegisterFile}\";");
//                    sb.AppendLine("\t\t\t\t}");
//                }
//                sb.AppendLine("\t\t\t}");
//                sb.AppendLine("\t\t}");
//            }
//            sb.AppendLine("\t}");
//            sb.AppendLine("}");

//            return sb.ToString();
//        }

//        private static BaseSetting ReadBaseSetting(CharReader reader, string propertyName)
//        {
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(propertyName);
//            reader.SkipWhiteSpace();
//            reader.ReadExpected("{");

//            BaseSetting baseSet = new BaseSetting();

//            reader.SkipWhiteSpace();
//            reader.ReadExpected(nameof(baseSet.LogInterval));
//            reader.ReadWhiteSpace();
//            baseSet.LogInterval = reader.ReadTime();
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(';');

//            reader.SkipWhiteSpace();
//            reader.ReadExpected(nameof(baseSet.ErrorLogEnable));
//            reader.ReadWhiteSpace();
//            baseSet.ErrorLogEnable = reader.ReadBool();
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(';');

//            reader.SkipWhiteSpace();
//            reader.ReadExpected(nameof(baseSet.ErrorLogInterval));
//            reader.ReadWhiteSpace();
//            baseSet.ErrorLogInterval = reader.ReadTime();
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(';');

//            reader.SkipWhiteSpace();
//            reader.ReadExpected(nameof(baseSet.DutConfigFile));
//            reader.ReadWhiteSpace();
//            baseSet.DutConfigFile = reader.ReadStringWrap('\"');
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(';');

//            reader.ReadWhiteSpace();
//            reader.ReadExpected(nameof(baseSet.ThermostatSettings));
//            reader.ReadWhiteSpace();
//            reader.ReadExpected('{');
//            reader.ReadWhiteSpace();
//            while (!reader.Try('}'))
//            {
//                ThermostatSetting source = ReadThermostatSetting(reader);

//                ThermostatSetting target = baseSet.ThermostatSettings.FirstOrDefault(t => t.Id == source.Id);
//                if (target != null)
//                {
//                    target.Enable = source.Enable;
//                    target.TargetTemperature = source.TargetTemperature;
//                }

//                reader.SkipWhiteSpace();
//            }
//            reader.ReadExpected('}');

//            reader.SkipWhiteSpace();
//            reader.ReadExpected('}');

//            return baseSet;
//        }

//        private static ThermostatSetting ReadThermostatSetting(CharReader reader)
//        {
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(nameof(ThermostatSetting));
//            reader.ReadWhiteSpace();

//            int id = int.Parse(reader.ReadStringWrap('\"'));
//            ThermostatSetting thermostatSetting = new ThermostatSetting(id);
//            reader.SkipWhiteSpace();
//            reader.ReadExpected("{");

//            reader.SkipWhiteSpace();
//            reader.ReadExpected(nameof(thermostatSetting.Enable));
//            reader.ReadWhiteSpace();
//            thermostatSetting.Enable = reader.ReadBool();
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(';');

//            reader.SkipWhiteSpace();
//            reader.ReadExpected(nameof(thermostatSetting.TargetTemperature));
//            reader.ReadWhiteSpace();
//            thermostatSetting.TargetTemperature = reader.ReadNumber<int>();
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(';');

//            reader.SkipWhiteSpace();
//            reader.ReadExpected('}');
//            return thermostatSetting;
//        }

//        private static PowerSetting ReadPowerSetting(CharReader reader, string propertyName)
//        {
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(propertyName);
//            reader.SkipWhiteSpace();
//            reader.ReadExpected("{");

//            PowerSetting powerSet = new PowerSetting();

//            List<DpsPlan> dpsPlans = new List<DpsPlan>();
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(nameof(powerSet.DpsPlans));
//            reader.SkipWhiteSpace();
//            reader.ReadExpected('{');
//            reader.SkipWhiteSpace();
//            while (!reader.Try('}'))
//            {
//                dpsPlans.Add(ReadDpsPlan(reader, powerSet));
//                reader.SkipWhiteSpace();
//            }
//            reader.ReadExpected('}');
//            powerSet.DpsPlans = new ObservableCollection<DpsPlan>(dpsPlans);

//            reader.SkipWhiteSpace();
//            reader.ReadExpected('}');

//            return powerSet;
//        }

//        private static ProcessSetting ReadProcessSetting(CharReader reader, string propertyName)
//        {
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(propertyName);
//            reader.SkipWhiteSpace();
//            reader.ReadExpected("{");

//            ProcessSetting processSet = new ProcessSetting();

//            List<Step> steps = new List<Step>();
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(nameof(processSet.Steps));
//            reader.SkipWhiteSpace();
//            reader.ReadExpected('{');
//            reader.SkipWhiteSpace();
//            while (!reader.Try('}'))
//            {
//                Step step = ReadStep(reader);
//                steps.Add(step);
//                reader.SkipWhiteSpace();
//            }
//            reader.ReadExpected('}');
//            processSet.Steps = new ObservableCollection<Step>(steps);

//            reader.SkipWhiteSpace();
//            reader.ReadExpected('}');

//            return processSet;
//        }

//        private static DpsPlan ReadDpsPlan(CharReader reader, PowerSetting powerSetting)
//        {
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(nameof(DpsPlan));
//            reader.ReadWhiteSpace();
//            DpsPlan dpsPlan = new DpsPlan(powerSetting)
//            {
//                Name = reader.ReadStringWrap('\"')
//            };
//            reader.SkipWhiteSpace();
//            reader.ReadExpected('{');

//            reader.SkipWhiteSpace();
//            reader.ReadExpected(nameof(dpsPlan.Purpose));
//            reader.ReadWhiteSpace();
//            dpsPlan.Purpose = reader.ReadEnum<DpsPlanPurpose>();
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(';');

//            reader.SkipWhiteSpace();
//            reader.ReadExpected(nameof(dpsPlan.PatternFile));
//            reader.ReadWhiteSpace();
//            dpsPlan.PatternFile = reader.ReadStringWrap('\"');
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(';');

//            reader.SkipWhiteSpace();
//            reader.ReadExpected(nameof(dpsPlan.RegisterFile));
//            reader.ReadWhiteSpace();
//            dpsPlan.RegisterFile = reader.ReadStringWrap('\"');
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(';');

//            reader.SkipWhiteSpace();
//            reader.ReadExpected(nameof(dpsPlan.PatternTimeout));
//            reader.ReadWhiteSpace();
//            dpsPlan.PatternTimeout = reader.ReadNumber<int>();
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(';');

//            dpsPlan.Dpss.Clear();
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(nameof(dpsPlan.Dpss));
//            reader.SkipWhiteSpace();
//            reader.ReadExpected('{');

//            reader.SkipWhiteSpace();
//            while (!reader.Try('}'))
//            {
//                Dps dps = ReadDps(reader, dpsPlan);
//                dpsPlan.Dpss.Add(dps);
//                reader.SkipWhiteSpace();
//            }
//            reader.ReadExpected('}');
//            reader.SkipWhiteSpace();
//            reader.ReadExpected('}');
//            return dpsPlan;
//        }

//        private static Dps ReadDps(CharReader reader, DpsPlan dpsPlan)
//        {
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(nameof(Dps));
//            reader.SkipWhiteSpace();
//            reader.ReadExpected("{");

//            Dps dps = new Dps(dpsPlan);

//            reader.SkipWhiteSpace();
//            reader.ReadExpected(nameof(dps.Enable));
//            reader.ReadWhiteSpace();
//            dps.Enable = reader.ReadBool();
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(';');

//            reader.SkipWhiteSpace();
//            reader.ReadExpected(nameof(dps.Id));
//            reader.ReadWhiteSpace();
//            dps.Id = reader.ReadNumber<int>();
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(';');

//            reader.SkipWhiteSpace();
//            reader.ReadExpected(nameof(dps.Vol));
//            reader.ReadWhiteSpace();
//            dps.Vol = reader.ReadNumber<int>();
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(';');

//            reader.SkipWhiteSpace();
//            reader.ReadExpected(nameof(dps.Uvp));
//            reader.ReadWhiteSpace();
//            dps.Uvp = reader.ReadNumber<int>();
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(';');

//            reader.SkipWhiteSpace();
//            reader.ReadExpected(nameof(dps.Ovp));
//            reader.ReadWhiteSpace();
//            dps.Ovp = reader.ReadNumber<int>();
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(';');

//            reader.SkipWhiteSpace();
//            reader.ReadExpected(nameof(dps.Ocp));
//            reader.ReadWhiteSpace();
//            dps.Ocp = reader.ReadNumber<int>();
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(';');

//            reader.SkipWhiteSpace();
//            reader.ReadExpected(nameof(dps.Raise));
//            reader.ReadWhiteSpace();
//            dps.Raise = reader.ReadNumber<int>();
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(';');

//            reader.SkipWhiteSpace();
//            reader.ReadExpected(nameof(dps.Delay));
//            reader.ReadWhiteSpace();
//            dps.Delay = reader.ReadNumber<int>();
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(';');

//            reader.SkipWhiteSpace();
//            reader.ReadExpected('}');
//            return dps;
//        }

//        private static Step ReadStep(CharReader reader)
//        {
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(nameof(Step));
//            reader.ReadWhiteSpace();
//            Step step = new Step()
//            {
//                Name = reader.ReadStringWrap('\"')
//            };
//            reader.SkipWhiteSpace();
//            reader.ReadExpected('{');

//            step.ExecuteStrategy = ReadRunStrategy(reader);

//            reader.SkipWhiteSpace();

//            step.LoopStrategy = ReadLoopStrategy(reader);

//            reader.SkipWhiteSpace();
//            reader.ReadExpected(nameof(step.DpsPlan));
//            reader.ReadWhiteSpace();
//            step.DpsPlan = reader.ReadStringWrap('\"');
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(';');

//            reader.SkipWhiteSpace();
//            reader.ReadExpected(nameof(step.SubSteps));
//            reader.SkipWhiteSpace();
//            reader.ReadExpected('{');
//            reader.SkipWhiteSpace();
//            List<SubStep> vectorSteps = new List<SubStep>();
//            while (!reader.Try('}'))
//            {
//                SubStep vectorStep = ReadSubStep(reader);
//                vectorSteps.Add(vectorStep);
//                reader.SkipWhiteSpace();
//            }
//            reader.ReadExpected('}');
//            reader.SkipWhiteSpace();
//            reader.ReadExpected('}');
//            foreach (SubStep vectorStep in vectorSteps)
//            {
//                step.SubSteps.Add(vectorStep);
//            }
//            return step;
//        }

//        private static ExecuteStrategy ReadRunStrategy(CharReader reader)
//        {
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(nameof(ExecuteStrategy));
//            reader.SkipWhiteSpace();
//            reader.ReadExpected('{');

//            ExecuteStrategy strategy = new ExecuteStrategy();

//            reader.SkipWhiteSpace();
//            reader.ReadExpected(nameof(strategy.Mode));
//            reader.ReadWhiteSpace();
//            strategy.Mode = reader.ReadEnum<ExecuteMode>();
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(';');

//            reader.SkipWhiteSpace();
//            reader.ReadExpected(nameof(strategy.Time));
//            reader.ReadWhiteSpace();
//            int value = reader.ReadNumber<int>();
//            reader.ReadWhiteSpace();
//            TimeUnit unit = reader.ReadEnum<TimeUnit>();
//            strategy.Time = new Time(value, unit);
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(';');

//            reader.SkipWhiteSpace();
//            reader.ReadExpected(nameof(strategy.Count));
//            reader.ReadWhiteSpace();
//            strategy.Count = reader.ReadNumber<int>();
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(';');

//            reader.SkipWhiteSpace();
//            reader.ReadExpected('}');
//            return strategy;
//        }

//        private static LoopStrategy ReadLoopStrategy(CharReader reader)
//        {
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(nameof(LoopStrategy));
//            reader.SkipWhiteSpace();
//            reader.ReadExpected('{');

//            LoopStrategy strategy = new LoopStrategy();

//            reader.SkipWhiteSpace();
//            reader.ReadExpected(nameof(strategy.Enable));
//            reader.ReadWhiteSpace();
//            strategy.Enable = reader.ReadBool();
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(';');

//            reader.SkipWhiteSpace();
//            reader.ReadExpected(nameof(strategy.Target));
//            reader.ReadWhiteSpace();
//            strategy.Target = reader.ReadStringWrap('\"');
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(';');

//            reader.SkipWhiteSpace();
//            reader.ReadExpected(nameof(strategy.Mode));
//            reader.ReadWhiteSpace();
//            strategy.Mode = reader.ReadEnum<ExecuteMode>();
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(';');

//            reader.SkipWhiteSpace();
//            reader.ReadExpected(nameof(strategy.Time));
//            reader.ReadWhiteSpace();
//            int value = reader.ReadNumber<int>();
//            reader.ReadWhiteSpace();
//            TimeUnit unit = reader.ReadEnum<TimeUnit>();
//            strategy.Time = new Time(value, unit);
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(';');

//            reader.SkipWhiteSpace();
//            reader.ReadExpected(nameof(strategy.Count));
//            reader.ReadWhiteSpace();
//            strategy.Count = reader.ReadNumber<int>();
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(';');

//            reader.SkipWhiteSpace();
//            reader.ReadExpected('}');
//            return strategy;
//        }

//        private static SubStep ReadSubStep(CharReader reader)
//        {
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(nameof(SubStep));
//            reader.ReadWhiteSpace();
//            SubStep subStep = new SubStep()
//            {
//                Name = reader.ReadStringWrap('\"')
//            };
//            reader.SkipWhiteSpace();
//            reader.ReadExpected('{');

//            reader.SkipWhiteSpace();
//            reader.ReadExpected(nameof(subStep.PatternFile));
//            reader.ReadWhiteSpace();
//            subStep.PatternFile = reader.ReadStringWrap('\"');
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(';');

//            reader.SkipWhiteSpace();
//            reader.ReadExpected(nameof(subStep.RegisterFile));
//            reader.ReadWhiteSpace();
//            subStep.RegisterFile = reader.ReadStringWrap('\"');
//            reader.SkipWhiteSpace();
//            reader.ReadExpected(';');

//            reader.SkipWhiteSpace();
//            reader.ReadExpected('}');

//            return subStep;
//        }

//        private static Time ReadTime(this CharReader reader)
//        {
//            reader.SkipWhiteSpace();
//            int value = reader.ReadNumber<int>();
//            reader.ReadWhiteSpace();
//            TimeUnit unit = reader.ReadEnum<TimeUnit>();
//            return new Time(value, unit);
//        }
//    }
//}
