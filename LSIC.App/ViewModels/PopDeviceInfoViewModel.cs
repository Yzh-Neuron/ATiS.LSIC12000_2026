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
            QueryVersionCommand = new AsyncRelayCommand(QueryVersion);
            ExportCommand = new RelayCommand(Export);
        }
        private async Task QueryVersion()
        {
            if (SelectedZone != null)
            {
                await SelectedZone.QueryVersionAsync();
            }
        }
        private void Export()
        {
            var dialog = new SaveFileDialog
            {
                Title = LangProvider.Instance.ExportAsCsvFileText,
                FileName = $"{LangProvider.Instance.DeviceInfoText}.csv",
                Filter = $"{LangProvider.Instance.CsvFileText}|*.csv"
            };
            if (dialog.ShowDialog() != true)
            {
                return;
            }

            var builder = new StringBuilder();
            builder.AppendLine(string.Join(",",
            Text("ZoneText"),
            Text("BoardTypeText"),
            Text("IdText"),
            Text("BoardIndexText"),
            "PN", "SN",
            Text("HardwareVersionText"),
            Text("FirmwareVersionText")));

            foreach (var zone in Zones)
            {
                foreach (var b in zone.Boards)
                {
                    builder.AppendLine(string.Join(",", zone.Id, b.DisplayName, b.Id, b.HasParent ? b.ParentId.ToString() : "-", b.PN, b.SN, b.HardwareVersion, string.Join(";", b.Firmwares.Select(f => f.ToString()))));
                }
            }

            try
            {
                File.WriteAllText(dialog.FileName, builder.ToString(), Encoding.GetEncoding("gb2312"));
                MessagePop.Show(LangProvider.Instance.FileExportSuccessMessage, MessageType.Success);
            }
            catch
            {
                MessagePop.Show(LangProvider.Instance.FileExportFailureMessage, MessageType.Error);
            }
        }
        private static string Text(string key) => LangProvider.Instance.GetString(key);
    }
}