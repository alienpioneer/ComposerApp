using AppComposer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public int BorderThickness => (IsSelected && !IsScaleMode) ? 1 : 0;

        // TODO Get from config
        public int ScaleGizmoSize { get; set; } = 14;

        public Thickness ScaleGizmoMargin => new Thickness(0, 0, -ScaleGizmoSize + 1, -ScaleGizmoSize + 1);

        public LayerBaseViewModel(LayerBase layer)
        {
            Layer = layer;
        }
    }
}
