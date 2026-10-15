namespace LSIC.App.ViewModels
{
    public class WinZoneListViewModel : ObservableObject
    {
        public Zone[] Zones { get; private set; }

        public WinZoneListViewModel()
        {
            Zones = Device.Instance.Zones.ToArray();
        }

    }
}
