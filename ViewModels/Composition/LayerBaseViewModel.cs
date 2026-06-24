using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppComposer.ViewModels.Composition
{
    public class LayerBaseViewModel : ViewModelBase
    {
        private bool m_isSelected = false;

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

        public int BorderThickness => IsSelected ? 1 : 0;

        public LayerBaseViewModel()
        { }
    }
}
