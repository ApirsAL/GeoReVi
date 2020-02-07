using Caliburn.Micro;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GeoReVi
{
    /// <summary>
    /// A class to calculate the basic statistical measures of a data set
    /// </summary>
    public class BasicUnivariateStatisticalMeasuresHelper : PropertyChangedBase, 
        IStatisticalAnalysis, IUnivariateDataSetHolder
    {
        #region Public properties

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


        private int count = 0;
        public int Count
        {
            get => this.count;
            private set
            {
                this.count = value;
                NotifyOfPropertyChange(() => Count);
            }
        }

        private double arithmeticMean = 0;
        public double ArithmeticMean
        {
            get => this.arithmeticMean;
            private set
            {
                this.arithmeticMean = value;
                NotifyOfPropertyChange(() => ArithmeticMean);
            }
        }

        private double geometricMean = 0;
        public double GeometricMean
        {
            get => this.geometricMean;
            private set
            {
                this.geometricMean = value;
                NotifyOfPropertyChange(() => GeometricMean);
            }
        }

        private double harmonicMean = 0;
        public double HarmonicMean
        {
            get => this.harmonicMean;
            private set
            {
                this.harmonicMean = value;
                NotifyOfPropertyChange(() => HarmonicMean);
            }
        }

        private double median = 0;
        public double Median
        {
            get => this.median;
            private set
            {
                this.median = value;
                NotifyOfPropertyChange(() => Median);
            }
        }

        private double sampleStandardDeviation = 0;
        public double SampleStandardDeviation
        {
            get => this.sampleStandardDeviation;
            private set
            {
                this.sampleStandardDeviation = value;
                NotifyOfPropertyChange(() => SampleStandardDeviation);
            }
        }

        private double sampleVariance = 0;
        public double SampleVariance
        {
            get => this.sampleVariance;
            private set
            {
                this.sampleVariance = value;
                NotifyOfPropertyChange(() => SampleVariance);
            }
        }


        private double maximum = 0;
        public double Maximum
        {
            get => this.maximum;
            private set
            {
                this.maximum = value;
                NotifyOfPropertyChange(() => Maximum);
            }
        }

        private double minimum = 0;
        public double Minimum
        {
            get => this.minimum;
            private set
            {
                this.minimum = value;
                NotifyOfPropertyChange(() => Minimum);
            }
        }

        private double range = 0;
        public double Range
        {
            get => this.range;
            private set
            {
                this.range = value;
                NotifyOfPropertyChange(() => Range);
            }
        }

        private double skewness = 0;
        public double Skewness
        {
            get => this.skewness;
            private set
            {
                this.skewness = value;
                NotifyOfPropertyChange(() => Skewness);
            }
        }

        private double kurtosis = 0;
        public double Kurtosis
        {
            get => this.kurtosis;
            private set
            {
                this.kurtosis = value;
                NotifyOfPropertyChange(() => Kurtosis);
            }
        }

        private bool isComputing;
        public bool IsComputing { get => isComputing; set => isComputing = value; }

        #endregion

        #region Constructor

        public BasicUnivariateStatisticalMeasuresHelper()
        {

        }

        public BasicUnivariateStatisticalMeasuresHelper(double[] _dataSet)
        {
            DataSet = _dataSet;
            Compute();
        }


        #endregion

        #region Public methods

        /// <summary>
        /// Computing the measures
        /// </summary>
        public async Task Compute()
        {
            CommandHelper ch = new CommandHelper();

            await ch.RunBackgroundWorkerWithFlagHelperAsync(() => IsComputing, async () =>
            {
                try
                {
                    Count = dataSet.Where(x => x != 0 && !Double.IsNaN(x)).Count();
                    ArithmeticMean = ComputeArithmeticMean(DataSet);
                    GeometricMean = ComputeGeometricMean(DataSet);
                    HarmonicMean = ComputeHarmonicMean(DataSet);
                    Maximum = DataSet.Where(x => x != 0 && !Double.IsNaN(x)).Max();
                    Minimum = DataSet.Where(x => x != 0 && !Double.IsNaN(x)).Min();
                    SampleStandardDeviation = DataSet.StdDev();
                    SampleVariance = Math.Pow(SampleStandardDeviation, 2);
                    Range = Maximum - Minimum;
                    Median = ComputeMedian(DataSet);
                    Skewness = ComputeSkewness(DataSet);
                    Kurtosis = ComputeKurtosis(DataSet);
                }
                catch
                {
                    return;
                }
            });
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Computes the arithmetic mean
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        public double ComputeArithmeticMean(double[] dataSet)
        {
            return dataSet.AsParallel().Average();
        }

        /// <summary>
        /// Computes the geometric mean
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        public double ComputeGeometricMean(double[] dataSet)
        {
            return dataSet.GeometricMean();
        }

        /// <summary>
        /// Computes the harmonic mean
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        public double ComputeHarmonicMean(double[] dataSet)
        {
            return dataSet.HarmonicMean();
        }

        /// <summary>
        /// Calculating the median from the dataset
        /// </summary>
        private double ComputeMedian(double[] dataSet)
        {
            return (double)DistributionHelper.CalculateMedian(dataSet);
        }

        /// <summary>
        /// Calculating the skewness of a data set
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        private double ComputeSkewness(double[] dataSet)
        {
            double sum = dataSet.Where(x => x!=0 && !Double.IsNaN(x)).Sum(x => Math.Pow((x - ArithmeticMean) / SampleStandardDeviation, 3));
            double diff = 1 / Convert.ToDouble(Count);
            return diff * sum;
        }

        /// <summary>
        /// Calculating the skewness of a data set
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        private double ComputeKurtosis(double[] dataSet)
        {
            double sum = dataSet.Sum(x => Math.Pow((x - ArithmeticMean) / SampleStandardDeviation, 4));
            double diff = 1 / Convert.ToDouble(Count);
            return diff * sum;
        }

        /// <summary>
        /// Calculating the nth percentile
        /// </summary>
        /// <param name="percentile"></param>
        /// <returns></returns>
        public double ComputePercentile(int percentile)
        {
            return (double)DistributionHelper.CalculateNthPercentile(dataSet, percentile);
        }

        #endregion
    }
}
