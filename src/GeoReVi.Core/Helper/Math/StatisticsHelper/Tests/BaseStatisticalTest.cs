using Caliburn.Micro;
using System.Threading.Tasks;

namespace GeoReVi
{
    public abstract class BaseStatisticalTest : PropertyChangedBase, IStatisticalAnalysis
    {
        #region Properties

        /// <summary>
        /// The result of the statistical test
        /// </summary>
        private double statistics = 0;
        public double Statistics
        {
            get => this.statistics;
            set
            {
                this.statistics = value;
                NotifyOfPropertyChange(() => Statistics);
            }
        }

        /// <summary>
        /// P value of the result
        /// </summary>
        private double pValue = 0;
        public double PValue
        {
            get => this.pValue;
            set
            {
                this.pValue = value;
                NotifyOfPropertyChange(() => PValue);
            }
        }

        /// <summary>
        /// Significance of the test result
        /// </summary>
        private bool significance = false;
        public bool Significance
        {
            get => this.significance;
            set
            {
                this.significance = value;
                NotifyOfPropertyChange(() => Significance);
            }
        }

        /// <summary>
        /// Checks if test is computing
        /// </summary>
        private bool isComputing = false;
        public bool IsComputing
        {
            get => isComputing;
            set
            {
                this.isComputing = value;
                NotifyOfPropertyChange(() => IsComputing);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Computing the test
        /// </summary>
        /// <returns></returns>
        public async virtual Task Compute()
        {

        }

        #endregion
    }
}
