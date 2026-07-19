using AppComposer.Helpers;
using AppComposer.Models;
using AppComposer.Models.Layers;
using OpenCvSharp;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text.Json;


namespace AppComposer.Services
{
    public class CompositionService
    {
        private readonly ImageService _imageService;

        public CompositionService(ImageService imageService)
        {
            _imageService = imageService;
        }

        public CompositionLayer? LoadCompositionLayer(string projectJsonPath)
        {
            Debug.WriteLine($"projectJsonPath {projectJsonPath}");

            string projectJsonData = File.ReadAllText(projectJsonPath);

            CompositionProject? compositionProject = JsonSerializer.Deserialize<CompositionProject>(projectJsonData);

            if (compositionProject == null)
            {
                Debug.WriteLine($"LoadCompositionLayer failed to deserialize {projectJsonPath}");
                return null;
            }

            string? projectDirectory = Path.GetDirectoryName(projectJsonPath);

            if (projectDirectory == null)
            {
                Debug.WriteLine("Project directory not found.");
                return null;
            }

            string projectRenderPath = Path.Combine(projectDirectory, compositionProject.RenderedImageName);

            CompositionLayer compositionLayer = new(projectJsonPath);
            _imageService.LoadCompLayerBitmap(projectRenderPath, compositionLayer);

            return compositionLayer;
        }

        public CompositionProject? LoadCompositionProject(string projectDirectory)
        {
            string jsonPath = Path.Combine(projectDirectory, "project.json");

            Debug.WriteLine($"Loading project {jsonPath}");

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
                    string imagePath = Path.Combine(projectDirectory, imageLayer.PreviewPath);

                    _imageService.LoadImageLayerBitmap(imagePath, imageLayer);
                }
                else if (layer is CompositionLayer compositionLayer)
                {
                    string compPreviewPath = Path.Combine(projectDirectory, compositionLayer.PreviewPath);
                    _imageService.LoadCompLayerBitmap(compPreviewPath, compositionLayer);
                }
            }

            return project;
        }

        public static void SaveCompositionProject(CompositionProject? project, string destinationProjectPath, string parentDirectory)
        {
            if(project == null)
            {
                Debug.WriteLine("Can't save a null CompositionProject");
                return;
            }

            string projectName = Path.GetFileNameWithoutExtension(destinationProjectPath);
            string projectDirectory = Path.Combine(parentDirectory, projectName);

            project.ProjectFile = destinationProjectPath;
            project.Name = projectName;

            //Debug.WriteLine($"Saving project name {projectName}");
            //Debug.WriteLine($"Saving project to {Path.GetDirectoryName(destinationProjectPath)}");

            Directory.CreateDirectory(projectDirectory);
            Directory.CreateDirectory(Path.Combine(projectDirectory, "layers"));

            string projectJsonPath = Path.Combine(projectDirectory, "project.json");

            SaveProjectImageLayers(project, projectDirectory);
            ImageService.RenderComposition(project, projectDirectory);

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                //ReferenceHandler = ReferenceHandler.Preserve
            };

            string json = JsonSerializer.Serialize(project, options);
            File.WriteAllText(projectJsonPath, json);

            project.IsModified = false;
        }

        public static void SaveProjectImageLayers(CompositionProject project, string projectDirectory)
        {
            foreach (LayerBase layer in project.Layers)
            {
                if (layer is ImageLayer imageLayer)
                {
                    string imagePreviewPath = Path.Combine(projectDirectory, imageLayer.PreviewPath);
                    Cv2.ImWrite(imagePreviewPath, imageLayer.Bitmap);
                }
                else if (layer is CompositionLayer compLayer)
                {
                    string compPreviewPath = Path.Combine(projectDirectory, compLayer.PreviewPath);
                    Cv2.ImWrite(compPreviewPath, compLayer.Bitmap);
                }
            }
        }
    }
}
