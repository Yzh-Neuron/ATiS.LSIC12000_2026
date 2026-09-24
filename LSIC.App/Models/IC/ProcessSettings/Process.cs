namespace LSIC.App.Models
{
    /// <summary>
    /// 流程
    /// </summary>
    public class Process
    {
        public Process()
        {
            var powerOn = StepPower.PowerOn();
            var powerOff = StepPower.PowerOff();
            StepItems.Add(powerOn);
            StepItems.Add(powerOff);
        }

        /// <summary>
        /// 流程条目（上电 + 向量（有序） + 下电）
        /// </summary>
        public ObservableCollection<StepItem> StepItems { get; } = new();

        /// <summary>
        /// 并列循环区间
        /// </summary>
        public ObservableCollection<Loop> Loops { get; } = new();

        /// <summary>
        /// 循环槽位
        /// </summary>
        public ObservableCollection<LoopSlot> LoopSlots { get; } = new();

        /// <summary>
        /// 插入顺序
        /// </summary>
        public int NewItemInsertIndex => StepItems.Count - 1;

        /// <summary>
        /// 新增一个向量条目：永远插在固定下电尾之前
        /// </summary>
        public void AddVector(StepVector stepVector)
        {
            StepItems.Insert(NewItemInsertIndex, stepVector);
            Refresh();
        }

        /// <summary>
        /// 移除向量条目
        /// </summary>
        /// <param name="stepItems"></param>
        public void RemoveVector(IList<StepItem> stepItems)
        {
            stepItems = stepItems.Where(t => !t.IsFixed).ToList();
            var affected = Loops.Where(loop => stepItems.Any(item => InLoop(loop, item)));

            //移除循环
            foreach (var loop in affected)
            {
                Loops.Remove(loop);
            }

            //移除向量
            foreach (var item in stepItems)
            {
                StepItems.Remove(item);
            }

            Refresh();
        }

        /// <summary>
        /// 生成循环
        /// </summary>
        /// <param name="stepItems"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void MakeLoop(IList<StepItem> stepItems)
        {
            IntClosedRange? range = LoopResolver.RangeOf(this, stepItems);

            if (range is null)
            {
                throw new InvalidOperationException(stepItems is { Count: > 0 } ? "请选择连续的中段向量条目（不能包含固定上电 / 下电块）。" : "请先选中一个或多个连续的向量条目。");
            }

            if (LoopResolver.IsBlocked(this, range.Value))
            {
                throw new InvalidOperationException("循环只能并列，不能嵌套或交叉，请重新选择连续条目。");
            }

            var loop = new Loop(StepItems[range.Value.Start]) { End = StepItems[range.Value.End] };
            Loops.Add(loop);

            Refresh();
        }

        /// <summary>
        /// 任何编辑后统一重算
        /// </summary>
        public void Refresh()
        {
            List<LoopSlot> slots = LoopResolver.BuildSlots(this);

            var inLoop = new HashSet<StepItem>();
            foreach (var slot in slots)
            {
                foreach (int i in slot.Range.GetAllValues())
                {
                    inLoop.Add(StepItems[i]);
                }
            }

            foreach (var item in StepItems)
            {
                item.IsInLoop = !item.IsFixed && inLoop.Contains(item);
            }

            LoopSlots.Clear();
            foreach (var slot in slots)
            {
                LoopSlots.Add(slot);
            }
        }

        /// <summary>
        /// 判断当前步是否在循环中
        /// </summary>
        private bool InLoop(Loop loop, StepItem item)
        {
            int a = StepItems.IndexOf(loop.Start);
            int b = StepItems.IndexOf(loop.End);
            int i = StepItems.IndexOf(item);
            if (a < 0 || b < 0 || i < 0)
            {
                return false;
            }
            return new IntClosedRange(a, b).Contains(i);
        }
    }
}
