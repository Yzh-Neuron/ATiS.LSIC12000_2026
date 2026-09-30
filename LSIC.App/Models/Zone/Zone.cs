using ATiS.Sim.Log;
using LSIC.App.Common.Enums;

namespace LSIC.App.Models
{
    public class Zone : ObservableObject, IDisposable
    {
        public Zone(ZoneInfo info)
        {
            Info = info ?? throw new ArgumentNullException(nameof(info));
        }

        public ZoneInfo Info { get; }
        public int Id => Info.Id;
        public string Ip => Info.Ip;
        public ushort Port => Info.Port;

        private TcpClient _client;
        private List<byte> _buffer;
        private RollingFileLogger _logger;
        private bool _disposed;
        private bool _connected;
        public bool Connected
        {
            get => _connected;
            private set => SetProperty(ref _connected, value);
        }
        private ZoneState _state = ZoneState.Offline;
        public ZoneState State
        {
            get => _state;
            private set => SetProperty(ref _state, value);
        }
        private int? _currentDpsGroupIndex;
        public int? CurrentDpsGroupIndex
        {
            get => _currentDpsGroupIndex;
            private set => SetProperty(ref _currentDpsGroupIndex, value);
        }
        private DpsRunInfo[] _dpss;
        public DpsRunInfo[] Dpss => _dpss ??= Device.Instance.SystemConfig.DpsInfos
            .Select(info => new DpsRunInfo(info)).ToArray();

        public void Startup()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(Zone));
            }

            if (_client != null)
            {
                Unsubscribe(_client);
                _client.Dispose();
            }

            _logger ??= new RollingFileLogger($@".\Log\Zone\Zone {Id}\Flow_{{date:yyyy-MM-dd}}.log",
                RollingInterval.Day, 256L * 1024 * 1024, LogLevel.Debug);

            _client = new TcpClient(Ip, Port) { CheckInterval = 3000 };
            _client.StateChanged += OnStateChanged;
            _client.ReceivedData += OnReceivedData;
            _client.ThrownException += OnClientException;
            _client.Start();
            _client.StartAutoReceive();
        }

        private Task OnStateChanged(object sender, SocketStateChangedArgs e)
        {
            Connected = e.Connected;
            _buffer = e.Connected ? new List<byte>(1024) : null;

            if (e.Connected)
            {
                State = ZoneState.Idle;
                CurrentDpsGroupIndex = null;
                _logger?.Info($"Zone {Id} connected.");
            }
            else
            {
                State = ZoneState.Offline;
                CurrentDpsGroupIndex = null;
                foreach (DpsRunInfo dps in Dpss)
                {
                    dps.Reset();
                }
                _logger?.Info($"Zone {Id} disconnected.");
            }

            return Task.CompletedTask;
        }
        private async Task OnReceivedData(object sender, SocketDataReceivedArgs e)
        {
            if (_buffer == null)
            {
                return;
            }

            _buffer.AddRange(e.Data);
            while (ZoneFrame.TryExtractFrame(_buffer, out byte code, out byte[] frameData))
            {
                try
                {
                    await AnalysisFrame((ZoneFunctionCode)code, frameData);
                }
                catch (Exception ex)
                {
                    _logger?.Error($"Zone {Id} analysis frame 0x{code:X2} error: {ex.Message}");
                }
            }
        }
        private Task OnClientException(object sender, SocketExceptionArgs e)
        {
            _logger?.Error($"Zone {Id} communication error: {e.Exception.Message}");
            return Task.CompletedTask;
        }
        private Task AnalysisFrame(ZoneFunctionCode code, byte[] data)
        {
            switch (code)
            {
                case ZoneFunctionCode.DpsState:
                    ReadDpsState(data);
                    break;
                case ZoneFunctionCode.CurrentDpsGroup:
                    int id = data[0];
                    if (id != 0xFF)
                    {
                        CurrentDpsGroupIndex = id;
                    }
                    break;
            }
            return Task.CompletedTask;
        }
        private void ReadDpsState(byte[] data)
        {
            int index = 0;
            int groupIndex = data[index++];
            if (groupIndex != 0xFF)
            {
                CurrentDpsGroupIndex = groupIndex;
            }

            int count = data[index++];
            for (int i = 0; i < count; i++)
            {
                int id = data[index++];
                DpsType type = (DpsType)data[index++];
                int inputVoltage = data.ToInt32(index, Endianness.Big); index += 4;
                int outputVoltage = data.ToInt32(index, Endianness.Big); index += 4;
                int outputCurrent = data.ToInt32(index, Endianness.Big); index += 4;
                int temperature = data.ToInt32(index, Endianness.Big); index += 4;
                DpsState state = (DpsState)data[index++];

                if (type != DpsType.Dps)
                {
                    continue; // VIH 条目字段已读，暂不处理
                }

                DpsRunInfo dps = Dpss.FirstOrDefault(j => j.Info.Id == id);
                if (dps == null)
                {
                    continue;
                }

                int symbol = dps.Info.Reverse ? -1 : 1;
                dps.Update(state, inputVoltage * symbol, outputVoltage * symbol, outputCurrent, temperature);
            }
        }

        /// <summary>
        /// 下发全部电源方案（先配置数量，再逐组下发）
        /// </summary>
        public async Task SetDpsPlanAsync(Ic ic)
        {
            ArgumentNullException.ThrowIfNull(ic);

            await SendAsync(ZoneFunctionCode.DpsGroupCount, (byte)ic.DpsPlans.Count);

            byte groupIndex = 0;
            foreach (DpsPlan plan in ic.DpsPlans)
            {
                await SendAsync(ZoneFunctionCode.DpsGroup, BuildDpsGroup(groupIndex++, plan));
            }
        }

        private static byte[] BuildDpsGroup(byte groupIndex, DpsPlan plan)
        {
            using MemoryStream stream = new MemoryStream();
            stream.WriteByte(groupIndex);
            stream.WriteByte((byte)plan.DpssSettings.Count);

            foreach (DpsSetting dps in plan.DpssSettings)
            {
                stream.WriteByte((byte)dps.Id);
                Write(stream, dps.Vol);
                Write(stream, dps.Uvp);
                Write(stream, dps.Ovp);
                Write(stream, dps.Ocp);
                Write(stream, int.MaxValue);
                Write(stream, dps.Delay);
                Write(stream, dps.Raise);
            }

            return stream.ToArray();
        }

        private static void Write(Stream stream, int value)
        {
            byte[] bytes = value.ToBytes(Endianness.Big);
            stream.Write(bytes, 0, bytes.Length);
        }

        private async Task SendAsync(ZoneFunctionCode code, params byte[] data)
        {
            if (!Connected || _client == null)
            {
                return;
            }

            byte[] frame = ZoneFrame.Build((byte)code, data);
            await _client.SendAsync(frame);
            _logger?.Debug($"Tx: {code} 0x{(byte)code:X2} {data.ToHex(" ")}");
        }

        private void Unsubscribe(TcpClient client)
        {
            client.StateChanged -= OnStateChanged;
            client.ReceivedData -= OnReceivedData;
            client.ThrownException -= OnClientException;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;

            if (_client != null)
            {
                Unsubscribe(_client);
                _client.Dispose();
                _client = null;
            }
            _logger?.Dispose();
            _logger = null;
            _buffer = null;
        }
    }
}
