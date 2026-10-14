namespace LSIC.App.ViewModels
{
    public class PopDeviceInfoViewModel : ObservableObject
    {
        public Zone[] Zones { get; }
        public ICommand QueryVersionCommand { get; }
        public ICommand ExportCommand { get; }
        private Zone _selectedZone;
        public Zone SelectedZone
        {
            get => _selectedZone;
            set => SetProperty(ref _selectedZone, value);
        }
        public PopDeviceInfoViewModel()
        {
            Zones = Device.Instance.Zones;
            SelectedZone = Zones.FirstOrDefault();
            QueryVersionCommand = new AsyncRelayCommand(QueryVersionAsync);
            ExportCommand = new RelayCommand(Export);
        }
        private async Task QueryVersionAsync()
        {
            if (SelectedZone != null)
            {
                await SelectedZone.QueryVersionAsync();
            }
        }
        private void Export()
        {
            string fileName = $"{LangProvider.Instance.DeviceInfoText}.csv";
            string filter = $"{LangProvider.Instance.CsvFileText}|*.csv";
            var msgResult = MessageBus.Default.Send(MsgArgs.From(PopDeviceInfoMessage.ExportCsv, [fileName, filter])).First();
            if (msgResult.Result is string target)
            {
                try
                {
                    var builder = new StringBuilder();
                    builder.AppendLine(string.Join(",",
                    LangProvider.Instance.ZoneText,
                    LangProvider.Instance.BoardTypeText,
                    LangProvider.Instance.GetString("IdText"),
                    LangProvider.Instance.BoardIndexText,
                     "PN", "SN",
                    LangProvider.Instance.HardwareVersionText,
                    LangProvider.Instance.FirmwareVersionText));

                    foreach (var zone in Zones)
                    {
                        foreach (var b in zone.Boards)
                        {
                            builder.AppendLine(string.Join(",", zone.Id, b.DisplayName, b.Id, b.HasParent ? b.ParentId.ToString() : "-", b.PN, b.SN, b.HardwareVersion, string.Join(";", b.Firmwares.Select(f => f.ToString()))));
                        }
                    }

                    File.WriteAllText(target, builder.ToString(), Encoding.GetEncoding("gb2312"));
                    MessageBus.Default.Send(MsgArgs.From(PopDeviceInfoMessage.Message, [LangProvider.Instance.FileExportSuccessMessage, MessageType.Success]));
                }
                catch
                {
                    MessageBus.Default.Send(MsgArgs.From(PopDeviceInfoMessage.Message, [LangProvider.Instance.FileExportFailureMessage, MessageType.Error]));
                }
            }
        }
    }
}