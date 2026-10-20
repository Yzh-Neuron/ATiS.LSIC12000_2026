namespace LSIC.App.Models
{
    public class UpgradeSession : ObservableObject
    {
        private readonly int _expectedCount;
        public UpgradeSession(int expectedCount)
        {
            _expectedCount = Math.Max(1, expectedCount);
            State = UpgradeState.Updating;
        }
        public ObservableCollection<UpgradeReport> Reports { get; } = new();
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
        public void SetReport(byte typeCode, byte id, byte progress, UpgradeState state)
        {
            var report = Reports.FirstOrDefault(r => r.TypeCode == typeCode && r.Id == id);
            if (report is null)
            {
                report = new UpgradeReport(typeCode, id);
                this.InvokeOnUI(() => Reports.Add(report));
            }
            report.Update(progress, state);
            Refresh();
        }
        public void Finish()
        {
            foreach (var r in Reports.Where(r => r.State is UpgradeState.Updating or UpgradeState.Erase))
            {
                r.Update(100, UpgradeState.Finish);
            }
            Refresh();
            var failed = Reports.FirstOrDefault(r => r.State != UpgradeState.Finish);
            State = failed is null ? UpgradeState.Finish : UpgradeState.Error;
            Progress = failed is null ? 100 : Progress;
        }
        private void Refresh()
        {
            Progress = Math.Min(100, Reports.Sum(r => r.Progress) / _expectedCount);

            var hasFail = Reports.Any(r => r.State >= UpgradeState.SwitchBoardFail);
            if (hasFail)
            {
                State = UpgradeState.Error;
            }
        }
    }
}
