using AppComposer.Models;
using AppComposer.Models.Layers;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using SkiaSharp;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using static OpenCvSharp.LineIterator;

namespace AppComposer.Services
{
    public class CompositionService
    {
        private readonly ImageService _imageService;

        public CompositionService(ImageService imageService)
        {
            _imageService = imageService;
        }

        public CompositionProject? LoadProject(string projectDirectory)
        {
            string jsonPath = Path.Combine(projectDirectory, "project.json");

            string projectJsonData = File.ReadAllText(jsonPath);

            CompositionProject? project = JsonSerializer.Deserialize<CompositionProject>(projectJsonData);

            if (project == null)
            {
                Debug.WriteLine($"LoadProject  failed to deserialize {projectDirectory}/project.json");
                return null;
            }

            foreach (LayerBase layer in project.Layers)
            {
                if (layer is ImageLayer imageLayer)
                {
                    string fullPath = Path.Combine(projectDirectory, imageLayer.PreviewPath);

                    _imageService.LoadLayerBitmap(fullPath, imageLayer);
                }
                else if (layer is CompositionLayer compositionLayer)
                {
                    // TODO
                }
            }

            return project;
        }

        public void SaveProject(CompositionProject project, string compositionName, string parentDirectory)
        {
            if(project == null)
            {
                Debug.WriteLine("Can't save a null CompositionProject");
                return;
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                //ReferenceHandler = ReferenceHandler.Preserve
            };

            string projectDirectory = Path.Combine(parentDirectory, compositionName);

            project.Name = compositionName;

            //Debug.WriteLine($"{projectDirectory}");

            Directory.CreateDirectory(projectDirectory);
            Directory.CreateDirectory(Path.Combine(projectDirectory, "layers"));

            SaveImageLayers(project, projectDirectory);
            RenderComposition(project, projectDirectory);

            string json = JsonSerializer.Serialize(project, options);

            File.WriteAllText( Path.Combine(projectDirectory, "project.json"), json);
        }

        public void RenderComposition(CompositionProject project, string projectDirectory)
        {
            Mat rendered = new(project.CanvasSettings.Height, project.CanvasSettings.Width, MatType.CV_8UC1, Scalar.White);

            foreach (LayerBase layer in project.Layers)
            {
                if (layer is ImageLayer imageLayer)
                {
                    Matrix m = Matrix.Identity;
                    m.ScaleAt(imageLayer.Scale, imageLayer.Scale, imageLayer.CenterX, imageLayer.CenterY);
                    m.RotateAt(imageLayer.Rotation, imageLayer.CenterX, imageLayer.CenterY);
                    m.Translate(imageLayer.PosX, imageLayer.PosY);

                    Mat affineMatrix = new Mat(2, 3, MatType.CV_64FC1);

                    affineMatrix.Set(0, 0, m.M11);
                    affineMatrix.Set(0, 1, m.M12);
                    affineMatrix.Set(0, 2, m.OffsetX);
                    affineMatrix.Set(1, 0, m.M21);
                    affineMatrix.Set(1, 1, m.M22);
                    affineMatrix.Set(1, 2, m.OffsetY);

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

                    using SKTypeface typeface =SKTypeface.FromFamilyName(textLayer.FontName);

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
                    Matrix m = Matrix.Identity;
                    m.ScaleAt(textLayer.Scale, textLayer.Scale, textLayer.CenterX, textLayer.CenterY);
                    m.RotateAt(textLayer.Rotation, textLayer.CenterX, textLayer.CenterY);
                    m.Translate(textLayer.PosX, textLayer.PosY);

                    using Mat affine = new(2, 3, MatType.CV_64FC1);

                    affine.Set(0, 0, m.M11);
                    affine.Set(0, 1, m.M12);
                    affine.Set(0, 2, m.OffsetX);
                    affine.Set(1, 0, m.M21);
                    affine.Set(1, 1, m.M22);
                    affine.Set(1, 2, m.OffsetY);

                    using Mat transformed = new();

                    Cv2.WarpAffine(textMat, transformed, affine, new Size(project.CanvasSettings.Width, project.CanvasSettings.Height),
                        InterpolationFlags.Linear, BorderTypes.Constant, Scalar.White);

                    // Mask the element
                    using Mat mask = new();
                    // if (pixel > threshold) pixel = 0 else pixel = maxval
                    Cv2.Threshold(transformed, mask, 250, 255, ThresholdTypes.BinaryInv);

                    transformed.CopyTo(rendered, mask);
                }

                // Save the rendered image
                Cv2.ImWrite(Path.Combine(projectDirectory, "rendered.bmp"), rendered);

                //Save preview
                Mat preview = new();

                // TODO Get Preview Size from project settings
                double scale = Math.Min(256.0 / rendered.Width, 256.0 / rendered.Height);

                Cv2.Resize(rendered, preview, new Size(0, 0), scale, scale, InterpolationFlags.Area);

                Cv2.ImWrite(Path.Combine(projectDirectory, "preview.bmp"), preview);
            }
            
        }

        public void SaveImageLayers(CompositionProject project, string projectDirectory)
        {
            foreach (LayerBase layer in project.Layers)
            {
                if (layer is ImageLayer imageLayer)
                {
                    string imagePreviewPath = Path.Combine(projectDirectory, imageLayer.PreviewPath);
                    Cv2.ImWrite(imagePreviewPath, imageLayer.Bitmap);
                }
            }
        }

    }
}
