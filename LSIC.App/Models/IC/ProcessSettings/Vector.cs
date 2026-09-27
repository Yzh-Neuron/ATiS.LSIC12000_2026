namespace LSIC.App.Models
{
    /// <summary>
    /// 向量
    /// </summary>
    public class Vector : ObservableValidator, ICloneable
    {
        private string _patternFile = string.Empty;
        /// <summary>
        /// 向量文件
        /// </summary>
        public string PatternFile
        {
            get => _patternFile;
            set => SetProperty(ref _patternFile, value, true);
        }
        private string _registerFile = string.Empty;
        /// <summary>
        /// 寄存器文件
        /// </summary>
        public string RegisterFile
        {
            get => _registerFile;
            set => SetProperty(ref _registerFile, value, true);
        }

        public object Clone()
        {
            return new Vector
            {
                PatternFile = PatternFile,
                RegisterFile = RegisterFile
            };
        }

        public void Copy(Vector clone)
        {
            PatternFile = clone.PatternFile;
            RegisterFile = clone.RegisterFile;
        }
    }
}
