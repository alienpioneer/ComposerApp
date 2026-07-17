using AppComposer.Commands;
using AppComposer.Models;
using AppComposer.Models.Layers;
using AppComposer.Services;
using AppComposer.ViewModels.Composition;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;

namespace AppComposer.ViewModels
{
    public class CompositionViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel m_mainWindowViewModel;

        public ICommand ShowHomeCommand { get; }
        public ICommand NewCompositionCommand { get; }
        public ICommand SaveAsCompositionCommand { get; }
        public ICommand LoadCompositionCommand { get; }
        public ICommand AddImageCommand { get; }
        public ICommand AddTextCommand { get; }
        public ICommand AddCompositionCommand { get; }
        public ICommand RemoveLayerCommand { get; }
        public ICommand ScaleLayerCommand { get; }
        public ICommand RotateLayerCommand { get; }
        public ICommand ResetLayerCommand { get; }

        public CompositionPageViewModel CompositionPageVM { get; private set; }
        public CanvasSettings CurrentCanvasSettings { get; private set; }
        public ImageService ImageService { get; }
        public CompositionService CompositionService { get; }

        public CompositionViewModel(MainWindowViewModel mainWindowViewModel)
        {
            m_mainWindowViewModel = mainWindowViewModel;

            CurrentCanvasSettings = new CanvasSettings();
            ImageService = new ImageService();
            CompositionService = new CompositionService(ImageService);

            CompositionPageVM = new CompositionPageViewModel(CurrentCanvasSettings);

            CurrentCanvasSettings.Width = ConfigurationService.Instance.CanvasWidth;
            CurrentCanvasSettings.Height = ConfigurationService.Instance.CanvasHeight;
            CurrentCanvasSettings.GridSize = ConfigurationService.Instance.GridSize;
            CurrentCanvasSettings.BackgroundColor = ConfigurationService.Instance.BackgroundColor;
            CurrentCanvasSettings.Dpi = ConfigurationService.Instance.ScreenDpi;
            CurrentCanvasSettings.RulerBandSize = ConfigurationService.Instance.RulerBandSize;
            CurrentCanvasSettings.RulerFontSize = ConfigurationService.Instance.RulerFontSize;

            ShowHomeCommand = new RelayCommand(() => m_mainWindowViewModel.ShowHome(), () => true);
            NewCompositionCommand = new RelayCommand(NewComposition, () => true);
            SaveAsCompositionCommand = new RelayCommand(SaveAsComposition, () => true);
            LoadCompositionCommand = new RelayCommand(LoadComposition, () => true);
            AddImageCommand = new RelayCommand(AddImageLayer, () => CompositionPageVM != null);
            AddTextCommand = new RelayCommand(AddTextLayer, () => CompositionPageVM != null);
            AddCompositionCommand = new RelayCommand(AddCompositionLayer, () => CompositionPageVM != null);
            RemoveLayerCommand = new RelayCommand(RemoveLayer, () => CompositionPageVM != null);
            ScaleLayerCommand = new RelayCommand(ScaleLayer, () => CompositionPageVM != null);
            RotateLayerCommand = new RelayCommand(RotateLayer, () => CompositionPageVM != null);
            ResetLayerCommand = new RelayCommand(ResetLayer, () => CompositionPageVM != null);
        }

        private void NewComposition()
        {
            Debug.WriteLine("New composition");

            CompositionPageVM.LoadProject(new CompositionProject(CurrentCanvasSettings));
            OnPropertyChanged(nameof(CompositionPageVM));

            // TODO: Add warning save/discard existing project
        }

        private void SaveAsComposition()
        {
            Debug.WriteLine("SaveAs composition");

            var dialog = new SaveFileDialog
            {
                Title = "Save Project"
            };

            if (dialog.ShowDialog() == false)
            {
                Debug.WriteLine("Internal dialog window problem while saving composition");
                return;
            }

            string parentDirectory = Path.GetDirectoryName(dialog.FileName)!;
            string projectName = Path.GetFileNameWithoutExtension(dialog.FileName);
            string projectDirectory = Path.Combine(parentDirectory, projectName);

            if (Directory.Exists(projectDirectory))
            {
                var result = MessageBox.Show(
                    $"The project '{projectName}' already exists.\n\nOverwrite it?",
                    "Overwrite",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes)
                {
                    return;
                }

                Directory.Delete(projectDirectory, true);
            }

            CompositionService.SaveCompositionProject(CompositionPageVM.CompositionProject, projectName, parentDirectory);
        }

        private void LoadComposition()
        {
            Debug.WriteLine("Load composition");

            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Project files (*.json)|*.json",
                Title = "Load Project"
            };

            if (dialog.ShowDialog() == true)
            {
                string projectFile = dialog.FileName;
                string projectFolder = Path.GetDirectoryName(projectFile)!;

                //Debug.WriteLine($"Project folder {projectFolder}");

                CompositionProject? project = CompositionService.LoadCompositionProject(projectFolder);

                if (project != null)
                {
                    CompositionPageVM.LoadProject(project);
                }
            }
        }

        private void AddImageLayer()
        {
            Debug.WriteLine("Add image layer");

            OpenFileDialog dlg = new();

            if (dlg.ShowDialog() == true)
            {
                ImageLayer layer = ImageService.LoadImageBitmapToLayer(dlg.FileName);
                CompositionPageVM.AddImageLayer(layer);
            }
        }

        private void AddTextLayer()
        {
            Debug.WriteLine("Add text layer");

            // TODO Text dialog Test only
            TextLayer layer = new("Test","Cambria",32);
            layer.PosX = 0;
            layer.PosY = 0;
            CompositionPageVM.AddTextLayer(layer);
        }

        private void AddCompositionLayer()
        {
            Debug.WriteLine("Add composition layer");

            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Project files (*.json)|*.json",
                Title = "Load Composition Layer"
            };

            if (dialog.ShowDialog() == true)
            {
                CompositionLayer? layer = CompositionService.LoadCompositionLayer(dialog.FileName);

                if (layer != null)
                {
                    Debug.WriteLine($"Loaded composition layer from {dialog.FileName}");
                    CompositionPageVM.AddCompositionLayer(layer);
                }
            }
        }

        private void RemoveLayer()
        {
            Debug.WriteLine("Remove layer");
            CompositionPageVM.RemoveSelectedLayer();
        }

        private void ScaleLayer()
        {
            Debug.WriteLine("Scale Mode");
            CompositionPageVM.SwitchToScaleMode();
        }

        private void RotateLayer()
        {
            Debug.WriteLine("Rotate layer");
            CompositionPageVM.RotateSelectedLayer();
        }

        private void ResetLayer()
        {
            Debug.WriteLine("Reset layer");
            CompositionPageVM.ResetSelectedLayer();
        }
    }
}