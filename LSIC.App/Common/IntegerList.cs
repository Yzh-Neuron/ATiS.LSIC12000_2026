namespace LSIC.App
{
    public class IntegerList : List<int>
    {
        private IntegerList(List<int> list) : base(list)
        {

        }

        public static implicit operator IntegerList(string expr)
        {
            using MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(expr));
            using CharReader reader = new CharReader(stream);

            List<int> list = new List<int>();

            reader.SkipWhiteSpace();
            reader.ReadExpected('[');
            reader.SkipWhiteSpace();

            while (!reader.End)
            {
                reader.SkipWhiteSpace();

                if (reader.Try(']'))
                {
                    reader.ReadExpected(']');
                    reader.SkipWhiteSpace();
                    continue;
                }

                int value = reader.ReadNumber<int>();
                list.Add(value);

                reader.SkipWhiteSpace();

                if (reader.Try(','))
                {
                    reader.ReadExpected(',');
                    reader.SkipWhiteSpace();
                    continue;
                }

                reader.ReadExpected(']');
                reader.SkipWhiteSpace();
            }

            return new IntegerList(list);
        }
    }
}