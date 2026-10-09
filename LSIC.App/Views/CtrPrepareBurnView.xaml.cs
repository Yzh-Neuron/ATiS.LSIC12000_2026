using System.Text.RegularExpressions;

namespace LSIC.App.Views
{
    public partial class CtrPrepareBurnView
    {
        public CtrPrepareBurnViewModel ViewModel { get; }
        public CtrPrepareBurnView()
        {
            InitializeComponent();
            ViewModel = new CtrPrepareBurnViewModel();
            DataContext = ViewModel;
        }
        public Zone Zone
        {
            get => (Zone)GetValue(ZoneProperty);
            set => SetValue(ZoneProperty, value);
        }
        public static readonly DependencyProperty ZoneProperty = DependencyProperty.Register(nameof(Zone), typeof(Zone), typeof(CtrPrepareBurnView), new PropertyMetadata(null, OnZoneChanged));
        private static void OnZoneChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CtrPrepareBurnView)?.UpdateZone(e.NewValue as Zone);
        }
        private void UpdateZone(Zone zone)
        {
            ViewModel.Zone = zone;
        }
    }
}

namespace LSIC.App.ViewModels
{
    public class CtrPrepareBurnViewModel : ObservableValidator
    {
        private static readonly Regex regex = new Regex(@"^[_a-zA-Z0-9]*$");
        private string filter;
        public string Filter
        {
            get => filter;
            set
            {
                if (filter != value)
                {
                    filter = value;
                    OnPropertyChanged(nameof(Filter));

                    Refresh();
                }
            }
        }
        private IcEntry[] ics;
        public IcEntry[] Ics
        {
            get => ics;
            private set => SetProperty(ref ics, value);
        }
        private Zone _zone;
        public Zone Zone
        {
            get => _zone;
            set => SetProperty(ref _zone, value);
        }
        private IcEntry selectedIc;
        public IcEntry SelectedIc
        {
            get => selectedIc;
            set => SetProperty(ref selectedIc, value);
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
            ConfirmCommand = new RelayCommand(Confirm);

            ValidateAllProperties();
        }
        private void Refresh()
        {
            Ics = IcConfigManager.GetIcMetas<IcEntry>(Filter);
        }
        private void Confirm()
        {
            List<ValidationResult> errors = this.GetAllErrorsRecursive();

            if (errors.Count > 0)
            {
                MessageBus.Default.Send(MsgArgs.From(WinMainMessage.ShowValidateError, [errors]));
                return;
            }

            try
            {
                Ic ic = IcConfigManager.GetIc(SelectedIc.Name);
                errors = ic.GetAllErrorsRecursive();
                if (errors.Count > 0)
                {
                    MessageBus.Default.Send(MsgArgs.From(WinMainMessage.ShowValidateError, [errors]));
                    return;
                }

                var burnInfo = SystemDatabase.Instance.CreateBurnInfo(Zone.Id, ic, Lot, Operator);
                Zone.ReloadBurnInfo(burnInfo);
            }
            catch (Exception e)
            {
                MessageBus.Default.Send(MsgArgs.From(WinMainMessage.Message, [e.Message, MessageType.Error]));
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
