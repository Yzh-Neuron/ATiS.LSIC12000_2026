namespace LSIC.App.Models
{
    public abstract class IcEntryBase : ObservableObject
    {
        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }

        public abstract void Load(Func<string, Ic> loader);
        public abstract void Attach(Ic ic);
    }
    public class IcEntry : IcEntryBase
    {
        private Ic _ic;
        public Ic Ic
        {
            get => _ic;
            set => SetProperty(ref _ic, value);
        }
        public override void Load(Func<string, Ic> loader)
        {
            Ic ??= loader(Name);
        }
        public override void Attach(Ic ic)
        {
            Ic ??= ic;
        }
    }
    public class EditableIcEntry : IcEntryBase, IDisposable
    {
        private DirtyObserver<Ic> _ic;
        public DirtyObserver<Ic> Ic
        {
            get => _ic;
            set
            {
                if (SetProperty(ref _ic, value))
                {
                    OnPropertyChanged(nameof(Target));
                }
            }
        }
        public Ic Target
        {
            get => _ic?.Target;
        }
        public override void Load(Func<string, Ic> loader)
        {
            if (Ic == null)
            {
                Attach(loader(Name));
            }
        }
        public override void Attach(Ic ic)
        {
            if (Ic == null)
            {
                Ic = new DirtyObserver<Ic>(ic);
            }
        }
        public void MarkDirty()
        {
            Ic?.MarkDirty();
        }
        public void ResetDirty()
        {
            Ic?.ResetDirty();
        }
        public void Dispose()
        {
            Ic?.Dispose();
        }
    }
}
