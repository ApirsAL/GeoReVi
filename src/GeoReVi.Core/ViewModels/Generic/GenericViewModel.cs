using Caliburn.Micro;

namespace GeoReVi
{
    public class GenericViewModel<T> : Screen where T : class, new()
    {
        #region Public properties

        private T vm;
        public T VM
        {
            get => this.vm;
            set
            {
                this.vm = value;
                NotifyOfPropertyChange(() => VM);
            }
        }

        #endregion

        #region Constructor

        public GenericViewModel(T _vm)
        {
            VM = _vm;
        }

        #endregion
    }
}
