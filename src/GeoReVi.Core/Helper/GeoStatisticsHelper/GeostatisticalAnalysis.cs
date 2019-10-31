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
    }
}
