namespace LSIC.App.Models
{
    public enum DutState : ulong
    {
        Idle = 0x01,
        Test = 0x0100,
        Ignore = 0x010000,
        Fail = 0x01000000,
        Pass = 0x0100000000,
    }
}