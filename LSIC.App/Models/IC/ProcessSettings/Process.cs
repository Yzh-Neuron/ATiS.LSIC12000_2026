namespace LSIC.App.Models
{
    /// <summary>
    /// 流程(完成)
    /// </summary>
    public class Process : ObservableObject
    {
        public Process()
        {
            var powerOn = StepPower.PowerOn();
            var powerOff = StepPower.PowerOff();
            StepItems.Add(powerOn);
#if DEBUG
            StepItems.Add(new StepVector("步骤1"));
            StepItems.Add(new StepVector("步骤2"));
#endif
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

        private List<LoopSlot> _loopSlots;

        /// <summary>
        /// 循环槽位（界面用）
        /// </summary>
        public List<LoopSlot> LoopSlots
        {
            get => _loopSlots;
            set => SetProperty(ref _loopSlots, value);
        }

        /// <summary>
        /// 插入顺序
        /// </summary>
        public int NewItemInsertIndex => StepItems.Count - 1;

        /// <summary>
        /// 新增向量步骤(在上下电之间)
        /// </summary>
        public void AddVector(StepVector stepVector)
        {
            StepItems.Insert(NewItemInsertIndex, stepVector);
        }

        /// <summary>
        /// 移除向量步骤
        /// </summary>
        /// <param name="stepItems"></param>
        public void RemoveVector(IList<StepItem> stepItems)
        {
           var steps = stepItems.Where(t => !t.IsFixed);
            var affected = Loops.Where(loop => steps.Any(item => InLoop(loop, item))).ToList();

            bool needRefresh = false;

            //移除向量步骤所在的循环
            foreach (var loop in affected)
            {
                if (Loops.Remove(loop))
                {
                    needRefresh = true;
                }
            }

            //移除向量步骤
            foreach (var item in stepItems)
            {
                StepItems.Remove(item);
            }

            if (needRefresh)
            {
                Refresh();
            }
        }

        /// <summary>
        /// 生成循环
        /// </summary>
        /// <param name="stepItems"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void MakeLoop(IList<StepItem> stepItems)
        {
            //选中步骤生成闭区间
            IntClosedRange? range = this.RangeOf(stepItems);

            if (range is null)
            {
                throw new InvalidOperationException(stepItems is { Count: > 0 } ? "请选择连续的中段向量条目（不能包含固定上电 / 下电块）。" : "请先选中一个或多个连续的向量条目。");
            }

            //判断是否有重叠
            if (this.IsBlocked(range.Value))
            {
                throw new InvalidOperationException("循环只能并列，不能嵌套或交叉，请重新选择连续条目。");
            }

            var loop = new Loop(StepItems[range.Value.Start]) { End = StepItems[range.Value.End] };
            Loops.Add(loop);

            Refresh();
        }

        /// <summary>
        /// 是否可以排序
        /// </summary>
        public bool CanMove(int oldIndex, int newIndex)
        {
            int count = StepItems.Count;

            //固定流程不可移动
            if (oldIndex < 1 || oldIndex > count - 2)
            {
                return false;
            }

            //不能移动到固定流程中
            if (newIndex < 1 || newIndex > count - 2)
            {
                return false;
            }

            //不能移动到自己上面
            if (oldIndex == newIndex)
            {
                return false;
            }

            // 模拟步骤移动后的结果
            List<StepItem> simulated = StepItems.ToList();
            StepItem moving = simulated[oldIndex];
            simulated.RemoveAt(oldIndex);
            simulated.Insert(newIndex, moving);

            var ranges = new List<IntClosedRange>();
            foreach (Loop loop in Loops)
            {
                int start = simulated.IndexOf(loop.Start);
                int end = simulated.IndexOf(loop.End);

                //模拟后的结果
                //1、起点不能大于终点
                //2、正常范围必须是排除首尾
                if (start <= 0 || start >= end || end >= count - 1)
                {
                    return false;
                }
                ranges.Add(new IntClosedRange(start, end));
            }

            // 循环闭区间按照起点排序
            ranges.Sort((a, b) => a.Start.CompareTo(b.Start));
            for (int i = 1; i < ranges.Count; i++)
            {
                //每个区间不能重叠
                if (ranges[i].IsOverlapWith(ranges[i - 1]))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 排序
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
        /// 重新计算循环视图
        /// </summary>
        public void Refresh()
        {
            //生成循环视图
            List<LoopSlot> slots = this.BuildSlots();

            //提取视图中的所有子步骤
            var inLoop = new HashSet<StepItem>();
            foreach (var slot in slots)
            {
                foreach (int i in slot.Range.GetAllValues())
                {
                    inLoop.Add(StepItems[i]);
                }
            }

            //更新步骤中的是否在循环中的状态
            foreach (var item in StepItems)
            {
                item.IsInLoop = !item.IsFixed && inLoop.Contains(item);
            }

            LoopSlots = slots;
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
