using AppComposer.Models;
using System.Windows;
using System.Windows.Media.Imaging;

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

        // TODO Get from config
        public int ScaleGizmoSize { get; set; } = 14;

        public Thickness ScaleGizmoMargin => new Thickness(ScaledWidth, ScaledHeight, 0, 0);

        public double ScaledWidth  => Layer.Width * Layer.Scale;
        public double ScaledHeight => Layer.Height * Layer.Scale;

        public double CenterX => ScaledWidth/2.0;
        public double CenterY => ScaledHeight/2.0;

        private bool Swapped => (Layer.Rotation % 180) != 0;

        public double DisplayWidth => Swapped ? ScaledHeight : ScaledWidth;
        public double DisplayHeight => Swapped ? ScaledWidth : ScaledHeight;

        public LayerBaseViewModel(LayerBase layer)
        {
            Layer = layer;

            Layer.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(LayerBase.Scale))
                {
                    OnPropertyChanged(nameof(ScaledWidth));
                    OnPropertyChanged(nameof(ScaledHeight));
                    OnPropertyChanged(nameof(DisplayWidth));
                    OnPropertyChanged(nameof(DisplayHeight));
                    OnPropertyChanged(nameof(ScaleGizmoMargin));
                }

                if (e.PropertyName == nameof(LayerBase.Height))
                {
                    OnPropertyChanged(nameof(ScaledHeight));
                    OnPropertyChanged(nameof(CenterY));
                    OnPropertyChanged(nameof(ScaleGizmoMargin));
                }

                if (e.PropertyName == nameof(LayerBase.Width))
                {
                    OnPropertyChanged(nameof(ScaledWidth));
                    OnPropertyChanged(nameof(CenterX));
                    OnPropertyChanged(nameof(ScaleGizmoMargin));
                }

                if (e.PropertyName == nameof(LayerBase.Rotation))
                {
                    OnPropertyChanged(nameof(DisplayWidth));
                    OnPropertyChanged(nameof(DisplayHeight));
                }
            };
        }

        public void UpdateScale(Point p)
        {
            double dx = p.X - Layer.PosX;
            double dy = p.Y - Layer.PosY;

            double scale = (dx * Layer.Width + dy * Layer.Height) / (Layer.Width * Layer.Width + Layer.Height * Layer.Height);

            Layer.Scale = Math.Max(0.05, scale);
        }

        public void UpdatePosition(Point p, int canvasWidth, int canvasHeight)
        {
            Layer.PosX = Math.Clamp((int)p.X - Layer.OffsetX, 0, canvasWidth - (int)DisplayWidth);
            Layer.PosY = Math.Clamp((int)p.Y - Layer.OffsetY, 0, canvasHeight - (int)DisplayWidth);
        }

        public bool CheckSelected(Point p)
        {
            bool result = false;

            if ((Layer.PosX <= p.X) &&
                (p.X <= (Layer.PosX + DisplayWidth)) &&
                (Layer.PosY <= p.Y) &&
                (p.Y <= (Layer.PosY + DisplayHeight)))
            {
                result = true;
            }

            return result;
        }
    }
}
