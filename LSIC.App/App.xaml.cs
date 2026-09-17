namespace LSIC.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            ////注册语言模块
            //GlobalLanguageManager.RegisterLanguageResources(UiLanguage.Chinese, [
            //new ResourceDictionary(){ Source=new Uri("pack://application:,,,/LSIC.App;component/Resources/Language/Chinese/Message.xaml")},
            //new ResourceDictionary(){ Source=new Uri("pack://application:,,,/LSIC.App;component/Resources/Language/Chinese/UI.xaml")}
            //]);

            //GlobalLanguageManager.RegisterLanguageResources(UiLanguage.English, [
            //new ResourceDictionary(){ Source=new Uri("pack://application:,,,/LSIC.App;component/Resources/Language/English/Message.xaml")},
            //new ResourceDictionary(){ Source=new Uri("pack://application:,,,/LSIC.App;component/Resources/Language/English/UI.xaml")}
            //]);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            new WinStartupView().Show();
        }
    }

}
