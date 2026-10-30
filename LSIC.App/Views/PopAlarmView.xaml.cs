namespace LSIC.App.Views
{
    public partial class PopAlarmView
    {
        public bool IsExiting { get; set; }
        public PopAlarmView()
        {
            InitializeComponent();
        }
        public void Add(Alarm alarm)
        {
            Paragraph paragraph = new Paragraph(new Run(alarm.ToString()))
            {
                Foreground = Brushes.Red
            };

            var blocks = LogFlowDocument.Blocks;
            blocks.Add(paragraph);

            if (blocks.Count > 500)
            {
                while (blocks.Count > 400)
                {
                    blocks.Remove(blocks.FirstBlock);
                }
            }

            TryScollToBottom();
        }
        private void TryScollToBottom()
        {
            LogBox.ScrollToEnd();
        }
        private void OnClosing(object sender, CancelEventArgs e)
        {
            if (IsExiting)
            {
                return;
            }
            e.Cancel = true;
            LogFlowDocument.Blocks.Clear();
            Hide();
        }
    }
}