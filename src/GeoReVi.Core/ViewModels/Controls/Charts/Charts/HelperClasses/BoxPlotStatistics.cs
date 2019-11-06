using Caliburn.Micro;
using System.Windows.Media;
using System.Linq;
using System;
using System.Collections.Generic;

namespace GeoReVi
{
    /// <summary>
    /// Boxplot statistics
    /// </summary>
    public class BoxPlotStatistics : PropertyChangedBase
    {
        #region Private members

        //Min
        private double min;
        //Max
        private double max;
        //Mean
        private double mean;
        //Median
        private double median;
        //First quartile
        private double lowerQuartile;
        //Third quartile
        private double upperQuartile;
        //Interquartile range
        private double iqr;
        //Outliers
        private List<double> outliers = new List<double>();
        //Data set
        private double[] dataset = new double[] { };
        //Name
        private string name = "Default";

        #endregion

        #region Public properties

        //Min
        public double Min
        {
            get => this.min;
            private set
            {
                this.min = value;
                NotifyOfPropertyChange(() => Min);
            }
        }
        //Max
        public double Max
        {
            get => this.max;
            private set
            {
                this.max = value;
                NotifyOfPropertyChange(() => Max);
            }
        }
        //Mean
        public double Mean
        {
            get => this.mean;
            private set
            {
                this.mean = value;
                NotifyOfPropertyChange(() => Mean);
            }
        }
        //Median
        public double Median
        {
            get => this.median;
            private set
            {
                this.median = value;
                NotifyOfPropertyChange(() => Median);
            }
        }
        //First quantile
        public double LowerQuartile
        {
            get => this.lowerQuartile;
            private set
            {
                this.lowerQuartile= value;
                NotifyOfPropertyChange(() => LowerQuartile);
            }
        }
        //Third quantile
        public double UpperQuartile
        {
            get => this.upperQuartile;
            private set
            {
                this.upperQuartile = value;
                NotifyOfPropertyChange(() => UpperQuartile);
            }
        }
        //Interquartile range
        public double InterQuartileRange
        {
            get => this.iqr;
            private set
            {
                this.iqr = value;
                NotifyOfPropertyChange(() => InterQuartileRange);
            }
        }

        //Outliers
        public List<double> Outliers
        {
            get => this.outliers;
            private set
            {
                this.outliers = value;
                NotifyOfPropertyChange(() => Outliers);
            }
        }
        //The data collection
        public double[] DataSet
        {
            get => this.dataset;
            set
            {
                this.dataset = value;
                NotifyOfPropertyChange(() => DataSet);
            }
        }
        //Name
        public string Name
        {
            get => this.name;
            set
            {
                this.name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="_dataSet"></param>
        public BoxPlotStatistics(double[] _dataSet, bool _outliersRemoved=false, double _outlierRange=1)
        {
            DataSet = _dataSet;

            if(_outliersRemoved)
                RemoveOutliers(_outlierRange);

            CalculateMean();
            CalculateMedian();
            CalculateLowerQuartile();
            CalculateUpperQuartile();
            CalculateMax();
            CalculateMin();
            Name = "n = " + _dataSet.Count().ToString();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Calculating the mean from the dataset
        /// </summary>
        private void CalculateMean()
        {
            try
            {
                Mean = DataSet.Average();
            }
            catch
            {
                Mean = 0;
            }
        }

        /// <summary>
        /// Calculating the median from the dataset
        /// </summary>
        private void CalculateMedian()
        {
            try
            {
                Median = (double)DistributionHelper.CalculateMedian(DataSet);
            }
            catch
            {
                Median = 0;
            }
        }

        /// <summary>
        /// Calculating the lower quantile from the dataset
        /// </summary>
        private void CalculateLowerQuartile()
        {
            try
            {
                LowerQuartile = (double)DistributionHelper.CalculateLowerQuartile(DataSet);
            }
            catch
            {
                LowerQuartile = 0;
            }
        }

        /// <summary>
        /// Calculating the upper quantile from the dataset
        /// </summary>
        private void CalculateUpperQuartile()
        {
            try
            {
                UpperQuartile = (double)DistributionHelper.CalculateUpperQuartile(DataSet);
            }
            catch
            {
                UpperQuartile = 0;
            }
        }

        /// <summary>
        /// Calculating the InterQuartileRange
        /// </summary>
        private void CalculateInterQuartileRange()
        {
            try
            {
                InterQuartileRange = UpperQuartile - LowerQuartile;
            }
            catch
            {
                InterQuartileRange = 0;
            }
        }

        /// <summary>
        /// Removing outliers in a recursive function
        /// </summary>
        private void RemoveOutliers(double _outlierRange = 1.5)
        {
            if (DataSet.Count() == 0)
            {
                return;
            }

            // Calculate Mean value of the set.
            double meanValue = DataSet.Average();
            // Find the Object whose value is farthest from the Mean.
            double objectFarthestFromMean = DataSet.OrderByDescending(o => Math.Abs(o - meanValue)).First();
            //Lower and upper quartiles and the inter quartile range
            double lowerQuart = (double)DistributionHelper.CalculateLowerQuartile(DataSet);
            double upperQuart = (double)DistributionHelper.CalculateUpperQuartile(DataSet);
            double iqr = upperQuart - lowerQuart;

            // Remove Object if its value is more than 1.5*IQR from the Mean.
            double minValue = meanValue - (_outlierRange * iqr);
            double maxValue = meanValue + (_outlierRange * iqr);

            //Removing the object from the data set and adding it to the outliers
            if ((objectFarthestFromMean < minValue) || (objectFarthestFromMean > maxValue) && Outliers.Count<5)
            {
                DataSet = DataSet.Where(x => x != objectFarthestFromMean).ToArray();
                Outliers.Add(objectFarthestFromMean);

                //Calling the method again
                RemoveOutliers(_outlierRange);
            }
        }

        /// <summary>
        /// Calculating the maximum value of the dataset
        /// </summary>
        private void CalculateMax()
        {
            try
            {
                Max = DataSet.Max();
            }
            catch
            {
                Max = 0;
            }
        }

        /// <summary>
        /// Calculating the minimum value of the dataset
        /// </summary>
        private void CalculateMin()
        {
            try
            {
                Min = DataSet.Min();
            }
            catch
            {
                Min = 0;
            }
        }

        #endregion
    }
}
