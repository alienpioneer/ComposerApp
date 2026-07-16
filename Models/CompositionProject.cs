using AppComposer.Models.Layers;
using System;

namespace AppComposer.Models
{
    public class CompositionProject
    {
        public Guid Id { get; set; }

        public String Name { get; set; } = "";

        public CanvasSettings CanvasSettings { get; set; }

        //public ProjectSettings ProjectSettings { get; set; }

        public List<LayerBase> Layers { get; set; }

        public string? PreviewImageName { get; set; }

        public string? RenderedImageName { get; set; }

        public int Version { get; set; } = 1;

        public CompositionProject(CanvasSettings canvasSettings)
        {
            Id = Guid.NewGuid();
            CanvasSettings = canvasSettings;
            Layers = new List<LayerBase>();
        }
    }
}
