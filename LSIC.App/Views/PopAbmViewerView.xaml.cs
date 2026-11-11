using ATIS.Pattern;

namespace LSIC.App.Views
{
    public partial class PopAbmViewerView
    {
        public PopAbmViewerViewModel ViewModel { get; }
        public PopAbmViewerView(string title, string file)
        {
            if (GlobalLanguageManager.CurrentLanguage == UiLanguage.English)
            {
                PatternLanguage.Notify(PatternLanguage.English);
            }
            else
            {
                PatternLanguage.Notify(PatternLanguage.Chinese);
            }

            InitializeComponent();
            Title = title;
            ViewModel = new PopAbmViewerViewModel();
            DataContext = ViewModel;

            try
            {
                AbmReader abmReader = new AbmReader(file);
                List<Segment> segments = new List<Segment>(abmReader.Boards.Count + 1);

                var head = new Segment(abmReader.ToString(), LangProvider.Instance.AbmFileText);
                segments.Add(head);

                foreach (var abmBoard in abmReader.Boards)
                {
                    segments.Add(new Segment(abmBoard.ToString(), LangProvider.Instance.DigitalBoardText + $" {abmBoard.Index}"));
                }
                ViewModel.Segments = segments;
                ViewModel.SelectedSegments = head;
            }
            catch (Exception ex)
            {
                Growl.Show(new GrowlInfo { Message = ex.Message, Type = MessageType.Error });
            }
        }
    }
}