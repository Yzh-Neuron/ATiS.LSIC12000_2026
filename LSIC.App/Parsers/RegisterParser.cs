namespace LSIC.App
{
    public static class RegisterParser
    {
        private const int MaxChannel = 128;

        public static Register Parse(string file)
        {
            Register registers = new Register();
            using CharReader reader = new CharReader(file);

            int rradr = 0;
            while (!reader.End)
            {
                reader.SkipWhiteSpace();

                bool success = reader.Try(out Register.Command cmd);
                if (!success)
                {
                    reader.ReadStringUntil('\n');
                    continue;
                }
                reader.ReadExpected(cmd);
                switch (cmd)
                {
                    case Register.Command.VIH:
                        {
                            reader.SkipWhiteSpace();
                            reader.ReadExpected(',');
                            reader.SkipWhiteSpace();
                            int index = reader.ReadNumber<int>();
                            reader.ReadWhiteSpace();
                            float vih = reader.ReadNumber<float>();
                            registers.VIH[index] = vih;
                        }
                        break;
                    case Register.Command.VIHCLAMP:
                        {
                            reader.SkipWhiteSpace();
                            reader.ReadExpected(',');
                            reader.SkipWhiteSpace();
                            int index = reader.ReadNumber<int>();
                            reader.ReadWhiteSpace();
                            float clamp = reader.ReadNumber<float>();
                            registers.VIHClamp[index] = clamp;
                        }
                        break;
                    case Register.Command.VH_SEL:
                        {
                            reader.ReadWhiteSpace();
                            int[] channels = ReadChannels(reader);
                            reader.ReadWhiteSpace();
                            int index = reader.ReadNumber<int>();
                            registers.VHSel.Add(new KeyValuePair<int[], int>(channels, index));
                        }
                        break;
                    case Register.Command.VRH:
                        {
                            reader.SkipWhiteSpace();
                            reader.ReadExpected(',');
                            reader.SkipWhiteSpace();
                            int index = reader.ReadNumber<int>();
                            reader.ReadWhiteSpace();
                            float vrh = reader.ReadNumber<float>();
                            registers.VRH[index] = vrh;
                        }
                        break;
                    case Register.Command.VRL:
                        {
                            reader.SkipWhiteSpace();
                            reader.ReadExpected(',');
                            reader.SkipWhiteSpace();
                            int index = reader.ReadNumber<int>();
                            reader.ReadWhiteSpace();
                            float vrl = reader.ReadNumber<float>();
                            registers.VRL[index] = vrl;
                        }
                        break;
                    case Register.Command.RRADR:
                        {
                            reader.ReadWhiteSpace();
                            rradr = reader.ReadNumber<int>();
                        }
                        break;
                    case Register.Command.REP_RATE:
                        {
                            reader.ReadWhiteSpace();
                            int period = reader.ReadNumber<int>();
                            registers.Period[rradr] = period;
                        }
                        break;
                    case Register.Command.CLOCK:
                        {
                            int n = reader.ReadNumber<int>();
                            reader.ReadWhiteSpace();
                            int[] channels = ReadChannels(reader);
                            reader.ReadWhiteSpace();
                            int le = reader.ReadNumber<int>();
                            reader.SkipWhiteSpace();
                            reader.ReadExpected(',');
                            reader.SkipWhiteSpace();
                            int te = reader.ReadNumber<int>();
                            if (!registers.Clock.ContainsKey(n))
                            {
                                registers.Clock[n] = new List<KeyValuePair<int[], KeyValuePair<int, int>>>();
                            }
                            registers.Clock[n].Add(new KeyValuePair<int[], KeyValuePair<int, int>>(channels, new KeyValuePair<int, int>(le, te)));
                        }
                        break;
                    case Register.Command.CLOCKLE:
                        {
                            int n = reader.ReadNumber<int>();
                            reader.ReadWhiteSpace();
                            int[] channels = ReadChannels(reader);
                            reader.ReadWhiteSpace();
                            int le = reader.ReadNumber<int>();
                            if (!registers.ClockLE.ContainsKey(n))
                            {
                                registers.ClockLE[n] = new List<KeyValuePair<int[], int>>();
                            }
                            registers.ClockLE[n].Add(new KeyValuePair<int[], int>(channels, le));
                        }
                        break;
                    case Register.Command.CLOCKTE:
                        {
                            int n = reader.ReadNumber<int>();
                            reader.ReadWhiteSpace();
                            int[] channels = ReadChannels(reader);
                            reader.ReadWhiteSpace();
                            int te = reader.ReadNumber<int>();
                            if (!registers.ClockTE.ContainsKey(n))
                            {
                                registers.ClockTE[n] = new List<KeyValuePair<int[], int>>();
                            }
                            registers.ClockTE[n].Add(new KeyValuePair<int[], int>(channels, te));
                        }
                        break;
                    case Register.Command.STROBE:
                        {
                            int n = reader.ReadNumber<int>();
                            reader.ReadWhiteSpace();
                            int[] channels = ReadChannels(reader);
                            reader.ReadWhiteSpace();
                            int strobe = reader.ReadNumber<int>();
                            if (!registers.Strobe.ContainsKey(n))
                            {
                                registers.Strobe[n] = new List<KeyValuePair<int[], int>>();
                            }
                            registers.Strobe[n].Add(new KeyValuePair<int[], int>(channels, strobe));
                        }
                        break;
                    case Register.Command.FORMAT:
                        {
                            reader.ReadWhiteSpace();
                            int[] channels = ReadChannels(reader);
                            reader.ReadWhiteSpace();
                            Register.OutputFormat fmt = reader.ReadEnum<Register.OutputFormat>();
                            registers.Format.Add(new KeyValuePair<int[], Register.OutputFormat>(channels, fmt));
                        }
                        break;
                    case Register.Command.MODSTADR:
                        {
                            reader.ReadWhiteSpace();
                            registers.ModStart = reader.ReadHexOrDec();
                        }
                        break;
                    case Register.Command.MODEOF:
                        {
                            reader.ReadWhiteSpace();
                            registers.ModEof = reader.ReadHexOrDec();
                        }
                        break;
                    case Register.Command.CSDRVID:
                        {
                            reader.ReadWhiteSpace();
                            int channel = ReadChannel(reader);
                            reader.ReadWhiteSpace();
                            int cs = reader.ReadHexOrDec();
                            registers.CsDrvid.Add(new KeyValuePair<int[], int>(new int[] { channel }, cs));
                        }
                        break;
                    case Register.Command.CSDEFALT:
                        {
                            reader.ReadWhiteSpace();
                            int[] channels = ReadChannels(reader);
                            reader.ReadWhiteSpace();
                            Register.ChipSelectOutputFormat fmt = reader.ReadEnum<Register.ChipSelectOutputFormat>();
                            registers.CsDefault.Add(new KeyValuePair<int[], Register.ChipSelectOutputFormat>(channels, fmt));
                        }
                        break;
                    case Register.Command.HFCFREQ:
                        {
                            reader.ReadWhiteSpace();
                            int index = reader.ReadNumber<int>();
                            reader.ReadWhiteSpace();
                            int value = reader.ReadNumber<int>();
                            registers.HightFrequency[index] = value;
                        }
                        break;
                    case Register.Command.COMPMODE:
                        {
                            reader.ReadWhiteSpace();
                            int[] channels = ReadChannels(reader);
                            reader.ReadWhiteSpace();
                            Register.CompareMode mode = reader.ReadEnum<Register.CompareMode>();
                            registers.CompareModes.Add(new KeyValuePair<int[], Register.CompareMode>(channels, mode));
                        }
                        break;
                }
                reader.SkipWhiteSpace();
            }
            return registers;
        }

        private static int ReadChannel(CharReader reader)
        {
            reader.SkipWhiteSpace();
            reader.ReadExpected('<');
            reader.SkipWhiteSpace();
            int channel = reader.ReadNumber<int>();
            reader.SkipWhiteSpace();
            reader.ReadExpected('>');
            return channel;
        }

        private static int[] ReadChannels(CharReader reader)
        {
            List<int> list = new List<int>();
            reader.SkipWhiteSpace();
            reader.ReadExpected('<');
            reader.SkipWhiteSpace();

            if (reader.Try("ALL"))
            {
                reader.ReadExpected("ALL");
                list.AddRange(Enumerable.Range(0, MaxChannel));
            }
            else
            {
                while (true)
                {
                    int start = reader.ReadNumber<int>();
                    reader.SkipWhiteSpace();
                    if (reader.Try("-"))
                    {
                        reader.ReadExpected('-');
                        reader.SkipWhiteSpace();
                        int end = reader.ReadNumber<int>();
                        for (int i = start; i <= end; i++)
                        {
                            if (!list.Contains(i))
                            {
                                list.Add(i);
                            }
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
                        if (!list.Contains(start))
                        {
                            list.Add(start);
                        }
                        reader.SkipWhiteSpace();
                    }
                    else
                    {
                        if (!list.Contains(start))
                        {
                            list.Add(start);
                        }
                        break;
                    }
                }
            }

            reader.SkipWhiteSpace();
            reader.ReadExpected('>');

            return list.ToArray();
        }
    }
}
