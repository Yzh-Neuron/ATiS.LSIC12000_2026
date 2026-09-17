using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LSIC.App.Models
{
    public record StepRange(int Start, int End)
    {
        /// <summary>
        /// 判断某个全局索引是否落在本区间内
        /// </summary>
        /// <param name="globalIndex"></param>
        /// <returns></returns>
        public bool Contains(int globalIndex)
        {
            return globalIndex >= Start && globalIndex <= End;
        }

        /// <summary>
        /// 判断本区间 和 另一个区间 是否重叠（交叉/包含都算重叠）
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsOverlapWith(StepRange other)
        {
            // 闭区间重叠公式：不是 (本区间完全在左边 || 本区间完全在右边)
            return !(End < other.Start || Start > other.End);
        }

        /// <summary>
        /// 获取区间内所有索引
        /// </summary>
        /// <returns></returns>
        public IEnumerable<int> GetAllIndexes()
        {
            for (int i = Start; i <= End; i++)
                yield return i;
        }

        /// <summary>
        /// 校验区间合法性
        /// </summary>
        /// <param name="maxStepCount"></param>
        /// <returns></returns>
        public bool IsValid(int maxStepCount)
        {
            return Start >= 0 && End < maxStepCount && Start <= End;
        }
    }
    public class Vector : ObservableValidator
    {
        private string patternFile;
        public string PatternFile
        {
            get => patternFile;
            set => SetProperty(ref patternFile, value);
        }

        private string registerFile;
        public string RegisterFile
        {
            get => registerFile;
            set => SetProperty(ref registerFile, value);
        }
    }
    public class Step : ObservableValidator
    {
        public int GlobalIndex { get; set; }
        public ObservableCollection<Vector> Vectors { get; set; } = new();
    }

    public partial class LoopSegment : ObservableValidator
    {
        public StepRange Range;

        /// <summary>
        /// 判断单个索引是否在本循环区间内
        /// </summary>
        /// <param name="globalIndex"></param>
        /// <returns></returns>
        public bool IsIndexInRange(int globalIndex)
        {
            return Range.Contains(globalIndex);
        }
        public IEnumerable<Step> GetSteps(ObservableCollection<Step> mainSteps)
        {
            if (!Range.IsValid(mainSteps.Count))
            {
                yield break;
            }

            for (int i = Range.Start; i <= Range.End; i++)
            {
                yield return mainSteps[i];
            }
        }
        public bool IsValidRange(ObservableCollection<Step> mainSteps)
        {
            return Range.IsValid(mainSteps.Count);
        }
    }

    public class FlowCompiler
    {
        public System.Collections.ObjectModel.ObservableCollection<Step> Steps { get; set; } = new();
        public ObservableCollection<LoopSegment> Loops { get; set; } = new();

        /// <summary>
        /// 【核心】选中的一批索引，是否和任意循环存在交集
        /// Shift多选后，判断选中步里面有没有已经属于别的循环
        /// </summary>
        public bool HasIndexOverlapWithAnyLoop(IEnumerable<int> selectedGlobalIndexes)
        {
            var indexList = selectedGlobalIndexes.ToList();
            return Loops.Any(loop => indexList.Any(idx => loop.IsIndexInRange(idx)));
        }

        /// <summary>
        /// 判断准备新建的区间，和已有循环是否重叠（禁止嵌套交叉）
        /// </summary>
        public bool IsRangeOverlapAnyLoop(StepRange newRange)
        {
            return Loops.Any(loop => loop.Range.IsOverlapWith(newRange));
        }

        public void Compiler(IEnumerable<Step> Steps)
        {
            // 编译逻辑
            Loops.Move(0, 1);
        }
    }
}
