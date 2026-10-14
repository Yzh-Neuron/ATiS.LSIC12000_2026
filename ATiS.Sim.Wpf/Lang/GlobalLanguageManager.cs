using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ATiS.Sim.Wpf
{
    /// <summary>
    /// 全局语言管理器
    /// </summary>
    public sealed class GlobalLanguageManager
    {
        private static readonly GlobalLanguageManager _instance;
        public static GlobalLanguageManager Instance => _instance;
        static GlobalLanguageManager()
        {
            _instance = new GlobalLanguageManager();
        }
        private volatile UiLanguage _currentLanguage = UiLanguage.Chinese;
        public static UiLanguage CurrentLanguage => _instance._currentLanguage;
        private volatile Dictionary<string, string> _mergedStringCache = new(StringComparer.Ordinal);
        private readonly Dictionary<UiLanguage, List<ResourceDictionary>> _languageResourceMap = new();
        private readonly List<LanguageProviderBase> _languageProviders = new();
        private readonly object _syncLock = new();
        public string this[string key]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    return string.Empty;
                }
                var cache = _mergedStringCache;
                return cache.TryGetValue(key, out var value) ? value : $"{key}";
            }
        }
        private GlobalLanguageManager()
        {
            foreach (UiLanguage lang in Enum.GetValues(typeof(UiLanguage)))
            {
                _languageResourceMap[lang] = new List<ResourceDictionary>();
            }

            InternalRegisterLanguageResources(
                UiLanguage.Chinese,
                new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/ATiS.Sim.Wpf;component/Themes/Basic/Languages/zh-CN.xaml")
                });
            InternalRegisterLanguageResources(
                UiLanguage.English,
                new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/ATiS.Sim.Wpf;component/Themes/Basic/Languages/en-US.xaml")
                });
        }
        private void InternalRegisterLanguageResources(UiLanguage language, params ResourceDictionary[] resourceDictionaries)
        {
            if (resourceDictionaries == null || resourceDictionaries.Length == 0)
            {
                return;
            }

            List<LanguageProviderBase>? providersCopy = null;

            lock (_syncLock)
            {
                if (_languageResourceMap.TryGetValue(language, out var dictList))
                {
                    dictList.AddRange(resourceDictionaries.Where(d => d != null));
                }

                if (_currentLanguage == language)
                {
                    RebuildMergedStringCache();
                    providersCopy = _languageProviders.ToList();
                }
            }

            if (providersCopy != null)
            {
                NotifyProviders(providersCopy);
            }
        }
        private void InternalRegisterLanguageProvider(LanguageProviderBase provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            List<LanguageProviderBase>? providersCopy = null;
            Type providerType = provider.GetType();

            lock (_syncLock)
            {
                if (_languageProviders.Any(t => t.GetType() == providerType))
                {
                    throw new InvalidOperationException(
                        $"The language provider of type '{providerType.FullName}' has already been registered. " +
                        $"Only one instance of the same type is allowed globally. " +
                        $"Do not instantiate it repeatedly or declare it multiple times in resource dictionaries. " +
                        $"Please reuse the instance via resource key reference or static singleton.");
                }

                provider.AttachLanguageManager(this);
                _languageProviders.Add(provider);
                providersCopy = _languageProviders.ToList();
            }

            NotifyProviders(providersCopy);
        }
        private void InternalSwitchLanguage(UiLanguage language)
        {
            List<LanguageProviderBase>? providersCopy = null;

            lock (_syncLock)
            {
                if (_currentLanguage == language)
                {
                    return;
                }

                _currentLanguage = language;
                RebuildMergedStringCache();
                providersCopy = _languageProviders.ToList();
            }

            NotifyProviders(providersCopy);
        }
        private void RebuildMergedStringCache()
        {
            var newCache = new Dictionary<string, string>(StringComparer.Ordinal);
            if (_languageResourceMap.TryGetValue(_currentLanguage, out var dictList))
            {
                foreach (var dict in dictList)
                {
                    EnumerateResourceDictionaryRecursive(dict, newCache);
                }
            }
            _mergedStringCache = newCache;
        }
        private void EnumerateResourceDictionaryRecursive(ResourceDictionary dict, Dictionary<string, string> targetCache, HashSet<ResourceDictionary>? visited = null)
        {
            visited ??= new HashSet<ResourceDictionary>();

            if (!visited.Add(dict))
            {
                return;
            }

            foreach (var keyObj in dict.Keys)
            {
                if (keyObj is string key && dict[key] is string value)
                {
                    targetCache[key] = value;
                }
            }

            foreach (var mergedDict in dict.MergedDictionaries)
            {
                EnumerateResourceDictionaryRecursive(mergedDict, targetCache, visited);
            }
        }
        private void NotifyProviders(List<LanguageProviderBase> providers)
        {
            foreach (var provider in providers)
            {
                try
                {
                    provider.OnLanguageChanged();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[{nameof(GlobalLanguageManager)}]{ex.Message}");
                }
            }
        }

        /// <summary>
        /// 注册指定语言的资源字典
        /// </summary>
        /// <param name="language">目标语言</param>
        /// <param name="resourceDictionaries">资源字典</param>
        public static void RegisterLanguageResources(UiLanguage language, params ResourceDictionary[] resourceDictionaries)
        {
            _instance.InternalRegisterLanguageResources(language, resourceDictionaries);
        }

        /// <summary>
        /// 切换界面语言
        /// </summary>
        /// <param name="language">目标语言</param>
        public static void SwitchLanguage(UiLanguage language)
        {
            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher != null && !dispatcher.CheckAccess())
            {
                dispatcher.Invoke(() => _instance.InternalSwitchLanguage(language));
                return;
            }
            _instance.InternalSwitchLanguage(language);
        }

        /// <summary>
        /// 注册语言提供者实例
        /// </summary>
        /// <param name="provider">语言提供者实例</param>
        /// <exception cref="ArgumentNullException">实例为null时抛出</exception>
        /// <exception cref="InvalidOperationException">同类型已注册时抛出</exception>
        internal static void RegisterLanguageProvider(LanguageProviderBase provider)
        {
            _instance.InternalRegisterLanguageProvider(provider);
        }
    }
}
