using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;

namespace AppComposer.Models
{
    public class LayerBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public Guid Id { get; set; }

        private int m_posX;
        private int m_posY;
        private double m_rotation;
        private double m_scale;
        private bool m_isVisible = false;

        public int PosX
        {
            get => m_posX;
            set => SetProperty(ref m_posX, value);
        }

        public int PosY
        {
            get => m_posY;
            set => SetProperty(ref m_posY, value);
        }

        public double Rotation 
        {
            get => m_rotation;
            set => SetProperty(ref m_rotation, value);
        }

        public double Scale 
        {
            get => m_scale;
            set => SetProperty(ref m_scale, value);
        }

        public int Height { get; set; }
        public int Width { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }

        public bool IsVisible
        {
            get => m_isVisible;
            set => SetProperty(ref m_isVisible, value);
        }

        public LayerBase()
        {
            Id = Guid.NewGuid();

            Debug.WriteLine($"Layer created with ID: {Id}");
        }
      
        public void UpdatePosition(Point p, int canvasWidth, int canvasHeight)
        {
            PosX = Math.Clamp((int)p.X-OffsetX, 0, canvasWidth-Width);
            PosY = Math.Clamp((int)p.Y-OffsetY, 0, canvasHeight-Height);
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
