namespace LSIC.App.Models
{
    public class BurnInfo : ObservableObject, IDisposable
    {
        public Ic Ic { get; private set; }
        public BurnLogger Logger { get; private set; }
        public string Lot { get; private set; }
        public string Operator { get; private set; }
        public Time LogInterval { get; private set; }
        public TimeSpan ErrorLogTimeSpan { get; private set; }
        public bool ForceStepLoad { get; private set; }

        /// <summary>
        /// 老化板
        /// </summary>
        public BurnInBoard BurnInBoard { get; private set; }

        /// <summary>
        /// 电源方案
        /// </summary>
        public BurnDpsPlan[] DpsPlans { get; private set; }

        /// <summary>
        /// 上电步
        /// </summary>
        public BurnStepPower PowerOnStep { get; private set; }

        /// <summary>
        /// 向量步
        /// </summary>
        public BurnStepVector[] Steps { get; private set; }

        /// <summary>
        /// 下电步
        /// </summary>
        public BurnStepPower PowerOffStep { get; private set; }

        /// <summary>
        /// Pattern 文件路径 → 索引
        /// </summary>
        public Dictionary<string, byte> PatternIndexByPath { get; private set; }

        /// <summary>
        /// Register 文件路径 → 索引
        /// </summary>
        public Dictionary<string, byte> RegisterIndexByPath { get; private set; }

        #region 运行时属性
        private DateTime? _start;
        public DateTime? Start
        {
            get => _start;
            private set => SetProperty(ref _start, value);
        }

        private int _totalSeconds;
        public int TotalSeconds
        {
            get => _totalSeconds;
            set => SetProperty(ref _totalSeconds, value);
        }

        private BurnDpsPlan _currentDpsPlan;
        public BurnDpsPlan CurrentDpsPlan
        {
            get => _currentDpsPlan;
            set => SetProperty(ref _currentDpsPlan, value);
        }

        private BurnStepVector _currentStep;
        public BurnStepVector CurrentStep
        {
            get => _currentStep;
            set => SetProperty(ref _currentStep, value);
        }

        private BurnVector _currentVector;
        public BurnVector CurrentVector
        {
            get => _currentVector;
            set => SetProperty(ref _currentVector, value);
        }

        private LoadMode _loadMode = LoadMode.Default;
        public LoadMode LoadMode
        {
            get => _loadMode;
            set => SetProperty(ref _loadMode, value);
        }

        private double _sendProcess;
        public double SendProcess
        {
            get => _sendProcess;
            set => SetProperty(ref _sendProcess, value);
        }

        private RunInfo _runInfo = new();
        public RunInfo RunInfo
        {
            get => _runInfo;
            private set => SetProperty(ref _runInfo, value);
        }

        private LoopInfo _loopInfo = new();
        public LoopInfo LoopInfo
        {
            get => _loopInfo;
            private set => SetProperty(ref _loopInfo, value);
        }

        #endregion

        /// <summary>
        /// 进入老化界面时封装老化信息
        /// </summary>
        public BurnInfo(Ic ic, string lot, string op)
        {
            Ic = ic;
            Lot = lot;
            Operator = op;
            LogInterval = ic.LogInterval;
            ErrorLogTimeSpan = ic.ErrorLogEnable ? TimeSpan.FromSeconds(ic.ErrorLogInterval.TotalSeconds) : TimeSpan.MaxValue;

            ForceStepLoad = ic.ForceStepLoad;

            if (ic.BurnInBoardSetting is null)
            {
                throw new InvalidOperationException("老化板配置未解析，无法进入老化界面");
            }
            BurnInBoard = new BurnInBoard(ic.BurnInBoardSetting);
            BuildSteps(ic);
            LoadMode = ResolveLoadMode();
        }

        /// <summary>
        /// 数据库加载老化信息
        /// </summary>
        public BurnInfo(Ic ic, string lot, string op, DateTime? start, byte[] dutMask) : this(ic, lot, op)
        {
            Start = start;
            BurnInBoard.DutMask = dutMask;
        }

        /// <summary>
        /// 发送器件时重置老化信息
        /// </summary>
        public void SendIc()
        {
            TotalSeconds = 0;
            RunInfo = new RunInfo();
            SendProcess = 0;
        }

        /// <summary>
        /// 开始老化时设置老化信息
        /// </summary>
        public void StartBurn(int zoneId, DateTime start, byte[] dutMask)
        {
            Start = start;

            Logger?.Dispose();
            Logger = null;
            Logger = new BurnLogger(zoneId, Ic.Name, Lot, Operator, Start.Value);

            BurnInBoard.DutMask = dutMask;


            TotalSeconds = 0;
            CurrentDpsPlan = null;
            CurrentStep = null;
            RunInfo = new RunInfo();
            LoopInfo = new LoopInfo();
            CurrentVector = null;
        }

        #region 辅助方法

