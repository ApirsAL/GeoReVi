using System;
using Caliburn.Micro;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace GeoReVi
{
    /// <summary>
    /// A hierarchical model for spatial layers
    /// </summary>
    public class LayerModel : PropertyChangedBase
    {
        private string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        private object objects;
        public object Objects
        {
            get => objects;
            set
            {
                this.objects = value;
                NotifyOfPropertyChange(() => Objects);
            }
        }

        public ObservableCollection<LayerModel> Children { get; } = new ObservableCollection<LayerModel>();
    }
}
