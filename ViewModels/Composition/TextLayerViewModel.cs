using AppComposer.Models;
using System.ComponentModel;

namespace AppComposer.ViewModels.Composition
{
    public class TextLayerViewModel : LayerBaseViewModel
    {
        public TextLayer Layer { get; set; }

        public TextLayerViewModel()
        {
            Layer = new("TEST", "Segoe", 24);
        }

        public TextLayerViewModel(TextLayer textLayer)
        {
            Layer = textLayer;
        }
    }
}
