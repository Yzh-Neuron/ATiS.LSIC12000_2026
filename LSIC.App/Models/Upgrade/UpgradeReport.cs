namespace LSIC.App.Models
{
    public class UpgradeReport : ObservableObject
    {
        public UpgradeReport(byte typeCode, byte id)
        {
            BoardTypeInfo typeInfo = BoardTypeCatalog.ByCode.TryGetValue(typeCode, out var def) ? def : BoardTypeInfo.Unknown(typeCode);
            DisplayName = LangProvider.Instance.GetString(typeInfo.BoardKey);
            TypeCode = typeCode;
            Id = id;
        }
        public string DisplayName { get; }
        public byte TypeCode { get; }
        public byte Id { get; }
        private int _progress;
        public int Progress
        {
            get => _progress;
            private set
            {
                if (SetProperty(ref _progress, value))
                {
                    OnPropertyChanged(nameof(Result));
                }
            }
        }
        private UpgradeState _state;
        public UpgradeState State
        {
            get => _state;
            private set
            {
                if (SetProperty(ref _state, value))
                {
                    OnPropertyChanged(nameof(Result));
                    HasError = value >= UpgradeState.SwitchBoardFail;
                }
            }
        }
        private bool _hasError;
        public bool HasError
        {
            get => _hasError;
            private set => SetProperty(ref _hasError, value);
        }
        public string Result => $"{DisplayName}_{{{Id}}}: {State.GetString()} / {Progress} %";
        public void Update(byte progress, UpgradeState state)
        {
            Progress = progress;
            State = state;
        }
    }
}