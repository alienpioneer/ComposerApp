using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using AppComposer.Models;
using AppComposer.Models.Layers;
using AppComposer.Views;

namespace AppComposer.ViewModels.Composition
{
    public class CompositionPageViewModel : ViewModelBase
    {
        bool m_isMouseDown = false;

        public CompositionProject? CompositionProject { get; set; }

        public ObservableCollection<LayerBaseViewModel> Layers { get; set; }

        public LayerBaseViewModel? SelectedLayer { get; set; } = null;

        public CompositionPageViewModel(CanvasSettings canvasSettings)        
        {
            CompositionProject = new CompositionProject(canvasSettings);
            Layers = new();
        }

        public void LoadProjectVM(CompositionProject project)
        {
            ClearCurrentView();
            CompositionProject = project;

            if (CompositionProject == null)
            {
                return;
            }

            foreach (LayerBase layer in CompositionProject.Layers)
            {
                if (layer is ImageLayer imageLayer)
                {
                    AddImageLayerViewModel(imageLayer);
                }
                else if (layer is TextLayer textLayer)
                {
                    AddTextLayerViewModel(textLayer);
                }
                else if (layer is CompositionLayer compositionLayer)
                {
                    AddCompositionLayerViewModel(compositionLayer);
                }
            }

            OnPropertyChanged();
        }

        public void AddImageLayer(ImageLayer imageLayer)
        {
            if (CompositionProject != null)
            {
                CompositionProject.Layers.Add(imageLayer);
                CompositionProject.IsModified = true;
            }
            
            AddImageLayerViewModel(imageLayer);
        }

        public void AddTextLayer(TextLayer textLayer)
        {
            if (CompositionProject != null)
            {
                CompositionProject.Layers.Add(textLayer);
                CompositionProject.IsModified = true;
            }
            
            AddTextLayerViewModel(textLayer);
        }

        public void AddCompositionLayer(CompositionLayer compositionLayer)
        {
            if (CompositionProject != null)
            {
                CompositionProject.Layers.Add(compositionLayer);
                CompositionProject.IsModified = true;
            }

            AddCompositionLayerViewModel(compositionLayer);
        }

        public void RemoveSelectedLayer()
        {
            if (SelectedLayer == null)
            {
                return;
            }

            if (CompositionProject != null)
            {
                CompositionProject.Layers.Remove(SelectedLayer.Layer);
                CompositionProject.IsModified = true;
            }

            Layers.Remove(SelectedLayer);
            SelectedLayer = null;
        }

        public void ResetSelectedLayer()
        {
            if (SelectedLayer == null)
            {
                return;
            }

            if (CompositionProject != null)
            {
                CompositionProject.IsModified = true;
            }

            SelectedLayer.ResetTransforms();
        }

#region Internals

        private void AddImageLayerViewModel(ImageLayer imageLayer)
        {
            Layers.Add(new ImageLayerViewModel(imageLayer));
        }

        private void AddTextLayerViewModel(TextLayer textLayer)
        {
            Layers.Add(new TextLayerViewModel(textLayer));
        }

        private void AddCompositionLayerViewModel(CompositionLayer compositionLayer)
        {
            Layers.Add(new CompositionLayerViewModel(compositionLayer));
        }

        private void ClearCurrentView()
        {
            CompositionProject = null;
            Layers.Clear();
            SelectedLayer = null;
        }

#endregion

#region Transformations

        public void SwitchToScaleMode()
        {
            if (SelectedLayer == null)
            {
                return;
            }

            SelectedLayer.SwitchMode(LayerInteractionMode.Scale);
        }

        public void ScaleSelectedLayer(Point p)
        {
            if (SelectedLayer == null || SelectedLayer.Mode != LayerInteractionMode.Scale)
            {
                return;
            }

            //Debug.WriteLine($"ScaleSelectedLayer()");

            SelectedLayer.ScaleLayer(p);

            if (CompositionProject != null)
            {
                CompositionProject.IsModified = true;
            }
        }

