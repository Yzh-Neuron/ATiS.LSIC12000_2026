namespace LSIC.App.Models
{
    /// <summary>
    /// 步
    /// </summary>
    public abstract class StepItem : ObservableValidator
    {
        private string _name = string.Empty;

        /// <summary>
        /// 步名称
        /// </summary>
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _dpsPlan = string.Empty;

        /// <summary>
        /// 电源方案名
        /// </summary>
        public string DpsPlan
        {
            get => _dpsPlan;
            set => SetProperty(ref _dpsPlan, value);
        }

        private bool _isInLoop;

        /// <summary>
        /// 是否落循环区间(视图用)
        /// </summary>
        public bool IsInLoop
        {
            get => _isInLoop;
            set => SetProperty(ref _isInLoop, value);
        }

        /// <summary>
        /// 子步次（有序）
        /// </summary>
        /// </summary>
        [ValidationMetadata(Method = nameof(ValidateVectors))]
        public ObservableCollection<Vector> Vectors { get; } = new();

        /// <summary>
        /// 条目类型(界面选择模板)
        /// </summary>
        public abstract StepItemType ItemType { get; }

        /// <summary>
        /// 是否固定块(视图用)
        /// </summary>
        public virtual bool IsFixed => false;

        public StepItem(string name)
        {
            Name = name;
            Vectors.CollectionChanged += (_, _) => ValidateProperty(nameof(Vectors));
            ValidateAllProperties();
        }

        #region validate methods
        protected ValidationResult ValidateVectors(ObservableCollection<Vector> value)
        {
            if (this is StepVector && value is not { Count: > 0 })
            {
                return new ValidationResult(LangProvider.Instance.AtleastOneSubStepMessage);
            }

            return ValidationResult.Success;
        }
        #endregion
    }
}
