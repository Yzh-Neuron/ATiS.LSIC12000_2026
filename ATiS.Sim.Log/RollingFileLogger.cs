using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace ATiS.Sim.Log
{
    /// <summary>
    /// 滚动日志
    /// </summary>
    public sealed class RollingFileLogger : IDisposable
    {
        /// <summary>
        /// 消息格式，花括号为参数：{level} 等级、{date:格式} 时间、{msg} 消息
        /// </summary>
        public const string DefaultMessageFormat = "[{level}][{date:yyyy-MM-dd HH:mm:ss}]{msg}";
        private const int MaxBatch = 512;
        private const int BatchBufferSize = 128 * 1024;
        private const int StreamBufferSize = 64 * 1024;
        private const int IdleWaitMs = 2000;
        private const int EnqueueSpinBudget = 16;
        private const string DateToken = "{date:";

        private readonly RingBuffer<Entry> _queue;
        private readonly Encoding _encoding;
        private readonly byte[] _newline;
        private readonly MessageTemplate _template;
        private readonly ManualResetEventSlim _wake = new ManualResetEventSlim(false);
        private readonly Thread _writer;

        private readonly bool _timeRolling;
        private readonly long _maxSize;
        private readonly string _pathPrefix = string.Empty;
        private readonly string _pathSuffix = string.Empty;
        private readonly string _fileDateFormat = string.Empty;
        private readonly string _staticPath = string.Empty;

        private FileStream _stream;
        private string _openPath;
        private long _currentSize;

        private volatile bool _running = true;
        private volatile bool _writerIdle;
        private bool _disposed;
        private long _dropped;

        /// <summary>
        /// 最低输出等级，低于它的调用直接丢弃
        /// </summary>
        public LogLevel MinimumLevel { get; set; }

        /// <summary>
        /// 后台写异常回调
        /// </summary>
        public event Action<Exception> WriterError;

        /// <param name="filePath">非 Infinite 时必须含 {date:格式}</param>
        /// <param name="interval">时间滚动粒度，默认 Infinite</param>
        /// <param name="maxFileSizeBytes">文件字节上限，超过归档 _1 _2，默认 64MB</param>
        /// <param name="minimumLevel">最低输出等级</param>
        /// <param name="messageFormat">消息格式，默认 [{level}][{date:Format}]{msg}，参数写错直接抛异常</param>
        /// <param name="ringCapacity">环形队列容量。</param>
        public RollingFileLogger(string filePath, RollingInterval interval = RollingInterval.Infinite, long maxFileSizeBytes = 64L * 1024 * 1024, LogLevel minimumLevel = LogLevel.Debug, string messageFormat = null, int ringCapacity = 4096)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

            if (maxFileSizeBytes <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxFileSizeBytes), "必须大于 0");
            }
            if (ringCapacity < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(ringCapacity), "必须大于 0");
            }

            MinimumLevel = minimumLevel;
            _maxSize = maxFileSizeBytes;
            _encoding = new UTF8Encoding(false);
            _newline = _encoding.GetBytes(Environment.NewLine);
            _template = new MessageTemplate(messageFormat ?? DefaultMessageFormat);
            _queue = new RingBuffer<Entry>(ringCapacity);

            string fullPath = Path.GetFullPath(filePath);
            bool hasToken = TryParsePathDate(fullPath, out string prefix, out string dateFormat, out string suffix);
            _timeRolling = interval != RollingInterval.Infinite;
            if (_timeRolling && !hasToken)
            {
                throw new ArgumentException($"{interval} 滚动要求文件路径包含 {{date:格式}}，例如 app_{{date:yyyyMMdd}}.log", nameof(filePath));
            }

            _pathPrefix = prefix;
            _pathSuffix = suffix;
            _fileDateFormat = dateFormat;

            if (!_timeRolling)
            {
                // Infinite：{date} 若存在只在创建时替换一次，得到静态文件名
                _staticPath = hasToken ? BuildPath(DateTimeOffset.Now) : fullPath;
                EnsureDirectory(_staticPath);
                SwitchFile(_staticPath);
            }
            else
            {
                EnsureDirectory(_pathPrefix); // 时间模式按写入时间惰性打开
            }

            _writer = new Thread(WriterLoop)
            {
                IsBackground = true,
                Name = "RollingFileLogger-Writer"
            };
            _writer.Start();
        }

        public void Debug(string message)
        {
            Log(LogLevel.Debug, message);
        }
        public void Info(string message)
        {
            Log(LogLevel.Info, message);
        }
        public void Warn(string message)
        {
            Log(LogLevel.Warn, message);
        }
        public void Error(string message)
        {
            Log(LogLevel.Error, message);
        }
        public void Fatal(string message)
        {
            Log(LogLevel.Fatal, message);
        }

        /// <summary>
        /// 写一条指定等级的日志
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        public void Log(LogLevel level, string message)
        {
            ArgumentNullException.ThrowIfNull(message);

            if (!_running || level < MinimumLevel)
            {
                return;
            }

            var now = DateTimeOffset.Now;
            string line = _template.Render(level, message, now);
            var entry = new Entry(line, now);

            // 队列空说明写线程可能在等待，需要唤醒
            bool needWake = _queue.Count == 0;
            if (!TryEnqueue(entry))
            {
                // 队列持续满（磁盘卡顿/日志风暴），丢弃并计数，绝不阻塞调用方业务线程
                Interlocked.Increment(ref _dropped);
                _wake.Set();
                return;
            }

            if (needWake)
            {
                _wake.Set();
            }
        }

        /// <summary>
        /// 入队；队列满时仅做有限次自旋等待（毫秒级），超时即放弃，
        /// 避免磁盘卡顿时把通信解析、定时、UI 等业务线程无限拖死
        /// </summary>
        private bool TryEnqueue(Entry entry)
        {
            if (_queue.TryEnqueue(entry))
            {
                return true;
            }

            _wake.Set();
            SpinWait spin = default;
            while (_running && spin.Count < EnqueueSpinBudget)
            {
                spin.SpinOnce();

                if (_queue.TryEnqueue(entry))
                {
                    return true;
                }
            }

            return _running && _queue.TryEnqueue(entry);
        }

        /// <summary>
        /// 写入磁盘
        /// </summary>
        /// <param name="timeoutMilliseconds"></param>
        /// <returns></returns>
        public bool Flush(int timeoutMilliseconds = 5000)
        {
            _wake.Set();
            var sw = Stopwatch.StartNew();
            while ((!_writerIdle || _queue.Count > 0) && sw.ElapsedMilliseconds < timeoutMilliseconds)
            {
                Thread.Sleep(1);
                _wake.Set();
            }
            return _writerIdle && _queue.Count == 0;
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            _running = false;
            _wake.Set();

            if (_writer.IsAlive)
            {
                _writer.Join(5000);
            }
            _stream?.Dispose();
            _wake.Dispose();
        }

        /// <summary>
        /// 写线程
        /// </summary>
        private void WriterLoop()
        {
            byte[] batch = ArrayPool<byte>.Shared.Rent(BatchBufferSize);
            int[] segEnds = ArrayPool<int>.Shared.Rent(MaxBatch);
            DateTimeOffset[] segTimes = ArrayPool<DateTimeOffset>.Shared.Rent(MaxBatch);

            try
            {
                while (true)
                {
                    int count = 0;
                    int used = 0;
                    try
                    {
                        long dropped = Interlocked.Exchange(ref _dropped, 0);
                        if (dropped > 0)
                        {
                            Entry notice = new Entry(_template.Render(LogLevel.Warn, $"日志队列已满，已丢弃 {dropped} 条日志", DateTimeOffset.Now), DateTimeOffset.Now);
                            used += EncodeLine(notice.Line, ref batch, used);
                            segEnds[count] = used;
                            segTimes[count] = notice.Time;
                            count++;
                        }

                        while (count < MaxBatch && _queue.TryDequeue(out Entry entry))
                        {
                            used += EncodeLine(entry.Line, ref batch, used);
                            segEnds[count] = used;
                            segTimes[count] = entry.Time;
                            count++;
                        }

                        if (count > 0)
                        {
                            WriteBatch(batch, segEnds, segTimes, count);
                        }
                        else
                        {
                            _stream?.Flush();
                            _writerIdle = true;
                            // 退出前必须确认队列已空：最后一条可能恰在判断期间入队
                            if (!_running && _queue.Count == 0)
                            {
                                break;
                            }
                            _wake.Wait(IdleWaitMs);
                            _wake.Reset();
                            _writerIdle = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        RaiseError(ex);
                        Thread.Sleep(20);
                    }

                    if (!_running && _queue.Count == 0)
                    {
                        break;
                    }
                }
            }
            finally
            {
                try { _stream?.Flush(); }
                catch (Exception ex) { RaiseError(ex); }

                ArrayPool<byte>.Shared.Return(batch);
                ArrayPool<int>.Shared.Return(segEnds);
                ArrayPool<DateTimeOffset>.Shared.Return(segTimes);
            }
        }

        /// <summary>
        /// 拼装消息
        /// </summary>
        private int EncodeLine(string line, ref byte[] batch, int used)
        {
            int total = _encoding.GetByteCount(line) + _newline.Length;
            if (used + total > batch.Length)
            {
                byte[] grown = ArrayPool<byte>.Shared.Rent(Math.Max(batch.Length << 1, used + total));
                Array.Copy(batch, grown, used);
                ArrayPool<byte>.Shared.Return(batch);
                batch = grown;
            }

            Span<byte> dst = batch.AsSpan(used);
            int n = _encoding.GetBytes(line.AsSpan(), dst);
            _newline.CopyTo(dst.Slice(n));
            return total;
        }

        /// <summary>
        /// 写入一个批次的消息
        /// </summary>
        private void WriteBatch(byte[] data, int[] segEnds, DateTimeOffset[] segTimes, int count)
        {
            int cursor = 0;
            for (int i = 0; i < count; i++)
            {
                int len = segEnds[i] - cursor;

                if (_timeRolling)
                {
                    string want = BuildPath(segTimes[i]);
                    if (_stream == null || !string.Equals(want, _openPath, StringComparison.Ordinal))
                    {
                        SwitchFile(want);
                    }
                }
                else if (_stream == null)
                {
                    SwitchFile(_staticPath);
                }

                if (_currentSize > 0 && _currentSize + len > _maxSize)
                {
                    RollSizeFile();
                }

                _stream.Write(data, cursor, len);
                _currentSize += len;
                cursor = segEnds[i];
            }
        }

        /// <summary>
        /// 打开（或切换到）指定文件：已存在则追加并继承其大小
        /// </summary>
        /// <param name="path"></param>
        private void SwitchFile(string path)
        {
            if (_stream != null)
            {
                _stream.Flush();
                _stream.Dispose();
                _stream = null;
            }

            EnsureDirectory(path);
            bool exists = File.Exists(path);
            _stream = new FileStream(
                path,
                exists ? FileMode.Append : FileMode.Create,
                FileAccess.Write,
                FileShare.Read,
                StreamBufferSize,
                FileOptions.SequentialScan);

            _openPath = path;
            _currentSize = exists ? new FileInfo(path).Length : 0;
        }

        /// <summary>
        /// 大小滚动：当前文件改名 xxx_N.log，再新建同名文件
        /// </summary>
        private void RollSizeFile()
        {
            string basePath = _openPath!;
            if (_stream != null)
            {
                _stream.Flush();
                _stream.Dispose();
                _stream = null;
            }

            if (File.Exists(basePath))
            {
                int index = 1;
                string archive = GetArchivePath(basePath, index);
                while (File.Exists(archive))
                {
                    archive = GetArchivePath(basePath, ++index);
                }
                File.Move(basePath, archive);
            }

            SwitchFile(basePath);
        }

        /// <summary>
        /// 获取带后缀的路径
        /// </summary>
        private string GetArchivePath(string basePath, int index)
        {
            string dir = Path.GetDirectoryName(basePath);
            string name = Path.GetFileNameWithoutExtension(basePath) + "_" +
                          index.ToString(CultureInfo.InvariantCulture) + Path.GetExtension(basePath);
            return string.IsNullOrEmpty(dir) ? name : Path.Combine(dir, name);
        }

        /// <summary>
        /// 拼装时间路径
        /// </summary>
        private string BuildPath(DateTimeOffset time)
        {
            return _pathPrefix + time.ToString(_fileDateFormat, CultureInfo.InvariantCulture) + _pathSuffix;
        }

        /// <summary>
        /// 解析路径中的 {date:格式}
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="prefix"></param>
        /// <param name="format"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private static bool TryParsePathDate(string fullPath, out string prefix, out string format, out string suffix)
        {
            int token = fullPath.IndexOf(DateToken, StringComparison.OrdinalIgnoreCase);
            if (token < 0)
            {
                prefix = format = suffix = string.Empty;
                return false;
            }

            int close = fullPath.IndexOf('}', token + DateToken.Length);
            if (close < 0)
            {
                throw new ArgumentException("文件路径中的 {date:格式} 缺少右花括号 }");
            }

            prefix = fullPath.Substring(0, token);
            format = fullPath.Substring(token + DateToken.Length, close - token - DateToken.Length);
            suffix = fullPath.Substring(close + 1);

            if (format.Length == 0)
            {
                throw new ArgumentException("{date:格式} 中的格式不能为空");
            }
            return true;
        }

        /// <summary>
        /// 确保文件夹存在
        /// </summary>
        /// <param name="filePath"></param>
        private static void EnsureDirectory(string filePath)
        {
            string dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        /// <summary>
        /// 上报写入时候的错误消息
        /// </summary>
        /// <param name="ex"></param>
        private void RaiseError(Exception ex)
        {
            try
            {
                WriterError?.Invoke(ex);
            }
            catch
            {
            }
        }

    }

    /// <summary>
    /// 消息块
    /// </summary>
    internal readonly struct Entry
    {
        public readonly string Line;
        public readonly DateTimeOffset Time;
        public Entry(string line, DateTimeOffset time)
        {
            Line = line;
            Time = time;
        }
    }


}
