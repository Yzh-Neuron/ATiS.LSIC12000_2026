namespace LSIC.App.Models
{
    public class FrameFilterItem : ObservableObject
    {
        public FrameFilterItem(Enum functionCode)
        {
            _functionCode = functionCode;
        }

        private Enum _functionCode;
        public Enum FunctionCode
        {
            get => _functionCode;
            set => SetProperty(ref _functionCode, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}
