using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoReVi
{
    public class UnivariateStatisticalTestViewModel : Screen
    {
        #region Properties

        /// <summary>
        /// An univariate test helper object collection
        /// </summary>
        private BindableCollection<KeyValuePair<string, UnivariateDistributionTestHelper>> univariateTestHelper;
        public BindableCollection<KeyValuePair<string, UnivariateDistributionTestHelper>> UnivariateTestHelper
        {
            get => this.univariateTestHelper;
            set
            {
                this.univariateTestHelper = value;
                NotifyOfPropertyChange(() => UnivariateTestHelper);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public UnivariateStatisticalTestViewModel()
        {

        }

        #endregion
    }
}
