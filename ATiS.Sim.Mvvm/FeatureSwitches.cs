using System;
using System.Runtime.CompilerServices;

namespace ATiS.Sim.Mvvm
{
    internal static class FeatureSwitches
    {
        private const string EnableINotifyPropertyChangingSupportPropertyName = "ENABLE_INOTIFYPROPERTYCHANGING_SUPPORT";
        private static int enableINotifyPropertyChangingSupport;
        public static bool EnableINotifyPropertyChangingSupport
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => GetConfigurationValue(EnableINotifyPropertyChangingSupportPropertyName, ref enableINotifyPropertyChangingSupport, true);
        }
        private static bool GetConfigurationValue(string propertyName, ref int cachedResult, bool defaultValue)
        {
            if (cachedResult < 0)
            {
                return false;
            }

            if (cachedResult > 0)
            {
                return true;
            }

            if (!AppContext.TryGetSwitch(propertyName, out bool isEnabled))
            {
                isEnabled = defaultValue;
            }

            cachedResult = isEnabled ? 1 : -1;
            return isEnabled;
        }
    }
}
