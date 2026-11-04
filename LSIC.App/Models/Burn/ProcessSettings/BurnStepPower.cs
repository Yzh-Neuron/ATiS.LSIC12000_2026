namespace LSIC.App.Models
{
    /// <summary>
    /// 电源步骤（运行时）
    /// </summary>
    public class BurnStepPower
    {
        public string Name { get; }

        /// <summary>
        /// 使能 若为0无后续数据
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// 上下电使用的电源方案的下标
        /// </summary>
        public byte DpsPlanIndex { get; }

        /// <summary>
        /// 上下电运行的向量和寄存器（无则为 null）
        /// </summary>
        public BurnVector Vector { get; }

        /// <summary>
        /// 向量运行超时时间 单位为秒
        /// </summary>
        public int Timeout { get; }

        public BurnStepPower(string name, byte dpsPlanIndex, BurnVector vector, int timeout)
        {
            Name = name;

            if (dpsPlanIndex!=0xFF)
            {
                Enable = true;
                DpsPlanIndex = dpsPlanIndex;
                Vector = vector;
                Timeout = timeout;
            }
        }

        public BurnStepPower(string name)
        {
            Name = name;
        }
    }
}