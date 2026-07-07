using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace AppComposer.Models
{
    public class CanvasSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int Width { get; set; } = 650;
        public int Height { get; set; } = 350;

        public string BackgroundColor { get; set; } = "#FFBBBB";

        private bool m_showGrid = false;
        public bool ShowGrid 
        {   get => m_showGrid; 
            set
            {
                m_showGrid = value;
                OnPropertyChanged(nameof(ShowGrid));
                OnPropertyChanged(nameof(CanvasBrush));
            }
        }

        public int GridSize { get; set; } = 10;

        public Rect GridViewport => new(0, 0, GridSize, GridSize);

        public bool ShowRulers { get; set; } = false;

        public Brush CanvasBrush
        {
            get
            {
                if (!ShowGrid)
                {
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString(BackgroundColor));
                }
                    

                return CreateGridBrush(GridSize);
            }
        }

        public CanvasSettings()
        {
        }

        private Brush CreateGridBrush(int gridSize)
        {
            var geometry = new GeometryGroup();
            geometry.Children.Add(new LineGeometry(new Point(gridSize, 0), new Point(gridSize, gridSize)));
            geometry.Children.Add(new LineGeometry( new Point(0, gridSize), new Point(gridSize, gridSize)));

            var background = new GeometryDrawing
            {
                Brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BackgroundColor)),
                Geometry = new RectangleGeometry(new Rect(0, 0, gridSize, gridSize))
            };

            var gridLines = new GeometryDrawing
            {
                Pen = new Pen(Brushes.DarkGray, 1),
                Geometry = geometry
            };

            var drawingGroup = new DrawingGroup();
            drawingGroup.Children.Add(background);
            drawingGroup.Children.Add(gridLines);

            // var drawing = new GeometryDrawing
            // {
            //     Brush = Brushes.LightGray,
            //     Pen = new Pen(Brushes.Black, 1),
            //     Geometry = geometry
            // };

            return new DrawingBrush(drawingGroup)
            {
                TileMode = TileMode.Tile,
                Viewport = new Rect(0, 0, gridSize, gridSize),
                ViewportUnits = BrushMappingMode.Absolute
            };
        }



    }
}
