using System.Text.RegularExpressions;

namespace LSIC.App.ViewModels
{
    public class CtrPrepareBurnViewModel : ObservableValidator
    {
        private static readonly Regex regex = new Regex(@"^[_a-zA-Z0-9]*$");
        private string _filter;
        public string Filter
        {
            get => _filter;
            set
            {
                if (_filter != value)
                {
                    _filter = value;
                    OnPropertyChanged(nameof(Filter));

                    Refresh();
                }
            }
        }
        private IcEntry[] _ics;
        public IcEntry[] Ics
        {
            get => _ics;
            private set => SetProperty(ref _ics, value);
        }
        private Zone _zone;
        public Zone Zone
        {
            get => _zone;
            set => SetProperty(ref _zone, value);
        }
        private IcEntry _selectedIc;
        public IcEntry SelectedIc
        {
            get => _selectedIc;
            set => SetProperty(ref _selectedIc, value);
        }
        private string _lot;
        [ValidationMetadata(Method = nameof(ValidateLot))]
        public string Lot
        {
            get => _lot;
            set => SetProperty(ref _lot, value, true);
        }
        private string _operator;
        [ValidationMetadata(Method = nameof(ValidateOperator))]
        public string Operator
        {
            get => _operator;
            set => SetProperty(ref _operator, value, true);
        }
        public RelayCommand RefreshCommand { get; private set; }
        public RelayCommand ConfirmCommand { get; private set; }
        public CtrPrepareBurnViewModel()
        {
            Ics = IcConfigManager.GetIcMetas<IcEntry>();
            RefreshCommand = new RelayCommand(Refresh);
            ConfirmCommand = new RelayCommand(Confirm, CanConfirm);

            ValidateAllProperties();
        }
        private void Refresh()
        {
            Ics = IcConfigManager.GetIcMetas<IcEntry>(Filter);
        }
        private bool CanConfirm()
        {
            return SelectedIc?.Name != null;
        }

        private void Confirm()
        {
            List<ValidationResult> errors = this.GetAllExistingErrorsRecursive();

            if (errors.Count > 0)
            {
                MessageBus.Default.Send(MsgArgs.From(WinMainMessage.Growl, [errors.ToMessageString(), MessageType.Error]));
                return;
            }

            try
            {
                Ic ic = IcConfigManager.GetIc(SelectedIc.Name);
                errors = ic.GetAllExistingErrorsRecursive();
                if (errors.Count > 0)
                {
                    MessageBus.Default.Send(MsgArgs.From(WinMainMessage.Growl, [errors.ToMessageString(), MessageType.Error]));
                    return;
                }

                var burnInfo = SystemDatabase.Instance.CreateBurnInfo(Zone.Id, ic, Lot, Operator);
                Zone.ReloadBurnInfo(burnInfo);
            }
            catch (Exception e)
            {
                MessageBus.Default.Send(MsgArgs.From(WinMainMessage.Growl, [e.Message, MessageType.Error]));
            }
        }

        #region validate methods
        private ValidationResult ValidateLot(string lot)
        {
            if (!string.IsNullOrEmpty(lot) && !regex.IsMatch(lot))
            {
                return new ValidationResult(LangProvider.Instance.ValidStringRuleMessage, [LangProvider.Instance.LotText]);
            }

            return ValidationResult.Success;
        }
        private ValidationResult ValidateOperator(string op)
        {
            if (!string.IsNullOrEmpty(op) && !regex.IsMatch(op))
            {
                return new ValidationResult(LangProvider.Instance.ValidStringRuleMessage, [LangProvider.Instance.OperatorText]);
            }

            return ValidationResult.Success;
        }
        #endregion
    }
}
