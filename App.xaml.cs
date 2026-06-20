using System.Windows;
using AppComposer.ViewModels;
using AppComposer.Views;
using AppComposer.Services;


namespace AppComposer
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ConfigurationService.Initialize("../../../config.ini");

            MainWindowViewModel mainWindowVM = new();

            MainWindow mainWindow = new()
            {
                DataContext = mainWindowVM
            };

            mainWindow.Show();
        }
    }

}
