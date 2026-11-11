namespace LSIC.App.ViewModels
{
    public class PopAbmViewerViewModel : ObservableObject
    {
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