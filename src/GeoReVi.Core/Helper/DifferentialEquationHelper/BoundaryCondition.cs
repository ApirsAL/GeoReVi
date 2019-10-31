using Caliburn.Micro;
using System.Collections.ObjectModel;

namespace GeoReVi
{
    public class BoundaryCondition : PropertyChangedBase
    {
        #region Public properties

        /// <summary>
        /// Type of the boundary condition
        /// </summary>
        private BoundaryConditionType type = BoundaryConditionType.Dirichlet;
        public BoundaryConditionType Type
        {
            get => this.type;
            set
            {
                this.type = value;
                NotifyOfPropertyChange(() => Type);
            }
        }

        /// <summary>
        /// Value of the boundary condition
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

        /// <summary>
        /// All points that are assigned to the boundary condition
        /// </summary>
        private ObservableCollection<LocationTimeValue> boundaryPoints = new ObservableCollection<LocationTimeValue>();
        public ObservableCollection<LocationTimeValue> BoundaryPoints
        {
            get => this.boundaryPoints;
            set
            {
                this.boundaryPoints = value;
                NotifyOfPropertyChange(() => BoundaryPoints);
            }
        }

        #endregion
    }
}
