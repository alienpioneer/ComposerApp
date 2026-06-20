using System;

namespace AppComposer.Models
{
    class ImageLayer: LayerBase
    {
        public string ImagePath { get; set; }

        public ImageLayer(string imagePath): base("Image Layer")
        {
            ImagePath = imagePath;
        }
    }
}
