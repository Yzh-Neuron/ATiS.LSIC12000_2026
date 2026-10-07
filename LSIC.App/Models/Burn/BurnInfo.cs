namespace LSIC.App.Models
{
    public class BurnInfo : ObservableObject
    {
        private string _lot;
        public string Lot
        {
            get { return _lot; }
            set { SetProperty(ref _lot, value); }
        }

        private string _operator;
        public string Operator
        {
            get { return _operator; }
            set { SetProperty(ref _operator, value); }
        }

        /// <summary>
        /// 老化板
        /// </summary>
        public BurnInBoard BurnInBoard { get; }

        /// <summary>
        /// 电源方案（运行时）
        /// </summary>
        public BurnDpsPlan[] DpsPlans { get; private set; }

        /// <summary>
        /// 步次（按 Id 排好，上下电在首尾）
        /// </summary>
        public BurnStep[] Steps { get; private set; }

        /// <summary>
        /// Pattern 文件路径 → 索引
        /// </summary>
        public Dictionary<string, byte> PatternIndexByPath { get; private set; }

        /// <summary>
        /// Register 文件路径 → 索引
        /// </summary>
        public Dictionary<string, byte> RegisterIndexByPath { get; private set; }

        /// <summary>
        /// 进入老化界面时封装老化信息
        /// </summary>
        public BurnInfo(Ic ic, string lot, string op)
        {
            Lot = lot;
            Operator = op;

            if (ic.BurnInBoardSetting == null)
            {
                throw new InvalidOperationException("老化板配置未解析，无法进入老化界面");
            }
            BurnInBoard = new BurnInBoard(ic.BurnInBoardSetting);

            BuildSteps(ic);
        }

        /// <summary>
        /// 步骤解析：① 电源方案 ② 循环区间展平 ③ 逐步骤映射（步索引 i 为贯穿主线）
        /// </summary>
        private void BuildSteps(Ic ic)
        {
            DpsPlans = new BurnDpsPlan[ic.DpsPlans.Count];
            for (int i = 0; i < ic.DpsPlans.Count; i++)
            {
                DpsPlans[i] = new BurnDpsPlan((byte)i, ic.DpsPlans[i]);
            }

            Dictionary<string, byte> patternMap = new Dictionary<string, byte>();
            Dictionary<string, byte> registerMap = new Dictionary<string, byte>();
            Dictionary<int, (Loop Loop, int Start)> loopByStep = BuildLoopMap(ic.Process);

            List<BurnStep> steps = new List<BurnStep>();
            for (int i = 0; i < ic.Process.StepItems.Count; i++)
            {
                steps.Add(MapStep(ic.Process.StepItems[i], i, patternMap, registerMap, loopByStep, DpsPlans));
            }
            Steps = steps.ToArray();

            PatternIndexByPath = patternMap;
            RegisterIndexByPath = registerMap;
        }

        /// <summary>
        /// ② 循环区间展平：步索引 → (所属循环, 循环起始步)
        /// LoopResolver 保证区间连续、并列不重叠
        /// </summary>
        private static Dictionary<int, (Loop Loop, int Start)> BuildLoopMap(Process process)
        {
            Dictionary<int, (Loop Loop, int Start)> map = new Dictionary<int, (Loop Loop, int Start)>();
            foreach (Loop loop in process.Loops)
            {
                int start = process.StepItems.IndexOf(loop.Start);
                int end = process.StepItems.IndexOf(loop.End);
                for (int i = 0; i <= end - start; i++)
                {
                    map[start + i] = (loop, start);
                }
            }
            return map;
        }

        /// <summary>
        /// ③ 单步映射：StepItem → BurnStep
        /// </summary>
        private static BurnStep MapStep(StepItem item, int index,
            Dictionary<string, byte> patternMap,
            Dictionary<string, byte> registerMap,
            Dictionary<int, (Loop Loop, int Start)> loopByStep,
            BurnDpsPlan[] dpsPlans)
        {
            List<BurnVector> vectors = new List<BurnVector>();
            foreach (Vector vector in item.Vectors)
            {
                vectors.Add(new BurnVector(vector, patternMap, registerMap));
            }

            loopByStep.TryGetValue(index, out (Loop Loop, int Start) loopInfo);
            Loop loop = loopInfo.Loop;

            // 电源方案：按名字匹配，取其 Id（0xFF 表示无匹配）
            byte dpsPlanIndex = 0xFF;
            foreach (BurnDpsPlan plan in dpsPlans)
            {
                if (plan.Name == item.DpsPlan)
                {
                    dpsPlanIndex = plan.Id;
                    break;
                }
            }

            ExecuteStrategy strategy = null;
            if (item is StepVector stepVector)
            {
                strategy = (ExecuteStrategy)stepVector.ExecuteStrategy.Clone();
            }

            byte loopTarget = 0xFF;
            if (loop != null)
            {
                loopTarget = (byte)loopInfo.Start;
            }

            int timeout = 0;
            if (item is StepPower stepPower)
            {
                timeout = stepPower.Timeout;
            }

            return new BurnStep((byte)index, item.Name, item.ItemType, dpsPlanIndex, strategy, vectors.ToArray(), loop, loopTarget, timeout);
        }
    }
}