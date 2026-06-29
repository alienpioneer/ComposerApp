using AppComposer.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

using System.Windows;

namespace AppComposer.ViewModels.Composition
{
    public class CompositionPageViewModel : ViewModelBase
    {
        bool m_isMouseDown = false;

        private CompositionPage m_compositionPage;

        public CompositionPage CompositionPage
        {
            get => m_compositionPage;
            set
            {
                m_compositionPage = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<LayerBaseViewModel> Layers { get; set; }

        public LayerBaseViewModel? SelectedLayer { get; set; } = null;

        public CompositionPageViewModel(CanvasSettings canvasSettings)        
        {
            m_compositionPage = new CompositionPage(canvasSettings);
            Layers = new();
        }

        public void AddImageLayer(ImageLayer imageLayer)
        {
            CompositionPage.Layers.Add(imageLayer);
            Layers.Add(new ImageLayerViewModel(imageLayer));
        }

        public void AddTextLayer(TextLayer textLayer)
        {
            CompositionPage.Layers.Add(textLayer);
            Layers.Add(new TextLayerViewModel(textLayer));
        }

        public void RemoveSelectedLayer()
        {
            if (SelectedLayer == null)
            {
                return;
            }

            CompositionPage.Layers.Remove(SelectedLayer.Layer);
            Layers.Remove(SelectedLayer);
            SelectedLayer = null;
        }

        public void ResetSelectedLayer()
        {
            if (SelectedLayer == null)
            {
                return;
            }

            SelectedLayer.Layer.Rotation = 0.0;
            SelectedLayer.Layer.Scale = 1.0;
        }

        public void OnMouseDown(Point p)
        {
            Debug.WriteLine($"OnMouseDown {p.X} {p.Y}");
            m_isMouseDown = true;
            GetMouseSelection(p);
        }

        public void OnMouseUp(Point p)
        {
            Debug.WriteLine($"OnMouseUp {p.X} {p.Y}");
            m_isMouseDown = false;
        }

        public void OnMouseMove(Point p)
        {
            if (!m_isMouseDown || SelectedLayer == null)
            {
                return;
            }

            //Debug.WriteLine($"OnMouseMove {p.X} {p.Y}");

            if(SelectedLayer.IsScaleMode)
            {
                ScaleSelectedLayer(p);
            }
            else
            {
                MoveSelectedLayer(p);
            }  
        }

        private void GetMouseSelection(Point p)
        {
            // Check for scale mode
            if (CheckScaleSelection(p))
            {
                return;
            }

            // Pass in new selection mode
            SwitchToScaleMode(false);
            SelectedLayer = null;

            foreach (var layer in Layers)
            {
                layer.IsSelected = false;
            }

            // Start in Z order with the top layers
            for (int i = Layers.Count - 1; i >= 0; --i)
            {

                if (Layers.ElementAt(i).Layer != null &&
                    Layers.ElementAt(i).CheckSelected(p))
                {
                    SelectedLayer = Layers.ElementAt(i);
                    Layers.ElementAt(i).Layer.OffsetX = (int)p.X - Layers.ElementAt(i).Layer.PosX;
                    Layers.ElementAt(i).Layer.OffsetY = (int)p.Y - Layers.ElementAt(i).Layer.PosY;
                    Layers.ElementAt(i).IsSelected = true;
                    break;
                }

                //Debug.WriteLine($"Layer selected {SelectedLayer?.Layer.Id}");
            }

            // Move it to Z front
            if (SelectedLayer != null)
            {
                int index = Layers.IndexOf(SelectedLayer);

                if (index >= 0)
                {
                    Layers.Move(index, Layers.Count - 1);
                }
            }
        }

        private bool CheckScaleSelection(Point p)
        {
            bool result = false;

            if (SelectedLayer == null || !SelectedLayer.IsScaleMode)
            {
                return result;
            }

            int scaleGizmoStartX = SelectedLayer.Layer.PosX + (int)SelectedLayer.DisplayWidth;
            int scaleGizmoStartY = SelectedLayer.Layer.PosY + (int)SelectedLayer.DisplayHeight;

            if ( p.X >= scaleGizmoStartX &&
                p.X <= (scaleGizmoStartX + SelectedLayer.ScaleGizmoSize) &&
                p.Y >= scaleGizmoStartY &&
                p.Y <= (scaleGizmoStartY + SelectedLayer.ScaleGizmoSize))
            {
                //Debug.WriteLine($"Scale Gizmo selected");
                result = true;
            }

            return result;
        }

        public void SwitchToScaleMode(bool switchMode)
        {
            if (SelectedLayer == null)
            {
                return;
            }

            SelectedLayer.IsScaleMode = switchMode;
        }

        public void ScaleSelectedLayer(Point p)
        {
            if (SelectedLayer == null || !SelectedLayer.IsScaleMode)
            {
                return;
            }

            //Debug.WriteLine($"ScaleSelectedLayer()");

            SelectedLayer.UpdateScale(p);
        }

        public void MoveSelectedLayer(Point p)
        {
            if (SelectedLayer == null || SelectedLayer.IsScaleMode)
            {
                return;
            }

            //Debug.WriteLine($"MoveSelectedLayer {p.X} {p.Y}");
            SelectedLayer.UpdatePosition(p, CompositionPage.CanvasSettings.Width, CompositionPage.CanvasSettings.Height);
        }

        public void RotateSelectedLayer()
        {
            if (SelectedLayer == null)
            {
                return;
            }

            SelectedLayer.Layer.Rotation += 90;

            // TODO Recalculate selection box after rotation
        }
    }
}
