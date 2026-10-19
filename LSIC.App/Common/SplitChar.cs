namespace LSIC.App
{
    public enum SplitChar
    {
        /// <summary>
        /// 无分隔符
        /// </summary>
        None,
        /// <summary>
        /// .
        /// </summary>
        Dot,
        /// <summary>
        /// ,
        /// </summary>
        Comma,
        /// <summary>
        /// ;
        /// </summary>
        Semicolon,
        /// <summary>
        /// 空格
        /// </summary>
        Space,
        /// <summary>
        /// 制表符
        /// </summary>
        Tab,
        /// <summary>
        /// |
        /// </summary>
        Stroke,
    }

    public class SplitCharEx
    {
        public SplitCharEx(string value)
        {
            if (Enum.TryParse(value, out SplitChar splitChar))
            {
                Value = splitChar.ToChar();
                return;
            }
            Value = value switch
            {
                "None" => null,
                "." => '.',
                "," => ',',
                ";" => ';',
                " " => ' ',
                "\t" => '\t',
                "|" => '|',
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, LangProvider.Instance.SplitCharUnsupportedMessage)
            };
        }

        public char? Value { get; }

        public char ToChar() => Value ?? throw new InvalidOperationException(LangProvider.Instance.SplitCharInvalidMessage);

        public static implicit operator SplitCharEx(string value) => new SplitCharEx(value);
    }
}