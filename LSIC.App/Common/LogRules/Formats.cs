using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ATiS.Base;

namespace LSIC.App
{
    /// <summary>
    /// 数据格式化条件
    /// </summary>
    public class FormatCondition
    {
        /// <summary>
        /// 精度（影响小数点位数）
        /// </summary>
        public int Precision { get; set; }

        /// <summary>
        /// 量级
        /// </summary>
        public int Magnitude { get; set; }

        /// <summary>
        /// 单位, 该量级下所表示的单位, 如: mV, 可以为空
        /// </summary>
        public string Unit { get; set; }
    }

    public class Formats
    {
        private Formats(FormatCondition[] conditions)
        {
            Conditions = conditions.OrderByDescending(o => o.Magnitude).ToArray();
        }

        public FormatCondition[] Conditions { get; private set; }

        public string Format<T>(T value)
        {
            TypeCode type = Type.GetTypeCode(value.GetType());
            switch (type)
            {
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Decimal:
                    {
                        dynamic va = value;
                        int prec = 1;
                        string unit = string.Empty;
                        double re = va;
                        foreach (FormatCondition condition in Conditions)
                        {
                            prec = condition.Precision;
                            unit = condition.Unit;

                            double sc = Math.Pow(10, condition.Magnitude);
                            re = va / sc;
                            if (re >= 1)
                            {
                                break;
                            }
                        }

                        return unit.IsNullOrEmptyOrWhiteSpace() || unit.Equals("null") ? $"{re.ToString($"F{prec}")}" : $"{re.ToString($"F{prec}")} {unit}";
                    }

                default:
                    return value.ToString();
            }
        }

        public static implicit operator Formats(string expr)
        {
            using MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(expr));
            using CharReader reader = new CharReader(stream);

            reader.SkipWhiteSpace();
            if (reader.Try('['))
            {
                reader.ReadExpected('[');
            }
            else
            {
                throw new CharReaderException(reader);
            }

            List<FormatCondition> conditions = new List<FormatCondition>();

            while (!reader.End)
            {
                reader.SkipWhiteSpace();
                if (reader.Try('{'))
                {
                    reader.ReadExpected('{');
                }
                else
                {
                    throw new CharReaderException(reader);
                }

                FormatCondition condition = new FormatCondition();

                reader.SkipWhiteSpace();
                condition.Magnitude = reader.ReadNumber<int>();

                reader.SkipWhiteSpace();
                if (reader.Try(','))
                {
                    reader.ReadExpected(',');
                }
                else
                {
                    throw new CharReaderException(reader);
                }

                reader.SkipWhiteSpace();
                condition.Precision = reader.ReadNumber<int>();

                reader.SkipWhiteSpace();
                if (reader.Try(','))
                {
                    reader.ReadExpected(',');
                    reader.SkipWhiteSpace();
                    condition.Unit = reader.ReadStringUntil('}');
                }
                else
                {
                    condition.Unit = null;
                }

                reader.SkipWhiteSpace();
                if (reader.Try('}'))
                {
                    reader.ReadExpected('}');
                }
                else
                {
                    throw new CharReaderException(reader);
                }

                conditions.Add(condition);

                reader.SkipWhiteSpace();
                if (reader.Try(','))
                {
                    reader.ReadExpected(',');
                }
                else
                {
                    break;
                }
            }

            reader.SkipWhiteSpace();
            if (reader.Try(']'))
            {
                reader.ReadExpected(']');
            }
            else
            {
                throw new CharReaderException(reader);
            }

            return new Formats(conditions.ToArray());
        }
    }
}
