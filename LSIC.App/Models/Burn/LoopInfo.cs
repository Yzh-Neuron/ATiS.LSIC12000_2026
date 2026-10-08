namespace LSIC.App.Models
{
    public class LoopInfo : ObservableObject
    {
        private bool _enable;
        public bool Enable
        {
            get => _enable;
            set => SetProperty(ref _enable, value);
        }

        private BurnStepVector _start;
        public BurnStepVector Start
        {
            get => _start;
            set => SetProperty(ref _start, value);
        }

        private BurnStepVector _end;
        public BurnStepVector End
        {
            get => _end;
            set => SetProperty(ref _end, value);
        }

        private ExecuteMode _mode;
        public ExecuteMode Mode
        {
            get => _mode;
            set => SetProperty(ref _mode, value);
        }

        public Progress Progress { get; } = new Progress();
    }
}