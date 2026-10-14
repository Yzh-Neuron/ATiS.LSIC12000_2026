namespace LSIC.App
{
    public class LogColumn
    {
        public IntegerList RuleIds { get; private set; }

        public string Header { get; private set; }

        public Formats Formats { get; private set; }

        public double Scale { get; private set; }

        public bool Plotable { get; private set; }
    }
}