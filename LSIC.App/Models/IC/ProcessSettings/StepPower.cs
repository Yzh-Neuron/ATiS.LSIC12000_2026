namespace LSIC.App.Models
{
    /// <summary>
    /// 电源步
    /// </summary>
    public class StepPower : StepItem
    {
        public override bool IsFixed => true;
        private readonly StepItemType _itemType;
        public override StepItemType ItemType => _itemType;
        private int _timeout = 3000;
        public int Timeout
        {
            get => _timeout;
            set => SetProperty(ref _timeout, value);
        }
        public StepPower(StepItemType itemType = StepItemType.PowerOn) : base(itemType == StepItemType.PowerOn ? "Power On" : "Power Off")
        {
            if (itemType != StepItemType.PowerOn && itemType != StepItemType.PowerOff)
            {
                throw new ArgumentOutOfRangeException(nameof(itemType));
            }

            _itemType = itemType;
        }
        public static StepPower PowerOn()
        {
            return new(StepItemType.PowerOn);
        }
        public static StepPower PowerOff()
        {
            return new(StepItemType.PowerOff);
        }
    }
}
