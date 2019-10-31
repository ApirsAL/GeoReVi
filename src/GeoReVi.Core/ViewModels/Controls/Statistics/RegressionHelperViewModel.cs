using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using Caliburn.Micro;

namespace GeoReVi
{
    /// <summary>
    /// A view model holding the information for a multivariate regression analysis
    /// </summary>
    public class RegressionHelperViewModel : PropertyChangedBase
    {
        #region Public properties

        /// <summary>
        /// The regression helper for this view model
        /// </summary>
        private RegressionHelper regressionHelper = new RegressionHelper();
        public RegressionHelper RegressionHelper
        {
            get => this.regressionHelper;
            set
            {
                this.regressionHelper = value;
                NotifyOfPropertyChange(() => RegressionHelper);
            }
        }

        #endregion

        #region MyRegion

        /// <summary>
        /// constructor with data set
        /// </summary>
        [ImportingConstructor]
        public RegressionHelperViewModel(IEnumerable<Mesh> _dataSet)
        {
            RegressionHelper = new RegressionHelper(_dataSet);
        }

        /// <summary>
        /// default constructor
        /// </summary>
        [ImportingConstructor]
        public RegressionHelperViewModel()
        {

        }

        #endregion
    }
}
