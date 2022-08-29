using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        /// Range
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
        /// Search angle around Z axis
        /// </summary>
        private double azimuth = 0;
        public double Azimuth
        {
            get => this.azimuth;
            set
            {
                this.azimuth = value;
                NotifyOfPropertyChange(() => Azimuth);
            }
        }

        /// <summary>
        /// Search angle around X axis
        /// </summary>
        private double dip = 0;
        public double Dip
        {
            get => this.dip;
            set
            {
                this.dip = value;
                NotifyOfPropertyChange(() => Dip);
            }
        }

        /// <summary>
        /// Search angle around Y axis
        /// </summary>
        private double plunge = 0;
        public double Plunge
        {
            get => this.plunge;
            set
            {
                this.plunge = value;
                NotifyOfPropertyChange(() => Plunge);
            }
        }


        /// <summary>
        /// Range in x direction
        /// </summary>
        private double rangeX = 9999;
        public double RangeX
        {
            get => this.rangeX;
            set
            {
                this.rangeX = value;
                NotifyOfPropertyChange(() => RangeX);
            }
        }

        /// <summary>
        /// Range in y direction
        /// </summary>
        private double rangeY = 9999;
        public double RangeY
        {
            get => this.rangeY;
            set
            {
                this.rangeY = value;
                NotifyOfPropertyChange(() => RangeY);
            }
        }

        /// <summary>
        /// Range in z direction
        /// </summary>
        private double rangeZ = 9999;
        public double RangeZ
        {
            get => this.rangeZ;
            set
            {
                this.rangeZ = value;
                NotifyOfPropertyChange(() => RangeZ);
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
        /// Checks whether the variogram should be optimized or not
        /// </summary>
        private bool optimize = false;
        public bool Optimize
        {
            get => this.optimize;
            set
            {
                this.optimize = value;
                NotifyOfPropertyChange(() => Optimize);
            }
        }

        /// <summary>
        /// Calculates the number of iterations
        /// </summary>
        private int numberOfIterations = 1000;
        public int NumberOfIterations
        {
            get => this.numberOfIterations;
            set
            {
                this.numberOfIterations = value;
                NotifyOfPropertyChange(() => NumberOfIterations);
            }
        }

        /// <summary>
        /// The maximum error of the approximation
        /// </summary>
        private double maximumError = 1.0;
        public double MaximumError
        {
            get => this.maximumError;
            set
            {
                this.maximumError = value;
                NotifyOfPropertyChange(() => MaximumError);
            }
        }

        /// <summary>
        /// Data sets used for the variogram analysis
        /// </summary>
        private BindableCollection<Mesh> dataSets = new BindableCollection<Mesh>();
        public BindableCollection<Mesh> DataSets
        {
            get => this.dataSets;
            set
            {
                this.dataSets = value;
                NotifyOfPropertyChange(() => DataSets);
            }
        }

        /// <summary>
        /// Calculated experimental variograms
        /// </summary>
        public List<List<XY>> Variogram
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
        public VariogramHelper(BindableCollection<Mesh> dataSet)
        {
            DataSets = dataSet;
        }

        #endregion

        /// <summary>
        /// Calculates the empirical variogram based on a LocationValue distribution
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="steps"></param>
        /// <returns></returns>
        public async Task ComputeExperimentalVariogram()
        {

            Variogram = new List<List<XY>>();

            for (int k = 0; k < DataSets.Count(); k++)
            {
                var dataSets = DataSets[k].Vertices.ToList();

                if(dataSets.Count >= 5000)
                {
                    dataSets.Shuffle();
                    dataSets = dataSets.Take(5000).ToList();
                }

                //Initializing the list of variogram values
                List<XY> variogramValues = new List<XY>();

                for (int a = 0; a < NumberBins + 1; a++)
                    variogramValues.Add(new XY());

                var values = await GeographyHelper.GetDifferences(dataSets, this);
                var distances = await GeographyHelper.GetDistances(dataSets, this);

                // Subdividing into bins
                double[] bins = DistributionHelper.Subdivide(distances, NumberBins);

                double range = bins[1] - bins[0];

                //Getting all values of a bin
                for (int i = 0; i < bins.Count(); i++)
                {
                    variogramValues[i].X = bins[i];

                    //Selecting the indices of all values-pairs which are located in a particular bin range
                    int[] valuesInRange = distances.Select((value, index) => new { index, Value = value })
                        .Where(x => x.Value >= 0 && x.Value >= bins[i] && x.Value <= bins[i] + range)
                        .Select(x => x.index)
                        .ToArray();

                    int n = valuesInRange.Count();

                    double val = 0;

                    for (int j = 0; j < valuesInRange.Count(); j++)
                    {
                        try
                        {
                            if (valuesInRange[j] <= values.Count())
                                    val += Math.Pow(values[valuesInRange[j]], 2);
                            else
                            {
                                n -= 1;
                                continue;
                            }
                        }
                        catch
                        {
                            n -= 1;
                            continue;
                        }
                    }

                    variogramValues[i].Y = val / (2 * n) == Double.NaN ? 0 : val / (2 * n);
                }

                Variogram.Add(variogramValues);
            }
        }

        /// <summary>
        /// Calculates a variogram model
        /// </summary>
        /// <param name="variogram"></param>
        /// <returns></returns>
        public void CalculateVariogramModel()
        {
            double xmin = Variogram.Min(x => x.Min(y => y.X));
            double xmax = Variogram.Max(x => x.Max(y => y.X));
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
                        ret.Add(new XY { X = i, Y = Nugget + Sill * (1 - (Math.Sin(i / Range)) / (i / Range)) });
                        break;
                }
            }

            VariogramModelPoints = ret;

            //Calculating the model error
            double error = 0;
            int itters = 0;

            Variogram.ForEach(x =>
                {
                    x.ForEach(y =>
                    {
                        if (!Double.IsNaN(y.Y))
                            error += Math.Pow(y.Y - CalculateSemivariance(new LocationTimeValue(y.X, 0, 0), new LocationTimeValue()), 2);
                    });
                }
            );

            SquaredDifferences = error / Convert.ToDouble(Variogram.Sum(x => x.Count()));
            double learningRate = 0.01;

            if (Optimize)
                while (itters < NumberOfIterations && SquaredDifferences > MaximumError)
                {
                    double previousNugget = Nugget;
                    double previousSill = Sill;
                    double previousRange = Range;

                    error = 0;

                    Variogram.ForEach(x =>
                    {
                        x.ForEach(y =>
                        {
                            if (!Double.IsNaN(y.Y))
                                error += Math.Pow(y.Y - CalculateSemivariance(new LocationTimeValue(y.X, 0, 0), new LocationTimeValue()), 2);
                        });
                    });

                    SquaredDifferences = error / Convert.ToDouble(Variogram.Sum(x => x.Count())); ;
                }

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
                    else if (distance > 0 && distance <= range)
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
                        semivariance = Nugget + Sill;
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

        /// <summary>
        /// Getting the Variance-Covariance-Matrix of a point set
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public double[,] GetVarianceCovarianceMatrix(List<LocationTimeValue> points)
        {
            double[,] ret = new double[points.Count(), points.Count()];

            try
            {
                //Calculating the semivariance matrix based on the variogram model
                for (int i = 0; i < points.Count(); i++)
                {
                    for (int k = 0; k < points.Count(); k++)
                    {
                        ret[i, k] = CalculateCovariance(points[i], points[k]);
                    }
                }
            }
            catch
            {

            }

            return ret;
        }
    }
}
