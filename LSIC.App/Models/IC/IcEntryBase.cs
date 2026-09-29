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
        public abstract void Load();
    }
    public class IcEntry : IcEntryBase
    {
        public Ic Ic { get; private set; }
        public override void Load()
        {
            if (Ic == null)
            {
                Ic = IcConfigManager.GetIc(Name);
            }
        }
    }
    public class EditableIcEntry : IcEntryBase, IDisposable
    {
        public DirtyObserver<Ic> Ic { get; private set; }
        public bool IsDirty => Ic?.IsDirty ?? false;
        public Ic Target => Ic?.Target;
        public override void Load()
        {
            if (Ic == null)
            {
                Ic = new DirtyObserver<Ic>(IcConfigManager.GetIc(Name));
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
