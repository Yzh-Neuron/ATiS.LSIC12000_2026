using ATIS.Pattern;

namespace LSIC.App.ViewModels
{
    public class PopAbmViewerViewModel : ObservableObject
    {
        private string _fileName;
        public string FileName
        {
            get => _fileName;
            set => SetProperty(ref _fileName, value);
        }
        private List<Segment> _segments;
        public List<Segment> Segments
        {
            get => _segments;
            set => SetProperty(ref _segments, value);
        }
        private Segment _selectedSegments;
        public Segment SelectedSegments
        {
            get => _selectedSegments;
            set => SetProperty(ref _selectedSegments, value);
        }
        public ICommand OpenAbmFileCommand { get; private set; }
        public PopAbmViewerViewModel()
        {
            OpenAbmFileCommand = new RelayCommand(OpenAbmFile);
        }
        private void OpenAbmFile()
        {
            string filter = $"{LangProvider.Instance.PatternFileText}|*.abm";
            string currentFile = FileName;

            var msgResult = MessageBus.Default.Send(MsgArgs.From(PopAbmViewerMessage.OpenFile, [filter, currentFile])).First();

            if (msgResult.Result is string filePath)
            {
                try
                {
                    FileName = filePath;
                    AbmReader abmReader = new AbmReader(filePath);
                    List<Segment> segments = new List<Segment>(abmReader.Boards.Count + 1);

                    var head = new Segment(abmReader.ToString(), LangProvider.Instance.AbmFileText);
                    segments.Add(head);

                    foreach (var abmBoard in abmReader.Boards)
                    {
                        segments.Add(new Segment(abmBoard.ToString(), LangProvider.Instance.DigitalBoardText + $" {abmBoard.Index}"));
                    }
                    Segments = segments;
                    SelectedSegments = head;
                }
                catch (Exception ex)
                {
                    MessageBus.Default.Send(MsgArgs.From(PopAbmViewerMessage.Growl, [ex.Message, MessageType.Error]));
                }
            }
        }
    }
    public class Segment
    {
        public Segment(object obj, string header)
        {
            Content = obj.ToString();
            Header = header;
        }
        public string Content { get; }
        public string Header { get; }
    }
}