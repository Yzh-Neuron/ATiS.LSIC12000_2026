namespace LSIC.App.Models
{
    /// <summary>
    /// 向量
    /// </summary>
    public class Vector : ObservableValidator, ICloneable
    {
        private string _patternFile = string.Empty;
        [ValidationMetadata(Method = nameof(ValidatePatternFile))]
        public string PatternFile
        {
            get => _patternFile;
            set => SetProperty(ref _patternFile, value, true);
        }

        private string _registerFile = string.Empty;
        [ValidationMetadata(Method = nameof(ValidateRegisterFile))]
        public string RegisterFile
        {
            get => _registerFile;
            set => SetProperty(ref _registerFile, value, true);
        }
        public Vector()
        {
            ValidateAllProperties();
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

        #region validate methods
        private ValidationResult ValidatePatternFile(string file)
        {
            if (string.IsNullOrWhiteSpace(file))
            {
                return new ValidationResult(string.Format(LangProvider.Instance.PleaseSetMessage, LangProvider.Instance.PatternFileText));
            }

            if (!File.Exists(file))
            {
                return new ValidationResult(LangProvider.Instance.FileNotExistMessage + $"[ {file} ]");
            }

            using FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read);
            string errorMessage = string.Format(LangProvider.Instance.FileLengthErrorMessage, file, 4);
            bool error = stream.Length < 4;
            if (!error)
            {
                byte[] temp = new byte[4];
                stream.ReadExactly(temp, 0, temp.Length);
                int block = temp.ToInt32(0, Endianness.Big);
                if (2L * (block + 4) + 4 != stream.Length)
                {
                    errorMessage = string.Format(LangProvider.Instance.AbmFileNotMatchMessage, file);
                    error = true;
                }
            }
            if (error)
            {
                return new ValidationResult(errorMessage);
            }

            return ValidationResult.Success;
        }
        private ValidationResult ValidateRegisterFile(string file)
        {
            if (string.IsNullOrWhiteSpace(file))
            {
                return new ValidationResult(string.Format(LangProvider.Instance.PleaseSetMessage, LangProvider.Instance.RegisterFileText));
            }

            if (!File.Exists(file))
            {
                return new ValidationResult(LangProvider.Instance.FileNotExistMessage + $"[ {file} ]");
            }

            try
            {
                _ = RegisterParser.Parse(file);
                return ValidationResult.Success;
            }
            catch (CharReaderException e)
            {
                return new ValidationResult(string.Format(LangProvider.Instance.FileParseErrorMessage, file, e.Line, e.Column));
            }
            catch (Exception e)
            {
                return new ValidationResult(e.Message);
            }
        }
        #endregion
    }
}
