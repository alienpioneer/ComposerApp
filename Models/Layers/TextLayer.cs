using System.Diagnostics;

namespace AppComposer.Models.Layers
{
    public class TextLayer : LayerBase
    {
        private string _text = "";
        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }

        private string _fontName = "";
        public string FontName
        {
            get => _fontName;
            set => SetProperty(ref _fontName, value);
        }

        private int _fontSize;
        public int FontSize
        {
            get => _fontSize;
            set => SetProperty(ref _fontSize, value);
        }

        public TextLayer(string text, string fontName, int fontSize): base()
        {
            Text = text;
            FontName = fontName;
            FontSize = fontSize;
        }
    }
}
