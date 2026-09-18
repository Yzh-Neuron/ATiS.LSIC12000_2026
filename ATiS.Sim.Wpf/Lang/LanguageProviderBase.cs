using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ATiS.Sim.Wpf
{
    /// <summary>
    /// 语言提供者抽象基类
    /// </summary>
    public abstract class LanguageProviderBase : INotifyPropertyChanged
    {
        private GlobalLanguageManager? _languageManager;
        protected LanguageProviderBase()
        {
            GlobalLanguageManager.RegisterLanguageProvider(this);
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        public string GetString(string key)
        {
            if (_languageManager == null)
            {
                throw new InvalidOperationException("Language manager is not initialized.");
            }
            return _languageManager[key];
        }
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            if (propertyName!=null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        protected internal abstract void OnLanguageChanged();

        internal void AttachLanguageManager(GlobalLanguageManager manager)
        {
            _languageManager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

    }
}
