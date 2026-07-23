using AppComposer.Helpers;

namespace AppComposer.Services
{
    public sealed class ConfigurationService
    {
        public static ConfigurationService Instance { get; private set; } = null!;

        public int CanvasWidth { get; }
        public int CanvasHeight { get; }

        public int GridSize { get; }

        public string BackgroundColor { get; }

        public int RulerBandSize { get; }
        public int RulerFontSize { get; }
        public int ScreenDpi { get; }

        public int ImagePreviewSize { get; }

        public bool UseTransparency { get; }
        public bool AllowTextScaling { get; }

        public static void Initialize(string iniPath)
        {
            Instance = new ConfigurationService(iniPath);
        }
        
        private ConfigurationService(string iniPath)
        {
            var reader = new IniReader(iniPath);

            CanvasWidth = Int32.Parse(reader.GetValue("CANVAS", "Width", "650"));
            CanvasHeight = Int32.Parse(reader.GetValue("CANVAS", "Height", "350"));

            GridSize = Int32.Parse(reader.GetValue("CANVAS", "GridSize", "20"));

            BackgroundColor = reader.GetValue("CANVAS", "BackgroundColor", "#FFFFFF");

            ScreenDpi = Int32.Parse(reader.GetValue("CANVAS", "ScreenDpi", "96"));
            RulerBandSize = Int32.Parse(reader.GetValue("CANVAS", "RulerBandSize", "20"));
            RulerFontSize = Int32.Parse(reader.GetValue("CANVAS", "RulerFontSize", "10"));

            ImagePreviewSize = Int32.Parse(reader.GetValue("IMAGES", "PreviewResolution", "256"));

            UseTransparency = bool.Parse(reader.GetValue("OPERATIONS", "UseTransparency", "false"));
            AllowTextScaling = bool.Parse(reader.GetValue("OPERATIONS", "AllowTextScaling", "false"));
        }
    }
}
