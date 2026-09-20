using System.Collections.Specialized;

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
        public IRelayCommand CreateNewCommand { get; private set; }
        public IRelayCommand SaveCommand { get; private set; }
        public IRelayCommand SaveAsCommand { get; private set; }
        public IRelayCommand DeleteCommand { get; private set; }
        public IRelayCommand ImportCommand { get; private set; }
        public IRelayCommand ExportCommand { get; private set; }
        public PopIcEditViewModel()
        {
            Ics.CollectionChanged += OnIcsCollectionChanged;
            CreateNewCommand = new RelayCommand(CreateNew);
            SaveCommand = new RelayCommand(Save, CanSave);
            //SaveAsCommand = new RelayCommand(SaveAs, CanSaveAs);
            DeleteCommand = new RelayCommand(Delete, CanDelete);
            //ImportCommand = new RelayCommand(Import, CanImport);
            //ExportCommand = new RelayCommand(Export, CanExport);
        }
        private void CreateNew()
        {
            var msgResult = MessageBus.Default.Send(MsgArgs.From(PopIcEditMessage.InputName, [LangProvider.Instance.CreateNewText, Ics.Select(i => i.Target.Name)])).First();

            if (msgResult.Result is string name)
            {
                var ic = new DirtyObserver<Ic>(new Ic(name));
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
        private void Delete()
        {
            var ic = SelectedIc;

            var msgResult = MessageBus.Default.Send(MsgArgs.From(PopIcEditMessage.Message, [LangProvider.Instance.DeleteAskMessage, MessageType.Question])).First();

            if (msgResult.Result is not true)
            {
                return;
            }
            SelectedIc = null;
            Ics.Remove(ic);
        }
        private bool CanDelete()
        {
            return SelectedIc != null;
        }
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
        }
        private void OnIsDirtyChanged(object sender, EventArgs e)
        {
            NotifyCanExecuteChanged();
        }
        private void NotifyCanExecuteChanged()
        {
            CreateNewCommand?.NotifyCanExecuteChanged();
            SaveCommand?.NotifyCanExecuteChanged();
            SaveAsCommand?.NotifyCanExecuteChanged();
            DeleteCommand?.NotifyCanExecuteChanged();
            ImportCommand?.NotifyCanExecuteChanged();
            ExportCommand?.NotifyCanExecuteChanged();
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