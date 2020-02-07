using System.Linq;
using System.Threading.Tasks;
using Accord.Statistics.Distributions.Univariate;
using Accord.Statistics.Testing;
using Caliburn.Micro;
namespace GeoReVi
{
    public class KolmogorovSmirnovHelper : BaseStatisticalTest
    {
        #region Properties

        /// <summary>
        /// The distribution to be tested
        /// </summary>
        private NormalDistribution distribution = NormalDistribution.Standard;

        /// <summary>
        /// Data set
        /// </summary>
        private double[] dataSet = new double[] { };
        public double[] DataSet
        {
            get => this.dataSet;
            private set
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
        public KolmogorovSmirnovHelper(double[] _dataSet)
        {
            DataSet = _dataSet;
            Compute();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Computing the test
        /// </summary>
        /// <returns></returns>
        public async override Task Compute()
        {
            distribution = new NormalDistribution(DataSet.Average(), DataSet.StdDev());

            var kstest = new KolmogorovSmirnovTest(DataSet, distribution);

            Statistics = kstest.Statistic; // 0.29
            PValue = kstest.PValue;       // 0.3067

            Significance = kstest.Significant; // false
        }

        #endregion
    }
}
