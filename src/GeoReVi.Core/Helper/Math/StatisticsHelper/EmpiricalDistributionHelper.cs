using Caliburn.Micro;
using System;

namespace GeoReVi
{
    public class EmpiricalDistributionHelper<T> : PropertyChangedBase where T : class, new()
    {
        private BindableCollection<T> _a;

        /// <summary>
        /// The type of distribution
        /// </summary>
        private DistributionType distributionType = DistributionType.Normal;
        public DistributionType DistributionType
        {
            get => this.distributionType;
            set
            {
                this.distributionType = value;

                if (_a != null)
                    if (_a.Count != 0)
                        _a.UpdateChart();

                NotifyOfPropertyChange(() => DistributionType);
            }
        }

        /// <summary>
        /// Check if distribution should be calculated
        /// </summary>
        private bool calculateDistribution = false;
        public bool CalculateDistribution
        {
            get => this.calculateDistribution;
            set
            {
                this.calculateDistribution = value;

                if (_a != null)
                    if (_a.Count != 0)
                        _a.UpdateChart();

                NotifyOfPropertyChange(() => CalculateDistribution);
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public EmpiricalDistributionHelper()
        {

        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public EmpiricalDistributionHelper(BindableCollection<T> a)
        {
            _a = a;
        }

        /// <summary>
        /// Returning a normal distribution value
        /// </summary>
        /// <param name="mean"></param>
        /// <param name="standardDeviation"></param>
        /// <param name="f">Y value of a distribution</param>
        /// <returns></returns>
        public double GetDistributionValue(double mean, double standardDeviation, double f)
        {
            return 1 / (standardDeviation * Math.Sqrt(2 * Math.PI)) * Math.Exp(-0.5 * (Math.Pow((f - mean) / standardDeviation, 2)));
        }


    }
}
