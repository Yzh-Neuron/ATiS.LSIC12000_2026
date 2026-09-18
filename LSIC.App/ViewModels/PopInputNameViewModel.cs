namespace LSIC.App.ViewModels
{
    public class PopInputNameViewModel : ObservableObject
    {
        public PopInputNameViewModel(IList<string> filters)
        {
            _filters = filters;
        }

        private static readonly char[] IllegalChars = { '\\', '/', ':', '*', '?', '.', '\"', '<', '>', '|' };
        private readonly IList<string> _filters;

        public Guid Guid { get; } = Guid.NewGuid();

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        #region 依赖命令

        public ICommand ConfirmCommand => new RelayCommand(Confirm, CanConfirm);

        public ICommand CancelCommand => new RelayCommand(Cancel);

        #endregion

        private void Confirm()
        {
            MessageBus.Default.Send(MsgArgs.From(PopInputNameMessage.Confirm, [Name]));
        }

        private bool CanConfirm()
        {
            //if (ValidateInputName(Name) is { HasError: false })
            //{
            //    return true;
            //}
            return false;
        }

        private void Cancel()
        {
            MessageBus.Default.Send(MsgArgs.From(PopInputNameMessage.Cancel, []));
        }
    }
}