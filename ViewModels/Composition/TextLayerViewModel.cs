using AppComposer.Models.Layers;
using System.ComponentModel;

namespace AppComposer.ViewModels.Composition
{
    public class TextLayerViewModel : LayerBaseViewModel
    {
        public TextLayer TextLayer => (TextLayer)Layer;

        public TextLayerViewModel(TextLayer layer) : base(layer)
        {
        }
    }
}