        public void MoveSelectedLayer(Point p)
        {
            if (SelectedLayer == null || SelectedLayer.Mode != LayerInteractionMode.Move)
            {
                return;
            }

            //Debug.WriteLine($"MoveSelectedLayer {p.X} {p.Y}");

            SelectedLayer.MoveLayer(p);

            if (CompositionProject != null)
            {
                CompositionProject.IsModified = true;
            }
        }

        public void RotateSelectedLayer()
        {
            if (SelectedLayer == null)
            {
                return;
            }

            SelectedLayer.RotateLayer();

            if (CompositionProject != null)
            {
                CompositionProject.IsModified = true;
            }
        }

#endregion

#region MouseOperations

        private bool CheckScaleHandleSelection(Point p)
        {
            bool result = false;

            if (SelectedLayer == null || SelectedLayer.Mode != LayerInteractionMode.Scale)
            {
                return result;
            }

            return SelectedLayer.CheckGizmoSelection(p);
        }

        private void CheckMouseSelection(Point p)
        {
            // Check for scale gizmo selection
            if (CheckScaleHandleSelection(p))
            {
                return;
            }

            // Pass in new selection mode
            SelectedLayer?.SwitchMode(LayerInteractionMode.None);
            SelectedLayer = null;

            // Start in Z order with the top layers
            for (int i = Layers.Count - 1; i >= 0; --i)
            {
                if (Layers.ElementAt(i).Layer != null &&
                    Layers.ElementAt(i).CheckLayerSelection(p))
                {
                    SelectedLayer = Layers.ElementAt(i);
                    SelectedLayer.Layer.OffsetX = (int)(p.X - SelectedLayer.Layer.PosX);
                    SelectedLayer.Layer.OffsetY = (int)(p.Y - SelectedLayer.Layer.PosY);
                    SelectedLayer.SwitchMode(LayerInteractionMode.Move);
                    SelectedLayer.InitialMovePosition = p;
                    break;
                }
            }

            // Move it to Z front and store transformation point
            if (SelectedLayer != null)
            {
                //Debug.WriteLine($"Layer selected {SelectedLayer.Layer.Id}");
                //Debug.WriteLine($"Layer Mode {SelectedLayer.Mode}");

                int index = Layers.IndexOf(SelectedLayer);

                if (index >= 0)
                {
                    Layers.Move(index, Layers.Count - 1);
                }
            }
        }

        public void OnMouseDown(Point p, int clickCount)
        {
            //Debug.WriteLine($"OnMouseDown {p.X} {p.Y}");

            if (clickCount == 1)
            {
                m_isMouseDown = true;
                CheckMouseSelection(p);
            }
            else if (clickCount == 2)
            {
                if ( SelectedLayer != null && SelectedLayer is TextLayerViewModel textLayerVM)
                {
                    // Open text dialog for editing
                    var textDialogVM = new TextDialogViewModel
                    {
                        Text = textLayerVM.TextLayer.Text,
                        SelectedFont = textLayerVM.TextLayer.FontName,
                        FontSize = textLayerVM.TextLayer.FontSize
                    };

                    var textDialog = new TextDialog(textDialogVM);

                    if (textDialog.ShowDialog() != true)
                    {
                        return;
                    }

                    // Update the layer with the new text properties
                    textLayerVM.TextLayer.Text = textDialogVM.Text;
                    textLayerVM.TextLayer.FontName = textDialogVM.SelectedFont;
                    textLayerVM.TextLayer.FontSize = textDialogVM.FontSize;

                    // Mark the project as modified
                    if (CompositionProject != null)
                    {
                        CompositionProject.IsModified = true;
                    }

                    // Update the bounding box of the layer
                    SelectedLayer.UpdateBBox();
                }
            }
        }

        public void OnMouseUp(Point p)
        {
            //Debug.WriteLine($"OnMouseUp {p.X} {p.Y}");
            m_isMouseDown = false;
            SelectedLayer?.UpdateBBox();
        }

        public void OnMouseMove(Point p)
        {
            if (!m_isMouseDown || SelectedLayer == null)
            {
                return;
            }

            if (SelectedLayer.Mode == LayerInteractionMode.Scale)
            {
                ScaleSelectedLayer(p);
            }
            else if (SelectedLayer.Mode == LayerInteractionMode.Move)
            {
                MoveSelectedLayer(p);
            }
        }

#endregion

    }
}
