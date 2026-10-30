using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ZoneSimulator
{
    /// <summary>老化区模拟器主界面</summary>
    public partial class MainWindow : Window
    {
        private SimServer _server;
        private readonly DispatcherTimer _refreshTimer;
        private readonly StringBuilder _logBuffer = new();
        private bool _updatingDpsRows;
        private ZoneVm _selectedZone;

        public ObservableCollection<ZoneVm> Zones { get; } = new();

        public ZoneVm SelectedZone
        {
            get => _selectedZone;
            set
            {
                _selectedZone = value;
                LoadDpsRows();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            _refreshTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            _refreshTimer.Tick += (_, __) => RefreshAll();
            _refreshTimer.Start();

            Loaded += (_, __) =>
            {
                Log("提示：启动服务后，软件连 127.0.0.1:8002 即对应 Config.ini 里的 Z0。");
                OnStartServer(null, null);
            };
        }

        #region 服务控制

        private void OnStartServer(object sender, RoutedEventArgs e)
        {
            if (_server is not null)
            {
                return;
            }

            if (!int.TryParse(PortBox.Text.Trim(), out int port) || port is < 1 or > 65535)
            {
                MessageBox.Show("端口不合法", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _server = new SimServer();
                _server.Logged += ServerLog;
                _server.SessionOpened += OnSessionOpened;
                _server.Start(new[] { port });
            }
            catch (Exception ex)
            {
                Log("启动失败：" + ex.Message);
                _server = null;
                return;
            }

            StartBtn.IsEnabled = false;
            PortBox.IsEnabled = false;
            StopBtn.IsEnabled = true;
            ServerStatus.Text = $"运行中 · 0.0.0.0:{port}";
            ServerStatus.Foreground = System.Windows.Media.Brushes.Green;
            Log($"服务已启动，监听 0.0.0.0:{port}");
        }

        private void OnStopServer(object sender, RoutedEventArgs e)
        {
            if (_server is null)
            {
                return;
            }

            _server.Dispose();
            _server = null;
            Zones.Clear();
            SelectedZone = null;
            DpsGrid.ItemsSource = null;

            StartBtn.IsEnabled = true;
            PortBox.IsEnabled = true;
            StopBtn.IsEnabled = false;
            ServerStatus.Text = "未启动";
            ServerStatus.Foreground = System.Windows.Media.Brushes.Gray;
            Log("服务已停止");
        }

        private void OnSessionOpened(ZoneSim sim)
        {
            Dispatcher.BeginInvoke(() =>
            {
                sim.Logged += ServerLog;
                if (Zones.All(z => z.Sim.Id != sim.Id))
                {
                    Zones.Add(new ZoneVm(sim));
                }

                if (SelectedZone is null)
                {
                    ZoneList.SelectedItem = Zones.LastOrDefault();
                }

                Log($"Z{sim.Id} 已连接（{sim.Remote}），可在左侧选中后手动控制");
            });
        }

        #endregion

        #region 定时刷新

        private void RefreshAll()
        {
            if (_server is null)
            {
                return;
            }

            HashSet<int> alive = _server.Sessions.Select(s => s.Id).ToHashSet();
            for (int i = Zones.Count - 1; i >= 0; i--)
            {
                if (!alive.Contains(Zones[i].Sim.Id))
                {
                    Zones.RemoveAt(i);
                    if (SelectedZone is not null && !alive.Contains(SelectedZone.Sim.Id))
                    {
                        SelectedZone = null;
                        DpsGrid.ItemsSource = null;
                    }
                }
            }

            foreach (ZoneVm zone in Zones)
            {
                zone.Refresh();
            }

            if (SelectedZone is not null)
            {
                ThermoActualText.Text = $"实际: {SelectedZone.Sim.ThermostatActual / 10.0:F1}℃ / 目标: {SelectedZone.Sim.ThermostatTarget / 10.0:F1}℃";
                PlugCheck.Checked -= OnPlugChanged;
                PlugCheck.Unchecked -= OnPlugChanged;
                PlugCheck.IsChecked = SelectedZone.Sim.Plugged;
                PlugCheck.Checked += OnPlugChanged;
                PlugCheck.Unchecked += OnPlugChanged;
                AutoCheck.Checked -= OnAutoChanged;
                AutoCheck.Unchecked -= OnAutoChanged;
                AutoCheck.IsChecked = SelectedZone.Sim.Auto;
                AutoCheck.Checked += OnAutoChanged;
                AutoCheck.Unchecked += OnAutoChanged;
            }

            if (DpsGrid.ItemsSource is IEnumerable<DpsRowVm> rows && !_updatingDpsRows && !IsGridEditing())
            {
                foreach (DpsRowVm row in rows)
                {
                    row.RefreshDisplay();
                }
            }
        }

        private bool IsGridEditing()
        {
            if (DpsGrid.SelectedIndex>0)
            {
                return DpsGrid.ItemContainerGenerator.ContainerFromIndex(DpsGrid.SelectedIndex) is DataGridRow row && row.IsEditing;
            }
            return false;
        }

        private void LoadDpsRows()
        {
            if (SelectedZone is null)
            {
                DpsGrid.ItemsSource = null;
                return;
            }

            _updatingDpsRows = true;
            DpsGrid.ItemsSource = SelectedZone.Sim.Dpss.Concat(SelectedZone.Sim.Vihs)
                .Select(d => new DpsRowVm(d))
                .ToList();
            _updatingDpsRows = false;
        }

        #endregion

        #region 操作目标

        private List<ZoneSim> Targets()
        {
            if (_server is null || _server.Sessions.Count == 0)
            {
                Log("当前没有已连接的老化区，请先在软件里连接");
                return new List<ZoneSim>();
            }

            if (AllZonesCheck?.IsChecked == true)
            {
                return _server.Sessions.ToList();
            }

            if (SelectedZone is null)
            {
                Log("请先在左侧选中一个老化区（或勾选“对全部已连接区生效”）");
                return new List<ZoneSim>();
            }

            return new List<ZoneSim> { SelectedZone.Sim };
        }

        private static bool TryGet(TextBox box, out int value)
        {
            return int.TryParse(box.Text.Trim(), out value);
        }

        #endregion

        #region 状态与老化

        private void OnSendState(object sender, RoutedEventArgs e)
        {
            if (StateCombo.SelectedItem is not ZoneState state)
            {
                return;
            }

            foreach (ZoneSim zone in Targets())
            {
                zone.SetState(state);
            }
        }

        private void OnPlugChanged(object sender, RoutedEventArgs e)
        {
            if (SelectedZone is null || !PlugCheck.IsChecked.HasValue)
            {
                return;
            }

            SelectedZone.Sim.SetPlugged(PlugCheck.IsChecked.Value);
        }

        private void OnStartBurn(object sender, RoutedEventArgs e)
        {
            foreach (ZoneSim zone in Targets())
            {
                zone.StartBurn();
            }
        }

        private void OnStopBurn(object sender, RoutedEventArgs e)
        {
            foreach (ZoneSim zone in Targets())
            {
                zone.StopBurn();
            }
        }

        private void OnReset(object sender, RoutedEventArgs e)
        {
            foreach (ZoneSim zone in Targets())
            {
                zone.SetBurn(0, zone.Total, 0, 0);
                zone.SetAllDps(false);
                zone.SetState(ZoneState.Idle);
            }
        }

        private void OnSendBurn(object sender, RoutedEventArgs e)
        {
            if (!TryGet(BurnCostBox, out int cost))
            {
                Log("老化进度参数不合法");
                return;
            }

            int? total = TryGet(BurnTotalBox, out int totalV) ? totalV : null;
            byte? step = TryGet(BurnStepBox, out int stepV) ? (byte)stepV : null;
            byte? vector = TryGet(BurnVectorBox, out int vectorV) ? (byte)vectorV : null;

            foreach (ZoneSim zone in Targets())
            {
                zone.SetBurn(cost, total, step, vector);
            }
        }

        private void OnAutoChanged(object sender, RoutedEventArgs e)
        {
            if (SelectedZone is null || !AutoCheck.IsChecked.HasValue)
            {
                return;
            }

            SelectedZone.Sim.Auto = AutoCheck.IsChecked.Value;
            Log($"Z{SelectedZone.Sim.Id} 自动推进 {(AutoCheck.IsChecked.Value ? "开启" : "关闭")}");
        }

        private void OnSpeedChanged(object sender, RoutedEventArgs e)
        {
            if (SelectedZone is null || !int.TryParse(SpeedBox.Text.Trim(), out int speed) || speed <= 0)
            {
                return;
            }

            SelectedZone.Sim.Speed = speed;
            Log($"Z{SelectedZone.Sim.Id} 每 500ms 推进 {speed}s");
        }

        private void OnThermo(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(ThermoBox.Text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double celsius))
            {
                Log("温度参数不合法");
                return;
            }

            foreach (ZoneSim zone in Targets())
            {
                zone.ThermostatTarget = (int)Math.Round(celsius * 10);
                zone.SendThermostat();
            }

            Log($"温控目标设为 {celsius:F1}℃");
        }

        private void OnPlan(object sender, RoutedEventArgs e)
        {
            if (!TryGet(PlanBox, out int planId))
            {
                return;
            }

            foreach (ZoneSim zone in Targets())
            {
                zone.SendCurrentPlan((byte)planId);
            }
        }

        private void OnTrigger(object sender, RoutedEventArgs e)
        {
            if (TriggerCombo.SelectedItem is not TriggerOutputType type || !TryGet(TriggerPinBox, out int pin))
            {
                return;
            }

            foreach (ZoneSim zone in Targets())
            {
                zone.Send(ZoneFunctionCode.Trigger, Payloads.TriggerReport(type, (byte)pin));
            }

            Log($"已上报 Trigger：{type} pin={pin}");
        }

        #endregion

        #region 电源

        private void OnApplyDps(object sender, RoutedEventArgs e)
        {
            if (SelectedZone is null)
            {
                return;
            }

            DpsGrid.CommitEdit(DataGridEditingUnit.Row, true);
            SelectedZone.Sim.SendDpsState();
            Log($"Z{SelectedZone.Sim.Id} 已按表格当前值上报电源状态");
        }

        private void OnAllDpsOn(object sender, RoutedEventArgs e)
        {
            foreach (ZoneSim zone in Targets())
            {
                zone.SetAllDps(true);
            }
        }

        private void OnAllDpsOff(object sender, RoutedEventArgs e)
        {
            foreach (ZoneSim zone in Targets())
            {
                zone.SetAllDps(false);
            }
        }

        private void OnSendDpsState(object sender, RoutedEventArgs e)
        {
            foreach (ZoneSim zone in Targets())
            {
                zone.SendDpsState();
            }
        }

        #endregion

        #region 故障注入

        private void OnErrDps(object sender, RoutedEventArgs e)
        {
            if (!TryGet(ErrDpsIdBox, out int id) || ErrDpsStateCombo.SelectedItem is not DpsState state)
            {
                return;
            }

            foreach (ZoneSim zone in Targets())
            {
                zone.SetError(ZoneErrorType.DPS, Payloads.DpsErrorDetail(state, DpsType.DPS, (byte)id));
            }

            Log($"已注入 DPS 故障：通道 {id} {state}");
        }

        private void OnErrPattern(object sender, RoutedEventArgs e)
        {
            if (ErrPatternCombo.SelectedItem is not PatternErrorType type)
            {
                return;
            }

            byte step = TryGet(ErrPatternStepBox, out int stepV) ? (byte)stepV : (byte)0;
            byte vector = TryGet(ErrPatternVectorBox, out int vectorV) ? (byte)vectorV : (byte)0;

            foreach (ZoneSim zone in Targets())
            {
                zone.SetError(ZoneErrorType.Pattern, Payloads.PatternErrorDetail(type, step, vector));
            }

            Log($"已注入向量错误：{type} step={step} vector={vector}");
        }

        private void OnErrDevice(object sender, RoutedEventArgs e)
        {
            byte[] detail = ParseHex(ErrDeviceBox.Text);

            foreach (ZoneSim zone in Targets())
            {
                zone.SetError(ZoneErrorType.Device, detail);
            }

            Log($"已注入设备错误：{BitConverter.ToString(detail)}");
        }

        private void OnCompareError(object sender, RoutedEventArgs e)
        {
            int count = TryGet(CmpCountBox, out int countV) ? countV : 5;
            byte step = TryGet(CmpStepBox, out int stepV) ? (byte)stepV : (byte)0;
            byte vector = TryGet(CmpVectorBox, out int vectorV) ? (byte)vectorV : (byte)0;

            foreach (ZoneSim zone in Targets())
            {
                zone.SendCompareErrors(count, step, vector);
            }
        }

        private void OnCheckPass(object sender, RoutedEventArgs e)
        {
            foreach (ZoneSim zone in Targets())
            {
                zone.SendDeviceCheck(false);
            }
        }

        private void OnCheckFail(object sender, RoutedEventArgs e)
        {
            foreach (ZoneSim zone in Targets())
            {
                zone.SendDeviceCheck(true);
            }
        }

        private void OnDpsCheckPass(object sender, RoutedEventArgs e)
        {
            foreach (ZoneSim zone in Targets())
            {
                zone.SimulateDpsCheck(null, false);
            }
        }

        private void OnDpsCheckFail(object sender, RoutedEventArgs e)
        {
            foreach (ZoneSim zone in Targets())
            {
                zone.SimulateDpsCheck(null, true);
            }
        }

        private void OnUpgrade(object sender, RoutedEventArgs e)
        {
            foreach (ZoneSim zone in Targets())
            {
                zone.SimulateUpgrade();
            }
        }

        private void OnBoards(object sender, RoutedEventArgs e)
        {
            foreach (ZoneSim zone in Targets())
            {
                zone.SendBoards();
            }
        }

        private void OnBurnState(object sender, RoutedEventArgs e)
        {
            foreach (ZoneSim zone in Targets())
            {
                zone.SendBurnState();
            }
        }

        #endregion

        #region 日志与工具

        private void ServerLog(string line)
        {
            Dispatcher.BeginInvoke(() => Log(line));
        }

        private void Log(string message)
        {
            _logBuffer.AppendLine($"{DateTime.Now:HH:mm:ss} {message}");
            if (_logBuffer.Length > 64 * 1024)
            {
                string text = _logBuffer.ToString();
                _logBuffer.Clear();
                _logBuffer.AppendLine(text[^(16 * 1024)..]);
            }

            LogBox.Text = _logBuffer.ToString();
            LogBox.ScrollToEnd();
        }

        private static byte[] ParseHex(string text)
        {
            text = (text ?? string.Empty).Replace(" ", string.Empty).Replace("-", string.Empty);
            if (text.Length % 2 != 0)
            {
                text = "0" + text;
            }

            byte[] bytes = new byte[text.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = byte.Parse(text.Substring(i * 2, 2), NumberStyles.HexNumber);
            }

            return bytes;
        }

        #endregion
    }
}
