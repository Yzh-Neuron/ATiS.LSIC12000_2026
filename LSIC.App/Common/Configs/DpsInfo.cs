namespace LSIC.App
{
    /// <summary>
    /// 直流电源规格信息
    /// </summary>
    public class DpsInfo
    {
        /// <summary>
        /// 电源编号
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// 电源名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 电压范围
        /// </summary>
        public Range VolRange { get; private set; }

        /// <summary>
        /// 默认电压
        /// </summary>
        public int VolDefault { get; private set; }

        /// <summary>
        /// 电流上限范围
        /// </summary>
        public Range CurRange { get; private set; }

        /// <summary>
        /// 默认电流上限
        /// </summary>
        public int CurDefault { get; private set; }

        /// <summary>
        /// 电压下限
        /// </summary>
        public int MinUvp { get; private set; }

        /// <summary>
        /// 电压上限
        /// </summary>
        public int MaxOvp { get; private set; }

        /// <summary>
        /// 最大功率
        /// </summary>
        public int MaxPower { get; private set; }

        /// <summary>
        /// 上升时间范围
        /// </summary>
        public Range RiseRange { get; private set; }

        /// <summary>
        /// 默认上升时间
        /// </summary>
        public int RiseDefault { get; private set; }

        /// <summary>
        /// 延时时间范围
        /// </summary>
        public Range DelayRange { get; private set; }

        /// <summary>
        /// 默认延时时间
        /// </summary>
        public int DelayDefault { get; private set; }

        /// <summary>
        /// 是否为负电源(反符号)
        /// </summary>
        public bool Reverse { get; private set; }
    }
}
