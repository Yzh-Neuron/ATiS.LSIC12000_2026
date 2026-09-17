namespace LSIC.App.Models
{
    public enum NodeActionType
    {
        PowerOn,    // 上电方案
        Normal,     // 老化方案
        PowerOff    // 下电方案
    }

    /// <summary>
    /// 流程块基类
    /// </summary>
    public abstract class ProcessBlockBase
    {
        public abstract NodeActionType NodeActionType { get; }
    }

    public abstract class PowerProcessBlock : ProcessBlockBase
    {
        /// <summary>
        /// 每个块都可以附加电源组
        /// </summary>
        public string PowerPlan { get; set; }

    }

    public class PowerOnProcessBlock : PowerProcessBlock
    {
        public override NodeActionType NodeActionType { get; } = NodeActionType.PowerOn;

        public int TimeOut { get; set; }

        public Vector Vector { get; set; }
    }

    public class PowerOffProcessBlock : PowerProcessBlock
    {
        public override NodeActionType NodeActionType { get; } = NodeActionType.PowerOff;

        public int TimeOut { get; set; }

        public Vector Vector { get; set; }
    }

    public class NormalProcessBlock : PowerProcessBlock
    {
        public override NodeActionType NodeActionType { get; } = NodeActionType.Normal;

        /// <summary>
        /// 子步次
        /// </summary>
        public ObservableCollection<Vector> Vectors { get; set; }


    }

    public class SumProcess
    {
        public PowerOnProcessBlock PowerOnProcessBlock { get; set; }
        public ObservableCollection<NormalProcessBlock> NormalProcessBlocks { get; set; }
        public PowerOffProcessBlock PowerOffProcessBlock { get; set; }
    }

}
