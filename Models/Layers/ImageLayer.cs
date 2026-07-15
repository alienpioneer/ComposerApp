using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System.IO;
using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Windows.Media.Imaging;

namespace AppComposer.Models.Layers
{
    public class ImageLayer: LayerBase
    {
        public string ImagePath { get; set; }
        public string PreviewPath { get; set; }

        [JsonIgnore]
        public Mat Bitmap { get; set; }
        [JsonIgnore]
        public BitmapSource? Preview { get; set; }

        public ImageLayer(string imagePath, Mat bitmap): base()
        {
            ImagePath = imagePath;
            Bitmap = bitmap;
            Width = bitmap.Width;
            Height = bitmap.Height;

            Preview = bitmap.ToBitmapSource();
            PreviewPath = Path.Combine("layers", $"{Id:N}.bmp");

            //Debug.WriteLine($"PreviewPath {PreviewPath}");
            //Debug.WriteLine($"{bitmap.Width} x {bitmap.Height}");
            //Debug.WriteLine($"{Preview.Width} x {Preview.Height}");
        }
    }
}
