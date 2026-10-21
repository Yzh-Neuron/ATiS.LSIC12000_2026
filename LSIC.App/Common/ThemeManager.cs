namespace LSIC.App
{
    public static class ThemeManager
    {
        private const string SkinLightUri = "pack://application:,,,/ATiS.Sim.Wpf;component/Themes/Skins/Basic/SkinLight.xaml";
        private const string SkinDarkUri = "pack://application:,,,/ATiS.Sim.Wpf;component/Themes/Skins/Basic/SkinDark.xaml";

        private static ThemeType? _current;

        public static ThemeType Current => _current ?? DetectCurrent();

        public static void Toggle()
        {
            Apply(Current == ThemeType.Light ? ThemeType.Dark : ThemeType.Light);
        }

        public static void Apply(ThemeType theme)
        {
            var dictionaries = Application.Current.Resources.MergedDictionaries;
            var next = new ResourceDictionary
            {
                Source = new Uri(theme == ThemeType.Light ? SkinLightUri : SkinDarkUri, UriKind.Absolute)
            };

            int index = IndexOfSkin(dictionaries);
            if (index >= 0)
            {
                dictionaries[index] = next;
            }
            else
            {
                dictionaries.Insert(0, next);
            }

            _current = theme;
        }

        private static ThemeType DetectCurrent()
        {
            int index = IndexOfSkin(Application.Current.Resources.MergedDictionaries);
            if (index < 0)
            {
                _current = ThemeType.Light;
                return _current.Value;
            }

            string source = Application.Current.Resources.MergedDictionaries[index].Source?.OriginalString ?? string.Empty;
            _current = source.Contains("SkinDark.xaml", StringComparison.Ordinal) ? ThemeType.Dark : ThemeType.Light;
            return _current.Value;
        }

        private static int IndexOfSkin(Collection<ResourceDictionary> dictionaries)
        {
            for (int i = 0; i < dictionaries.Count; i++)
            {
                string source = dictionaries[i].Source?.OriginalString;
                if (source != null && (source.Contains("Skins/SkinLight.xaml", StringComparison.Ordinal) || source.Contains("Skins/SkinDark.xaml", StringComparison.Ordinal)))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}