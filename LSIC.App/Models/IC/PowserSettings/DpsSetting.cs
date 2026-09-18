namespace LSIC.App.Models
{
    public class DpsSetting : ObservableObject
    {
        private DpsInfo _dpsInfo;

        public int Id { get; private set; }
        public string Name { get; private set; }

        private bool _enable;
        public bool Enable
        {
            get => _enable;
            set => SetProperty(ref _enable, value);
        }

        private int _vol;
        public int Vol
        {
            get => _vol;
            set => SetProperty(ref _vol, value);
        }

        private int _uvp;
        public int Uvp
        {
            get => _uvp;
            set => SetProperty(ref _uvp, value);
        }

        private int _ovp;
        public int Ovp
        {
            get => _ovp;
            set => SetProperty(ref _ovp, value);
        }

        private int _ocp;
        public int Ocp
        {
            get => _ocp;
            set => SetProperty(ref _ocp, value);
        }

        private int _raise;
        public int Raise
        {
            get => _raise;
            set => SetProperty(ref _raise, value);
        }

        private int _delay;
        public int Delay
        {
            get => _delay;
            set => SetProperty(ref _delay, value);
        }

        public static DpsSetting Create(DpsInfo dpsInfo)
        {
            DpsSetting dpsSetting = new DpsSetting
            {
                Name = dpsInfo.Name,
                Id = dpsInfo.Id,
                _dpsInfo = dpsInfo
            };

            return dpsSetting;
        }
    }
}
