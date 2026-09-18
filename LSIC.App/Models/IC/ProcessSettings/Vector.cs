namespace LSIC.App.Models
{
    /// <summary>
    /// 向量
    /// </summary>
    public class Vector : ObservableObject
    {
        private string _patternFile = string.Empty;
        /// <summary>
        /// 向量文件
        /// </summary>
        public string PatternFile
        {
            get => _patternFile;
            set => SetProperty(ref _patternFile, value);
        }
        private string _registerFile = string.Empty;
        /// <summary>
        /// 寄存器文件
        /// </summary>
        public string RegisterFile
        {
            get => _registerFile;
            set => SetProperty(ref _registerFile, value);
        }
    }
}
