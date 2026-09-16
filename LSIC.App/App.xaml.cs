using LSIC.App.Views;
using System.Windows;

namespace LSIC.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            new WinStartupView().Show();
        }
    }

}
