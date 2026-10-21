namespace LSIC.App
{
    public static class ThemeManager
    {
        private sealed record SkinPair(Uri Light, Uri Dark, string Marker);
        private static readonly SkinPair[] Skins =
        {
                new(new Uri("pack://application:,,,/ATiS.Sim.Wpf;component/Themes/Basic/Skins/SkinLight.xaml", UriKind.Absolute),
                    new Uri("pack://application:,,,/ATiS.Sim.Wpf;component/Themes/Basic/Skins/SkinDark.xaml",  UriKind.Absolute),
                    "ATiS.Sim.Wpf"),

                new(new Uri("pack://application:,,,/LSIC.App;component/Themes/Basic/Skins/SkinLight.xaml", UriKind.Absolute),
                    new Uri("pack://application:,,,/LSIC.App;component/Themes/Basic/Skins/SkinDark.xaml",  UriKind.Absolute),
                "LSIC.App")
            };

        private static ThemeType? _current;
        public static ThemeType Current => _current ?? DetectCurrent();
        public static void Toggle()
        {
            Apply(Current == ThemeType.Light ? ThemeType.Dark : ThemeType.Light);
        }
        public static void Apply(ThemeType theme)
        {
            var dictionaries = Application.Current.Resources.MergedDictionaries;

            foreach (var skin in Skins)
            {
                var desired = theme == ThemeType.Light ? skin.Light : skin.Dark;
                int index = FindSkinIndex(dictionaries, skin.Marker);

                if (index >= 0)
                {
                    if (string.Equals(dictionaries[index].Source?.OriginalString, desired.OriginalString, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    dictionaries[index] = new ResourceDictionary { Source = desired };
                }
                else
                {
                    dictionaries.Insert(0, new ResourceDictionary { Source = desired });
                }
            }

            _current = theme;
        }
        private static ThemeType DetectCurrent()
        {
            foreach (var skin in Skins)
            {
                int index = FindSkinIndex(Application.Current.Resources.MergedDictionaries, skin.Marker);
                if (index < 0)
                {
                    continue;
                }

                string source = Application.Current.Resources.MergedDictionaries[index].Source?.OriginalString ?? string.Empty;
                _current = source.Contains("SkinDark", StringComparison.OrdinalIgnoreCase) ? ThemeType.Dark : ThemeType.Light;
                return _current.Value;
            }

            _current = ThemeType.Light;
            return _current.Value;
        }
        private static int FindSkinIndex(Collection<ResourceDictionary> dictionaries, string assemblyMarker)
        {
            for (int i = 0; i < dictionaries.Count; i++)
            {
                string source = dictionaries[i].Source?.OriginalString;
                if (source is null)
                {
                    continue;
                }

                if (source.Contains(assemblyMarker, StringComparison.OrdinalIgnoreCase)
                    && source.Contains("/Skins/Skin", StringComparison.OrdinalIgnoreCase)
                    && source.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}