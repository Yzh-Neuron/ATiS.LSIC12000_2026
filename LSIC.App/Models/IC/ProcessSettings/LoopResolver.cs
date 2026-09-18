namespace LSIC.App.Models
{
    /// <summary>
    /// 循环解析器
    /// </summary>
    public static class LoopResolver
    {
        /// <summary>
        /// 生成循环视图
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public static List<LoopSlot> BuildSlots(Process process)
        {
            return ResolveRanges(process)
                .Select(x => new LoopSlot { Loop = x.Loop, Range = x.Range })
                .OrderBy(slot => slot.Row)
                .ToList();
        }

        /// <summary>
        /// 把流程的所选步解析成连续闭区间
        /// </summary>
        /// <param name="process"></param>
        /// <param name="selected"></param>
        /// <returns></returns>
        public static IntClosedRange? RangeOf(Process process, IList<StepItem> selected)
        {
            if (selected is null || selected.Count == 0)
            {
                return null;
            }

            // 固定块不允许进循环
            if (selected.Any(item => item.IsFixed))
            {
                return null;
            }

            var indexes = selected
                .Select(process.StepItems.IndexOf)
                .Where(i => i >= 0)
                .Distinct()
                .OrderBy(i => i)
                .ToList();

            if (indexes.Count == 0)
            {
                return null;
            }

            for (int i = 1; i < indexes.Count; i++)
            {
                if (indexes[i] != indexes[i - 1] + 1)
                {
                    return null;
                }
            }
            return new IntClosedRange(indexes[0], indexes[^1]);
        }

        /// <summary>
        /// 判断候选区间是否与流程中的循环有重叠
        /// </summary>
        /// <param name="process">流程</param>
        /// <param name="candidate">候选区间</param>
        /// <returns></returns>
        public static bool IsBlocked(Process process, IntClosedRange candidate)
        {
            foreach (var existing in ResolveRanges(process))
            {
                if (existing.Range.IsOverlapWith(candidate))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取流程中的所有循环
        /// </summary>
        private static List<(Loop Loop, IntClosedRange Range)> ResolveRanges(Process process)
        {
            var index = new Dictionary<StepItem, int>();
            for (int i = 0; i < process.StepItems.Count; i++)
            {
                index[process.StepItems[i]] = i;
            }

            var result = new List<(Loop, IntClosedRange)>();
            foreach (var loop in process.Loops)
            {
                if (!index.TryGetValue(loop.Start, out int start) || !index.TryGetValue(loop.End, out int end))
                {
                    throw new InvalidOperationException("循环引用了流程中不存在的条目。");
                }

                var range = new IntClosedRange(start, end);
                if (!range.IsValid(process.StepItems.Count))
                {
                    throw new InvalidOperationException("循环区间非法：首条目必须排在末条目之前，且不能越界。");
                }

                // 循环只能落在 [1,Count-1]
                if (start <= 0 || end >= process.StepItems.Count - 1)
                {
                    throw new InvalidOperationException("循环不能包含固定的上电 / 下电块。");
                }

                result.Add((loop, range));
            }
            return result;
        }
    }

}
