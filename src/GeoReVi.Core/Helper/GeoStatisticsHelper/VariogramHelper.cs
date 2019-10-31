using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoReVi
{
    /// <summary>
    /// Static class for variogram calculations
    /// </summary>
    public class VariogramHelper : PropertyChangedBase
    {
        private int numberBins = 10;
        public int NumberBins
        {
            get => this.numberBins;
            set
            {
                if (value > 0 && value < 100)
                    this.numberBins = value;
                else
                    this.numberBins = 10;

                NotifyOfPropertyChange(() => NumberBins);
            }
        }

        /// <summary>
        /// The squared differences of the model to the experimental values
        /// </summary>
        private double squaredDifferences = 0;
        public double SquaredDifferences
        {
            get => this.squaredDifferences;
            set
            {
                this.squaredDifferences = value;
                NotifyOfPropertyChange(() => SquaredDifferences);
            }
        }

        /// <summary>
        /// Nugget effect
        /// </summary>
        private double nugget = 0;
        public double Nugget
        {
            get => this.nugget;
            set
            {
                this.nugget = value;
                NotifyOfPropertyChange(() => Nugget);
            }
        }

        /// <summary>
        /// Sill
        /// </summary>
        private double sill = 1;
        public double Sill
        {
            get => this.sill;
            set
            {
                this.sill = value;
                NotifyOfPropertyChange(() => Sill);
            }
        }

        /// <summary>
        /// Sill
        /// </summary>
        private double range = 1;
        public double Range
        {
            get => this.range;
            set
            {
                this.range = value;
                NotifyOfPropertyChange(() => Range);
            }
        }

        /// <summary>
        /// The variogram model
        /// </summary>
        private VariogramModel model = VariogramModel.Spherical;
        public VariogramModel Model
        {
            get => this.model;
            set
            {
                this.model = value;
                NotifyOfPropertyChange(() => Model);
            }
        }

        /// <summary>
        /// Check whether the variogram should be an indicator or not
        /// </summary>
        private bool isIndicator = false;
        public bool IsIndicator
        {
            get => this.isIndicator;
            set
            {
                this.isIndicator = value;
                NotifyOfPropertyChange(() => IsIndicator);
            }
        }

        /// <summary>
        /// Data set used for the analysis
        /// </summary>
        public BindableCollection<LocationTimeValue> DataSet
        {
            get;
            set;
        }

        /// <summary>
        /// Calculated experimental variogram
        /// </summary>
        public List<XY> Variogram
        {
            get;
            private set;
        }

        /// <summary>
        /// Variogram model points
        /// </summary>
        public List<XY> VariogramModelPoints
        {
            get;
            private set;
        }

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public VariogramHelper()
        {
        }

        /// <summary>
        /// Specific constructor
        /// </summary>
        /// <param name="dataSet"></param>
        public VariogramHelper(BindableCollection<LocationTimeValue> dataSet)
        {
            DataSet = dataSet;
        }

        #endregion

        /// <summary>
        /// Calculates the empirical variogram based on a LocationValue distribution
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="steps"></param>
        /// <returns></returns>
        public void ComputeExperimentalVariogram()
        {
            //Initializing the list of variogram values
            List<XY> variogramValues = new List<XY>();
            for (int a = 0; a < NumberBins+1; a++)
                variogramValues.Add(new XY());

            if (DataSet != null)
            {
                if (DataSet.Count < 1)
                {
                    Variogram = variogramValues;
                }
            }
            else
            {
                Variogram = variogramValues;
            }

            List<XY> valuesDistance = new List<XY>();
            DataSet = new BindableCollection<LocationTimeValue>(DataSet.OrderBy(x => x.X).OrderBy(x => x.Y).OrderBy(x => x.Z));

            //Subdividing into bins
            valuesDistance = GeographyHelper.DistanceMatrix(DataSet);

            double[] bins = DistributionHelper.Subdivide(valuesDistance.Select(x => x.Y).ToArray(), NumberBins);
            double range = bins[1] - bins[0];

            //Getting all values of a bin
            for (int i = 0; i < bins.Count(); i++)
            {
                variogramValues[i].X = bins[i];

                List<XY> valuesInRange = new List<XY>();

                valuesInRange = new List<XY>((from valueDistance in valuesDistance
                                              where valueDistance.Y >= bins[i] - range && valueDistance.Y <= bins[i]
                                              select valueDistance).ToList());

                int n = valuesInRange.Count();
                double val = 0;

                for (int j = 0; j < valuesInRange.Count(); j++)
                {
                    val += Math.Pow(valuesInRange[j].X, 2);

                }

                variogramValues[i].Y = val / (2 * n) == Double.NaN ? 0 : val / (2 * n);
            }

            Variogram = variogramValues;
        }

        /// <summary>
        /// Calculates a variogram model
        /// </summary>
        /// <param name="variogram"></param>
        /// <returns></returns>
        public void OptimizeVariogramModel()
        {
            try
            {
                //Optimizing the variogram model
                double xmin = Variogram.Min(x => x.X);
                double xmax = Variogram.Max(x => x.X);
                double yNugget = Variogram.Where(x => x.X == 0).Select(x => x.Y).FirstOrDefault();
                double yMean = Variogram.Select(x => x.Y).Average();

                Nugget = yNugget;
                Sill = yMean - yNugget;
                double previousAverageDeviation = 0;

                List<double> gradients = new List<double>();

                for (int i = 0; i < Variogram.Count;i++)
                {
                    if(i!=0)
                    {
                        double diff = Variogram[i].Y - Variogram[i - 1].Y;
                        gradients.Add(diff);

                        double actualAverageDeviation = gradients.GetRange(0, i).Average() - gradients.GetRange(0, i).Average();

                        if (previousAverageDeviation == 0)
                            previousAverageDeviation = actualAverageDeviation;
                        else
                        {
                            if (actualAverageDeviation / previousAverageDeviation <= 10)
                            {
                                Range = Variogram[i].X;
                                break;
                            }
                            else
                            {
                                if (i == Variogram.Count)
                                    Range = Variogram.Select(x => x.X).Average();
                            }
                        }
                    }
                    else
                        continue;
                }
            }
            catch
            {
                return;
            }
            
        }
        /// <summary>
        /// Calculates a variogram model
        /// </summary>
        /// <param name="variogram"></param>
        /// <returns></returns>
        public void CalculateVariogramModel()
        {
            double xmin = Variogram.Min(x => x.X);
            double xmax = Variogram.Max(x => x.X);
            double step = (xmax - xmin) / 100;

            List<XY> ret = new List<XY>();

            for (double i = 0; i < xmax; i += step)
            {
                switch (Model)
                {
                    case GeoReVi.VariogramModel.Spherical:
                        if (i == 0)
                            ret.Add(new XY { X = i, Y = Nugget });
                        else if (i > 0 && i <= range)
                            ret.Add(new XY { X = i, Y = Nugget + Sill * (1.5 * (i / Range) - 0.5 * (Math.Pow(i, 3) / Math.Pow(Range, 3))) });
                        else
                            ret.Add(new XY { X = i, Y = Nugget + Sill });
                        break;
                    case GeoReVi.VariogramModel.Exponential:
                        ret.Add(new XY { X = i, Y = Nugget + Sill * (1 - Math.Exp(-1 * Math.Abs(i / Range))) });
                        break;
                    case GeoReVi.VariogramModel.Gaussian:
                        ret.Add(new XY { X = i, Y = Nugget + Sill * (1 - Math.Exp(-1 * Math.Pow(i, 2) / Math.Pow(Range, 2))) });
                        break;
                    case GeoReVi.VariogramModel.Linear:
                        ret.Add(new XY { X = i, Y = Nugget + Sill * i });
                        break;
                    case GeoReVi.VariogramModel.Power:
                        ret.Add(new XY { X = i, Y = Nugget + Sill * Math.Pow(i, 2) });
                        break;
                    case GeoReVi.VariogramModel.Kardinal_Sinus:
                        ret.Add(new XY { X = i, Y = Nugget + Sill * (1-(Math.Sin(i/ Range))/(i/ Range)) });
                        break;
                }
            }

            VariogramModelPoints = ret;

            //Calculating the model error
            double error = 0;

            Variogram.ForEach(x =>
                {
                    if(!Double.IsNaN(x.Y))
                        error += Math.Pow(x.Y - CalculateSemivariance(new LocationTimeValue(x.X, 0, 0), new LocationTimeValue()), 2);
                }
            );

            SquaredDifferences = error;
        }

        /// <summary>
        /// Calculating the variance between to points
        /// </summary>
        /// <param name="point1">Point 1</param>
        /// <param name="point2">Point 2</param>
        /// <returns></returns>
        public double CalculateSemivariance(LocationTimeValue point1, LocationTimeValue point2)
        {
            double semivariance = 0;
            double distance = GeographyHelper.EuclideanDistance(point1.X - point2.X, point1.Y - point2.Y, point1.Z - point2.Z);

            switch (Model)
            {
                case GeoReVi.VariogramModel.Spherical:
                    if (distance == 0)
                        semivariance = Nugget;
                    else if (distance > 0 && distance <= range)
                        semivariance = Nugget + Sill * (1.5 * (distance / Range) - 0.5 * (Math.Pow(distance, 3) / Math.Pow(Range, 3)));
                    else
                        semivariance = Nugget + Sill;
                    break;
                case GeoReVi.VariogramModel.Exponential:
                    if (distance == 0)
                        semivariance = Nugget;
                    else if(distance > 0 && distance <= range)
                        semivariance = Nugget + Sill * (1 - Math.Exp(-1 * Math.Abs(distance / Range)));
                    else
                        semivariance = Nugget + Sill;
                    break;
                case GeoReVi.VariogramModel.Gaussian:
                    if (distance == 0)
                        semivariance = Nugget;
                    else if (distance > 0)
                        semivariance = Nugget + Sill * (1 - Math.Exp(-1 * Math.Pow(distance, 2) / Math.Pow(Range, 2)));
                    else
                        semivariance = Nugget + Sill;
                    break;
                case GeoReVi.VariogramModel.Linear:
                    semivariance = Nugget + Sill * distance;
                    break;
                case GeoReVi.VariogramModel.Power:
                    semivariance = Nugget + Sill * Math.Pow(distance, 2);
                    break;
                case GeoReVi.VariogramModel.Kardinal_Sinus:
                    if (distance > 0 && distance <= range)
                        semivariance = Nugget + Sill * (1 - (Math.Sin(distance / Range)) / (distance / Range));
                    else
                        semivariance = Nugget+Sill;
                    break;
            }

            return semivariance;

        }

        /// <summary>
        /// Calculating the covariance between to points
        /// </summary>
        /// <param name="point1">Point 1</param>
        /// <param name="point2">Point 2</param>
        /// <returns></returns>
        public double CalculateCovariance(LocationTimeValue point1, LocationTimeValue point2)
        {
            return (Sill + Nugget) - CalculateSemivariance(point1, point2);
        }
    }
}
