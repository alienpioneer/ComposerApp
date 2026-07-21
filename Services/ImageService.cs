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
            using Mat image = Cv2.ImRead(filename, ImreadModes.Color);
            using Mat bwImage = ConvertTo1bpp(image);

            if (layer.TransparentWhite)
            {
                using Mat preview = AddBWAlpha(bwImage);
                layer.SetBitmap(preview.Clone());
            }
            else
            {
                layer.SetBitmap(bwImage.Clone());
            }
        }

        // Overloaded method to load an image layer and return it
        public ImageLayer LoadImageLayerBitmap(string filename)
        {
            using Mat image = Cv2.ImRead(filename, ImreadModes.Color);
            using Mat bwImage = ConvertTo1bpp(image);

            ImageLayer layer = new ImageLayer(filename);

            if (layer.TransparentWhite)
            {
                using Mat preview = AddBWAlpha(bwImage);
                layer.SetBitmap(preview.Clone());
            }
            else
            {
                layer.SetBitmap(bwImage.Clone());
            }

            return layer;
        }

        public void LoadCompLayerBitmap(string filename, CompositionLayer layer)
        {
            using Mat image = Cv2.ImRead(filename, ImreadModes.Color);

            // Keep the anti-aliased grayscale image
            using Mat grayImage = new();
            Cv2.CvtColor(image, grayImage, ColorConversionCodes.BGR2GRAY);

            // Build the mask only to locate the bounding box
            using Mat mask = new();

            //Cv2.Threshold(grayImage, mask, 250, 255, ThresholdTypes.BinaryInv);

            Cv2.Compare(grayImage, Scalar.Black, mask, CmpTypes.EQ);

            // Check if the mask is empty (no black pixels)
            if (Cv2.CountNonZero(mask) == 0)
            {
                layer.SetBitmap(new Mat());
                return;
            }

            Rect bbox = Cv2.BoundingRect(mask);
            using Mat cropped = new(grayImage, bbox);

            if (layer.TransparentWhite)
            {
                using Mat preview = AddBWAlpha(cropped);

                // Store the preview with alpha for WPF
                layer.SetBitmap(preview.Clone());
            }
            else
            {
                // Store the grayscale image
                layer.SetBitmap(cropped.Clone());
            }
        }

        public Mat ConvertTo1bpp(Mat image)
        {
            Mat grayImage = new Mat();

            switch (image.Channels())
            {
                case 1:
                    grayImage = image.Clone();
                    break;

                case 3:
                    Cv2.CvtColor(image, grayImage, ColorConversionCodes.BGR2GRAY);
                    break;

                case 4:
                    Cv2.CvtColor(image, grayImage, ColorConversionCodes.BGRA2GRAY);
                    break;

                default:
                    Debug.WriteLine("Unsupported image format.");
                    break;
            }

            Mat bwImage = new Mat();
            Cv2.Threshold(grayImage, bwImage, 128, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);

            return bwImage;
        }

        public Mat AddBWAlpha(Mat image)
        {
            Mat alphaChannel = new Mat();
            Cv2.Threshold(image, alphaChannel, 250, 255, ThresholdTypes.BinaryInv);

            Mat imageWithAlpha = new Mat();
            Cv2.CvtColor(image, imageWithAlpha, ColorConversionCodes.GRAY2BGRA);

            Cv2.InsertChannel(alphaChannel, imageWithAlpha, 3);

            return imageWithAlpha;
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

                    using Mat affineMatrix = new Mat(2, 3, MatType.CV_64FC1);

                    affineMatrix.Set(0, 0, transformMatrix.M11);
                    affineMatrix.Set(0, 1, transformMatrix.M21);
                    affineMatrix.Set(0, 2, transformMatrix.OffsetX);
                    affineMatrix.Set(1, 0, transformMatrix.M12);
                    affineMatrix.Set(1, 1, transformMatrix.M22);
                    affineMatrix.Set(1, 2, transformMatrix.OffsetY);

                    using Mat transformed = new();

                    Cv2.WarpAffine(imageLayer.Bitmap, transformed, affineMatrix, new Size(project.CanvasSettings.Width, project.CanvasSettings.Height),
                        InterpolationFlags.Linear, BorderTypes.Constant, Scalar.White);

                    //TODO Check transparency and mask problem. Do not overfill with white if the image is not transparent.
                    if (imageLayer.TransparentWhite)
                    {
                        using Mat mask = new();
                        Cv2.Compare(transformed, Scalar.Black, mask, CmpTypes.EQ);
                        //Cv2.Threshold(transformed, mask, 250, 255, ThresholdTypes.BinaryInv);
                        transformed.CopyTo(rendered, mask);
                    }
                    else
                    {
                        using Mat originalMask = Mat.Ones(imageLayer.Bitmap.Rows, imageLayer.Bitmap.Cols, MatType.CV_8UC1);
                        using Mat warpedMask = new();
                        Cv2.WarpAffine(originalMask, warpedMask, affineMatrix, new Size(project.CanvasSettings.Width, project.CanvasSettings.Height), InterpolationFlags.Linear, BorderTypes.Constant, Scalar.Black);
                        transformed.CopyTo(rendered, warpedMask);
                    }
                }
                else if (layer is TextLayer textLayer)
                {
                    // Render text at (0,0) using SkiaSharp to measure the text size and create a bitmap
                    using SKTypeface typeface = SKTypeface.FromFamilyName(textLayer.FontName);
                    using SKFont font = new(typeface, textLayer.FontSize);

                    SKRect bounds = new();
                    font.MeasureText(textLayer.Text, out bounds);

                    int width = (int)Math.Ceiling(bounds.Width);
                    int height = (int)Math.Ceiling(bounds.Height);

                    // Recreate the bitmap with the measured size
                    using SKBitmap skBitmap = new(width, height, SKColorType.Gray8, SKAlphaType.Opaque);
                    using SKCanvas canvas = new(skBitmap);

                    canvas.Clear(SKColors.White);

                    using SKPaint paint = new()
                    {
                        Color = SKColors.Black,
                        IsAntialias = true
                    };

                    canvas.DrawText(textLayer.Text, -bounds.Left, -bounds.Top, SKTextAlign.Left, font, paint);

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

                    using Mat affineMatrix = new Mat(2, 3, MatType.CV_64FC1);

                    affineMatrix.Set(0, 0, transformMatrix.M11);
                    affineMatrix.Set(0, 1, transformMatrix.M21);
                    affineMatrix.Set(0, 2, transformMatrix.OffsetX);
                    affineMatrix.Set(1, 0, transformMatrix.M12);
                    affineMatrix.Set(1, 1, transformMatrix.M22);
                    affineMatrix.Set(1, 2, transformMatrix.OffsetY);

                    using Mat transformed = new();

                    Cv2.WarpAffine(compLayer.Bitmap, transformed, affineMatrix, new Size(project.CanvasSettings.Width, project.CanvasSettings.Height),
                        InterpolationFlags.Linear, BorderTypes.Constant, Scalar.White);

                    if (compLayer.TransparentWhite)
                    {
                        using Mat mask = new();
                        Cv2.Compare(transformed, Scalar.Black, mask, CmpTypes.EQ);
                        //Cv2.Threshold(transformed, mask, 250, 255, ThresholdTypes.BinaryInv);
                        transformed.CopyTo(rendered, mask);
                    }
                    else
                    {
                        using Mat originalMask = Mat.Ones(compLayer.Bitmap.Rows, compLayer.Bitmap.Cols, MatType.CV_8UC1);
                        using Mat warpedMask = new();
                        Cv2.WarpAffine(originalMask, warpedMask, affineMatrix, new Size(project.CanvasSettings.Width, project.CanvasSettings.Height), InterpolationFlags.Linear, BorderTypes.Constant, Scalar.Black);
                        transformed.CopyTo(rendered, warpedMask);
                    }
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
