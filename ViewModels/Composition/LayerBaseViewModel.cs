using AppComposer.Models.Layers;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace AppComposer.ViewModels.Composition
{
    public enum LayerInteractionMode
    {
        None,
        Move,
        Scale,
        Rotate
    }

    public abstract class LayerBaseViewModel : ViewModelBase
    {
        public LayerBase Layer { get; }
        
        // [top_left_x, top_left_y, bottom_right_x, bottom_right_y]
        public double[] BBox { get; set; } = new double[4];
        public double BBoxWidth => BBox[2] - BBox[0];
        public double BBoxHeight => BBox[3] - BBox[1];

        public Point InitialMovePosition { get; set; }

        public LayerInteractionMode Mode { get; set; }

        // TODO Get from config
        public int ScaleGizmoSize { get; set; } = 14;
        public Thickness ScaleGizmoMargin => new Thickness(BBox[2] - Layer.PosX, BBox[3] - Layer.PosY, 0, 0);

        public bool ShowScaleManipulator { get; set; }

        private bool m_isSelected = false;

        public bool IsSelected
        {
            get => m_isSelected;
            set
            {
                if (m_isSelected != value)
                {
                    m_isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                    OnPropertyChanged(nameof(SelectionBorderThickness));
                }
            }
        }

        public double SelectionBorderThickness => (IsSelected && Mode != LayerInteractionMode.Scale) ? 1.0 / Layer.Scale : 0.0;
        public double ScaleOutlineBorderThickness => Mode == LayerInteractionMode.Scale ? 1.0 / Layer.Scale : 0.0;

        public LayerBaseViewModel(LayerBase layer)
        {
            Layer = layer;

            Layer.PropertyChanged += (_, e) =>
            {
                UpdateBBox();

                if (e.PropertyName == nameof(LayerBase.Scale))
                {
                    OnPropertyChanged(nameof(ScaleGizmoMargin));
                    OnPropertyChanged(nameof(ScaleOutlineBorderThickness));
                }

                if (e.PropertyName == nameof(LayerBase.Height))
                {
                    OnPropertyChanged(nameof(ScaleGizmoMargin));
                }

                if (e.PropertyName == nameof(LayerBase.Width))
                {
                    OnPropertyChanged(nameof(ScaleGizmoMargin));
                }

                if (e.PropertyName == nameof(LayerBase.Rotation))
                {
                    OnPropertyChanged(nameof(ScaleGizmoMargin));
                }
            };
        }

        public void MoveLayer(Point p)
        {
            Layer.PosX = (int)p.X - Layer.OffsetX;
            Layer.PosY = (int)p.Y - Layer.OffsetY;
        }

        public void ScaleLayer(Point p)
        {
            // project point p onto the line from the center of the layer to the corner of the layer
            double centerX = Layer.PosX + Layer.CenterX;
            double centerY = Layer.PosY + Layer.CenterY;

            double dx = Math.Abs(p.X - centerX);
            double dy = Math.Abs(p.Y - centerY);

            double scale = (dx * Layer.CenterX + dy * Layer.CenterY) / (Layer.CenterX * Layer.CenterX + Layer.CenterY * Layer.CenterY);
            Layer.Scale = Math.Max(0.05, scale);
        }

        public void RotateLayer()
        {
            Layer.Rotation += 90;
        }

        public bool CheckLayerSelection(Point p)
        {
            bool result = false;

            //Debug.WriteLine($"CheckLayerSelection()");

            UpdateBBox();

            if (p.X >= BBox[0] &&
                p.X <= BBox[2] &&
                p.Y >= BBox[1] &&
                p.Y <= BBox[3])
            {
                result = true;
                //Debug.WriteLine($"Selection Hit !");
            }

            return result;
        }

        public bool CheckGizmoSelection(Point p)
        {
            bool result = false;

            if (p.X >= BBox[2] &&
                p.X <= (BBox[2] + ScaleGizmoSize) &&
                p.Y >= BBox[3] &&
                p.Y <= (BBox[3] + ScaleGizmoSize))
            {
                result = true;
                //Debug.WriteLine($"Gizmo Hit !");
            }

            return result;
        }

        public void SwitchMode(LayerInteractionMode mode)
        {
            Mode = mode;

            switch (Mode)
            {
                case LayerInteractionMode.None:
                    IsSelected = false;
                    ShowScaleManipulator = false;
                    OnPropertyChanged(nameof(ShowScaleManipulator));
                break;

                case LayerInteractionMode.Move:
                    IsSelected = true;
                break;

                case LayerInteractionMode.Rotate:
                break;

                case LayerInteractionMode.Scale:
                    ShowScaleManipulator = true;
                    OnPropertyChanged(nameof(ShowScaleManipulator));
                    OnPropertyChanged(nameof(ScaleOutlineBorderThickness));
                    OnPropertyChanged(nameof(SelectionBorderThickness)); 
                    OnPropertyChanged(nameof(ScaleGizmoMargin));
                    Debug.WriteLine(ScaleGizmoMargin);
                    break;
            }
        }

        public void UpdateBBox()
        {   
            Matrix matrix = BuildTransformationMatrix();

            // Top Left
            Point p0 = matrix.Transform(new Point(0, 0));
            // Bottom Right
            Point p1 = matrix.Transform(new Point(Layer.Width, Layer.Height));
            // Top Right
            Point p2 = matrix.Transform(new Point(Layer.Width, 0));
            // Bottom Left
            Point p3 = matrix.Transform(new Point(0, Layer.Height));

            double left = Math.Min(Math.Min(p0.X, p1.X), Math.Min(p2.X, p3.X));
            double top = Math.Min(Math.Min(p0.Y, p1.Y), Math.Min(p2.Y, p3.Y));
            double right = Math.Max(Math.Max(p0.X, p1.X), Math.Max(p2.X, p3.X));
            double bottom = Math.Max(Math.Max(p0.Y, p1.Y), Math.Max(p2.Y, p3.Y));

            BBox[0] = left;
            BBox[1] = top;
            BBox[2] = right;
            BBox[3] = bottom;

            OnPropertyChanged(nameof(BBox));
            OnPropertyChanged(nameof(BBoxWidth));
            OnPropertyChanged(nameof(BBoxHeight));
            OnPropertyChanged(nameof(ScaleGizmoMargin));

            //Debug.WriteLine(matrix);
            //Debug.WriteLine(matrix.Transform(new Point(0, 0)));
        }

        private Matrix BuildTransformationMatrix()
        {
            Matrix matrix = Matrix.Identity;

            matrix.ScaleAt(Layer.Scale, Layer.Scale, Layer.CenterX, Layer.CenterY);
            matrix.RotateAt(Layer.Rotation, Layer.CenterX, Layer.CenterY);
            matrix.Translate(Layer.PosX, Layer.PosY);

            return matrix;
        }

        public void ResetTransforms()
        {
            Layer.Rotation = 0.0;
            Layer.Scale = 1.0;
        }

    }
}
