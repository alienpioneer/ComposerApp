using AppComposer.Models.Layers;
using System;
using System.Text.Json.Serialization;

namespace AppComposer.Models
{
    public class CompositionProject
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = "";

        public string ProjectFile { get; set; } = "";

        // TODO update canvas when loading accordingly
        public CanvasSettings CanvasSettings { get; set; }

        // TODO public ProjectSettings ProjectSettings { get; set; }

        public List<LayerBase> Layers { get; set; }

        public string PreviewImageName { get; } = "preview.bmp";

        public string RenderedImageName { get; set; } = "rendered.bmp";

        public string Version { get; set; } = "1.00";

        [JsonIgnore]
        public bool IsModified { get; set; } = false;

        public CompositionProject(CanvasSettings canvasSettings)
        {
            Id = Guid.NewGuid();
            CanvasSettings = canvasSettings;
            Layers = new List<LayerBase>();
        }
    }
}
