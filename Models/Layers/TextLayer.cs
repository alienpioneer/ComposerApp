using System.Diagnostics;

namespace AppComposer.Models.Layers
{
    public class TextLayer : LayerBase
    {
        public string Text { get; set; }
        public string FontName { get; set; }
        public int FontSize { get; set; }

        public TextLayer(string text, string fontName, int fontSize): base()
        {
            Text = text;
            FontName = fontName;
            FontSize = fontSize;
        }
    }
}
