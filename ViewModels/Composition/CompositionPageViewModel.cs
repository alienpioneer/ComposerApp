using AppComposer.Models;
using AppComposer.ViewModels;
using System.Collections.ObjectModel;


namespace AppComposer.ViewModels.Composition
{
    public class CompositionPageViewModel : ViewModelBase
    {
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

        public ObservableCollection<ViewModelBase> Layers { get; set; }

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

        public void RemoveLayer(ViewModelBase layerViewModel)
        {
            if (layerViewModel is ImageLayerViewModel imageLayerVM)
            {
                if (imageLayerVM.Layer != null)
                {
                    CompositionPage.Layers.Remove(imageLayerVM.Layer);
                }
            }
            else if (layerViewModel is TextLayerViewModel textLayerVM)
            {
                if (textLayerVM.Layer != null)
                {
                    CompositionPage.Layers.Remove(textLayerVM.Layer);
                }

            }

            Layers.Remove(layerViewModel);
        }
    }
}
