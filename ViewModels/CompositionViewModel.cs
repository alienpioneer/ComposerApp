using AppComposer.Commands;
using AppComposer.Models;
using AppComposer.Services;
using System.Windows.Input;
using System.Diagnostics;

namespace AppComposer.ViewModels
{
    public class CompositionViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel m_mainWindowViewModel;

        public ICommand ShowHomeCommand { get; }
        public ICommand NewCompositionCommand { get; }
        public ICommand SaveCompositionCommand { get; }
        public ICommand LoadCompositionCommand { get; }
        public ICommand AddImageCommand { get; }
        public ICommand AddTextCommand { get; }

        public CompositionPage? CurrentPage { get; private set; }
        public CanvasSettings CurrentCanvasSettings { get; private set; }

        public CompositionViewModel(MainWindowViewModel mainWindowViewModel)
        {
            m_mainWindowViewModel = mainWindowViewModel;

            CurrentCanvasSettings = new CanvasSettings();
            CurrentCanvasSettings.Width = ConfigurationService.Instance.CanvasWidth;
            CurrentCanvasSettings.Height = ConfigurationService.Instance.CanvasHeight;
            // TODO Get canvas settings from user preferences or default settings

            ShowHomeCommand = new RelayCommand(() => m_mainWindowViewModel.ShowHome(), () => true);
            NewCompositionCommand = new RelayCommand(CreateNewComposition, () => true);
            SaveCompositionCommand = new RelayCommand(SaveComposition, () => true);
            LoadCompositionCommand = new RelayCommand(LoadComposition, () => true);
            AddImageCommand = new RelayCommand(AddImage, () => CurrentPage != null);
            AddTextCommand = new RelayCommand(AddText, () => CurrentPage != null);
        }

        private void CreateNewComposition()
        {
            Debug.WriteLine("Create new composition");
            CurrentPage = new CompositionPage(CurrentCanvasSettings);
            OnPropertyChanged(nameof(CurrentPage));
        }

        private void SaveComposition()
        {
            Debug.WriteLine("Save composition");

            if (CurrentPage != null)
            {
                // Implement saving logic here
                // For example, serialize CurrentPage to a file
            }
        }

        private void LoadComposition()
        {
            Debug.WriteLine("Load composition");
            // Implement loading logic here
            // For example, deserialize a file to CurrentPage
            // After loading, call OnPropertyChanged(nameof(CurrentPage));
        }

        private void AddImage()
        {
            Debug.WriteLine("Add image");
            // TODO
        }

        private void AddText()
        {
            Debug.WriteLine("Add text");
            // TODO
        }

    }

}
