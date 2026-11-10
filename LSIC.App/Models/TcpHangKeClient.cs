using TcpClient = ATiS.Sim.Communication.TcpClient;

namespace LSIC.App.Models
{
    public abstract class TcpHangKeClient<TEnum> : ObservableObject, IDisposable where TEnum : Enum
    {
        protected const ushort FrameHeader = 0x55AA;
        protected const ushort FrameEnding = 0xEAA5;
        protected const int MaxPayloadLength = 4 * 1024 * 1024;
        protected bool _disposed;
        protected string _name;
        protected TcpClient _client;
        protected List<byte> _buffer;
        protected readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1); // 发送锁
        protected readonly SemaphoreSlim _receiveLock = new SemaphoreSlim(1, 1); // 接收锁
        public int Id { get; protected set; }
        public string Ip { get; protected set; }
        public ushort Port { get; protected set; }
        protected bool _connected = false;
        public bool Connected
        {
            get => _connected;
            private set => SetProperty(ref _connected, value);
        }
        public FlowLogger Logger { get; protected set; }
        public TcpHangKeClient(int id, string ip, ushort port)
        {
            _name = GetType().Name;
            Id = id;
            Ip = ip;
            Port = port;
        }
        public virtual void Startup()
        {
            if (_client is not null)
            {
                _client.StateChanged -= OnStateChanged;
                _client.ReceivedData -= OnReceivedData;
                _client.Dispose();
                _client = null;
            }

            _client = new TcpClient(Ip, Port);
            _client.StateChanged += OnStateChanged;
            _client.ReceivedData += OnReceivedData;
            _client.Start();
            _client.StartAutoReceive();
        }
        protected async Task OnStateChanged(object sender, SocketStateChangedArgs e)
        {
            try
            {
                Connected = e.Connected;
                _buffer = Connected ? new List<byte>(1024) : null;
                if (e.Connected)
                {
                    await OnConnectedAsync().ConfigureAwait(false);
                }
                else
                {
                    await OnDisconnectedAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError($"{_name} {Id} Error: {ex.Message}");
            }
        }
        protected async Task OnReceivedData(object sender, SocketDataReceivedArgs e)
        {
            if (!await _receiveLock.WaitAsync(500).ConfigureAwait(false))
            {
                if (Device.Instance.Mode == DeviceMode.Administrator)
                {
                    Logger?.LogDebug(string.Format(LangProvider.Instance.ReceiveBusyDropMessage, e.Data?.Length ?? 0));
                }
                return;
            }
            try
            {
                if (_buffer is null)
                {
                    return;
                }
                _buffer.AddRange(e.Data);
                while (_buffer.Count > 7)//缓冲区字节数量至少大于7个才能组成有效的报文帧格式
                {
                    //如果前两个字节无法组成有效帧头，移除第一个字节
                    ushort header = _buffer.Slice(0, 2).ToUInt16(0, Endianness.Big);
                    if (header != FrameHeader)
                    {
                        _buffer.RemoveAt(0);
                        continue;
                    }
                    //获取数据长度
                    int length = _buffer.Slice(2 + 1, 4).ToInt32(0, Endianness.Big);

                    //长度非法（负数或超出上限）时丢弃当前帧头并重同步，防止解析卡死与内存堆积
                    if (length < 0 || length > MaxPayloadLength)
                    {
                        Logger?.LogError(string.Format(LangProvider.Instance.AnalysisFrameErrorMessage, string.Format(LangProvider.Instance.InvalidFrameLengthMessage, length)));
                        _buffer.RemoveAt(0);
                        continue;
                    }
                    int total = 2 + 1 + 4 + length + 4 + 2;//帧头 功能码 数据长度 数据数值 CRC校验 帧尾
                    if (_buffer.Count < total)
                    {
                        break;//终止对本次接收到数据解析，保留缓冲区数据到下次数据接收
                    }
                    //CRC32一致性验证
                    byte[] data = _buffer.Slice(2 + 1 + 4, length).ToArray();
                    uint receiveCrc32 = _buffer.Slice(7 + length, 4).ToUInt32(0, Endianness.Big);
                    uint calcCrc32 = SecurityUtil.CRC32(data);
                    if (calcCrc32 != receiveCrc32)
                    {
                        Logger?.LogError(string.Format(LangProvider.Instance.AnalysisFrameErrorMessage, string.Format(LangProvider.Instance.Crc32NotMatchMessage, $"Rx: {data.ToHex()} RxCRC32: {receiveCrc32.ToBytes(Endianness.Big).ToHex()} CaCRC32: {calcCrc32.ToBytes(Endianness.Big).ToHex()}")));
                        _buffer.RemoveRange(0, total);
                        continue;
                    }

                    ushort ending = _buffer.Slice(2 + 1 + 4 + length + 4, 2).ToUInt16(0, Endianness.Big);
                    if (ending != FrameEnding)
                    {
                        Logger?.LogError(string.Format(LangProvider.Instance.AnalysisFrameErrorMessage, LangProvider.Instance.FrameEndingNotMatchMessage));
                        _buffer.RemoveRange(0, total);
                        continue;
                    }

                    byte code = _buffer[2];
                    _buffer.RemoveRange(0, total);
                    await OnFrameReceivedAsync(code, data, receiveCrc32.ToBytes(Endianness.Big)).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError(string.Format(LangProvider.Instance.AnalysisFrameErrorMessage, $"{ex.Message}"));
            }
            finally
            {
                _receiveLock.Release();
            }
        }
        protected async Task SendAsync(TEnum code, params byte[] data)
        {
            if (!Connected)
            {
                return;
            }

            bool semaphoreHeld = false;
            try
            {
                await _semaphore.WaitAsync().ConfigureAwait(false);
                semaphoreHeld = true;

                int length = data?.Length ?? 0;
                byte[] frame = new byte[length + 13];

                int index = 0;
                // 帧头
                Array.Copy(FrameHeader.ToBytes(Endianness.Big), 0, frame, index, 2);
                index += 2;
                // 功能代码
                frame[index++] = Convert.ToByte(code);
                // 数据长度
                Array.Copy(length.ToBytes(Endianness.Big), 0, frame, index, 4);
                index += 4;
                // 数据内容
                if (data is not null)
                {
                    Array.Copy(data, 0, frame, index, data.Length);
                    index += data.Length;
                }
                // CRC32校验
                uint check = 0u;
                if (data is not null)
                {
                    check = SecurityUtil.CRC32(data);
                }
                byte[] checkBytes = check.ToBytes(Endianness.Big);
                Array.Copy(checkBytes, 0, frame, index, 4);
                index += 4;
                // 帧尾
                Array.Copy(FrameEnding.ToBytes(Endianness.Big), 0, frame, index, 2);

                // 发送
                await _client.SendAsync(frame).ConfigureAwait(false);


                if (Logger is not null && Device.Instance.Mode == DeviceMode.Administrator && FrameFilterUtility.Filter(Logger, code))
                {
                    Logger?.LogDebug($"Tx: [ {code.GetString()} ](0x{Convert.ToUInt64(code):X2}) [ CRC32 {checkBytes.ToHex()} ] {data?.ToHex()}");
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError($"Tx: [ {code.GetString()} ](0x{Convert.ToUInt64(code):X2}) Error: {ex.Message}");
            }
            finally
            {
                if (semaphoreHeld)
                {
                    _semaphore.Release();
                }
            }
        }
        protected async Task SendAsync(TEnum code, Stream stream, Action<int> readCallback, params byte[] beforeStream)
        {
            if (!Connected)
            {
                return;
            }

            bool semaphoreHeld = false;
            try
            {
                await _semaphore.WaitAsync().ConfigureAwait(false);
                semaphoreHeld = true;

                // 计算数据长度，超过 4096 字节则分多次发送，否则直接调用 SendAsync 发送
                long beforeLength = beforeStream?.Length ?? 0;
                long streamLength = stream?.Length ?? 0;
                long length = beforeLength + streamLength;
                if (length > uint.MaxValue)
                {
                    throw new ArgumentOutOfRangeException(nameof(length));
                }

                // 大包：帧头 + 数据流式发送 + 帧尾
                // 发送帧头
                byte[] header = BuildFrameHeader(code, (uint)length);
                await _client.SendAsync(header).ConfigureAwait(false);
                // 发送数据内容
                uint check = SecurityUtil.CRC32(Array.Empty<byte>()); // 初始化 CRC32
                if (beforeStream is { Length: > 0 })
                {
                    check = SecurityUtil.CRC32(check, beforeStream);
                    await _client.SendAsync(beforeStream).ConfigureAwait(false);
                }
                if (stream is not null)
                {
                    byte[] buffer = new byte[1024];
                    int read;
                    while ((read = await stream.ReadAsync(buffer).ConfigureAwait(false)) > 0)
                    {
                        if (!Connected)
                        {
                            return;
                        }
                        if (read == buffer.Length)
                        {
                            check = SecurityUtil.CRC32(check, buffer);
                            await _client.SendAsync(buffer).ConfigureAwait(false);
                        }
                        else
                        {
                            byte[] temp = new byte[read];
                            Array.Copy(buffer, 0, temp, 0, read);
                            check = SecurityUtil.CRC32(check, temp);
                            await _client.SendAsync(temp).ConfigureAwait(false);
                        }
                        readCallback?.Invoke(read);
                    }
                }
                // 发送帧尾
                byte[] checkBytes = check.ToBytes(Endianness.Big);
                byte[] tail = BuildFrameTail(check);
                await _client.SendAsync(tail).ConfigureAwait(false);

                if (Logger is not null && FrameFilterUtility.Filter(Logger, code))
                {
                    Logger?.LogInfo($"Tx: [ {code.GetString()} ](0x{Convert.ToUInt64(code):X2}) [ CRC32 {checkBytes.ToHex()} ] {beforeStream?.ToHex()}");
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError($"Tx: [ {code.GetString()} ](0x{Convert.ToUInt64(code):X2}) Error: {ex.Message}");
            }
            finally
            {
                if (semaphoreHeld)
                {
                    _semaphore.Release();
                }
            }
        }
        protected static byte[] BuildFrameHeader(TEnum code, uint length)
        {
            byte[] header = new byte[7];
            int index = 0;
            Array.Copy(FrameHeader.ToBytes(Endianness.Big), 0, header, index, 2); index += 2;
            header[index++] = Convert.ToByte(code);
            Array.Copy(length.ToBytes(Endianness.Big), 0, header, index, 4);
            return header;
        }
        protected static byte[] BuildFrameTail(uint check)
        {
            byte[] tail = new byte[6];
            int index = 0;
            Array.Copy(check.ToBytes(Endianness.Big), 0, tail, index, 4); index += 4;
            Array.Copy(FrameEnding.ToBytes(Endianness.Big), 0, tail, index, 2);
            return tail;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                if (_client is not null)
                {
                    _client.StateChanged -= OnStateChanged;
                    _client.ReceivedData -= OnReceivedData;
                    _client.Dispose();
                    _client = null;
                }
                Logger?.Dispose();
                Logger = null;
                _semaphore.Dispose();
                _receiveLock.Dispose();
            }
            _disposed = true;
        }

        /// <summary>
        /// 由派生类实现连接后的动作
        /// </summary>
        protected abstract Task OnConnectedAsync();

        /// <summary>
        /// 由派生类实现断开后的清理
        /// </summary>
        protected abstract Task OnDisconnectedAsync();

        /// <summary>
        /// 由派生类实现具体的报文解析
        /// </summary>
        protected abstract Task OnFrameReceivedAsync(byte code, byte[] data, byte[] crcBytes);
    }
}