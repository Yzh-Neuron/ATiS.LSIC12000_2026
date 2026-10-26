using System.Data;
using OxyPlot;
using OxyPlot.Axes;

namespace LSIC.App.ViewModels
{
    public class CtrLogBrowseViewModel : ObservableObject
    {
        private readonly BusyCounter _busyCounter;
        private readonly LogRule[] _rules;

        private LogRule _matchedRule;
        private LineReader _reader;
        private long _start;
        private long _end;

        private string _currentFile;
        private int _pageSize = 20;
        private long _currentPage = 1;
        private long _totalPage;
        private DateTime _displayStartTime;
        private DateTime _displayEndTime;
        private DateTime _startTime;
        private DateTime _endTime;
        private DataLine[] _lines;
        private bool _busy;
        private double _progress;

        public string CurrentFile
        {
            get => _currentFile;
            private set => SetProperty(ref _currentFile, value);
        }
        public int PageSize
        {
            get => _pageSize;
            set => SetProperty(ref _pageSize, value);
        }
        public long CurrentPage
        {
            get => _currentPage;
            private set
            {
                if (SetProperty(ref _currentPage, value))
                {
                    if (value != 0)
                    {
                        RunBusyAsync(RefreshAsync);
                    }
                }
            }
        }
        public long TotalPage
        {
            get => _totalPage;
            private set => SetProperty(ref _totalPage, value);
        }
        public DateTime DisplayStartTime
        {
            get => _displayStartTime;
            private set => SetProperty(ref _displayStartTime, value);
        }
        public DateTime DisplayEndTime
        {
            get => _displayEndTime;
            private set => SetProperty(ref _displayEndTime, value);
        }
        public DateTime StartTime
        {
            get => _startTime;
            set => SetProperty(ref _startTime, value);
        }
        public DateTime EndTime
        {
            get => _endTime;
            set => SetProperty(ref _endTime, value);
        }
        public DataLine[] Lines
        {
            get => _lines;
            private set => SetProperty(ref _lines, value);
        }
        public bool Busy
        {
            get => _busy;
            private set => SetProperty(ref _busy, value);
        }
        public double Progress
        {
            get => _progress;
            private set => SetProperty(ref _progress, value);
        }

        public ICommand ChooseFileCommand { get; }
        public ICommand RefreshSourceCommand { get; }
        public ICommand FilterCommand { get; }
        public ICommand FirstPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand LastPageCommand { get; }
        public ICommand ViewPlotCommand { get; }
        public ICommand ExportCsvCommand { get; }
        public ICommand BatchExportCommand { get; }

        public CtrLogBrowseViewModel()
        {
            _busyCounter = new BusyCounter(v => Busy = v);
            _rules = Device.Instance.SystemConfig.LogRules;

            ChooseFileCommand = new RelayCommand(ChooseFile);
            RefreshSourceCommand = new RelayCommand(RefreshSource, CanRefreshSource);
            FilterCommand = new RelayCommand(Filter, CanFilter);
            FirstPageCommand = new RelayCommand(FirstPage, CanFirstPage);
            PreviousPageCommand = new RelayCommand(PreviousPage, CanPreviousPage);
            NextPageCommand = new RelayCommand(NextPage, CanNextPage);
            LastPageCommand = new RelayCommand(LastPage, CanLastPage);
            ViewPlotCommand = new RelayCommand(ViewPlot, CanViewPlot);
            ExportCsvCommand = new RelayCommand(ExportCsv, CanExportCsv);
            BatchExportCommand = new RelayCommand(BatchExport);
        }

