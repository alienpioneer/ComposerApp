using System;

namespace AppComposer.Models.Layers
{
    public class CompositionLayer : LayerBase
    {
        public string CompositionFile { get; set; }

        public string PreviewImage { get; set; }

        public CompositionLayer(string compositionFile, string previewImage) : base()
        {
            CompositionFile = compositionFile;
            PreviewImage = previewImage;
        }

        // Parameterless constructor for deserialization
        public CompositionLayer() : this(string.Empty, string.Empty)
        {
        }
    }
}
