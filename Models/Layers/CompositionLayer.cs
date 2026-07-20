using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System.IO;
using System.Text.Json.Serialization;
using System.Windows.Media.Imaging;

namespace AppComposer.Models.Layers
{
    public class CompositionLayer : LayerBase
    {
        public string SourceProject { get; set; } = "";
        public string PreviewPath { get; set; } = "";

        public bool TransparentWhite { get; set; } = true;

        [JsonIgnore]
        public Mat Bitmap { get; set; } = new();
        [JsonIgnore]
        public BitmapSource? Preview { get; private set; }

        public CompositionLayer() : base()
        {
        }

        public CompositionLayer(string projectPath) : base()
        {
            SourceProject = projectPath;
            PreviewPath = Path.Combine("layers", $"{Id:N}.bmp");
        }

        public void SetBitmap(Mat bitmap)
        {
            Bitmap = bitmap;
            Width = bitmap.Width;
            Height = bitmap.Height;
            Preview = bitmap.ToBitmapSource();
        }
    }
}
