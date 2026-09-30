namespace LSIC.App.Models
{
    /// <summary>
    /// 电源方案
    /// </summary>
    public class DpsPlan : ObservableValidator
    {
        public static DpsPlan None { get; } = new DpsPlan(string.Empty);
        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        public ObservableCollection<DpsSetting> DpssSettings { get; } = new();
        public DpsPlan(string name)
        {
            Name = name;
        }

    }
}