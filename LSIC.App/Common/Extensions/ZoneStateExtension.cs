namespace LSIC.App
{
    public static class ZoneStateExtension
    {
        public static Func<ZoneState, bool> CanSendIcFunc { get; } = CanSendIc;
        public static Func<ZoneState, bool> CanGotoBurnFunc { get; } = CanGotoBurn;
        public static Func<ZoneState, bool> CanStartFunc { get; } = CanStart;
        public static Func<ZoneState, bool> CanStopFunc { get; } = CanStop;
        public static Func<ZoneState, bool> CanUpgradeFunc { get; } = CanUpgrade;
        public static Func<ZoneState, bool> RunningFunc { get; } = Running;
        public static Func<ZoneState, bool> SendingFunc { get; } = Sending;
        public static bool CanSendIc(this ZoneState state)
        {
            return state == ZoneState.Idle ||
                   state == ZoneState.Ready ||
                   state == ZoneState.Stop ||
                   state == ZoneState.Error;
        }
        public static bool CanGotoBurn(this ZoneState state)
        {
            return state == ZoneState.Idle ||
                   state == ZoneState.Ready ||
                   state == ZoneState.Stop ||
                   state == ZoneState.Error;
        }
        public static bool CanStart(this ZoneState state)
        {
            return state == ZoneState.Ready ||
                   state == ZoneState.Stop;
        }
        public static bool CanStop(this ZoneState state)
        {
            return state == ZoneState.PowerUp ||
                   state == ZoneState.Burning;
        }
        public static bool CanUpgrade(this ZoneState state)
        {
            return state == ZoneState.Idle ||
                   state == ZoneState.Ready ||
                   state == ZoneState.Stop ||
                   state == ZoneState.DeviceCheckFail ||
                   state == ZoneState.IcError ||
                   state == ZoneState.Error;
        }
        public static bool Running(this ZoneState state)
        {
            return state == ZoneState.PowerUp ||
                   state == ZoneState.Burning ||
                   state == ZoneState.PowerDown ||
                   state == ZoneState.Transition;
        }
        public static bool Sending(this ZoneState state)
        {
            return state == ZoneState.Sending ||
                   state == ZoneState.Transition ||
                   state == ZoneState.IcError;
        }
    }
}