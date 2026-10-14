namespace LSIC.App
{
    public class DataLine
    {
        public DataLine(DateTime time, string[] columns)
        {
            Time = time;
            Columns = columns;
        }

        public DateTime Time { get; private set; }

        public string[] Columns { get; private set; }
    }

}
