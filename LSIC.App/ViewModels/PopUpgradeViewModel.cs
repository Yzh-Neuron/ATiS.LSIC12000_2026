namespace LSIC.App.ViewModels
{
    public class PopUpgradeViewModel : ObservableObject, IDisposable
    {
        public Zone[] Zones { get; private set; }
        public UpgradeItem[] Items { get; }
        public ICommand BrowseCommand { get; }
        public ICommand UpgradeCommand { get; }
        public ICommand ClearUpgradeFileCommand { get; }
        public bool? IsAllSelected
        {
            get => Zones.All(z => z.UpgradeSelected) ? true : Zones.Any(z => z.UpgradeSelected) ? null : false;
            set
            {
                bool selected = value == true;
                foreach (var z in Zones)
                {
                    z.UpgradeSelected = selected;
                }
            }
        }
        public PopUpgradeViewModel()
        {
            Zones = Device.Instance.Zones;
            Items = BoardTypeCatalog.ForNode(nameof(Zone)).Where(t => t.Upgradable).Select(t => new UpgradeItem(t)).ToArray();

            foreach (var z in Zones)
            {
                z.PropertyChanged -= OnZonePropertyChanged;
                z.PropertyChanged += OnZonePropertyChanged;
            }

            BrowseCommand = new RelayCommand<UpgradeItem>(Browse);
            UpgradeCommand = new RelayCommand(Upgrade, CanUpgrade);
            ClearUpgradeFileCommand = new RelayCommand<UpgradeItem>(ClearUpgradeFile);
        }
        private void ClearUpgradeFile(UpgradeItem item)
        {
            if (item != null)
            {
                item.File = string.Empty;
            }
        }
        private void Browse(UpgradeItem item)
        {
            string filter = $"{LangProvider.Instance.FirmwareText}|*.bin|*|*.*";
            var msgResult = MessageBus.Default.Send(MsgArgs.From(PopUpgradeMessage.OpenFile, [filter, null])).First();

            if (msgResult.Result is string filePath)
            {
                if (!string.IsNullOrWhiteSpace(item.Type.FilePrefix) && !Path.GetFileName(filePath).StartsWith(item.Type.FilePrefix, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBus.Default.Send(MsgArgs.From(PopUpgradeMessage.Growl, [string.Format(LangProvider.Instance.UpgradeFilePrefixMismatchMessage, item.Type.FilePrefix), MessageType.Warning]));
                    return;
                }
                item.File = filePath;
            }
        }
        private bool CanUpgrade()
        {
            return Zones?.Any(z => z.UpgradeSelected) == true && Items?.Any(i => !string.IsNullOrWhiteSpace(i.File)) == true;
        }
        private void Upgrade()
        {
            var zones = Zones.Where(z => z.UpgradeSelected).ToArray();
            if (zones.Length == 0)
            {
                MessageBus.Default.Send(MsgArgs.From(PopUpgradeMessage.Growl, [LangProvider.Instance.UpgradeSelectZoneMessage, MessageType.Warning]));
                return;
            }
            var ready = Items.Where(i => !string.IsNullOrWhiteSpace(i.File)).ToArray();
            if (ready.Length == 0)
            {
                MessageBus.Default.Send(MsgArgs.From(PopUpgradeMessage.Growl, [LangProvider.Instance.UpgradeSelectFirmwareMessage, MessageType.Warning]));
                return;
            }

            byte[] payload = Build(ready);
            foreach (var z in zones)
            {
                z.SendFirmware(payload, ready.Length);
                z.UpgradeSelected = false;
            }
        }
        private void OnZonePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Zone.UpgradeSelected))
            {
                OnPropertyChanged(nameof(IsAllSelected));
            }
        }
        public static byte[] Build(IEnumerable<UpgradeItem> items)
        {
            var ready = items.Where(i => !string.IsNullOrWhiteSpace(i.File)).ToArray();
            using var ms = new MemoryStream();
            ms.Write(DateTime.Now.Timestamp().ToBytes(Endianness.Big), 0, 8);
            ms.WriteByte((byte)ready.Length);
            foreach (var item in ready)
            {
                var firmware = File.ReadAllBytes(item.File);
                ms.WriteByte(item.TypeCode);
                ms.Write(firmware.Length.ToBytes(Endianness.Big), 0, 4);
                ms.Write(firmware, 0, firmware.Length);
            }
            return ms.ToArray();
        }
        public void Dispose()
        {
            foreach (var z in Zones)
            {
                z.PropertyChanged -= OnZonePropertyChanged;
            }
            Zones = null;
        }
    }
}