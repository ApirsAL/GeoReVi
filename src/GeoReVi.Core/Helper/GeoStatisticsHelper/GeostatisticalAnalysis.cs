using Caliburn.Micro;

namespace GeoReVi
{
    /// <summary>
    /// An abstract class that provides the basic properties and methods for multivariate statistical analysis
    /// </summary>
    public abstract class GeostatisticalAnalysis : PropertyChangedBase, IGeostatisticalAnalysis
    {
        /// <summary>
        /// Checks whether a computation takes place ATM or not
        /// </summary>
        private bool isComputing = false;
        public bool IsComputing
        {
            get => this.isComputing;
            set
            {
                this.isComputing = value;
                NotifyOfPropertyChange(() => IsComputing);
            }
        }

        /// <summary>
        /// The status of the calculation
        /// </summary>
        private double status = 0;
        public double Status
        {
            get => this.status;
            set
            {
                this.status = value;
                NotifyOfPropertyChange(() => Status);
            }
        }

        /// <summary>
        /// The computation time in seconds
        /// </summary>
        private double computationTime = 0;
        public double ComputationTime
        {
            get => this.computationTime;
            set
            {
                this.computationTime = value;
                NotifyOfPropertyChange(() => ComputationTime);
            }
        }
    }
}
