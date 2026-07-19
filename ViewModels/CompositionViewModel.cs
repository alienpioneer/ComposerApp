using AppComposer.Commands;
using AppComposer.Helpers;
using AppComposer.Models;
using AppComposer.Models.Layers;
using AppComposer.Services;
using AppComposer.ViewModels.Composition;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
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
        public ICommand SaveCompositionCommand { get; }
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
            SaveCompositionCommand = new RelayCommand(SaveComposition, () => true);
            LoadCompositionCommand = new RelayCommand(LoadComposition, () => true);
            AddImageCommand = new RelayCommand(AddImageLayer, () => CompositionPageVM != null);
            AddTextCommand = new RelayCommand(AddTextLayer, () => CompositionPageVM != null);
            AddCompositionCommand = new RelayCommand(AddCompositionLayer, () => CompositionPageVM != null);
            RemoveLayerCommand = new RelayCommand(RemoveLayer, () => CompositionPageVM != null);
            ScaleLayerCommand = new RelayCommand(ScaleLayer, () => CompositionPageVM != null);
            RotateLayerCommand = new RelayCommand(RotateLayer, () => CompositionPageVM != null);
            ResetLayerCommand = new RelayCommand(ResetLayer, () => CompositionPageVM != null);
        }

        private bool ConfirmDiscardChanges()
        {
            if (CompositionPageVM.CompositionProject != null &&
                CompositionPageVM.CompositionProject.IsModified)
            {
                var result = MessageBox.Show(
                    $"All the changes you made will be lost.\n\nAre you sure?",
                    "Warning",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes)
                {
                    return false;
                }
            }

            return true;
        }

        private void NewComposition()
        {
            Debug.WriteLine("New composition");

            if (!ConfirmDiscardChanges())
            { 
                return;
            }

            CompositionPageVM.LoadProjectVM(new CompositionProject(CurrentCanvasSettings));
            OnPropertyChanged(nameof(CompositionPageVM));
        }

        private void LoadComposition()
        {
            Debug.WriteLine("Load composition");

            if (!ConfirmDiscardChanges())
            {
                return;
            }

            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Composition Project (*.comp)|*.comp",
                Title = "Load Project"
            };

            if (dialog.ShowDialog() == true)
            {
                string projectFile = dialog.FileName;
                string projectName = Path.GetFileNameWithoutExtension(dialog.FileName);

                //Debug.WriteLine($"Project folder {projectFolder}");

                using (var temp = new TemporaryProjectDirectory())
                {
                    Debug.WriteLine($"Extracting project file {projectFile} to {temp.Path}");

                    ZipFile.ExtractToDirectory(projectFile, temp.Path);

                    Debug.WriteLine($"Loading project from {temp.Path}");

                    CompositionProject? project = CompositionService.LoadCompositionProject(Path.Combine(temp.Path, projectName));

                    if (project != null)
                    {
                        CompositionPageVM.LoadProjectVM(project);
                    }

                    if (CompositionPageVM.CompositionProject != null)
                    {
                        CompositionPageVM.CompositionProject.IsModified = false;
                    }
                }
            }
        }

        private void SaveAsComposition()
        {
            Debug.WriteLine("SaveAs composition");

            var dialog = new SaveFileDialog
            {
                Title = "Save Project",
                Filter = "Composition Project (*.comp)|*.comp",
                DefaultExt = ".comp",
                AddExtension = true,
                OverwritePrompt = true
            };

            if (dialog.ShowDialog() == false)
            {
                Debug.WriteLine("Internal dialog window problem while saving composition");
                return;
            }

            using (var temp = new TemporaryProjectDirectory())
            {
                CompositionService.SaveCompositionProject(CompositionPageVM.CompositionProject, dialog.FileName, temp.Path);

                ZipFile.CreateFromDirectory(temp.Path, dialog.FileName, CompressionLevel.Optimal, false); 
            }

            if (CompositionPageVM.CompositionProject != null)
            {
                CompositionPageVM.CompositionProject.IsModified = false;
            }
        }

        private void SaveComposition()
        {
            Debug.WriteLine("Save composition");

            if (string.IsNullOrWhiteSpace(CompositionPageVM.CompositionProject?.ProjectFile))
            {
                SaveAsComposition();
                return;
            }

            using (var temp = new TemporaryProjectDirectory())
            {
                CompositionService.SaveCompositionProject(
                    CompositionPageVM.CompositionProject, 
                    CompositionPageVM.CompositionProject.ProjectFile,
                    temp.Path);

                // Replace existing archive
                File.Delete(CompositionPageVM.CompositionProject.ProjectFile);

                ZipFile.CreateFromDirectory(temp.Path, CompositionPageVM.CompositionProject.ProjectFile, CompressionLevel.Optimal, false);
            }

            if (CompositionPageVM.CompositionProject != null)
            {
                CompositionPageVM.CompositionProject.IsModified = false;
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