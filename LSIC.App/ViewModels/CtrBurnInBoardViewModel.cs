namespace LSIC.App.ViewModels
{
    public class CtrBurnInBoardViewModel : ObservableObject
    {
        private BurnInBoard _burnInBoard = null;
        public BurnInBoard BurnInBoard
        {
            get => _burnInBoard;
            set => SetProperty(ref _burnInBoard, value);
        }

        private BurnDut _selectedBurnDut = null;
        public BurnDut SelectedBurnDut
        {
            get => _selectedBurnDut;
            set => SetProperty(ref _selectedBurnDut, value);
        }
    }
}
