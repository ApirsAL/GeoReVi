using Caliburn.Micro;

namespace GeoReVi
{
    public class Mesh : PropertyChangedBase, IGeometry
    {
        #region Public properties

        private string name = "";
        public string Name
        {
            get => this.name;
            set
            {
                this.name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        private Dimensionality dimensionality = Dimensionality.OneD;
        public Dimensionality Dimensionality
        {
            get => this.dimensionality;
            set
            {
                this.dimensionality = value;
                NotifyOfPropertyChange(() => Dimensionality);
            }
        }

        #endregion

        #region Constructor

        public Mesh()
        {

        }

        #endregion

        #region Public methods



        #endregion

    }
}
