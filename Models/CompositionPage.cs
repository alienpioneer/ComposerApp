using System;

namespace AppComposer.Models
{
    public class CompositionPage
    {
        public CanvasSettings CanvasSettings { get; set; }

        public List<LayerBase> Layers { get; set; } = new List<LayerBase>();

        public CompositionPage(CanvasSettings canvasSettings)
        {
            CanvasSettings = canvasSettings;
        }
    }
}
