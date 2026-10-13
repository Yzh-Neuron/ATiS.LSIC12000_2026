namespace LSIC.App.Models
{
    public class UpgradeReport : ObservableObject
    {
        public UpgradeReport(byte typeCode, byte id)
        {
            TypeCode = typeCode;
            Id = id;
        }

        public byte TypeCode { get; }
        public byte Id { get; }

        private int _progress;
        public int Progress
        {
            get => _progress;
            private set => SetProperty(ref _progress, value);
        }

        private UpgradeState _state;
        public UpgradeState State
        {
            get => _state;
            private set => SetProperty(ref _state, value);
        }

        public void Update(byte progress, UpgradeState state)
        {
            Progress = progress;
            State = state;
        }
    }
}