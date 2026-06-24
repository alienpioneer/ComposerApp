

namespace AppComposer.Models
{
    public class CompositionPage
    {
        public CanvasSettings CanvasSettings { get; set; }
        public List<LayerBase> Layers { get; set; }

        public CompositionPage(CanvasSettings canvasSettings)
        {
            CanvasSettings = canvasSettings;
            Layers = new List<LayerBase>();
        }
    }
}
