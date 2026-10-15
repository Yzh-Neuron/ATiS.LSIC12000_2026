namespace LSIC.App.ViewModels
{
    public class PopDutMaskViewModel : ObservableObject
    {
        public int Rows { get; }
        public int Cols { get; }
        public ObservableCollection<DutMaskItem> Duts { get; }
        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }

        public PopDutMaskViewModel(BurnInBoard board)
        {
            Rows = board.Row;
            Cols = board.Column;

            Duts = new ObservableCollection<DutMaskItem>();
            foreach (BurnDut dut in board.Duts)
            {
                Duts.Add(new DutMaskItem(dut.Id, dut.Masked));
            }

            ConfirmCommand = new RelayCommand(Confirm);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void Confirm()
        {
            byte[] result = Duts.Where(x => x.Masked).Select(x => x.Id).IntegersToMask(32);
            MessageBus.Default.Send(MsgArgs.From(PopDutMaskMessage.Confirm, [result]));
        }

        private void Cancel()
        {
            MessageBus.Default.Send(MsgArgs.From(PopDutMaskMessage.Cancel, []));
        }

        /// <summary>
        /// 工位屏蔽项
        /// </summary>
        public class DutMaskItem : ObservableObject
        {
            public int Id { get; }

            private bool _masked;
            public bool Masked
            {
                get => _masked;
                set => SetProperty(ref _masked, value);
            }

            public DutMaskItem(int id, bool masked)
            {
                Id = id;
                _masked = masked;
            }
        }
    }
}