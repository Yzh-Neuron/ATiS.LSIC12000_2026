namespace LSIC.App
{
    public enum DpsState : byte
    {
        Output = 0x00,
        Ovp = 0x01,
        Uvp = 0x02,
        Ocp = 0x04,
        Idle = 0x80
    }
}
