namespace LSIC.App
{
    public static class SplitCharExtension
    {
        public static char? ToChar(this SplitChar splitChar)
        {
            return splitChar switch
            {
                SplitChar.None => null,
                SplitChar.Dot => '.',
                SplitChar.Comma => ',',
                SplitChar.Semicolon => ';',
                SplitChar.Space => ' ',
                SplitChar.Tab => '\t',
                SplitChar.Stroke => '|',
                _ => throw new ArgumentOutOfRangeException(nameof(splitChar), splitChar, null),
            };
        }
    }
}