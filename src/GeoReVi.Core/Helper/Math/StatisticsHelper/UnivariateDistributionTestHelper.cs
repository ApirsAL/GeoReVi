using Caliburn.Micro;

namespace GeoReVi
{
    public class UnivariateDistributionTestHelper : BasicUnivariateStatisticalMeasuresHelper, IStatisticalAnalysis
    {
        #region Public properties

        /// <summary>
        /// Kolmogorov-Smirnoff Helper
        /// </summary>
        private KolmogorovSmirnovHelper kolmogorovSmirnovHelper;
        public KolmogorovSmirnovHelper KolmogorovSmirnovHelper
        {
            get => this.kolmogorovSmirnovHelper;
            set
            {
                this.kolmogorovSmirnovHelper = value;
                NotifyOfPropertyChange(() => KolmogorovSmirnovHelper);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public UnivariateDistributionTestHelper(double[] _dataSet)
        {
            KolmogorovSmirnovHelper = new KolmogorovSmirnovHelper(_dataSet);
        }

        #endregion

        #region Methods

        public void ComputeKolmogorovSmirnov()
        {

        }

        #endregion
    }
}
