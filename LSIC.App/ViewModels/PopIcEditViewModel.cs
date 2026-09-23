namespace LSIC.App.ViewModels
{
    public class PopIcEditViewModel : ObservableObject, IDisposable
    {
        public ObservableCollection<DirtyObserver<Ic>> Ics { get; } = new();
        private DirtyObserver<Ic> _selectedIc;
        public DirtyObserver<Ic> SelectedIc
        {
            get => _selectedIc;
            set
            {
                if (SetProperty(ref _selectedIc, value))
                {
                    CreateDpsPlanCommand?.NotifyCanExecuteChanged();
                    DeleteDpsPlanCommand?.NotifyCanExecuteChanged();
                }
            }
        }

        //器件
        public IRelayCommand CreateIcCommand { get; private set; }
        public IRelayCommand SaveIcCommand { get; private set; }
        public IRelayCommand SaveAsIcCommand { get; private set; }
        public IRelayCommand DeleteIcCommand { get; private set; }
        public IRelayCommand ImportIcCommand { get; private set; }
        public IRelayCommand ExportIcCommand { get; private set; }
        public IRelayCommand SelectBurnInBoardFileCommand { get; private set; }

        //工位
        private DutSettingWrapper _dutSettingWrapper;
        public DutSettingWrapper DutSettingWrapper
        {
            get => _dutSettingWrapper;
            set => SetProperty(ref _dutSettingWrapper, value);
        }

        //Dps电源
        private DpsPlan _selectedDpsPlan;
        public DpsPlan SelectedDpsPlan
        {
            get => _selectedDpsPlan;
            set
            {
                if (SetProperty(ref _selectedDpsPlan, value))
                {
                    CreateDpsPlanCommand?.NotifyCanExecuteChanged();
                    DeleteDpsPlanCommand?.NotifyCanExecuteChanged();
                    CreateDpsSettingCommand?.NotifyCanExecuteChanged();
                    DeleteDpsSettingCommand?.NotifyCanExecuteChanged();
                }
            }
        }
        public IRelayCommand CreateDpsPlanCommand { get; private set; }
        public IRelayCommand DeleteDpsPlanCommand { get; private set; }

        private DpsSetting _selectedDpsSetting;
        public DpsSetting SelectedDpsSetting
        {
            get => _selectedDpsSetting;
            set
            {
                if (SetProperty(ref _selectedDpsSetting, value))
                {
                    CreateDpsSettingCommand?.NotifyCanExecuteChanged();
                    DeleteDpsSettingCommand?.NotifyCanExecuteChanged();
                }
            }
        }
        public IRelayCommand CreateDpsSettingCommand { get; private set; }
        public IRelayCommand DeleteDpsSettingCommand { get; private set; }
        public IRelayCommand EditDpsSettingCommand { get; private set; }




        public PopIcEditViewModel()
        {
            Ics.CollectionChanged += OnIcsCollectionChanged;

            SelectBurnInBoardFileCommand = new RelayCommand(SelectBurnInBoardFile);
            CreateIcCommand = new RelayCommand(CreateIc);
            SaveIcCommand = new RelayCommand(SaveIc, CanSaveIc);
            DeleteIcCommand = new RelayCommand(DeleteIc, CanDeleteIc);

            //Dps电源
            CreateDpsPlanCommand = new RelayCommand(CreateDpsPlan, CanCreateDpsPlan);
            DeleteDpsPlanCommand = new RelayCommand(DeleteDpsPlan, CanDeleteDpsPlan);
            CreateDpsSettingCommand = new RelayCommand(CreateDpsSetting, CanCreateDpsSetting);
            DeleteDpsSettingCommand = new RelayCommand(DeleteDpsSetting, CanDeleteDpsSetting);
            EditDpsSettingCommand = new RelayCommand<DpsSetting>(EditDpsSetting);
#if DEBUG
            DirtyObserver<Ic> ic = new(new Ic("test ic"));
            Ics.Add(ic);
            SelectedIc = ic;

            DpsPlan dpsPlan = new DpsPlan("test dps");
            SelectedIc.Target.DpsPlans.Add(dpsPlan);
            SelectedDpsPlan = dpsPlan;
#endif
        }

        private void CreateIc()
        {
            var msgResult = MessageBus.Default.Send(MsgArgs.From(PopIcEditMessage.InputName, [LangProvider.Instance.CreateNewText, Ics.Select(i => i.Target.Name)])).First();

            if (msgResult.Result is string name)
            {
                var ic = new DirtyObserver<Ic>(new Ic(name));
                ic.MarkDirty();
                Ics.Add(ic);
                SelectedIc = ic;
            }
        }
        private bool CanDeleteIc()
        {
            return SelectedIc != null;
        }
        private void DeleteIc()
        {
            var msgResult = MessageBus.Default.Send(MsgArgs.From(PopIcEditMessage.Message, [LangProvider.Instance.DeleteAskMessage, MessageType.Question])).First();

            if (msgResult.Result is not true)
            {
                return;
            }

            var ic = SelectedIc;
            Ics.Remove(ic);
            SelectedIc = null;
        }
        private void SaveIc()
        {

        }
        private bool CanSaveIc()
        {
            return SelectedIc != null;
        }
        private void SelectBurnInBoardFile()
        {
            string filter = $"{LangProvider.Instance.DutConfigFileText}|*.elm";
            string currentFile = SelectedIc.Target.BurnInBoardFile;

            var msgResult = MessageBus.Default.Send(MsgArgs.From(PopIcEditMessage.SelectFile, [filter, currentFile])).First();

            if (msgResult.Result is string filePath)
            {
                SelectedIc.Target.BurnInBoardFile = filePath;
            }
        }

        #region Dps电源
        private bool CanCreateDpsPlan()
        {
            return SelectedIc != null;
        }
        private void CreateDpsPlan()
        {
            var msgResult = MessageBus.Default.Send(MsgArgs.From(PopIcEditMessage.InputName, [LangProvider.Instance.CreateNewText, SelectedIc.Target.DpsPlans.Select(i => i.Name)])).First();

            if (msgResult.Result is string name)
            {

                DpsPlan dpsPlan = new DpsPlan(name);
                SelectedIc.Target.DpsPlans.Add(dpsPlan);
                SelectedDpsPlan = dpsPlan;
            }
        }
        private bool CanDeleteDpsPlan()
        {
            return SelectedDpsPlan != null;
        }
        private void DeleteDpsPlan()
        {
            var dpsPlan = SelectedDpsPlan;
            SelectedIc.Target.DpsPlans.Remove(dpsPlan);
            SelectedDpsPlan = null;
        }


        private bool CanCreateDpsSetting()
        {
            return SelectedDpsPlan != null;
        }
        private void CreateDpsSetting()
        {
            IEnumerable<DpsInfo> optional = Device.Instance.SystemConfig.DpsInfos.Except(SelectedDpsPlan.DpssSettings.Select(i => i.DpsInfo));
            DpsInfo first = optional.FirstOrDefault();
            if (first != null)
            {
                var create = new DpsSetting(first);
                var msgResult = MessageBus.Default.Send(MsgArgs.From(PopIcEditMessage.CreateDpsSetting, [optional, create])).First();
                if (msgResult.Result is DpsSetting dps)
                {
                    SelectedDpsPlan.DpssSettings.Add(dps);
                    SelectedDpsSetting = dps;
                }
            }
        }
        private bool CanDeleteDpsSetting()
        {
            return SelectedDpsSetting != null;
        }
        private void DeleteDpsSetting()
        {


        }
        private void EditDpsSetting(DpsSetting dpsSetting)
        {
            if (dpsSetting != null)
            {
                var clone = dpsSetting.Clone();

                List<DpsInfo> optional = Device.Instance.SystemConfig.DpsInfos.Except(SelectedDpsPlan.DpssSettings.Select(i => i.DpsInfo)).ToList();
                optional.Add(clone.DpsInfo);

                var msgResult = MessageBus.Default.Send(MsgArgs.From(PopIcEditMessage.EditDpsSetting, [optional, clone])).First();
                if (msgResult.Result is DpsSetting dps)
                {
                    dpsSetting.Copy(dps);
                }
            }
        }


        #endregion

        private void OnIcsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<DirtyObserver<Ic>>())
                {
                    item.IsDirtyChanged += OnIsDirtyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems.Cast<DirtyObserver<Ic>>())
                {
                    item.IsDirtyChanged -= OnIsDirtyChanged;
                    item.Dispose();
                }
            }

            DeleteIcCommand?.NotifyCanExecuteChanged();
        }
        private void OnIsDirtyChanged(object sender, EventArgs e)
        {
            SaveIcCommand?.NotifyCanExecuteChanged();
            SaveAsIcCommand?.NotifyCanExecuteChanged();
            ImportIcCommand?.NotifyCanExecuteChanged();
            ExportIcCommand?.NotifyCanExecuteChanged();
        }
        public void Dispose()
        {
            Ics.CollectionChanged -= OnIcsCollectionChanged;

            foreach (var item in Ics)
            {
                item.IsDirtyChanged -= OnIsDirtyChanged;
                item.Dispose();
            }

            Ics.Clear();
        }
    }
}