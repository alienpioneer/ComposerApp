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

            if (SelectedLayer is ImageLayerViewModel selectedImageLayerVM)
            {
                if (selectedImageLayerVM.Layer != null)
                {
                    CompositionPage.Layers.Remove(selectedImageLayerVM.Layer);
                }
            }
            else if (SelectedLayer is TextLayerViewModel selectedTextLayerVM)
            {
                if (selectedTextLayerVM.Layer != null)
                {
                    CompositionPage.Layers.Remove(selectedTextLayerVM.Layer);
                }
            }

            Layers.Remove(SelectedLayer);
            SelectedLayer = null;
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

            if (SelectedLayer is ImageLayerViewModel imageLayerVM)
            {
                if (imageLayerVM.Layer != null)
                {
                    //Debug.WriteLine($"Image Layer selected {p.X} {p.Y}");
                    imageLayerVM.Layer.UpdatePosition(p, CompositionPage.CanvasSettings.Width, CompositionPage.CanvasSettings.Height);
                }
            }
            else if (SelectedLayer is TextLayerViewModel textLayerVM)
            {
                if (textLayerVM.Layer != null)
                {
                    //Debug.WriteLine($"Text Layer selected {p.X} {p.Y}");
                    textLayerVM.Layer.UpdatePosition(p, CompositionPage.CanvasSettings.Width, CompositionPage.CanvasSettings.Height);
                }
            }
        }

        private void GetMouseSelection(Point p)
        {
            SelectedLayer = null;

            foreach (var layer in Layers)
            {
                layer.IsSelected = false;
            }

            // Start in Z order with the top layers
            for (int i = Layers.Count - 1; i >= 0; --i)
            {
                if (Layers.ElementAt(i) is ImageLayerViewModel imageLayerVM)
                {
                    if (imageLayerVM.Layer != null &&
                        CheckLayerSelection(imageLayerVM.Layer, p))
                    {
                        //Debug.WriteLine($"Image Layer selected {p.X} {p.Y}");
                        SelectedLayer = imageLayerVM;
                        imageLayerVM.Layer.OffsetX = (int)p.X - imageLayerVM.Layer.PosX;
                        imageLayerVM.Layer.OffsetY = (int)p.Y - imageLayerVM.Layer.PosY;
                        imageLayerVM.IsSelected = true;
                        break;
                    }
                }
                else if (Layers.ElementAt(i) is TextLayerViewModel textLayerVM)
                {
                    if (textLayerVM.Layer != null &&
                        CheckLayerSelection(textLayerVM.Layer, p))
                    {
                        //Debug.WriteLine($"Text Layer selected {p.X} {p.Y}");
                        SelectedLayer = textLayerVM;
                        textLayerVM.Layer.OffsetX = (int)p.X - textLayerVM.Layer.PosX;
                        textLayerVM.Layer.OffsetY = (int)p.Y - textLayerVM.Layer.PosY;
                        textLayerVM.IsSelected = true;
                        break;
                    }
                }
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

        private bool CheckLayerSelection(LayerBase layer, Point p)
        {
            bool result = false;

            if ((layer.PosX <= p.X) &&
                (p.X <= (layer.PosX + layer.Width)) &&
                (layer.PosY <= p.Y) &&
                (p.Y <= (layer.PosY + layer.Height)))
            {
                result = true;
            }

            return result;
        }

    }
}
