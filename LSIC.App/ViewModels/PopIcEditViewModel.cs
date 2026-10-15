namespace LSIC.App.ViewModels
{
    public class PopIcEditViewModel : ObservableObject, IDisposable
    {
        //器件
        public ObservableCollection<EditableIcEntry> Ics { get; private set; }
        private EditableIcEntry _selectedIc;
        public EditableIcEntry SelectedIc
        {
            get => _selectedIc;
            set
            {
                if (SetProperty(ref _selectedIc, value))
                {
                    value?.Load(IcConfigManager.GetIc);
                }
            }
        }
        private DutSettingWrapper _dutSettingWrapper;
        public DutSettingWrapper DutSettingWrapper
        {
            get => _dutSettingWrapper;
            set => SetProperty(ref _dutSettingWrapper, value);
        }
        public ICommand CreateIcCommand { get; private set; }
        public ICommand SaveIcCommand { get; private set; }
        public ICommand SaveAsIcCommand { get; private set; }
        public ICommand DeleteIcCommand { get; private set; }
        public ICommand ImportIcCommand { get; private set; }
        public ICommand ExportIcCommand { get; private set; }
        public ICommand SelectBurnInBoardFileCommand { get; private set; }

        //DPS电源
        private DpsPlan _selectedDpsPlan;
        public DpsPlan SelectedDpsPlan
        {
            get => _selectedDpsPlan;
            set => SetProperty(ref _selectedDpsPlan, value);
        }
        public ICommand CreateDpsPlanCommand { get; private set; }
        public ICommand DeleteDpsPlanCommand { get; private set; }

        private DpsSetting _selectedDpsSetting;
        public DpsSetting SelectedDpsSetting
        {
            get => _selectedDpsSetting;
            set => SetProperty(ref _selectedDpsSetting, value);
        }
        public ICommand CreateDpsSettingCommand { get; private set; }
        public ICommand DeleteDpsSettingCommand { get; private set; }
        public ICommand EditDpsSettingCommand { get; private set; }

        //流程
        private StepItem _selectedStepItem;
        public StepItem SelectedStepItem
        {
            get => _selectedStepItem;
            set => SetProperty(ref _selectedStepItem, value);
        }
        public ICommand CreateStepItemCommand { get; private set; }
        public ICommand DeleteStepItemCommand { get; private set; }
        public ICommand MakeLoopCommand { get; private set; }
        public ICommand CanDragStepCommand { get; private set; }
        public ICommand CanDropStepCommand { get; private set; }
        public ICommand DropStepCommand { get; private set; }
        public ICommand RemoveLoopCommand { get; private set; }

        private Vector _selectedVector;
        public Vector SelectedVector
        {
            get => _selectedVector;
            set => SetProperty(ref _selectedVector, value);
        }
        public ICommand CreateVectorCommand { get; private set; }
        public ICommand DeleteVectorCommand { get; private set; }
        public ICommand EditVectorCommand { get; private set; }
        public ICommand EditRunStrategyCommand { get; private set; }
        public ICommand EditLoopStrategyCommand { get; private set; }
        public PopIcEditViewModel()
        {
            Ics = new(IcConfigManager.GetIcMetas<EditableIcEntry>());
            SelectedIc = Ics.FirstOrDefault();

            SelectBurnInBoardFileCommand = new RelayCommand(SelectBurnInBoardFile);

            //Ic
            CreateIcCommand = new RelayCommand(CreateIc);
            SaveIcCommand = new RelayCommand(SaveIc, CanSaveIc);
            DeleteIcCommand = new RelayCommand(DeleteIc, CanDeleteIc);
            SaveAsIcCommand = new RelayCommand(SaveAsIc, CanSaveAsIc);
            ImportIcCommand = new RelayCommand(ImportIc);
            ExportIcCommand = new RelayCommand(ExportIc, CanExportIc);


            //DPS电源
            CreateDpsPlanCommand = new RelayCommand(CreateDpsPlan, CanCreateDpsPlan);
            DeleteDpsPlanCommand = new RelayCommand(DeleteDpsPlan, CanDeleteDpsPlan);
            CreateDpsSettingCommand = new RelayCommand(CreateDpsSetting, CanCreateDpsSetting);
            DeleteDpsSettingCommand = new RelayCommand(DeleteDpsSetting, CanDeleteDpsSetting);
            EditDpsSettingCommand = new RelayCommand<DpsSetting>(EditDpsSetting);

            //流程
            CreateStepItemCommand = new RelayCommand(CreateStepItem, CanCreateStepItem);
            DeleteStepItemCommand = new RelayCommand<IList>(DeleteStepItem, CanDeleteStepItem);
            MakeLoopCommand = new RelayCommand<IList>(MakeLoop, CanMakeLoop);
            CanDragStepCommand = new RelayCommand<StepItem>(null, CanDragStep);
            CanDropStepCommand = new RelayCommand<DragDropArgs>(null, CanDropStep);
            DropStepCommand = new RelayCommand<DragDropArgs>(DropStep);
            RemoveLoopCommand = new RelayCommand<Loop>(RemoveLoop, CanRemoveLoop);
            CreateVectorCommand = new RelayCommand(CreateVector, CanCreateVector);
            DeleteVectorCommand = new RelayCommand(DeleteVector, CanDeleteVector);
            EditVectorCommand = new RelayCommand<Vector>(EditVector);

            //编辑策略
            EditRunStrategyCommand = new RelayCommand<ExecuteStrategy>(EditRunStrategy);
            EditLoopStrategyCommand = new RelayCommand<ExecuteStrategy>(EditLoopStrategy);
        }

        #region IC相关命令
        private void CreateIc()
        {
            var msgResult = MessageBus.Default.Send(MsgArgs.From(PopIcEditMessage.InputName, [LangProvider.Instance.CreateNewText, Ics.Select(i => i.Name)])).First();

            if (msgResult.Result is string name)
            {
                var ic = new EditableIcEntry { Name = name };
                ic.Attach(new Ic(name));
                ic.MarkDirty();
                Ics.Add(ic);
                SelectedIc = ic;
            }
        }
        private bool CanDeleteIc()
        {
            return SelectedIc?.Target != null;
        }
        private void DeleteIc()
        {
            var msgResult = MessageBus.Default.Send(MsgArgs.From(PopIcEditMessage.Message, [LangProvider.Instance.DeleteAskMessage, MessageType.Question])).First();

            if (msgResult.Result is not true)
            {
                return;
            }

            var ic = SelectedIc;
            if (Ics.Remove(ic))
            {
                IcConfigManager.DeleteIc(ic.Target);
            }
            ic.Dispose();
            SelectedIc = Ics.LastOrDefault();
        }
        private bool CanSaveIc()
        {
            return SelectedIc?.Ic?.IsDirty == true;
        }
        private void SaveIc()
        {
            Ic ic = SelectedIc.Target;
            List<ValidationResult> errors = ic.GetAllExistingErrorsRecursive();
            if (errors.Count > 0)
            {
                MessageBus.Default.Send(MsgArgs.From(PopIcEditMessage.Growl, [errors.ToMessageString(), MessageType.Error]));
                return;
            }

            if (ic.Process.StepItems.Count == 0)
            {
                MessageBus.Default.Send(MsgArgs.From(PopIcEditMessage.Growl, [LangProvider.Instance.AtleastOneStepMessage, MessageType.Error]));
                return;
            }

            IcConfigManager.SaveIc(ic);
            SelectedIc.ResetDirty();
        }
        private bool CanSaveAsIc()
        {
            return SelectedIc?.Ic?.IsDirty == false;
        }
        private void SaveAsIc()
        {
            var msgResult = MessageBus.Default.Send(MsgArgs.From(PopIcEditMessage.InputName, [LangProvider.Instance.SaveAsText, Ics.Select(i => i.Name)])).First();

            if (msgResult.Result is string name)
            {
                Ic newIc = IcConfigManager.SaveAsIc(SelectedIc.Target, name);
                var ic = new EditableIcEntry { Name = name };
                ic.Attach(newIc);
                Ics.Add(ic);
                SelectedIc = ic;
            }
        }
        private void ImportIc()
        {
            string filter = $"{LangProvider.Instance.IcFileText}|*.{IcConfigManager.IcSuffix}";
            var msgResult = MessageBus.Default.Send(MsgArgs.From(PopIcEditMessage.OpenFile, [filter, null])).First();

            if (msgResult.Result is string path)
            {
                Ic importIc;
                try
                {
                    importIc = IcParser.Parse(path);
                }
                catch (CharReaderException ex)
                {
                    string error = string.Format(LangProvider.Instance.FileParseErrorMessage, path, ex.Line, ex.Column);
                    MessageBus.Default.Send(MsgArgs.From(PopIcEditMessage.Growl, [LangProvider.Instance.FileImportFailureMessage, MessageType.Error]));
                    return;
                }
                catch (Exception ex)
                {
                    MessageBus.Default.Send(MsgArgs.From(PopIcEditMessage.Growl, [LangProvider.Instance.FileImportFailureMessage + ":\r\n" + ex.Message, MessageType.Error]));
                    return;
                }

                if (Ics.Any(i => i.Name == importIc.Name))
                {
                    msgResult = MessageBus.Default.Send(MsgArgs.From(PopIcEditMessage.Message, [LangProvider.Instance.ImportIcFileExistMessage, MessageType.Question])).First();

                    if (msgResult.Result is true)
                    {
                        msgResult = MessageBus.Default.Send(MsgArgs.From(PopIcEditMessage.InputName, [LangProvider.Instance.RenameText, Ics.Select(i => i.Name)])).First();

                        if (msgResult.Result is string name)
                        {
                            importIc.Name = name;
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }

                var ic = new EditableIcEntry { Name = importIc.Name };
                ic.Attach(importIc);
                ic.MarkDirty();
                Ics.Add(ic);
                SelectedIc = ic;

                MessageBus.Default.Send(MsgArgs.From(PopIcEditMessage.Growl, [LangProvider.Instance.FileImportSuccessMessage, MessageType.Success]));
            }
        }
        private bool CanExportIc()
        {
            return SelectedIc?.Ic?.IsDirty == false;
        }
        private void ExportIc()
        {
            string fileName = IcConfigManager.GetIcFileName(SelectedIc.Target);
            string filter = $"{LangProvider.Instance.IcFileText}|*.{IcConfigManager.IcSuffix}";
            var msgResult = MessageBus.Default.Send(MsgArgs.From(PopIcEditMessage.Export, [filter, fileName])).First();

            if (msgResult.Result is string path)
            {
                try
                {
                    IcConfigManager.ExportIc(SelectedIc.Target, path);
                    MessageBus.Default.Send(MsgArgs.From(PopIcEditMessage.Growl, [LangProvider.Instance.FileExportSuccessMessage, MessageType.Success]));

                }
                catch (Exception ex)
                {
                    MessageBus.Default.Send(MsgArgs.From(PopIcEditMessage.Growl, [LangProvider.Instance.FileExportFailureMessage + ":\r\n" + ex.Message, MessageType.Error]));
                }
            }
        }
        private void SelectBurnInBoardFile()
        {
            string filter = $"{LangProvider.Instance.DutConfigFileText}|*.elm";
            string currentFile = SelectedIc.Target.BurnInBoardFile;

            var msgResult = MessageBus.Default.Send(MsgArgs.From(PopIcEditMessage.OpenFile, [filter, currentFile])).First();

            if (msgResult.Result is string filePath)
            {
                SelectedIc.Target.BurnInBoardFile = filePath;
            }
        }
        #endregion

        #region DPS相关命令
        private bool CanCreateDpsPlan()
        {
            return SelectedIc?.Target != null;
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
            return SelectedIc?.Target != null && SelectedDpsPlan != null;
        }
        private void DeleteDpsPlan()
        {
            var dpsPlan = SelectedDpsPlan;
            SelectedIc.Target.DpsPlans.Remove(dpsPlan);
            SelectedDpsPlan = SelectedIc.Target.DpsPlans.LastOrDefault();

            SelectedIc.Target.Process.StepItems.ForEach(stepItem =>
            {
                if (stepItem.DpsPlan == dpsPlan.Name)
                {
                    stepItem.DpsPlan = string.Empty;
                }
            });
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
            return SelectedDpsPlan != null && SelectedDpsSetting != null;
        }
        private void DeleteDpsSetting()
        {
            var dpsSetting = SelectedDpsSetting;
            SelectedDpsPlan.DpssSettings.Remove(dpsSetting);
            SelectedDpsSetting = SelectedDpsPlan.DpssSettings.LastOrDefault();
        }
        private void EditDpsSetting(DpsSetting dpsSetting)
        {
            if (dpsSetting != null)
            {
                var clone = (DpsSetting)dpsSetting.Clone();

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

        #region 流程相关命令
        private bool CanCreateStepItem()
        {
            return SelectedIc?.Target != null;
        }
        private void CreateStepItem()
        {
            var msgResult = MessageBus.Default.Send(MsgArgs.From(PopIcEditMessage.InputName, [LangProvider.Instance.CreateNewText, SelectedIc.Target.Process.StepItems.Select(i => i.Name)])).First();

            if (msgResult.Result is string name)
            {
                StepVector stepVector = new StepVector(name);
                SelectedIc.Target.Process.AddVector(stepVector);
                SelectedStepItem = stepVector;
            }
        }
        private bool CanDeleteStepItem(IList obj)
        {
            var stepItems = obj?.Cast<StepItem>();
            if (SelectedIc?.Target != null
                && stepItems?.Count() > 0
                && stepItems.All(i => !i.IsFixed))
            {
                return true;
            }
            return false;
        }
        private void DeleteStepItem(IList obj)
        {
            var stepItems = obj.Cast<StepItem>().ToList();
            SelectedIc.Target.Process.RemoveVector(stepItems);
        }
        private bool CanMakeLoop(IList obj)
        {
            var stepItems = obj?.Cast<StepItem>();
            if (SelectedIc?.Target != null
                && stepItems?.Count() > 1
                && stepItems.All(i => !i.IsFixed))
            {
                return true;
            }
            return false;
        }
        private void MakeLoop(IList obj)
        {
            try
            {
                var stepItems = obj.Cast<StepItem>()?.ToList();
                SelectedIc.Target.Process.MakeLoop(stepItems);
            }
            catch (Exception ex)
            {
                MessageBus.Default.Send(MsgArgs.From(PopIcEditMessage.Growl, [ex.Message, MessageType.Error]));
            }
        }
        private bool CanDragStep(StepItem item)
        {
            return item is { IsFixed: false };
        }
        private bool CanDropStep(DragDropArgs args)
        {
            return args != null && SelectedIc?.Target?.Process?.CanMove(args.FromIndex, args.ToIndex) == true;
        }
        private void DropStep(DragDropArgs args)
        {
            SelectedIc.Target.Process.Move(args.FromIndex, args.ToIndex);
        }
        private bool CanRemoveLoop(Loop obj)
        {
            return SelectedIc?.Target != null;
        }
        private void RemoveLoop(Loop loop)
        {
            SelectedIc.Target.Process.RemoveLoop(loop);
        }

        private bool CanDeleteVector()
        {
            return SelectedVector != null && SelectedStepItem != null;
        }
        private void DeleteVector()
        {
            var vector = SelectedVector;
            SelectedStepItem.Vectors.Remove(SelectedVector);
            SelectedVector = null;
        }
        private bool CanCreateVector()
        {
            if (SelectedStepItem == null)
            {
                return false;
            }

            if (SelectedStepItem.Vectors.Count > 0 && SelectedStepItem is StepPower)
            {
                return false;
            }

            return true;
        }
        private void CreateVector()
        {
            var vector = new Vector();
            var msgResult = MessageBus.Default.Send(MsgArgs.From(PopIcEditMessage.CreateVector, [vector])).First();
            if (msgResult.Result is Vector vec)
            {
                SelectedStepItem.Vectors.Add(vec);
                SelectedVector = vec;
            }
        }
        private void EditVector(Vector vector)
        {
            if (vector != null)
            {
                var msgResult = MessageBus.Default.Send(MsgArgs.From(PopIcEditMessage.EditVector, [(Vector)vector.Clone()])).First();
                if (msgResult.Result is Vector clone)
                {
                    vector.Copy(clone);
                }
            }
        }
        private void EditRunStrategy(ExecuteStrategy strategy)
        {
            var msgResult = MessageBus.Default.Send(MsgArgs.From(PopIcEditMessage.EditExecuteStrategy, [(ExecuteStrategy)strategy.Clone(), false])).First();

            if (msgResult.Result is ExecuteStrategy clone)
            {
                strategy.Copy(clone);
            }
        }
        private void EditLoopStrategy(ExecuteStrategy strategy)
        {
            var msgResult = MessageBus.Default.Send(MsgArgs.From(PopIcEditMessage.EditExecuteStrategy, [(ExecuteStrategy)strategy.Clone(), true])).First();

            if (msgResult.Result is ExecuteStrategy clone)
            {
                strategy.Copy(clone);
            }
        }

        #endregion

        public void Dispose()
        {
            foreach (var item in Ics)
            {
                item.Dispose();
            }

            Ics.Clear();
        }
    }
}