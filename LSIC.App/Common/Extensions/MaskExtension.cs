namespace LSIC.App
{
    internal static class MaskExtension
    {
        /// <summary>
        /// values = { 0, 1, 8 }, length = 2
        /// mask[]: ┌───────────────────────┐┌──────────────── ───  ┐
        ///         │bit7 bit6 bit5 bit4 bit3 bit2 bit1 bit0       ││bit7 bit6 bit5 bit4 bit3 bit2 bit1 bit0  │
        ///         │ 15   14   13   12   11   10    9 【8】       ││  7    6    5    4    3   2  【1】 【0】 │
        ///         │      mask[1]                                 ││      mask[0]                            │
        ///         └───────────────────────┘└──────────────── ───  ┘
        /// </summary>
        /// <param name="values"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte[] IntegersToMask(this IEnumerable<int> values, int length)
        {
            byte[] mask = new byte[length];
            foreach (int value in values)
            {
                int index = value / 8;
                int bits = value % 8;
                if (index < length)
                {
                    mask[index] |= (byte)(0x1 << bits);
                }
            }
            return mask;
        }

        public static int[] MaskToIntegers(this byte[] mask)
        {
            List<int> values = new List<int>();
            for (int i = 0; i < mask.Length; i++)
            {
                byte temp = mask[i];
                for (int j = 0; j < 8; j++)
                {
                    if (((temp >> j) & 0x1) == 0x1)
                    {
                        values.Add(i * 8 + j);
                    }
                }
            }
            return values.ToArray();
        }

    }
}
