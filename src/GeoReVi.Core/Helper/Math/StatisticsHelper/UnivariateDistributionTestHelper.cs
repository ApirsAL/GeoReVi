using Caliburn.Micro;

namespace GeoReVi
{
    public class UnivariateDistributionTestHelper : PropertyChangedBase, IUnivariateDataSetHolder
    {
        #region Public properties

        /// <summary>
        /// The data set that is hold
        /// </summary>
        private double[] dataSet = new double[] { };
        public double[] DataSet
        {
            get => dataSet;
            set
            {
                this.dataSet = value;
                NotifyOfPropertyChange(() => DataSet);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public UnivariateDistributionTestHelper()
        {

        }

        /// <summary>
        /// Constructor with dataset
        /// </summary>
        /// <param name="_dataSet">The univariate data set array</param>
        public UnivariateDistributionTestHelper(double[] _dataSet)
        {
            DataSet = _dataSet;
        }

        #endregion
    }
}
