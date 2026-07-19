using AppComposer.ViewModels;
using System.Windows;

namespace AppComposer.Views
{
    public partial class TextDialog : Window
    {
        public TextDialog(TextDialogViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
