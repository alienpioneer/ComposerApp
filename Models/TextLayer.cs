using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppComposer.Models
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
