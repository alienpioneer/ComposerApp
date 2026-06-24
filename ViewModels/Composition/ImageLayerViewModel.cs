using AppComposer.Models;
using System.ComponentModel;

namespace AppComposer.ViewModels.Composition
{
    public class ImageLayerViewModel : LayerBaseViewModel
    {
        public ImageLayer Layer { get; set; }

        public ImageLayerViewModel()
        {
            Layer = new("");
        }

        public ImageLayerViewModel(ImageLayer imageLayer)
        {
            Layer = imageLayer;
        }

    }
}
