using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace AppComposer.Models.Layers
{
    [JsonPolymorphic]
    [JsonDerivedType(typeof(ImageLayer), "image")]
    [JsonDerivedType(typeof(TextLayer), "text")]
    public abstract class LayerBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public Guid Id { get; set; }

        private double m_posX = 0;
        private double m_posY = 0;
        private int m_width = 0;
        private int m_height = 0;
        private double m_rotation = 0.0;
        private double m_scale = 1.0;
        private bool m_isVisible = true;

        public double PosX
        {
            get => m_posX;
            set => SetProperty(ref m_posX, value);
        }

        public double PosY
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

        public int Height
        {
            get => m_height;
            set => SetProperty(ref m_height, value);
        }

        public int Width
        {
            get => m_width;
            set => SetProperty(ref m_width, value);
        }

        [JsonIgnore]
        public int OffsetX { get; set; }
        [JsonIgnore]
        public int OffsetY { get; set; }

        [JsonIgnore]
        public double CenterX => Width / 2.0;
        [JsonIgnore]
        public double CenterY => Height / 2.0;

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