        /// <summary>
        /// 步骤解析
        /// </summary>
        private void BuildSteps(Ic ic)
        {
            Process process = ic.Process;

            // 电源方案
            DpsPlans = new BurnDpsPlan[ic.DpsPlans.Count];
            for (int i = 0; i < ic.DpsPlans.Count; i++)
            {
                DpsPlans[i] = new BurnDpsPlan((byte)i, ic.DpsPlans[i]);
            }

            // 文件去重表
            Dictionary<string, byte> patternMap = new Dictionary<string, byte>();
            Dictionary<string, byte> registerMap = new Dictionary<string, byte>();

            // 建立向量步相对位置（排除上下电，从 0 开始）
            Dictionary<StepItem, int> vectorIndexByItem = new Dictionary<StepItem, int>();
            int vectorCount = 0;
            foreach (StepItem item in process.StepItems)
            {
                if (item.ItemType == StepItemType.Vector)
                {
                    vectorIndexByItem.Add(item, vectorCount);
                    vectorCount++;
                }
            }

            // 循环映射：Start 位置 → (Loop, End 位置)
            Dictionary<int, (Loop Loop, int End)> loopByVectorIndex = new Dictionary<int, (Loop Loop, int End)>();
            foreach (Loop loop in process.Loops)
            {
                int startRelative = vectorIndexByItem[loop.Start];
                int endRelative = vectorIndexByItem[loop.End];
                loopByVectorIndex[startRelative] = (loop, endRelative);
            }

            // 上下电单独存，向量步按位置顺序收集
            List<BurnStepVector> vectorSteps = new List<BurnStepVector>();
            BurnStepPower powerOnStep = null;
            BurnStepPower powerOffStep = null;
            int relative = 0;

            foreach (StepItem item in process.StepItems)
            {
                switch (item)
                {
                    case StepVector stepVector:
                        {
                            loopByVectorIndex.TryGetValue(relative, out (Loop Loop, int End) loopInfo);
                            vectorSteps.Add(MapVectorStep(stepVector, patternMap, registerMap, loopInfo, DpsPlans));
                            relative++;
                            break;
                        }
                    case StepPower stepPower:
                        {
                            if (item.ItemType == StepItemType.PowerOn)
                            {
                                powerOnStep = MapPowerStep(stepPower, patternMap, registerMap, DpsPlans);
                            }
                            else if (item.ItemType == StepItemType.PowerOff)
                            {
                                powerOffStep = MapPowerStep(stepPower, patternMap, registerMap, DpsPlans);
                            }
                            break;
                        }
                }
            }

            PowerOnStep = powerOnStep;
            Steps = vectorSteps.ToArray();
            PowerOffStep = powerOffStep;
            PatternIndexByPath = patternMap;
            RegisterIndexByPath = registerMap;
        }

        /// <summary>
        /// 映射向量步
        /// </summary>
        private BurnStepVector MapVectorStep(StepVector stepVector, Dictionary<string, byte> patternMap, Dictionary<string, byte> registerMap, (Loop Loop, int End) loopInfo, BurnDpsPlan[] dpsPlans)
        {
            // 向量
            List<BurnVector> vectors = new List<BurnVector>();
            foreach (Vector vector in stepVector.Vectors)
            {
                vectors.Add(new BurnVector(vector, patternMap, registerMap));
            }

            // 运行策略
            ExecuteStrategy strategy = (ExecuteStrategy)stepVector.ExecuteStrategy.Clone();

            // 电源方案
            byte dpsPlanIndex = ResolveDpsPlanIndex(stepVector.DpsPlan, dpsPlans);

            // 循环目标（End 位置），非循环步为 0xFF
            Loop loop = loopInfo.Loop;
            byte loopTarget = 0xFF;
            if (loop is not null)
            {
                loopTarget = (byte)loopInfo.End;
            }

            return new BurnStepVector(stepVector.Name, dpsPlanIndex, strategy, vectors.ToArray(), loop, loopTarget);
        }

        /// <summary>
        /// 映射上电/下电步
        /// </summary>
        private BurnStepPower MapPowerStep(StepPower stepPower, Dictionary<string, byte> patternMap, Dictionary<string, byte> registerMap, BurnDpsPlan[] dpsPlans)
        {
            if (stepPower.DpsPlan is not null)
            {
                byte dpsPlanIndex = ResolveDpsPlanIndex(stepPower.DpsPlan, dpsPlans);

                BurnVector vector = null;
                if (stepPower.Vectors.Count > 0)
                {
                    vector = new BurnVector(stepPower.Vectors[0], patternMap, registerMap);
                }

                int timeout = stepPower.Timeout;

                return new BurnStepPower(stepPower.Name, dpsPlanIndex, vector, timeout);
            }

            return new BurnStepPower(stepPower.Name);
        }

        /// <summary>
        /// 按电源方案名解析索引，未匹配返回 0xFF
        /// </summary>
        private byte ResolveDpsPlanIndex(string dpsPlanName, BurnDpsPlan[] dpsPlans)
        {
            foreach (BurnDpsPlan plan in dpsPlans)
            {
                if (plan.Name == dpsPlanName)
                {
                    return plan.Id;
                }
            }
            return 0xFF;
        }

        /// <summary>
        /// 按向量总行数确定加载模式
        /// </summary>
        private LoadMode ResolveLoadMode()
        {
            long lineCount = 0;
            foreach (string file in PatternIndexByPath.Keys)
            {
                try
                {
                    lineCount += PatternFileUtility.GetCommandCount(file);
                }
                catch
                {
                    return LoadMode.Step;
                }
            }
            if (lineCount > 16_000_000 || ForceStepLoad)
            {
                return LoadMode.Step;
            }
            return LoadMode.Default;
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            if (Logger is null)
            {
                return;
            }
            Logger.Dispose();
            Logger = null;
        }
        #endregion
    }
}
