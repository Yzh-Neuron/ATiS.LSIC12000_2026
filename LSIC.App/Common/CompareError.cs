namespace LSIC.App
{
    public class CompareError
    {
        public ulong Id { get; set; }

        public int Cs { get; set; }

        public int Dr { get; set; }

        public string Step { get; set; }

        public string Pattern { get; set; }

        public string Register { get; set; }

        public int Line { get; set; }

        public DateTime Time { get; set; }

        public static ulong CreateCompareErrorId(int stepId, int vectorStepId, int cs, int line, int dr)
        {
            /*
             error id construction
             ------------------------------------------
             | FF   | FF      | FF | FF FF FF FF | FF |
             | ----   ------- | -- | ----------- | -- |
             | Step | Vector  | CS | Line        | Dr |
             ------------------------------------------
            6 + 6 + 6 + 24 + 8
            */
            ulong id = (ulong)stepId << 56;
            id |= (ulong)vectorStepId << 48;
            id |= (ulong)cs << 40;
            id |= (ulong)line << 8;
            id |= (ulong)dr << 0;
            return id;
        }
    }
}
