namespace LSIC.App.Models
{
    public class Ic : ObservableValidator
    {
        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        //记录间隔时间
        private Time _logInterval;
        public Time LogInterval
        {
            get => _logInterval;
            set => SetProperty(ref _logInterval, value);
        }
        private bool _errorLogEnable;
        public bool ErrorLogEnable
        {
            get => _errorLogEnable;
            set => SetProperty(ref _errorLogEnable, value);
        }
        private Time _errorLogInterval;
        public Time ErrorLogInterval
        {
            get => _errorLogInterval;
            set => SetProperty(ref _errorLogInterval, value);
        }


        //老化板配置信息
        private string _burnInBoardFile;
        public string BurnInBoardFile
        {
            get => _burnInBoardFile;
            set => SetProperty(ref _burnInBoardFile, value);
        }
        private DutSetting[] _dutSettings;
        public DutSetting[] DutSettings
        {
            get => _dutSettings;
            set => SetProperty(ref _dutSettings, value);
        }
        private ThermostatSetting[] _thermostatSettings;
        public ThermostatSetting[] ThermostatSettings
        {
            get => _thermostatSettings;
            set => SetProperty(ref _thermostatSettings, value);
        }

        //临时变量

        ///// <summary>
        ///// 更新老化板配置文件路径
        ///// </summary>
        ///// <param name="bibFile"></param>
        //private void UpdateBurnInBoardFile(string bibFile)
        //{
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(bibFile) && File.Exists(bibFile))
        //        {
        //            var boards = BurnInBoardParser.Parse2(bibFile);

        //            //所有温控模块的去重创建新的温控对象
        //            var thermostats = boards
        //                .SelectMany(board => board.Duts)
        //                .Select(dut => dut.Thermostat?.Id)
        //                .Where(id => id.HasValue)
        //                .Select(id => id.Value)
        //                .Distinct()
        //                .Select(id => new Thermostat(id))
        //                .ToList();

        //            var thermostatMap = thermostats.ToDictionary(t => t.Id);

        //            foreach (var dut in boards.SelectMany(board => board.Duts))
        //            {
        //                if (dut.Thermostat != null)
        //                {
        //                    //保证温控对象是唯一的
        //                    dut.Thermostat = thermostatMap[dut.Thermostat.Id];
        //                }
        //            }

        //            BurnInBoards = boards;
        //            Thermostats = new(thermostats);
        //            return;
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //    BurnInBoards = null;
        //    Thermostats?.Clear();
        //}


        //流程
        public Process Process { get; set; } = new Process();
        public Ic(string name)
        {
            Name = name;
        }
    }
}
