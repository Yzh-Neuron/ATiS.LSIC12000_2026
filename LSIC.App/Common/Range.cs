namespace LSIC.App
{
    public class Limit
    {
        public Limit(bool includeFloor, long floor, bool includeCeiling, long ceiling)
        {
            IncludeFloor = includeFloor;
            Floor = floor;
            IncludeCeiling = includeCeiling;
            Ceiling = ceiling;
        }

        public bool IncludeFloor { get; private set; }

        public long Floor { get; private set; }

        public bool IncludeCeiling { get; private set; }

        public long Ceiling { get; private set; }

        public bool Contains<T>(T value) where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
        {
            // 将 Floor 和 Ceiling 转换为 T 类型
            T floorValue = (T)Convert.ChangeType(Floor, typeof(T));
            T ceilingValue = (T)Convert.ChangeType(Ceiling, typeof(T));

            // 通过比较 value 与 Floor 和 Ceiling 的大小关系来判断 value 是否在范围内
            int floor = value.CompareTo(floorValue);
            int ceiling = value.CompareTo(ceilingValue);
            return (IncludeFloor ? floor >= 0 : floor > 0) &&
                   (IncludeCeiling ? ceiling <= 0 : ceiling < 0);
        }

        /// <summary>
        /// 获取范围描述
        /// </summary>
        public string GetRangString(long scale, string format = null)
        {
            double scaleFloorValue = (double)Floor / scale;
            double scaleCeiliValue = (double)Ceiling / scale;

            return $"{scaleFloorValue.ToString(format)} ~ {scaleCeiliValue.ToString(format)}";
        }

        /// <summary>
        /// 获取范围描述
        /// </summary>
        public string GetRangString(long floorScale, string floorUnit, long ceilingScale, string ceilingUnit, string format = null)
        {
            double floor = (double)Floor / floorScale;
            double ceili = (double)Ceiling / ceilingScale;

            floorUnit = floorUnit.IsNullOrEmptyOrWhiteSpace() ? string.Empty : $" {floorUnit}";
            ceilingUnit = ceilingUnit.IsNullOrEmptyOrWhiteSpace() ? string.Empty : $" {ceilingUnit}";

            return $"{floor.ToString(format)}{floorUnit} ~ {ceili.ToString(format)}{ceilingUnit}";
        }
    }

    public class Limit<T> where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
    {
        public Limit(bool includeFloor, T floor, bool includeCeiling, T ceiling)
        {
            IncludeFloor = includeFloor;
            Floor = floor;
            IncludeCeiling = includeCeiling;
            Ceiling = ceiling;
        }

        public bool IncludeFloor { get; private set; }

        public T Floor { get; private set; }

        public bool IncludeCeiling { get; private set; }

        public T Ceiling { get; private set; }

        public bool Contains<TValue>(TValue value) where TValue : IComparable
        {
            // 将 Floor 和 Ceiling 转换为 T 类型
            TValue floorValue = (TValue)Convert.ChangeType(Floor, typeof(TValue));
            TValue ceilingValue = (TValue)Convert.ChangeType(Ceiling, typeof(TValue));

            // 通过比较 value 与 Floor 和 Ceiling 的大小关系来判断 value 是否在范围内
            int floor = value.CompareTo(floorValue);
            int ceiling = value.CompareTo(ceilingValue);
            return (IncludeFloor ? floor >= 0 : floor > 0) &&
                   (IncludeCeiling ? ceiling <= 0 : ceiling < 0);
        }

        /// <summary>
        /// 获取范围描述
        /// </summary>
        public string GetRangString(double scale, string format = null)
        {
            // 将 Floor 和 Ceiling 转换为 T 类型
            double floorValue = (double)Convert.ChangeType(Floor, typeof(double));
            double ceilingValue = (double)Convert.ChangeType(Ceiling, typeof(double));

            double scaleFloorValue = floorValue / scale;
            double scaleCeiliValue = ceilingValue / scale;

            if (format.IsNullOrEmptyOrWhiteSpace())
            {
                return $"{scaleFloorValue} ~ {scaleCeiliValue}";
            }
            return $"{scaleFloorValue.ToString(format)} ~ {scaleCeiliValue.ToString(format)}";
        }

        /// <summary>
        /// 获取范围描述
        /// </summary>
        public string GetRangString(long floorScale, string floorUnit, long ceilingScale, string ceilingUnit, string format = null)
        {
            // 将 Floor 和 Ceiling 转换为 T 类型
            double floor = (double)Convert.ChangeType(Floor, typeof(double));
            double ceili = (double)Convert.ChangeType(Ceiling, typeof(double));

            double floorValue = floor / floorScale;
            double ceilingValue = ceili / ceilingScale;

            floorUnit = floorUnit.IsNullOrEmptyOrWhiteSpace() ? string.Empty : $" {floorUnit}";
            ceilingUnit = ceilingUnit.IsNullOrEmptyOrWhiteSpace() ? string.Empty : $" {ceilingUnit}";

            if (format.IsNullOrEmptyOrWhiteSpace())
            {
                return $"{floorValue}{floorUnit} ~ {ceilingValue}{ceilingUnit}";
            }
            return $"{floorValue.ToString(format)}{floorUnit} ~ {ceilingValue.ToString(format)}{ceilingUnit}";
        }
    }

    public class Range
    {
        private Range(KeyValuePair<Limit, Limit>[] windows)
        {
            Windows = windows;
        }

        public KeyValuePair<Limit, Limit>[] Windows { get; private set; }

        public bool TryGetValue(long key, out Limit value)
        {
            value = null;
            foreach (KeyValuePair<Limit, Limit> window in Windows)
            {
                if (window.Key is null || window.Key.Contains(key))
                {
                    value = window.Value;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取上限
        /// </summary>
        public long GetCeilingValue()
        {
            return Windows.Max(w => w.Value.Ceiling);
        }

        /// <summary>
        /// 获取下限
        /// </summary>
        public long GetFloorValue()
        {
            return Windows.Min(w => w.Value.Floor);
        }

        public static implicit operator Range(string expr)
        {
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(expr)))
            {
                using (CharReader reader = new CharReader(stream))
                {
                    List<KeyValuePair<Limit, Limit>> windows = new List<KeyValuePair<Limit, Limit>>();
                    while (!reader.End)
                    {
                        reader.SkipWhiteSpace();

                        bool includeFloor;
                        if (reader.Try('['))
                        {
                            reader.ReadExpected('[');
                            includeFloor = true;
                        }
                        else if (reader.Try('('))
                        {
                            reader.ReadExpected('(');
                            includeFloor = false;
                        }
                        else
                        {
                            throw new CharReaderException(reader);
                        }

                        reader.SkipWhiteSpace();

                        long floor = reader.ReadNumber<long>();

                        reader.SkipWhiteSpace();

                        reader.ReadExpected(',');

                        reader.SkipWhiteSpace();

                        long ceiling = reader.ReadNumber<long>();

                        bool includeCeiling;
                        if (reader.Try(']'))
                        {
                            reader.ReadExpected(']');
                            includeCeiling = true;
                        }
                        else if (reader.Try(')'))
                        {
                            reader.ReadExpected(')');
                            includeCeiling = false;
                        }
                        else
                        {
                            throw new CharReaderException(reader);
                        }

                        Limit limit = new Limit(includeFloor, floor, includeCeiling, ceiling);

                        reader.SkipWhiteSpace();

                        if (reader.Try('>'))
                        {
                            reader.ReadExpected('>');

                            Limit key = limit;

                            if (reader.Try('['))
                            {
                                reader.ReadExpected('[');
                                includeFloor = true;
                            }
                            else if (reader.Try('('))
                            {
                                reader.ReadExpected('(');
                                includeFloor = false;
                            }
                            else
                            {
                                throw new CharReaderException(reader);
                            }

                            reader.SkipWhiteSpace();

                            floor = reader.ReadNumber<int>();

                            reader.SkipWhiteSpace();

                            reader.ReadExpected(',');

                            reader.SkipWhiteSpace();

                            ceiling = reader.ReadNumber<int>();

                            if (reader.Try(']'))
                            {
                                reader.ReadExpected(']');
                                includeCeiling = true;
                            }
                            else if (reader.Try(')'))
                            {
                                reader.ReadExpected(')');
                                includeCeiling = false;
                            }
                            else
                            {
                                throw new CharReaderException(reader);
                            }

                            Limit value = new Limit(includeFloor, floor, includeCeiling, ceiling);
                            windows.Add(new KeyValuePair<Limit, Limit>(key, value));
                        }
                        else
                        {
                            if (reader.End &&
                                windows.Count == 0)
                            {
                                Limit key = null;
                                Limit value = limit;
                                windows.Add(new KeyValuePair<Limit, Limit>(key, value));
                                break;
                            }
                            else
                            {
                                throw new CharReaderException(reader);
                            }
                        }
                    }
                    return new Range(windows.ToArray());
                }
            }
        }
    }
}
