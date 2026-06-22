using AppComposer.Models;

namespace AppComposer.ViewModels.Composition
{
    public class ImageLayerViewModel : ViewModelBase
    {
        public ImageLayer? Layer { get; set; }

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
