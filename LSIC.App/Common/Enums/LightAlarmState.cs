namespace LSIC.App
{
    /// <summary>
    /// 三色灯状态
    /// </summary>
    [Flags]
    public enum LightAlarmState
    {
        Red = 0x01,
        Yellow = 0x02,
        Green = 0x04,
        Alarming = 0x08
    }

    public static class LightAlarmStateExtension
    {
        public static string GetText(this LightAlarmState state)
        {
            List<string> states = new List<string>();
            if (state.HasFlag(LightAlarmState.Red))
            {
                states.Add(LangProvider.Instance.Enum_LightAlarmState_Red);
            }

            if (state.HasFlag(LightAlarmState.Yellow))
            {
                states.Add(LangProvider.Instance.Enum_LightAlarmState_Yellow);
            }

            if (state.HasFlag(LightAlarmState.Green))
            {
                states.Add(LangProvider.Instance.Enum_LightAlarmState_Green);
            }

            if (state.HasFlag(LightAlarmState.Alarming))
            {
                states.Add(LangProvider.Instance.Enum_LightAlarmState_Alarming);
            }

            return string.Join(" | ", states);
        }
    }
}