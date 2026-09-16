using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATiS.Sim.Log
{
    /// <summary>
    /// 消息格式模板 {level} {msg} {date:格式}
    /// </summary>
    internal sealed class MessageTemplate
    {
        private static readonly string[] LevelNames = { "Debug", "Info", "Warn", "Error", "Fatal" };

        private readonly Segment[] _segments;

        public MessageTemplate(string format)
        {
            ArgumentNullException.ThrowIfNull(format);

            var list = new List<Segment>();
            int i = 0;

            while (i < format.Length)
            {
                int open = format.IndexOf('{', i);
                if (open < 0)
                {
                    list.Add(new Segment(Kind.Literal, format.Substring(i)));
                    break;
                }

                if (open > i)
                {
                    list.Add(new Segment(Kind.Literal, format.Substring(i, open - i)));
                }

                int close = format.IndexOf('}', open + 1);
                if (close < 0)
                {
                    throw new FormatException("消息格式缺少右花括号 }：" + format);
                }

                string token = format.Substring(open + 1, close - open - 1).Trim();
                list.Add(Match(token, format));
                i = close + 1;
            }

            _segments = list.ToArray();
        }

        private static Segment Match(string token, string full)
        {
            if (token.Equals("level", StringComparison.OrdinalIgnoreCase))
            {
                return new Segment(Kind.Level, string.Empty);
            }
            if (token.Equals("msg", StringComparison.OrdinalIgnoreCase))
            {
                return new Segment(Kind.Message, string.Empty);
            }
            if (token.StartsWith("date:", StringComparison.OrdinalIgnoreCase))
            {
                string fmt = token.Substring(5).Trim();
                if (fmt.Length == 0)
                {
                    throw new FormatException("{date:格式} 中的格式不能为空：" + full);
                }
                return new Segment(Kind.Date, fmt);
            }

            throw new FormatException(
                $"消息格式含未知参数 {{{token}}}，只支持 {{level}}、{{msg}}、{{date:格式}}：{full}");
        }

        public string Render(LogLevel level, string message, DateTimeOffset time)
        {
            var sb = new StringBuilder(128);
            Span<char> dateBuffer = stackalloc char[96];

            foreach (Segment seg in _segments)
            {
                switch (seg.Kind)
                {
                    case Kind.Literal:
                        sb.Append(seg.Text);
                        break;
                    case Kind.Level:
                        sb.Append(LevelNames[(byte)level]);
                        break;
                    case Kind.Message:
                        sb.Append(message);
                        break;
                    case Kind.Date:
                        if (time.TryFormat(dateBuffer, out int n, seg.Text, CultureInfo.InvariantCulture))
                        {
                            sb.Append(dateBuffer.Slice(0, n));
                        }
                        else
                        {
                            sb.Append(time.ToString(seg.Text, CultureInfo.InvariantCulture));
                        }
                        break;
                }
            }

            return sb.ToString();
        }

    }

    internal readonly struct Segment
    {
        public readonly Kind Kind;
        public readonly string Text;

        public Segment(Kind kind, string text)
        {
            Kind = kind;
            Text = text;
        }
    }

    internal enum Kind
    {
        Literal,
        Level,
        Date,
        Message
    }
}
