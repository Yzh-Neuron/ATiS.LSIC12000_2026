namespace LSIC.App.Models
{
    public class BurnInBoardSetting
    {
        public int Row { get; private set; }

        public int Column { get; private set; }

        public DutSetting[] DutSettings { get; private set; }

        public BurnInBoardSetting(int row, int column)
        {
            Row = row;
            Column = column;
            DutSettings = new DutSetting[row * column];
        }

    }
}
