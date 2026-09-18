namespace LSIC.App.ViewModels
{
    public class PopIcEditViewModel : ObservableObject
    {
        private ObservableCollection<Ic> _ics;
        public ObservableCollection<Ic> Ics
        {
            get => _ics;
            set => SetProperty(ref _ics, value);
        }
        private Ic _selectedIc;
        public Ic SelectedIc
        {
            get => _selectedIc;
            set => SetProperty(ref _selectedIc, value);
        }
        public ICommand CreateNewCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand SaveAsCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand ImportCommand { get; private set; }
        public ICommand ExportCommand { get; private set; }
        public PopIcEditViewModel()
        {
            Ics = new ObservableCollection<Ic>();
            CreateNewCommand = new RelayCommand(CreateNew);
            SaveCommand = new RelayCommand(Save, CanSave);
            //SaveAsCommand = new RelayCommand(SaveAs, CanSaveAs);
            DeleteCommand = new RelayCommand(Delete, CanDelete);
            //ImportCommand = new RelayCommand(Import, CanImport);
            //ExportCommand = new RelayCommand(Export, CanExport);
        }


        private void CreateNew()
        {
            var msgResult = MessageBus.Default.Send(MsgArgs.From(PopIcEditMessage.InputName, [LangProvider.Instance.CreateNewText, Ics.Select(i => i.Name)])).First();

            if (msgResult.Result is string name)
            {
                Ic ic = new Ic(name);
                Ics.Add(ic);
                SelectedIc = ic;
            }
        }
        private void Save()
        {

        }
        private bool CanSave()
        {
            return true;
        }



        private bool CanDelete()
        {
            return SelectedIc != null ;
        }

        private void Delete()
        {
            Ic ic = SelectedIc;

            var msgResult = MessageBus.Default.Send(MsgArgs.From(PopIcEditMessage.Message, [LangProvider.Instance.DeleteAskMessage, MessageType.Question])).First();

            if (msgResult.Result is not true)
            {
                return;
            }

            Ics.Remove(ic);
        }
    }
}