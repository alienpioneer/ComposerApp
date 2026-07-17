using AppComposer.Models.Layers;


namespace AppComposer.ViewModels.Composition
{
    public class CompositionLayerViewModel : LayerBaseViewModel
    {
        public CompositionLayer CompositionLayer => (CompositionLayer)Layer;

        public CompositionLayerViewModel(CompositionLayer layer)
            : base(layer)
        {

        }
    }
}

