﻿namespace LSIC.App.Models
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
        /// 拖动排序前校验：固定块不可移动，且重排后所有循环区间仍然合法（首尾有序、不含固定块）且互不交叠。
        /// </summary>
        public bool CanMove(int oldIndex, int newIndex)
        {
            int count = StepItems.Count;
            if (oldIndex < 1 || oldIndex > count - 2)
            {
                return false;
            }
            if (newIndex < 1 || newIndex > count - 2)
            {
                return false;
            }
            if (oldIndex == newIndex)
            {
                return false;
            }

            // 模拟移动后的顺序
            List<StepItem> simulated = StepItems.ToList();
            StepItem moving = simulated[oldIndex];
            simulated.RemoveAt(oldIndex);
            simulated.Insert(newIndex, moving);

            var ranges = new List<IntClosedRange>();
            foreach (Loop loop in Loops)
            {
                int start = simulated.IndexOf(loop.Start);
                int end = simulated.IndexOf(loop.End);
                if (start < 0 || end < 0 || start >= end)
                {
                    return false;
                }
                // 循环区间不能触及固定的首尾块
                if (start <= 0 || end >= count - 1)
                {
                    return false;
                }
                ranges.Add(new IntClosedRange(start, end));
            }

            // 循环只允许并列：按起点排序后两两不得交叠
            ranges.Sort((a, b) => a.Start.CompareTo(b.Start));
            for (int i = 1; i < ranges.Count; i++)
            {
                if (ranges[i].IsOverlapWith(ranges[i - 1]))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 拖动排序：校验通过后移动条目并重算循环视图。
        /// </summary>
        public void Move(int oldIndex, int newIndex)
        {
            if (!CanMove(oldIndex, newIndex))
            {
                throw new InvalidOperationException("当前拖动会破坏循环结构（交叉、嵌套或包含固定块），已取消。");
            }

            StepItems.Move(oldIndex, newIndex);
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
