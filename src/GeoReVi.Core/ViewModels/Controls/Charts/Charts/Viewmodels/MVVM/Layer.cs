using Caliburn.Micro;

namespace GeoReVi
{
    public abstract class Layer : PropertyChangedBase
    {

        /// <summary>
        /// Top coordinate of the layer
        /// </summary>
        private double _top = 1;
        public double Top
        {
            get => this._top;
            set
            {
                this._top = value;
                NotifyOfPropertyChange(() => Top);
            }
        }

        /// <summary>
        /// Base of the layer
        /// </summary>
        private double _base = 0;
        public double Base
        {
            get => this._base;
            set
            {
                this._base = value;
                NotifyOfPropertyChange(() => Base);
            }
        }
    }
}
