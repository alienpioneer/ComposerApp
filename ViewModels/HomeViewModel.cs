using AppComposer.Commands;
using System.Windows.Input;

namespace AppComposer.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel m_mainWindowViewModel;

        public ICommand ShowComposerCommand { get; }

        public HomeViewModel(MainWindowViewModel mainWindowViewModel)
        {
            m_mainWindowViewModel = mainWindowViewModel;

            ShowComposerCommand = new RelayCommand(() => m_mainWindowViewModel.ShowComposer(), ()=>true);
        }
    }
}
