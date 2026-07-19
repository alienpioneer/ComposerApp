using System.Collections.ObjectModel;
using System.Windows.Media;

namespace AppComposer.ViewModels
{
    public class TextDialogViewModel
    {
        public string Text { get; set; }

        public string SelectedFont { get; set; }

        public double FontSize { get; set; }

        public ObservableCollection<string> TextFonts { get; }

        public ObservableCollection<int> AvailableFontSizes { get; }

        public TextDialogViewModel()
        {
            Text = "NoText";
            SelectedFont = "Arial";
            FontSize = 32;

            TextFonts = new ObservableCollection<string>(Fonts.SystemFontFamilies.Select(f => f.Source).OrderBy(f => f));

            AvailableFontSizes = new ObservableCollection<int>{ 8, 9, 10, 11, 12, 14, 16, 18, 20, 24, 28, 32, 36, 48, 64, 72 };

        }
    }
}
