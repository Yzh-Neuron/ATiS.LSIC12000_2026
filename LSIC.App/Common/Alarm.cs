namespace LSIC.App
{
    public class Alarm
    {
        public Alarm(string source, string message)
        {
            Time = DateTime.Now;
            Source = source;
            Message = message;
        }

        public DateTime Time { get; private set; }

        public string Source { get; private set; }

        public string Message { get; private set; }

        public string Suggestion { get; private set; }

        public override string ToString()
        {
            return $"[{Time:MM-dd HH:mm:ss}] [ {Source} ] : {Message}";
        }

        public string ToStringIgnoreTime()
        {
            return $"({Source}): {Message}";
        }
    }
}
