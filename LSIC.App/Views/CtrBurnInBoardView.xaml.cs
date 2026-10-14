namespace LSIC.App.Views
{
    public partial class CtrBurnInBoardView
    {
        public CtrBurnInBoardViewModel ViewModel { get; }
        public CtrBurnInBoardView()
        {
            InitializeComponent();
            ViewModel = new CtrBurnInBoardViewModel();
            DataContext = ViewModel;
        }
        public BurnInBoard BurnInBoard
        {
            get { return (BurnInBoard)GetValue(BurnInBoardProperty); }
            set { SetValue(BurnInBoardProperty, value); }
        }

        public static readonly DependencyProperty BurnInBoardProperty = DependencyProperty.Register(nameof(BurnInBoard), typeof(BurnInBoard), typeof(CtrBurnInBoardView), new PropertyMetadata(null, OnBurnInBoardChanged));

        private static void OnBurnInBoardChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CtrBurnInBoardView)?.UpdateBurnInfo(e.NewValue as BurnInBoard);
        }

        private void UpdateBurnInfo(BurnInBoard burnInBoard)
        {
            ViewModel.BurnInBoard = burnInBoard;
        }
    }
}

namespace LSIC.App.ViewModels
{
    public class CtrBurnInBoardViewModel : ObservableObject
    {
        private BurnInBoard _burnInBoard = null;
        public BurnInBoard BurnInBoard
        {
            get { return _burnInBoard; }
            set { SetProperty(ref _burnInBoard, value); }
        }

        private BurnDut _selectedBurnDut = null;
        public BurnDut SelectedBurnDut
        {
            get { return _selectedBurnDut; }
            set { SetProperty(ref _selectedBurnDut, value); }
        }
    }
}
