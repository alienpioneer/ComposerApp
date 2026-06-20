using System;

namespace AppComposer.Models
{
    public class LayerBase
    {
        public string Name { get; set; }

        public Guid Id { get; set; } = Guid.NewGuid();

        public int PosX { get; set; }
        public int PosY { get; set; }

        public int Height { get; set; }
        public int Width { get; set; }

        public double Rotation { get; set; }
        public double Scale { get; set; }

        public bool IsVisible { get; set; } = true;

        public LayerBase(string name)
        {
            Name = name;
        }
      
    }
}
