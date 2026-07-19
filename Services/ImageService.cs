using AppComposer.Models;
using AppComposer.Models.Layers;
using OpenCvSharp;
using SkiaSharp;
using System.Diagnostics;
using System.IO;
using System.Windows.Media;

namespace AppComposer.Services
{
    public class ImageService
    {
        public ImageService() 
        {
        
        }

        public void LoadImageLayerBitmap(string filename, ImageLayer layer)
        {
            Mat image = Cv2.ImRead(filename, ImreadModes.Color);
            Mat bwImage = ConvertTo1bpp(image);

            layer.SetBitmap(bwImage);
        }

        public void LoadCompLayerBitmap(string filename, CompositionLayer layer)
        {
            Mat image = Cv2.ImRead(filename, ImreadModes.Color);
            Mat bwImage = ConvertTo1bpp(image);

            //Debug.WriteLine($"LoadImageLayerBitmap() {bwImage.Type()}  Channels={bwImage.Channels()}");

            Mat mask = new();

            //Cv2.Threshold(bwImage, mask, 250, 255, ThresholdTypes.BinaryInv);
            Cv2.Compare(bwImage, Scalar.Black, mask, CmpTypes.EQ);

            // Prevents BoundingRect() from failing if the image is completely white
            if (Cv2.CountNonZero(mask) == 0)
            {
                layer.SetBitmap(new Mat());
                return;
            }

            Rect bbox = Cv2.BoundingRect(mask);
            Mat cropped = new(bwImage, bbox);

            layer.SetBitmap(cropped.Clone());
        }

        public ImageLayer LoadImageBitmapToLayer(string filename)
        {
            Mat image = Cv2.ImRead(filename, ImreadModes.Color);
            Mat bwImage = ConvertTo1bpp(image);

            ImageLayer layer = new ImageLayer(filename);
            layer.SetBitmap(bwImage);

            return layer;
        }

        public Mat ConvertTo1bpp(Mat image)
        {
            Mat grayImage = new Mat();
            Mat bwImage = new Mat();
            Cv2.CvtColor(image, grayImage, ColorConversionCodes.BGR2GRAY);
            Cv2.Threshold(grayImage, bwImage, 128, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);
            return bwImage;
        }

