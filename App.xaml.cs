using System.Windows;
using AppComposer.ViewModels;
using AppComposer.Views;


namespace AppComposer
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindowViewModel mainWindowVM = new();

            MainWindow mainWindow = new()
            {
                DataContext = mainWindowVM
            };

            mainWindow.Show();
        }
    }

}
