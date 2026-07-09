using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace AppComposer.Components
{
    public partial class RulerCanvas : Canvas
    {
        public static readonly DependencyProperty DpiProperty = DependencyProperty.Register(
            nameof(Dpi),
            typeof(double),
            typeof(RulerCanvas),
            new FrameworkPropertyMetadata(96.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(
            nameof(FontSize),
            typeof(double),
            typeof(RulerCanvas),
            new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty RulerSizeProperty = DependencyProperty.Register(
            nameof(RulerSize),
            typeof(int),
            typeof(RulerCanvas),
            new FrameworkPropertyMetadata(20, FrameworkPropertyMetadataOptions.AffectsRender));

        public double Dpi
        {
            get => (double)GetValue(DpiProperty);
            set => SetValue(DpiProperty, value);
        }

        public double FontSize 
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public int RulerSize
        {
            get => (int)GetValue(RulerSizeProperty);
            set => SetValue(RulerSizeProperty, value);
        }

        public bool IsVertical { get; set; } = false;

        protected override void OnRender(DrawingContext drawContext)
        {
            base.OnRender(drawContext);

            Brush rulerBrush = CreateRulerTicks(IsVertical);

            drawContext.DrawRectangle(
                rulerBrush,
                null,
                new Rect(0, 0, ActualWidth, ActualHeight));

            DrawLabels(drawContext);
        }

        private void DrawLabels(DrawingContext drawContext)
        {
            double pixelsPerCm = Dpi / 2.54;
            double length = IsVertical ? ActualHeight : ActualWidth;

            for (int i = 0; i * pixelsPerCm <= length; i++)
            {
                FormattedText text = new FormattedText(
                    i.ToString(),
                    CultureInfo.CurrentCulture,
                    System.Windows.FlowDirection.LeftToRight,
                    new Typeface("Segoe UI"),
                    FontSize,
                    Brushes.Black,
                    VisualTreeHelper.GetDpi(this).PixelsPerDip);

                double posX = i * pixelsPerCm;

                Rect textGlyphBounds = text.BuildGeometry(new Point()).Bounds;

                Point position = IsVertical
                                ? i==0 ? new Point(1, -textGlyphBounds.Top+1) : new Point(1, posX-text.Height/2)
                                : i==0 ? new Point(-textGlyphBounds.Left+1, -textGlyphBounds.Top+1) : new Point(posX-textGlyphBounds.Width/2, -textGlyphBounds.Top+1);

                drawContext.DrawText(text, position);
            }
        }

        private Brush CreateRulerTicks(bool isVertical)
        {
            double pixelsPerCm = Dpi / 2.54;
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

            for (int i = 0; i <= 10; i++)
            {
                double lineHeight = i == 0 ? RulerSize / 2 :
                                    i == 5 ? RulerSize / 3 : RulerSize / 4;

                if (isVertical)
                {
                    geometryGroup.Children.Add(new LineGeometry(new Point(RulerSize - lineHeight, i * pixelsPerMm),
                                                                new Point(RulerSize, i * pixelsPerMm)));
                }
                else
                {
                    geometryGroup.Children.Add(new LineGeometry(new Point(i * pixelsPerMm, RulerSize),
                                                                new Point(i * pixelsPerMm, RulerSize - lineHeight)));
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
