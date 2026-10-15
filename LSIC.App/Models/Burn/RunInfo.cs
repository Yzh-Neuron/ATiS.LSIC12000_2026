namespace LSIC.App.Models
{
    public class RunInfo : ObservableObject
    {
        private ExecuteMode _mode;
        public ExecuteMode Mode
        {
            get => _mode;
            set => SetProperty(ref _mode, value);
        }

        public Progress Progress { get; } = new Progress();
    }
}