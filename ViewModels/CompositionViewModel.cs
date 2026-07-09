using AppComposer.Commands;
using AppComposer.Models;
using AppComposer.Services;
using AppComposer.ViewModels.Composition;
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
        public ICommand RemoveLayerCommand { get; }
        public ICommand ScaleLayerCommand { get; }
        public ICommand RotateLayerCommand { get; }
        public ICommand ResetLayerCommand { get; }

        public CompositionPageViewModel? CurrentPageVM { get; private set; }
        public CanvasSettings CurrentCanvasSettings { get; private set; }

        public CompositionViewModel(MainWindowViewModel mainWindowViewModel)
        {
            m_mainWindowViewModel = mainWindowViewModel;

            CurrentCanvasSettings = new CanvasSettings();
            CurrentCanvasSettings.Width = ConfigurationService.Instance.CanvasWidth;
            CurrentCanvasSettings.Height = ConfigurationService.Instance.CanvasHeight;
            CurrentCanvasSettings.GridSize = ConfigurationService.Instance.GridSize;
            CurrentCanvasSettings.BackgroundColor = ConfigurationService.Instance.BackgroundColor;
            CurrentCanvasSettings.Dpi = ConfigurationService.Instance.ScreenDpi;
            CurrentCanvasSettings.RulerBandSize = ConfigurationService.Instance.RulerBandSize;
            CurrentCanvasSettings.RulerFontSize = ConfigurationService.Instance.RulerFontSize;

            ShowHomeCommand = new RelayCommand(() => m_mainWindowViewModel.ShowHome(), () => true);
            NewCompositionCommand = new RelayCommand(NewComposition, () => true);
            SaveCompositionCommand = new RelayCommand(SaveComposition, () => true);
            LoadCompositionCommand = new RelayCommand(LoadComposition, () => true);
            AddImageCommand = new RelayCommand(AddImage, () => CurrentPageVM != null);
            AddTextCommand = new RelayCommand(AddText, () => CurrentPageVM != null);
            RemoveLayerCommand = new RelayCommand(RemoveLayer, () => CurrentPageVM != null);
            ScaleLayerCommand = new RelayCommand(ScaleLayer, () => CurrentPageVM != null);
            RotateLayerCommand = new RelayCommand(RotateLayer, () => CurrentPageVM != null);
            ResetLayerCommand = new RelayCommand(ResetLayer, () => CurrentPageVM != null);

            NewComposition();
        }

        private void NewComposition()
        {
            Debug.WriteLine("New composition");

            CurrentPageVM = new CompositionPageViewModel(CurrentCanvasSettings);

            OnPropertyChanged(nameof(CurrentPageVM));

            // TODO: Add logic to initialize a new composition page, e.g., clear existing layers, reset settings, etc.
        }

        private void SaveComposition()
        {
            Debug.WriteLine("Save composition");

            if (CurrentPageVM != null)
            {
                // TODO Implement saving logic here
                // For example, serialize CurrentPage to a file
            }
        }

        private void LoadComposition()
        {
            Debug.WriteLine("Load composition");
            // TODO Implement loading logic here
            // For example, deserialize a file to CurrentPage
            // After loading, call OnPropertyChanged(nameof(CurrentPage));
        }

        private void AddImage()
        {
            Debug.WriteLine("Add image");

            // Test only
            ImageLayer layer = new("");
            layer.Height = 50;
            layer.Width = 70;
            layer.PosX = 0;
            layer.PosY = 0;

            CurrentPageVM?.AddImageLayer(layer);
        }

        private void AddText()
        {
            Debug.WriteLine("Add text");

            // Test only
            TextLayer layer = new("Test","Cambria",32);
            layer.PosX = 0;
            layer.PosY = 0;
            CurrentPageVM?.AddTextLayer(layer);
        }

        private void RemoveLayer()
        {
            Debug.WriteLine("Remove layer");
            CurrentPageVM?.RemoveSelectedLayer();
        }

        private void ScaleLayer()
        {
            Debug.WriteLine("Scale Mode");
            CurrentPageVM?.SwitchToScaleMode();
        }

        private void RotateLayer()
        {
            Debug.WriteLine("Rotate layer");
            CurrentPageVM?.RotateSelectedLayer();
        }

        private void ResetLayer()
        {
            Debug.WriteLine("Reset layer");
            CurrentPageVM?.ResetSelectedLayer();
        }
    }
}