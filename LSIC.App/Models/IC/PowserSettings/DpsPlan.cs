namespace LSIC.App.Models
{
    /// <summary>
    /// 电源方案
    /// </summary>
    public class DpsPlan : ObservableObject
    {
        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        private ObservableCollection<DpsSetting> _dpssSettings;
        public ObservableCollection<DpsSetting> DpssSettings
        {
            get => _dpssSettings;
            set => SetProperty(ref _dpssSettings, value);
        }

    }
}
