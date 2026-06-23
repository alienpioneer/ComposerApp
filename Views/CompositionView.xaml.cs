using AppComposer.ViewModels;
using AppComposer.ViewModels.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AppComposer.Views
{
    public partial class CompositionView : UserControl
    {
        // Important way to get the viewModel instance
        //private CompositionViewModel CompViewModel => (CompositionViewModel)DataContext;
        private CompositionPageViewModel? CurrentPageVM =>((CompositionViewModel)DataContext).CurrentPageVM;

        public CompositionView()
        {
            InitializeComponent();
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(CompositionCanvas);
            CurrentPageVM?.OnMouseDown(p);
            e.Handled = true;
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(CompositionCanvas);
            CurrentPageVM?.OnMouseMove(p);
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(CompositionCanvas);
            CurrentPageVM?.OnMouseUp(p);
            e.Handled = true;
        }

        private void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is TextBlock textBlock)
            {
                int actualWidth = (int)textBlock.ActualWidth;
                int actualHeight = (int)textBlock.ActualHeight;

                if (textBlock.DataContext is TextLayerViewModel textLayerVM)
                {
                    if (textLayerVM.Layer != null)
                    {
                        textLayerVM.Layer.Width = actualWidth;
                        textLayerVM.Layer.Height = actualHeight;
                    }  
                }
            }
        }

        private void TextBlock_SizeChanged(object sender, RoutedEventArgs e)
        {
            if (sender is TextBlock textBlock)
            {
                int actualWidth = (int)textBlock.ActualWidth;
                int actualHeight = (int)textBlock.ActualHeight;

                if (textBlock.DataContext is TextLayerViewModel textLayerVM)
                {
                    if (textLayerVM.Layer != null)
                    {
                        textLayerVM.Layer.Width = actualWidth;
                        textLayerVM.Layer.Height = actualHeight;
                    }
                }
            }
        }
    }
}
