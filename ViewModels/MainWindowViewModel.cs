using System.Diagnostics;

namespace AppComposer.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ViewModelBase m_currentViewModel;
        public ViewModelBase CurrentViewModel { 
            get => m_currentViewModel; 
            set { 
                m_currentViewModel = value; 
                OnPropertyChanged(); 
            } 
        }

        public HomeViewModel HomeViewModel { get; }
        public CompositionViewModel CompositionViewModel { get; }

        public MainWindowViewModel() 
        {
            HomeViewModel = new(this);
            CompositionViewModel = new(this);
            m_currentViewModel = HomeViewModel;
        }

        public void ShowComposer()
        {
            Debug.WriteLine("Show Composer");
            CurrentViewModel = CompositionViewModel;
        }

        public void ShowHome()
        {
            Debug.WriteLine("Show Home");
            CurrentViewModel = HomeViewModel;
        }

    }
}