        private void ChooseFile()
        {
            string current = CurrentFile;
            string filter = $"{LangProvider.Instance.DataLogText}|*.log";

            var msgResult = MessageBus.Default.Send(MsgArgs.From(CtrLogBrowseMessage.OpenFile, [filter, current])).First();
            if (msgResult.Result is string filePath)
            {
                RunBusyAsync(async () =>
                {
                    LogRule rule = _rules.FirstOrDefault(i => filePath.IsMatch(i.PathPattern));
                    if (rule is not null)
                    {
                        if (_matchedRule != rule)
                        {
                            _matchedRule = rule;

                            this.InvokeOnUI(() =>
                            {
                                Lines = null;
                                MessageBus.Default.Send(MsgArgs.From(CtrLogBrowseMessage.LogRuleChanged, [rule]));
                            });
                        }

                        CurrentFile = filePath;

                        _reader?.Dispose();
                        _reader = null;
                        _reader = new LineReader(filePath, rule.MinLogLineBytes, new UTF8Encoding(false));

                        if (_reader.LineCount == 0)
                        {
                            _start = 0;
                            _end = -1;
                            DisplayStartTime = DisplayEndTime = DateTime.Now;
                            StartTime = EndTime = DateTime.Now.Date;
                            ResetPagination();
                            return;
                        }

                        _start = 0;
                        _end = Math.Max(0, _reader.LineCount - 1);

                        // 获取开始时间
                        string line = await _reader.GetLineAsync(_start).ConfigureAwait(false);
                        string timeString = line.Substring(0, _matchedRule.TimeFormat.Length);
                        DisplayStartTime = DateTime.ParseExact(timeString, _matchedRule.TimeFormat, null);
                        StartTime = DisplayStartTime.Date;

                        // 获取结束时间
                        line = await _reader.GetLineAsync(_end).ConfigureAwait(false);
                        timeString = line.Substring(0, _matchedRule.TimeFormat.Length);
                        DisplayEndTime = DateTime.ParseExact(timeString, _matchedRule.TimeFormat, null);
                        EndTime = DisplayEndTime.Date;

                        CurrentPage = 0;
                        TotalPage = (long)Math.Ceiling(_reader.LineCount * 1f / PageSize);
                        CurrentPage = 1;
                    }
                    else
                    {
                        MessageBus.Default.Send(MsgArgs.From(CtrLogBrowseMessage.Growl, [LangProvider.Instance.UnsupportedDataFileMessage, MessageType.Error]));
                    }
                });
            }
        }
        private bool CanRefreshSource()
        {
            return _reader is not null;
        }
        private void RefreshSource()
        {
            RunBusyAsync(() =>
            {
                string path = _reader.Path;
                _reader?.Dispose();
                _reader = null;
                _reader = new LineReader(path, _matchedRule.MinLogLineBytes, new UTF8Encoding(false));

                return FilterAsync();
            });
        }
        private bool CanFilter()
        {
            return _reader is not null;
        }
        private void Filter()
        {
            RunBusyAsync(FilterAsync);
        }
        private bool CanFirstPage()
        {
            return _reader is not null && _currentPage > 1;
        }
        private void FirstPage()
        {
            CurrentPage = 1;
        }
        private bool CanPreviousPage()
        {
            return _reader is not null && _currentPage > 1;
        }
        private void PreviousPage()
        {
            CurrentPage--;
        }
        private bool CanNextPage()
        {
            return _reader is not null && _currentPage < _totalPage;
        }
        private void NextPage()
        {
            CurrentPage++;
        }
        private bool CanLastPage()
        {
            return _reader is not null && _currentPage < _totalPage;
        }
        private void LastPage()
        {
            CurrentPage = _totalPage;
        }
        private bool CanExportCsv()
        {
            return _matchedRule is not null && _reader is not null;
        }
        private void ExportCsv()
        {
            string name = Path.GetFileNameWithoutExtension(_reader.Path);
            string fileName = $"{name}.csv";
            string filter = $"{LangProvider.Instance.CsvFileText}|*.csv";
            var msgResult = MessageBus.Default.Send(MsgArgs.From(CtrLogBrowseMessage.Export, [filter, fileName])).First();
            if (msgResult.Result is string target)
            {
                RunBusyAsync(async () =>
                {
                    long total = _end - _start + 1;

                    var fileOptions = new FileStreamOptions
                    {
                        Mode = FileMode.OpenOrCreate,
                        Access = FileAccess.Write,
                        Share = FileShare.Read,
                        BufferSize = 64 * 1024,
                        Options = FileOptions.Asynchronous | FileOptions.SequentialScan
                    };

                    await using (var stream = new FileStream(target, fileOptions))
                    await using (var writer = new StreamWriter(stream, Encoding.GetEncoding("gb2312"), bufferSize: 8192, leaveOpen: false))
                    {
                        LogColumn[] orderColumns = _matchedRule.Columns;
                        int columnCount = _matchedRule.Columns.Length;

                        StringBuilder builder = new StringBuilder();
                        builder = builder.Append($"{LangProvider.Instance.TimeText},");
                        builder = builder.Append(string.Join(",", orderColumns.Select(i => LangProvider.Instance.GetString(i.Header))));
                        await writer.WriteLineAsync(builder).ConfigureAwait(false);
                        builder.Clear();

                        for (long i = _start; i <= _end; i++)
                        {
                            string line = await _reader.GetLineAsync(i).ConfigureAwait(false);
                            if (line is null)
                            {
                                continue;
                            }

                            string timeString = line.Substring(0, _matchedRule.TimeFormat.Length);
                            line = line.Substring(_matchedRule.TimeFormat.Length + 1);
                            DateTime time = DateTime.ParseExact(timeString, _matchedRule.TimeFormat, null);
                            builder = builder.Append(time.ToString("yyyy-MM-dd HH:mm:ss"));
                            builder = builder.Append(',');

                            string[] arr = line.Split(_matchedRule.SplitChar.ToChar());
                            string[] columns = new string[columnCount];

                            for (int j = 0; j < columnCount; j++)
                            {
                                LogColumn col = _matchedRule.Columns[j];

                                if (j < arr.Length && int.TryParse(arr[j], out int value))
                                {
                                    double val = col.Scale == 0 ? value : value * 1d / col.Scale;

                                    if (col.Formats is null)
                                    {
                                        columns[j] = val.ToString(CultureInfo.InvariantCulture);
                                    }
                                    else
                                    {
                                        columns[j] = col.Formats.Format(val);
                                    }
                                }
                                else
                                {
                                    columns[j] = "N/A";
                                }
                            }

                            builder = builder.Append(string.Join(",", columns));
                            await writer.WriteLineAsync(builder).ConfigureAwait(false);
                            builder.Clear();
                            Progress = (i - _start + 1) * 100d / total;
                        }
                    }
                    MessageBus.Default.Send(MsgArgs.From(CtrLogBrowseMessage.Growl, [LangProvider.Instance.FileExportSuccessMessage, MessageType.Success]));
                });
            }
        }
        private bool CanViewPlot()
        {
            return _matchedRule is not null && _reader is not null;
        }
        private void ViewPlot()
        {
            var msgResult = MessageBus.Default.Send(MsgArgs.From(CtrLogBrowseMessage.SelectColumns, [_matchedRule])).First();
            if (msgResult.Result is LogColumn[] columns)
            {
                RunBusyAsync(async () =>
                {
                    double min = double.MaxValue;
                    double max = double.MinValue;

                    KeyValuePair<LogColumn, List<DataPoint>>[] data = new KeyValuePair<LogColumn, List<DataPoint>>[columns.Length];
                    for (int i = 0; i < columns.Length; i++)
                    {
                        data[i] = new KeyValuePair<LogColumn, List<DataPoint>>(columns[i], new List<DataPoint>());
                    }
                    long total = _end - _start + 1;

                    var columnIndex = new Dictionary<LogColumn, int>();
                    for (int k = 0; k < _matchedRule.Columns.Length; k++)
                    {
                        columnIndex[_matchedRule.Columns[k]] = k;
                    }

                    for (long i = _start; i <= _end; i++)
                    {
                        string line = await _reader.GetLineAsync(i).ConfigureAwait(false);
                        if (line is null)
                        {
                            break;
                        }

                        string timeString = line.Substring(0, _matchedRule.TimeFormat.Length);
                        line = line.Substring(_matchedRule.TimeFormat.Length + 1);
                        DateTime time = DateTime.ParseExact(timeString, _matchedRule.TimeFormat, null);
                        string[] arr = line.Split(_matchedRule.SplitChar.ToChar());

                        for (int j = 0; j < columns.Length; j++)
                        {
                            LogColumn column = columns[j];
                            int index = columnIndex[column];

                            if (arr.Length > index && int.TryParse(arr[index], out int value))
                            {
                                double val = column.Scale == 0 ? value : value * 1d / column.Scale;

                                min = Math.Min(min, val);
                                max = Math.Max(max, val);

                                DataPoint point = DateTimeAxis.CreateDataPoint(time, val);
                                data[j].Value.Add(point);
                            }
                        }
                        Progress = (i - _start + 1) * 100d / total;
                    }

                    LogPlot plotData = new LogPlot(min, max, data);

                    this.InvokeOnUI(() =>
                    {
                        MessageBus.Default.Send(MsgArgs.From(CtrLogBrowseMessage.Plot, [plotData, MessageType.Info]));
                    });
                });
            }
        }
        private void BatchExport()
        {
            //throw new NotImplementedException();
        }
        private async Task RefreshAsync()
        {
            if (_reader is null)
            {
                Lines = Array.Empty<DataLine>();
                return;
            }

            List<DataLine> lines = new List<DataLine>(PageSize);
            long index = _start + (CurrentPage - 1) * PageSize;
            int columnCount = _matchedRule.Columns.Length;

            for (int i = 0; i < PageSize; i++)
            {
                if (index > _end)
                {
                    break;
                }

                string line = await _reader.GetLineAsync(index++).ConfigureAwait(false);
                if (line is null)
                {
                    continue;
                }

                string timeString = line.Substring(0, _matchedRule.TimeFormat.Length);
                DateTime time = DateTime.ParseExact(timeString, _matchedRule.TimeFormat, null);

                line = line.Substring(_matchedRule.TimeFormat.Length + 1);
                string[] arr = line.Split(_matchedRule.SplitChar.ToChar());
                string[] columns = new string[columnCount];

                for (int j = 0; j < columnCount; j++)
                {
                    LogColumn col = _matchedRule.Columns[j];

                    if (j < arr.Length && int.TryParse(arr[j], out int value))
                    {
                        double val = col.Scale == 0 ? value : value * 1d / col.Scale;

                        if (col.Formats is null)
                        {
                            columns[j] = val.ToString(CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            columns[j] = col.Formats.Format(val);
                        }
                    }
                    else
                    {
                        columns[j] = "N/A";
                    }
                }

                lines.Add(new DataLine(time, columns));
            }
            Lines = lines.ToArray();
        }
        private async Task FilterAsync()
        {
            if (StartTime > EndTime)
            {
                _start = 0;
                _end = -1;
                ResetPagination();
            }
            else
            {
                long maxLine = Math.Max(0, _reader.LineCount - 1);
                long? startLine = null;
                long endLine = 0;
                DateTime startDate = StartTime;
                DateTime endDate = EndTime.AddDays(1d).AddSeconds(-1d);

                for (long i = 0; i <= maxLine; i++)
                {
                    string line = await _reader.GetLineAsync(i).ConfigureAwait(false);
                    if (line is null)
                    {
                        continue;
                    }

                    string timeString = line.Substring(0, _matchedRule.TimeFormat.Length);
                    DateTime time = DateTime.ParseExact(timeString, _matchedRule.TimeFormat, null);

                    if (!startLine.HasValue && time >= startDate)
                    {
                        startLine = i;
                        continue;
                    }

                    if (time <= endDate)
                    {
                        endLine = i;
                    }
                    else
                    {
                        break;
                    }
                }

                if (!startLine.HasValue)
                {
                    _start = 0;
                    _end = -1;
                }
                else
                {
                    _start = startLine.Value;
                    _end = endLine;
                }

                ResetPagination();
            }
        }
        private void ResetPagination()
        {
            CurrentPage = 0;
            TotalPage = Math.Max(1, (long)Math.Ceiling(Math.Max(0, _end - _start + 1) / (double)PageSize));
            CurrentPage = 1;
        }
        private Task RunBusyAsync(Func<Task> action)
        {
            return Task.Run(async () =>
            {
                using (_busyCounter.Enter())
                {
                    Progress = 0d;
                    try
                    {
                        await action();
                    }
                    catch (Exception ex)
                    {
                        MessageBus.Default.Send(MsgArgs.From(CtrLogBrowseMessage.Growl, [ex.Message, MessageType.Error]));
                    }
                    finally
                    {
                        Progress = 100d;
                    }
                }
            });
        }
    }
}