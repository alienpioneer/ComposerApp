using System.Diagnostics;

namespace AppComposer.Models
{
    public class LayerBase
    {
        public Guid Id { get; set; }

        public int PosX { get; set; }
        public int PosY { get; set; }

        public int Height { get; set; }
        public int Width { get; set; }

        public double Rotation { get; set; }
        public double Scale { get; set; }

        public bool IsVisible { get; set; } = true;

        public LayerBase()
        {
            Id = Guid.NewGuid();
            Debug.WriteLine($"Layer created with ID: {Id}");
        }
      
    }
}
