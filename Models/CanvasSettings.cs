using System;

namespace AppComposer.Models
{
    public class CanvasSettings
    {
        public int Width { get; set; } = 650;
        public int Height { get; set; } = 350;

        public string BackgroundColor { get; set; } = "#FFFFFF";

        public bool ShowGrid { get; set; } = false;
        public int GridSize { get; set; } = 20;

        public bool ShowRulers { get; set; } = false;

        public CanvasSettings()
        {
        }
    }
}
