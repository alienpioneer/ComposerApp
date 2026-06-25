using AppComposer.Models;
using System.ComponentModel;

namespace AppComposer.ViewModels.Composition
{
    public class ImageLayerViewModel : LayerBaseViewModel
    {
        public ImageLayer ImageLayer => (ImageLayer)Layer;

        public ImageLayerViewModel(ImageLayer layer) : base(layer)
        {
        }

    }
}
