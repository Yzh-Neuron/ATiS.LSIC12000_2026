namespace LSIC.App.Models
{
    public class UpgradeItem : ObservableObject
    {
        public UpgradeItem(BoardTypeInfo type) => Type = type;

        public BoardTypeInfo Type { get; }
        public byte TypeCode => (byte)Type.TypeCode;
        public string DisplayName => LangProvider.Instance.GetString(Type.BoardKey);

        private string _file;
        public string File
        {
            get => _file;
            set => SetProperty(ref _file, value);
        }
    }
}
