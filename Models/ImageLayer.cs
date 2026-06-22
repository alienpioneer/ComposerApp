using System;

namespace AppComposer.Models
{
    public class ImageLayer: LayerBase
    {
        public string ImagePath { get; set; }

        public ImageLayer(string imagePath): base()
        {
            ImagePath = imagePath;
        }
    }
}
