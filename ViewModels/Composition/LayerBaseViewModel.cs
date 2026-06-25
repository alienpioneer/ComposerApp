using AppComposer.Models;
using System.Windows;

namespace AppComposer.ViewModels.Composition
{
    public abstract class LayerBaseViewModel : ViewModelBase
    {
        public LayerBase Layer { get; }

        private bool m_isSelected = false;
        private bool m_isScaleMode = false;

        public bool IsSelected
        {
            get => m_isSelected;
            set
            {
                if (m_isSelected != value)
                {
                    m_isSelected = value;
                    OnPropertyChanged(nameof(BorderThickness));
                }
            }
        }

        public bool IsScaleMode
        {
            get => m_isScaleMode;
            set
            {
                if (m_isScaleMode != value)
                {
                    m_isScaleMode = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(BorderThickness));
                }
            }
        }

        public double BorderThickness => (IsSelected && !IsScaleMode) ? 1.0 / Layer.Scale : 0.0;

        // public int diagonalSquared => (int)((Layer.Width * Layer.Scale)*(Layer.Width * Layer.Scale) + (Layer.Height * Layer.Scale)*(Layer.Height * Layer.Scale));

        // TODO Get from config
        public int ScaleGizmoSize { get; set; } = 14;
        // public Thickness ScaleGizmoMargin => new Thickness(0, 0, -ScaleGizmoSize + 1, -ScaleGizmoSize + 1);
        public Thickness ScaleGizmoMargin => new Thickness(ScaledWidth, ScaledHeight, 0, 0);

        public double ScaledWidth  => Layer.Width * Layer.Scale;
        public double ScaledHeight => Layer.Height * Layer.Scale;

        public LayerBaseViewModel(LayerBase layer)
        {
            Layer = layer;

            Layer.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(LayerBase.Scale))
                {
                    OnPropertyChanged(nameof(ScaledWidth));
                    OnPropertyChanged(nameof(ScaledHeight));
                    OnPropertyChanged(nameof(ScaleGizmoMargin));
                }

                if (e.PropertyName == nameof(LayerBase.Height))
                {
                    OnPropertyChanged(nameof(ScaledHeight));
                    OnPropertyChanged(nameof(ScaleGizmoMargin));
                }

                if (e.PropertyName == nameof(LayerBase.Width))
                {
                    OnPropertyChanged(nameof(ScaledWidth));
                    OnPropertyChanged(nameof(ScaleGizmoMargin));
                }
            };
        }

        public void UpdatePosition(Point p, int canvasWidth, int canvasHeight)
        {
            Layer.PosX = Math.Clamp((int)p.X - Layer.OffsetX, 0, canvasWidth - (int)ScaledWidth);
            Layer.PosY = Math.Clamp((int)p.Y - Layer.OffsetY, 0, canvasHeight - (int)ScaledHeight);
        }
    }
}
