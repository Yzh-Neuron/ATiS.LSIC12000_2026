using System.Collections;

namespace LSIC.App.ViewModels
{
    public class PopIcEditViewModel : ObservableObject, IDisposable
    {
        public ObservableCollection<DirtyObserver<Ic>> Ics { get; } = new();
        private DirtyObserver<Ic> _selectedIc;
        public DirtyObserver<Ic> SelectedIc
        {
            get => _selectedIc;
            set => SetProperty(ref _selectedIc, value);
        }

        //器件
        public ICommand CreateIcCommand { get; private set; }
        public ICommand SaveIcCommand { get; private set; }
        public ICommand SaveAsIcCommand { get; private set; }
        public ICommand DeleteIcCommand { get; private set; }
        public ICommand ImportIcCommand { get; private set; }
        public ICommand ExportIcCommand { get; private set; }
        public ICommand SelectBurnInBoardFileCommand { get; private set; }

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
        public ICommand RemoveLoopCommand { get; private set; }
        public ICommand CanDragStepCommand { get; private set; }
        public ICommand CanDropStepCommand { get; private set; }
        public ICommand DropStepCommand { get; private set; }
        public PopIcEditViewModel()
        {
            SelectBurnInBoardFileCommand = new RelayCommand(SelectBurnInBoardFile);

            //Ic
            CreateIcCommand = new RelayCommand(CreateIc);
            SaveIcCommand = new RelayCommand(SaveIc, CanSaveIc);
            DeleteIcCommand = new RelayCommand(DeleteIc, CanDeleteIc);

            //Dps电源
            CreateDpsPlanCommand = new RelayCommand(CreateDpsPlan, CanCreateDpsPlan);
            DeleteDpsPlanCommand = new RelayCommand(DeleteDpsPlan, CanDeleteDpsPlan);
            CreateDpsSettingCommand = new RelayCommand(CreateDpsSetting, CanCreateDpsSetting);
            DeleteDpsSettingCommand = new RelayCommand(DeleteDpsSetting, CanDeleteDpsSetting);
            EditDpsSettingCommand = new RelayCommand<DpsSetting>(EditDpsSetting);

            //流程
            CreateStepItemCommand = new RelayCommand(CreateStepItem, CanCreateStepItem);
            DeleteStepItemCommand = new RelayCommand<IList>(DeleteStepItem, CanDeleteStepItem);
            MakeLoopCommand = new RelayCommand<IList>(MakeLoop, CanMakeLoop);
            RemoveLoopCommand = new RelayCommand<IList>(RemoveLoop, CanRemoveLoop);
            CanDragStepCommand = new RelayCommand<StepItem>(null, CanDragStep);
            CanDropStepCommand = new RelayCommand<DragDropArgs>(null, CanDropStep);
            DropStepCommand = new RelayCommand<DragDropArgs>(DropStep);

#if DEBUG
            DirtyObserver<Ic> ic = new(new Ic("test ic"));
            Ics.Add(ic);
            SelectedIc = ic;

            DpsPlan dpsPlan = new DpsPlan("test dps");
            SelectedIc.Target.DpsPlans.Add(dpsPlan);
            SelectedDpsPlan = dpsPlan;
#endif
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

        #region IC相关命令
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

        #endregion

        #region Dps相关命令
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

        #region 流程相关命令
        private bool CanCreateStepItem()
        {
            return SelectedIc != null;
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
            if (SelectedIc != null && stepItems?.Count() > 0 && stepItems.All(i => !i.IsFixed))
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
        private bool CanRemoveLoop(IList obj)
        {
            var stepItems = obj.Cast<StepItem>();
            if (SelectedIc != null && stepItems?.Count() > 0 && stepItems.All(i => !i.IsFixed))
            {
                return true;
            }

            return false;
        }
        private void RemoveLoop(IList obj)
        {

        }
        private bool CanMakeLoop(IList obj)
        {
            return true;
        }
        private void MakeLoop(IList obj)
        {
            var stepItems = obj.Cast<StepItem>()?.ToList();
            SelectedIc.Target.Process.MakeLoop(stepItems);
        }
        private bool CanDragStep(StepItem item)
        {
            return SelectedIc != null && item is { IsFixed: false };
        }
        private bool CanDropStep(DragDropArgs args)
        {
            return SelectedIc != null
                && args is not null
                && SelectedIc.Target.Process.CanMove(args.FromIndex, args.ToIndex);
        }
        private void DropStep(DragDropArgs args)
        {
            if (SelectedIc == null || args is null)
            {
                return;
            }

            if (!SelectedIc.Target.Process.CanMove(args.FromIndex, args.ToIndex))
            {
                return;
            }

            SelectedIc.Target.Process.Move(args.FromIndex, args.ToIndex);
            SelectedIc.MarkDirty();
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