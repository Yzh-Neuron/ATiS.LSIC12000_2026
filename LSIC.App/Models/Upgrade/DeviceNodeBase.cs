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
        public async Task SendFirmware(byte[] payload, int boardCount)
        {
            Session = new UpgradeSession(boardCount);
            await SendUpgrade(payload);
        }
        protected abstract Task SendUpgrade(byte[] payload);
        protected void ClearBoards()
        {
            this.InvokeOnUI(Boards.Clear);
        }
        protected void AddBoard(byte[] data)
        {
            var board = Board.FromBytes(data, BoardTypeCatalog.ByCode);
            this.InvokeOnUI(() => Boards.Add(board));
        }
        protected void OnUpgradeProgress(byte[] data)
        {
            int index = 0;
            byte code = data[index++];              // 板卡类型
            byte id = data[index++];                // id
            byte progress = data[index++];          // 进度
            var state = (UpgradeState)data[index];  // 状态
            Session?.SetReport(code, id, progress, state);
        }
        protected void OnUpgradeFinish()
        {
            Session?.Finish();
        }
    }
}