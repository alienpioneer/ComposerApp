using AppComposer.Models;
using AppComposer.Models.Layers;
using System.Collections.ObjectModel;
using System.Diagnostics;

using System.Windows;

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

        public void LoadProject(CompositionProject project)
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
                    // TODO
                }
            }

            OnPropertyChanged();
        }

        public void AddImageLayer(ImageLayer imageLayer)
        {
            CompositionProject?.Layers.Add(imageLayer);
            AddImageLayerViewModel(imageLayer);
        }

        public void AddTextLayer(TextLayer textLayer)
        {
            CompositionProject?.Layers.Add(textLayer);
            AddTextLayerViewModel(textLayer);
        }

        public void RemoveSelectedLayer()
        {
            if (SelectedLayer == null)
            {
                return;
            }

            CompositionProject?.Layers.Remove(SelectedLayer.Layer);
            Layers.Remove(SelectedLayer);
            SelectedLayer = null;
        }

        public void ResetSelectedLayer()
        {
            if (SelectedLayer == null)
            {
                return;
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
        }

        public void MoveSelectedLayer(Point p)
        {
            if (SelectedLayer == null || SelectedLayer.Mode != LayerInteractionMode.Move)
            {
                return;
            }

            //Debug.WriteLine($"MoveSelectedLayer {p.X} {p.Y}");

            SelectedLayer.MoveLayer(p);
        }

        public void RotateSelectedLayer()
        {
            if (SelectedLayer == null)
            {
                return;
            }

            SelectedLayer.RotateLayer();
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

        public void OnMouseDown(Point p)
        {
            //Debug.WriteLine($"OnMouseDown {p.X} {p.Y}");
            m_isMouseDown = true;
            CheckMouseSelection(p);
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
