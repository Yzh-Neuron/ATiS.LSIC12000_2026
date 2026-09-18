namespace LSIC.App.ViewModels
{
    public class PopAboutViewModel : ObservableObject
    {
        public PopAboutViewModel()
        {
            //Title = Device.Instance.SystemConfig.DeviceInfo.Title;

            Assembly assembly = Assembly.GetExecutingAssembly();
            Version = assembly.GetName().Version?.ToString();
            Company = assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company;
            Copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright;
        }

        public string Title { get; private set; }

        public string Version { get; private set; }

        public string Company { get; private set; }

        public string Copyright { get; private set; }

    }
}