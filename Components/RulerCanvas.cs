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

        public bool IsVertical { get; set; } = false;

        protected override void OnRender(DrawingContext drawContext)
        {
            base.OnRender(drawContext); // Draws the existing Background brush

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
    }
}
