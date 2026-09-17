namespace LSIC.App.Models
{
    /// <summary>
    /// 向量（包含一个向量文件，寄存器文件）
    /// </summary>
    public class Vector : ObservableValidator, ICloneable
    {
        private string patternFile;
        public string PatternFile
        {
            get => patternFile;
            set => SetProperty(ref patternFile, value);
        }

        private string registerFile;
        public string RegisterFile
        {
            get => registerFile;
            set => SetProperty(ref registerFile, value);
        }

        public object Clone()
        {
            return new Vector
            {
                PatternFile = PatternFile,
                RegisterFile = RegisterFile
            };
        }
    }
}
