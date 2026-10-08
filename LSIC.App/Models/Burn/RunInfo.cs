namespace LSIC.App.Models
{
    public class RunInfo : ObservableObject
    {
        private ExecuteMode mode;
        public ExecuteMode Mode
        {
            get => mode;
            set => SetProperty(ref mode, value);
        }

        public Progress Progress { get; } = new Progress();
    }
}