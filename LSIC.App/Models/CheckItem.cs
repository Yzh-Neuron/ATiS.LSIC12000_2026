namespace LSIC.App.Models
{
    public class CheckItem : ObservableObject
    {
        public CheckItem(string name)
        {
            Name = name;
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private bool? _connected;
        public bool? Connected
        {
            get => _connected;
            set => SetProperty(ref _connected, value);
        }
    }
}
