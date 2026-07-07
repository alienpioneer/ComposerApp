using System.Windows;

namespace AppComposer.Models
{
    public class CanvasSettings
    {
        public int Width { get; set; } = 650;
        public int Height { get; set; } = 350;

        public string BackgroundColor { get; set; } = "#FFFFFF";

        public bool ShowGrid { get; set; } = false;

        public int GridSize { get; set; } = 15;

        public Rect GridViewport => new(0, 0, GridSize, GridSize);

        public bool ShowRulers { get; set; } = false;

        public CanvasSettings()
        {
        }
    }
}
