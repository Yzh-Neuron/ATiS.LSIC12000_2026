namespace LSIC.App
{
    public static class BurnInBoardSettingParser
    {
        public static BurnInBoardSetting Parse(string file)
        {
            if (string.IsNullOrWhiteSpace(file) || !File.Exists(file))
            {
                return null;
            }

            using CharReader reader = new CharReader(file);

            reader.SkipWhiteSpace();
            reader.ReadExpected("SIZE");
            reader.ReadWhiteSpace();
            int rows = reader.ReadNumber<int>();
            reader.SkipWhiteSpace();
            reader.ReadExpected(',');
            reader.SkipWhiteSpace();
            int columns = reader.ReadNumber<int>();

            var boardSetting = new BurnInBoardSetting(rows, columns);

            for (int i = 0; i < rows; i++)
            {
                reader.SkipWhiteSpace();
                reader.ReadExpected("ROW");
                reader.ReadWhiteSpace();
                reader.ReadExpected(i);

                for (int j = 0; j < columns; j++)
                {
                    DutSetting dut = ReadDut(reader, i, j);
                    boardSetting.DutSettings[i * columns + j] = dut;
                }
            }
            reader.SkipWhiteSpace();
            reader.ReadEnd();
            return boardSetting;
        }

        private static DutSetting ReadDut(CharReader reader, int row, int column)
        {
            reader.SkipWhiteSpace();
            reader.ReadExpected($"C{column}");

            reader.ReadWhiteSpace();

            int id = reader.ReadNumber<int>();

            reader.ReadWhiteSpace();

            List<int> drs = new List<int>();
            if (reader.Try(out int _))
            {
                while (true)
                {
                    int start = reader.ReadNumber<int>();
                    reader.SkipWhiteSpace();
                    if (reader.Try('-'))
                    {
                        reader.ReadExpected('-');
                        reader.SkipWhiteSpace();
                        int end = reader.ReadNumber<int>();
                        for (int i = start; i <= end; i++)
                        {
                            drs.Add(i);
                        }

                        reader.SkipWhiteSpace();

                        if (reader.Try(','))
                        {
                            reader.ReadExpected(',');
                            reader.SkipWhiteSpace();
                        }
                        else
                        {
                            break;
                        }
                    }
                    else if (reader.Try(','))
                    {
                        reader.ReadExpected(',');
                        drs.Add(start);
                        reader.SkipWhiteSpace();
                    }
                    else
                    {
                        drs.Add(start);
                        break;
                    }
                }
            }

            int cs = 0;
            if (reader.Try("CS"))
            {
                reader.ReadExpected("CS");
                cs = reader.ReadNumber<int>();
                reader.ReadWhiteSpace();
            }

            List<int> dpss = new List<int>();
            if (reader.Try("PWRSPLY"))
            {
                reader.ReadExpected("PWRSPLY");
                reader.ReadWhiteSpace();

                while (true)
                {
                    int start = reader.ReadNumber<int>();
                    reader.SkipWhiteSpace();
                    if (reader.Try('-'))
                    {
                        reader.ReadExpected('-');
                        reader.SkipWhiteSpace();
                        int end = reader.ReadNumber<int>();
                        for (int i = start; i <= end; i++)
                        {
                            dpss.Add(i);
                        }

                        reader.SkipWhiteSpace();

                        if (reader.Try(','))
                        {
                            reader.ReadExpected(',');
                            reader.SkipWhiteSpace();
                        }
                        else
                        {
                            break;
                        }
                    }
                    else if (reader.Try(','))
                    {
                        reader.ReadExpected(',');
                        dpss.Add(start);
                        reader.SkipWhiteSpace();
                    }
                    else
                    {
                        dpss.Add(start);
                        break;
                    }
                }
            }
            int? thermostatChannel = null;
            if (reader.Try("TEMPSPLY"))
            {
                reader.ReadExpected("TEMPSPLY");
                reader.ReadWhiteSpace();
                thermostatChannel = reader.ReadNumber<int>();
            }
            return new DutSetting(id, row, column, cs, drs.ToArray(), dpss.ToArray(), thermostatChannel);
        }
    }
}
