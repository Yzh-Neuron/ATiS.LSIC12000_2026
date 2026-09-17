using System;
using System.Windows.Markup;

namespace ATiS.Sim.Wpf
{
    internal sealed partial class FrameworkLanguageProvider : LanguageProviderBase
    {
        public static FrameworkLanguageProvider Instance { get; } = new();
        public UiLanguage CurrentLanguage => GlobalLanguageManager.CurrentLanguage;

        /// <summary>消息</summary>
        public string Info => GetString("Info");
        /// <summary>警告</summary>
        public string Warning => GetString("Warning");
        /// <summary>错误</summary>
        public string Error => GetString("Error");
        /// <summary>致命</summary>
        public string Fatal => GetString("Fatal");
        /// <summary>成功</summary>
        public string Success => GetString("Success");
        /// <summary>询问</summary>
        public string Question => GetString("Question");
        /// <summary>确认</summary>
        public string ConfirmText => GetString("ConfirmText");
        /// <summary>取消</summary>
        public string CancelText => GetString("CancelText");
        /// <summary>是</summary>
        public string YesText => GetString("YesText");
        /// <summary>否</summary>
        public string NoText => GetString("NoText");
        /// <summary>无数据</summary>
        public string NullDataSet => GetString("NullDataSet");
        /// <summary>清除</summary>
        public string ClearText => GetString("ClearText");

        protected internal override void OnLanguageChanged()
        {
            OnPropertyChanged(nameof(CurrentLanguage));
            OnPropertyChanged(nameof(Info));
            OnPropertyChanged(nameof(Warning));
            OnPropertyChanged(nameof(Error));
            OnPropertyChanged(nameof(Fatal));
            OnPropertyChanged(nameof(Success));
            OnPropertyChanged(nameof(Question));
            OnPropertyChanged(nameof(ConfirmText));
            OnPropertyChanged(nameof(CancelText));
            OnPropertyChanged(nameof(YesText));
            OnPropertyChanged(nameof(NoText));
            OnPropertyChanged(nameof(NullDataSet));
            OnPropertyChanged(nameof(ClearText));
        }
    }

    internal static class LgK
    {
        /// <summary>消息</summary>
        public const string Info = nameof(Info);
        /// <summary>警告</summary>
        public const string Warning = nameof(Warning);
        /// <summary>错误</summary>
        public const string Error = nameof(Error);
        /// <summary>致命</summary>
        public const string Fatal = nameof(Fatal);
        /// <summary>成功</summary>
        public const string Success = nameof(Success);
        /// <summary>询问</summary>
        public const string Question = nameof(Question);
        /// <summary>确认</summary>
        public const string ConfirmText = nameof(ConfirmText);
        /// <summary>取消</summary>
        public const string CancelText = nameof(CancelText);
        /// <summary>是</summary>
        public const string YesText = nameof(YesText);
        /// <summary>否</summary>
        public const string NoText = nameof(NoText);
        /// <summary>无数据</summary>
        public const string NullDataSet = nameof(NullDataSet);
        /// <summary>清除</summary>
        public const string ClearText = nameof(ClearText);
    }


    [MarkupExtensionReturnType(typeof(object))]
    internal class LgExtension : MarkupExtension
    {
        public string PropertyName { get; }

        public LgExtension(string propertyName)
        {
            PropertyName = propertyName;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var binding = new System.Windows.Data.Binding(PropertyName)
            {
                Source = FrameworkLanguageProvider.Instance
            };
            return binding.ProvideValue(serviceProvider);
        }
    }
}