        public static void RenderComposition(CompositionProject project, string projectDirectory)
        {
            Mat rendered = new(project.CanvasSettings.Height, project.CanvasSettings.Width, MatType.CV_8UC1, Scalar.White);

            foreach (LayerBase layer in project.Layers)
            {
                if (layer is ImageLayer imageLayer)
                {
                    Matrix transformMatrix = Matrix.Identity;
                    transformMatrix.ScaleAt(imageLayer.Scale, imageLayer.Scale, imageLayer.CenterX, imageLayer.CenterY);
                    transformMatrix.RotateAt(imageLayer.Rotation, imageLayer.CenterX, imageLayer.CenterY);
                    transformMatrix.Translate(imageLayer.PosX, imageLayer.PosY);

                    Mat affineMatrix = new Mat(2, 3, MatType.CV_64FC1);

                    affineMatrix.Set(0, 0, transformMatrix.M11);
                    affineMatrix.Set(0, 1, transformMatrix.M21);
                    affineMatrix.Set(0, 2, transformMatrix.OffsetX);
                    affineMatrix.Set(1, 0, transformMatrix.M12);
                    affineMatrix.Set(1, 1, transformMatrix.M22);
                    affineMatrix.Set(1, 2, transformMatrix.OffsetY);

                    Mat transformed = new();

                    Cv2.WarpAffine(imageLayer.Bitmap, transformed, affineMatrix, new Size(project.CanvasSettings.Width, project.CanvasSettings.Height),
                        InterpolationFlags.Linear, BorderTypes.Constant, Scalar.White);

                    Mat mask = new();

                    Cv2.Compare(transformed, Scalar.Black, mask, CmpTypes.EQ);

                    transformed.CopyTo(rendered, mask);
                }
                else if (layer is TextLayer textLayer)
                {
                    // Render text at (0,0)
                    using SKBitmap skBitmap = new(textLayer.Width, textLayer.Height, SKColorType.Gray8, SKAlphaType.Opaque);

                    using SKCanvas canvas = new(skBitmap);
                    canvas.Clear(SKColors.White);

                    using SKTypeface typeface = SKTypeface.FromFamilyName(textLayer.FontName);

                    using SKFont font = new(typeface, textLayer.FontSize);

                    using SKPaint paint = new()
                    {
                        Color = SKColors.Black,
                        IsAntialias = true
                    };

                    canvas.DrawText(textLayer.Text, 0.0f, font.Size, SKTextAlign.Left, font, paint);

                    // Convert to OpenCV Mat
                    using Mat textMat = Mat.FromPixelData(skBitmap.Height, skBitmap.Width, MatType.CV_8UC1, skBitmap.GetPixels(), skBitmap.RowBytes);

                    // Calculate the transformation and convert to OpenCV format
                    Matrix transformMatrix = Matrix.Identity;
                    transformMatrix.ScaleAt(textLayer.Scale, textLayer.Scale, textLayer.CenterX, textLayer.CenterY);
                    transformMatrix.RotateAt(textLayer.Rotation, textLayer.CenterX, textLayer.CenterY);
                    transformMatrix.Translate(textLayer.PosX, textLayer.PosY);

                    using Mat affineMatrix = new(2, 3, MatType.CV_64FC1);

                    affineMatrix.Set(0, 0, transformMatrix.M11);
                    affineMatrix.Set(0, 1, transformMatrix.M21);
                    affineMatrix.Set(0, 2, transformMatrix.OffsetX);
                    affineMatrix.Set(1, 0, transformMatrix.M12);
                    affineMatrix.Set(1, 1, transformMatrix.M22);
                    affineMatrix.Set(1, 2, transformMatrix.OffsetY);

                    using Mat transformed = new();

                    Cv2.WarpAffine(textMat, transformed, affineMatrix, new Size(project.CanvasSettings.Width, project.CanvasSettings.Height),
                        InterpolationFlags.Linear, BorderTypes.Constant, Scalar.White);

                    // Mask the element
                    using Mat mask = new();
                    // if (pixel > threshold) pixel = 0 else pixel = maxval
                    Cv2.Threshold(transformed, mask, 250, 255, ThresholdTypes.BinaryInv);

                    transformed.CopyTo(rendered, mask);
                }
                else if (layer is CompositionLayer compLayer)
                {
                    Matrix transformMatrix = Matrix.Identity;
                    transformMatrix.ScaleAt(compLayer.Scale, compLayer.Scale, compLayer.CenterX, compLayer.CenterY);
                    transformMatrix.RotateAt(compLayer.Rotation, compLayer.CenterX, compLayer.CenterY);
                    transformMatrix.Translate(compLayer.PosX, compLayer.PosY);

                    Mat affineMatrix = new Mat(2, 3, MatType.CV_64FC1);

                    affineMatrix.Set(0, 0, transformMatrix.M11);
                    affineMatrix.Set(0, 1, transformMatrix.M21);
                    affineMatrix.Set(0, 2, transformMatrix.OffsetX);
                    affineMatrix.Set(1, 0, transformMatrix.M12);
                    affineMatrix.Set(1, 1, transformMatrix.M22);
                    affineMatrix.Set(1, 2, transformMatrix.OffsetY);

                    Mat transformed = new();

                    Cv2.WarpAffine(compLayer.Bitmap, transformed, affineMatrix, new Size(project.CanvasSettings.Width, project.CanvasSettings.Height),
                        InterpolationFlags.Linear, BorderTypes.Constant, Scalar.White);

                    Mat mask = new();

                    Cv2.Compare(transformed, Scalar.Black, mask, CmpTypes.EQ);

                    transformed.CopyTo(rendered, mask);
                }
            }

            string renderedImagePath = Path.Combine(projectDirectory, project.RenderedImageName);
            string previewImagePath = Path.Combine(projectDirectory, project.PreviewImageName);

            // Save the rendered image
            Cv2.ImWrite(renderedImagePath, rendered);

            //Save preview
            Mat preview = new();

            // TODO Get Preview Size from project settings
            double scale = Math.Min(256.0 / rendered.Width, 256.0 / rendered.Height);

            Cv2.Resize(rendered, preview, new Size(0, 0), scale, scale, InterpolationFlags.Area);

            Cv2.ImWrite(previewImagePath, preview);
        }
    }
}
