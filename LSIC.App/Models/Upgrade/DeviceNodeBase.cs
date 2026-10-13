namespace LSIC.App.Models
{
    public abstract class DeviceNodeBase : ObservableObject
    {
        public ObservableCollection<Board> Boards { get; } = new();
        private bool _upgradeSelected;
        public bool UpgradeSelected
        {
            get => _upgradeSelected;
            set => SetProperty(ref _upgradeSelected, value);
        }
        private UpgradeSession _session;
        public UpgradeSession Session
        {
            get => _session;
            set => SetProperty(ref _session, value);
        }
        public abstract Task QueryVersionAsync();
        public void SendFirmware(byte[] payload, int boardCount)
        {
            Session = new UpgradeSession(boardCount);
            SendUpgrade(payload);
        }
        protected abstract void SendUpgrade(byte[] payload);
        protected void ClearBoards()
        {
            Application.Current.Dispatcher.Invoke(Boards.Clear);
        }
        protected void AddBoard(byte[] data)
        {
            var board = Board.FromBytes(data, BoardTypeCatalog.ByCode);
            Application.Current.Dispatcher.Invoke(() => Boards.Add(board));
        }
        protected void OnUpgradeProgress(byte[] data)
        {
            int index = 0;
            byte code = data[index++];
            byte id = data[index++];
            byte progress = data[index++];
            var state = (UpgradeState)data[index];
            Session?.SetReport(code, id, progress, state);
        }
        protected void OnUpgradeFinish()
        {
            Session?.Finish();
        }
    }
}