using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
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

        public int GridSize { get; set; } = 10;

        public int RulerSize { get; set; } = 20;
        public int RulerFontSize { get; set; } = 10;
        public int Dpi { get; set; } = 96;

        private bool m_showGrid = false;
        public bool ShowGrid
        {
            get => m_showGrid;
            set
            {
                m_showGrid = value;
                OnPropertyChanged(nameof(ShowGrid));
                OnPropertyChanged(nameof(CanvasBrush));
            }
        }

        private bool m_showRulers = false;
        public bool ShowRulers
        {
            get => m_showRulers;
            set
            {
                m_showRulers = value;
                OnPropertyChanged(nameof(ShowRulers));
            }
        }

        public CanvasSettings()
        {
        }

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

        public Brush HorizontalRulerBrush
        {
            get
            {
                return CreateRulerBrush(Dpi, false);
            }
        }

        public Brush VerticalRulerBrush
        {
            get
            {
                return CreateRulerBrush(Dpi, true);
            }
        }

        private Brush CreateGridBrush(int gridSize)
        {
            var geometry = new GeometryGroup();
            geometry.Children.Add(new LineGeometry(new Point(gridSize, 0), new Point(gridSize, gridSize)));
            geometry.Children.Add(new LineGeometry(new Point(0, gridSize), new Point(gridSize, gridSize)));

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

            return new DrawingBrush(drawingGroup)
            {
                TileMode = TileMode.Tile,
                Viewport = new Rect(0, 0, gridSize, gridSize),
                ViewportUnits = BrushMappingMode.Absolute
            };
        }

        private Brush CreateRulerBrush(double dpi, bool isVertical)
        {
            double pixelsPerCm = dpi / 2.54;
            double pixelsPerMm = pixelsPerCm / 10.0;

            Rect backgroundFrame;

            if (isVertical)
            {
                backgroundFrame = new Rect(0, 0, RulerSize, pixelsPerCm);
            }
            else 
            {
                backgroundFrame = new Rect(0, 0, pixelsPerCm, RulerSize);
            }

            var background = new GeometryDrawing
            {
                Brush = Brushes.LightGray,
                Geometry = new RectangleGeometry(backgroundFrame)
            };

            var geometryGroup = new GeometryGroup();

            for (int i=0; i<=10; i++)
            {
                double lineHeight = i==0 ? RulerSize/2 :
                                    i==5 ? RulerSize/3 : RulerSize/4;

                if (isVertical)
                {
                    geometryGroup.Children.Add(new LineGeometry(new Point(RulerSize - lineHeight, i * pixelsPerMm),
                                                                new Point(RulerSize, i * pixelsPerMm)));
                }
                else
                {
                    geometryGroup.Children.Add(new LineGeometry(new Point(i*pixelsPerMm, RulerSize),
                                                                new Point(i*pixelsPerMm, RulerSize-lineHeight)));
                }
            }

            var rulerLines = new GeometryDrawing
            {
                Pen = new Pen(Brushes.Black, 1),
                Geometry = geometryGroup
            };

            var drawingGroup = new DrawingGroup();
            drawingGroup.Children.Add(background);
            drawingGroup.Children.Add(rulerLines);

            return new DrawingBrush(drawingGroup)
            {
                TileMode = TileMode.Tile,
                Viewport = backgroundFrame,
                ViewportUnits = BrushMappingMode.Absolute,
                Stretch = Stretch.None
            };
        }
    }
}
