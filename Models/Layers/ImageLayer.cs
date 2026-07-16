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
        public string ImagePath { get; set; } = "";
        public string PreviewPath { get; set; } = "";

        [JsonIgnore]
        public Mat Bitmap { get; set; } = new();
        [JsonIgnore]
        public BitmapSource? Preview { get; private set; }

        public ImageLayer()
        {
        }

        public ImageLayer(string imagePath): base()
        {
            ImagePath = imagePath;
            PreviewPath = Path.Combine("layers", $"{Id:N}.bmp");
        }

        public void SetBitmap(Mat bitmap)
        {
            Bitmap = bitmap;
            Width = bitmap.Width;
            Height = bitmap.Height;
            Preview = bitmap.ToBitmapSource();

            //Debug.WriteLine($"{bitmap.Width} x {bitmap.Height}");
            //Debug.WriteLine($"{Preview.Width} x {Preview.Height}");
        }

    }
}
