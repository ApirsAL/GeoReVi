using Caliburn.Micro;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoReVi
{
    public class ThermalConductionHelper: PropertyChangedBase
    {
        #region Properties

        /// <summary>
        /// Thermal diffusivity 
        /// </summary>
        private BindableCollection<Number> thermalDiffusivity = new BindableCollection<Number>()
        {    new Number(1),
             new Number(1),
             new Number(1)
        };
        public BindableCollection<Number> ThermalDiffusivity
        {
            get => this.thermalDiffusivity;
            set
            {
                this.thermalDiffusivity = value;
                NotifyOfPropertyChange(() => ThermalDiffusivity);
            }
        }

        /// <summary>
        /// Returns the thermal diffusivity matrix
        /// </summary>
        public Matrix<double> ThermalDiffusivityMatrix
        {
            get
            {
                return Matrix<double>.Build.DenseOfDiagonalVector(Vector<double>.Build.DenseOfEnumerable(ThermalDiffusivity.Select(x => x.Num).ToList()));
            }
        }


        #endregion

        #region Constructor

        #endregion

        #region Methods

        #endregion
    }
}
