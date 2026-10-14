namespace LSIC.App.Models
{
    /// <summary>
    /// 电源自检脚本命令（.dps 单行解析结果）。
    /// 电压、电流统一为 mV、mA；上升时间、采样间隔为 ms。
    /// </summary>
    public class DpsCommand
    {
        public DpsCommand(int line, int id, int vol, int uvp, int ovp, int ocp, int raise, int times, int interval)
        {
            Line = line;
            Id = id;
            Vol = vol;
            Uvp = uvp;
            Ovp = ovp;
            Ocp = ocp;
            Raise = raise;
            Times = times;
            Interval = interval;
        }

        /// <summary>所在脚本行号（用于定位校验错误）</summary>
        public int Line { get; }

        /// <summary>电源编号</summary>
        public int Id { get; }

        /// <summary>设置电压（mV）</summary>
        public int Vol { get; }

        /// <summary>电压下限（mV）</summary>
        public int Uvp { get; }

        /// <summary>电压上限（mV）</summary>
        public int Ovp { get; }

        /// <summary>电流上限（mA）</summary>
        public int Ocp { get; }

        /// <summary>上升时间（ms）</summary>
        public int Raise { get; }

        /// <summary>采样次数（必须大于 0）</summary>
        public int Times { get; }

        /// <summary>采样间隔（ms）</summary>
        public int Interval { get; }
    }
}
