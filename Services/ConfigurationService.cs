using System;

namespace AppComposer.Services
{
    public sealed class ConfigurationService
    {
        public static ConfigurationService Instance { get; private set; } = null!;
        public int CanvasWidth { get; }
        public int CanvasHeight { get; }

        public static void Initialize(string iniPath)
        {
            Instance = new ConfigurationService(iniPath);
        }

        private ConfigurationService(string iniPath)
        {
            var reader = new IniReader(iniPath);

            CanvasWidth = Int32.Parse(reader.GetValue("CANVAS", "Width", "650"));
            CanvasHeight = Int32.Parse(reader.GetValue("CANVAS", "Height", "650"));
        }

    }

}
