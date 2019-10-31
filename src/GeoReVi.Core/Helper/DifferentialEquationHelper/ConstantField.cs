using Caliburn.Micro;

namespace GeoReVi
{
    public class SpatialField : PropertyChangedBase
    {
        #region Public properties

        /// <summary>
        /// Name of the property
        /// </summary>
        private string parameter = "";
        public string Parameter
        {
            get => this.parameter;
            set
            {
                this.parameter = value;
                NotifyOfPropertyChange(() => Parameter);
            }
        }

        /// <summary>
        /// The field parameter mesh
        /// </summary>
        private Mesh mesh = new Mesh();
        public Mesh Mesh
        {
            get => this.mesh;
            set
            {
                this.mesh = value;
                NotifyOfPropertyChange(() => Mesh);
            }
        }

        /// <summary>
        /// Checks if the field is constant
        /// </summary>
        private bool isConstant = true;
        public bool IsConstant
        {
            get => this.isConstant;
            set
            {
                this.isConstant = value;
                NotifyOfPropertyChange(() => IsConstant);
            }
        }

        /// <summary>
        /// Value of the field if constant
        /// </summary>
        private double value = 0;
        public double Value
        {
            get => this.value;
            set
            {
                this.value = value;
                NotifyOfPropertyChange(() => Value);
            }
        }
        #endregion
    }
}
