namespace LSIC.App.Models
{
    public class StepVector : ObservableObject, ICloneable
    {
        private string _patternFile = string.Empty;
        public string PatternFile
        {
            get => _patternFile;
            set => SetProperty(ref _patternFile, value);
        }

        private string _registerFile = string.Empty;
        public string RegisterFile
        {
            get => _registerFile;
            set => SetProperty(ref _registerFile, value);
        }

        public object Clone()
        {
            return new StepVector
            {
                PatternFile = PatternFile,
                RegisterFile = RegisterFile
            };
        }
    }
}
