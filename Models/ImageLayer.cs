using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Diagnostics;
using System.Windows.Media.Imaging;

namespace AppComposer.Models
{
    public class ImageLayer: LayerBase
    {
        public string ImagePath { get; set; }

        public Mat Bitmap { get; set; }

        public BitmapSource? Preview { get; set; }

        public ImageLayer(string imagePath, Mat bitmap): base()
        {
            ImagePath = imagePath;
            Bitmap = bitmap;
            Width = bitmap.Width;
            Height = bitmap.Height;

            Preview = BitmapSourceConverter.ToBitmapSource(bitmap);

            //Debug.WriteLine($"{bitmap.Width} x {bitmap.Height}");
            Debug.WriteLine($"{Preview.Width} x {Preview.Height}");
        }
    }
}
