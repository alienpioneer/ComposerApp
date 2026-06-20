using System;

namespace AppComposer.Models
{
    public class CanvasSettings
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public string BackgroundColor { get; set; }

        public bool ShowGrid { get; set; }
        public int GridSize { get; set; }

        public bool ShowRulers { get; set; }


        public CanvasSettings()
        {
            Width = 650;
            Height = 350;
            BackgroundColor = "#FFFFFF"; // Default to white
            ShowGrid = false;
            ShowRulers = false;
            GridSize = 20;
        }
    }
}
