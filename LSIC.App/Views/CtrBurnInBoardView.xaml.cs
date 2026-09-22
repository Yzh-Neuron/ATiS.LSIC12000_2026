using System.Collections;
using System.Windows;
using System.Windows.Controls;
using LSIC.App.Models;

namespace LSIC.App.Views
{
    /// <summary>
    /// DUT 信息矩阵视图。矩阵布局/行列头/滚动/多选由 <see cref="ATiS.Sim.Wpf.MatrixListBox"/> 提供，
    /// 本视图只对外暴露数据（Duts 经 DataContext=DutInfo 绑定）与选中项、温控设置。
    /// </summary>
    public partial class CtrBurnInBoardView : UserControl
    {
        public static readonly DependencyProperty SelectedDutProperty = DependencyProperty.Register(
            nameof(SelectedDut), typeof(Dut), typeof(CtrBurnInBoardView),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>当前选中的工位（Extended 多选时为单选锚点；如需多选集合可再加依赖属性）。</summary>
        public Dut? SelectedDut
        {
            get => (Dut?)GetValue(SelectedDutProperty);
            set => SetValue(SelectedDutProperty, value);
        }

        public static readonly DependencyProperty ThermostatSettingsProperty = DependencyProperty.Register(
            nameof(ThermostatSettings), typeof(IEnumerable), typeof(CtrBurnInBoardView),
            new PropertyMetadata(null));

        /// <summary>
        /// 温控设置集合，用于工位温度编辑。
        /// TODO：按实际类型改为具体集合类型（如 ObservableCollection&lt;ThermostatSetting&gt;）。
        /// </summary>
        public IEnumerable? ThermostatSettings
        {
            get => (IEnumerable?)GetValue(ThermostatSettingsProperty);
            set => SetValue(ThermostatSettingsProperty, value);
        }

        public CtrBurnInBoardView()
        {
            InitializeComponent();
        }
    }
}
