using Accord.Statistics.Distributions.Univariate;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GeoReVi
{
    /// <summary>
    /// Helper class for calculating distribution parameters
    /// </summary>
    public static class DistributionHelper
    {
        private static Random rnd = new Random();
        private static double? _spareValue = null;

        /// <summary>
        /// A static method to subdivide a distribution into bins
        /// </summary>
        /// <param name="array"></param>
        /// <param name="countBins"></param>
        /// <returns></returns>
        public static double[] Subdivide(double[] array, int countBins = 0, int count = 1)
        {
            //Getting meta properties of the array
            int length = array.Length;

            if (length == 0)
                return new double[] { };

            if (count == 0)
                return new double[] { };

            double max = array.Max();
            double min = array.Min();
            double mean = array.Sum() / Convert.ToDouble(length);

            if (min == max)
                return new double[] { };

            double[] ret = new double[] { };

            var binSize = (max - min) / countBins;

            if (min >= 0 && max > 0)
            {
                double step = (max - min) / countBins;

                if (ret.Count() == 0)
                {
                    Array.Resize(ref ret, countBins + 1);

                    for (int i = 0; i < countBins + 1; i++)
                    {
                        if (i == 0)
                        {
                            ret[i] = min;
                        }
                        else
                        {
                            ret[i] = ret[i - 1] + step;
                        }
                    }
                }
            }
            else
            {
                //probably normal distribution
                if ((min / max) > (1E-20) || (min / max) < 1E-20)
                {
                    double step = Math.Round((max - min) / countBins, count + 1);

                    if (step != 0)
                    {
                        min = Math.Round(min, count + 1);
                        max = Math.Round(max, count + 1);
                    }
                    else
                    {
                        ret = Subdivide(array, countBins + 1, count += 1);
                    }

                    if (ret.Count() == 0)
                    {
                        Array.Resize(ref ret, countBins + 1);

                        for (int i = 0; i < countBins + 1; i++)
                        {
                            if (i == 0)
                            {
                                ret[i] = min;
                            }
                            else
                            {
                                ret[i] = ret[i - 1] + step;
                            }
                        }
                    }
                }

                else if (min / max == 0)
                {
                    double step = Math.Round((max - min) / countBins, count + 1);

                    if (step != 0)
                    {
                        min = Math.Round(min, count + 1);
                        max = Math.Round(max, count + 1);
                    }
                    else
                    {
                        ret = Subdivide(array, countBins, count += 1);
                    }

                    if (ret.Count() == 0)
                    {
                        Array.Resize(ref ret, countBins + 1);

                        for (int i = 0; i < countBins; i++)
                        {
                            if (i == 0)
                            {
                                ret[i] = min;
                            }
                            else
                            {
                                ret[i] = ret[i - 1] + step;
                            }
                        }
                    }
                }
            }

            return ret;
        }

        //Counting occurences of doubles withing a range of bins
        public static int[] Counts(double[] distribution, double[] bins)
        {
            int[] y = new int[bins.Count()];

            //Counting based on the bin
            for (int i = 0; i < distribution.Count(); i++)
            {
                for (int j = 0; j < bins.Count(); j++)
                {
                    if (j == bins.Count() - 1)
                    {
                        if (distribution[i] > bins[j])
                        {
                            y[j] += 1;
                        }
                    }
                    else if (j == 0)
                    {
                        if (distribution[i] >= bins[j] && distribution[i] <= bins[j + 1])
                        {
                            y[j] += 1;
                        }
                    }
                    else if (distribution[i] > bins[j] && distribution[i] <= bins[j + 1])
                    {
                        y[j] += 1;
                    }
                }
            }

            return y;
        }

        /// <summary>
        /// Calculating the median from the dataset
        /// </summary>
        public static double? CalculateMedian(double[] distribution)
        {
            int count = distribution.Count();
            var orderedDataSet = distribution.OrderBy(p => p).ToArray();
            if (count > 0)
                return (orderedDataSet.ElementAt(count / 2) + orderedDataSet.ElementAt((count - 1) / 2)) / 2;
            else
                return 0;
        }

        /// <summary>
        /// Calculating the lower quantile from the dataset
        /// </summary>
        public static double? CalculateLowerQuartile(double[] distribution)
        {
            double? b = CalculateMedian(distribution);

            var a = distribution.Where(x => x < b).ToArray();

            if (a.Count() > 0)
                return a.Average();

            else return 0;
        }

        /// <summary>
        /// Calculating the upper quantile from the dataset
        /// </summary>
        public static double? CalculateUpperQuartile(double[] distribution)
        {
            double? b = CalculateMedian(distribution);

            var a = distribution.Where(x => x > b).ToArray();

            if (a.Count() > 0)
                return a.Average();

            return 0;
        }

        /// <summary>
        /// Calculating the median from the dataset
        /// </summary>
        public static double? CalculateStandardDeviation(double[] distribution)
        {
            try
            {
                int count = distribution.Count();

                var orderedDataSet = distribution.OrderBy(p => p).ToArray();
                if (count > 0)
                {
                    double av = distribution.Average();
                    double dissimilarity = 0;

                    for (int i = 0; i < distribution.Count(); i++)
                    {
                        dissimilarity += Math.Pow(distribution[i] - av, 2);
                    }

                    return Math.Sqrt(dissimilarity / (count - 1));

                }
                else
                    return 0;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Calculating the nth percentile
        /// </summary>
        /// <param name="distribution">The 1D distribution</param>
        /// <param name="percentile">An integer percentile with 0<percentile<=100</param>
        /// <returns></returns>
        public static double? CalculateNthPercentile(double[] distribution, int percentile)
        {
            if (percentile <= 0 || percentile > 100)
                return 0;

            double perc = 0;
            double index = 0;

            try
            {
                distribution = distribution.OrderBy(x => x).ToArray();

                index = Convert.ToDouble(percentile) / 100 * distribution.Count();

                if ((index % 1) == 0)
                {
                    double sum = 0;

                    perc = distribution[Convert.ToInt32(index)];
                }
                else
                {
                    index = Math.Round(index, 0, MidpointRounding.AwayFromZero);

                    double perc1 = distribution[Convert.ToInt32(index)];
                    double perc2 = distribution[Convert.ToInt32(index - 1)];

                    perc = (perc1 + perc2) / 2;
                }
            }
            catch
            {
                return 0;
            }

            return perc;
        }

        /// <summary>
        /// Calculates the coefficient of variation
        /// </summary>
        /// <param name="distribution"></param>
        /// <returns></returns>
        public static double? CalculateCoefficientOfVariation(double[] distribution)
        {
            try
            {
                if (distribution == null || distribution.Count() == 0)
                {
                    double sta = (double)DistributionHelper.CalculateStandardDeviation(distribution);
                    double av = distribution.Average();

                    return sta / av;
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Calculates the DykstraParsonCoefficient
        /// </summary>
        /// <param name="distribution"></param>
        /// <returns></returns>
        public static double? CalculateDykstraParsonCoefficient(double[] distribution)
        {
            try
            {
                if (distribution == null || distribution.Count() == 0)
                {
                    double perc84 = (double)DistributionHelper.CalculateNthPercentile(distribution, 84);
                    double med = (double)DistributionHelper.CalculateMedian(distribution);

                    return (med - perc84) / med;
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                return 0;
            }

        }

        /// <summary>
        /// Reproduces an initial distribution of a variable according to the empirical histogram
        /// </summary>
        /// <param name="mesh">Mesh</param>
        /// <param name="array">Value distribution</param>
        /// <param name="countBins">Count of bins</param>
        /// <returns></returns>
        public static void ReproduceDistribution(this Mesh mesh, double[] array, int countBins = 10)
        {
            try
            {
                //Subdividing into bins
                double[] bins = DistributionHelper.Subdivide(array, countBins);

                //Getting value counts for each bin
                int[] counts = DistributionHelper.Counts(array, bins);

                List<double> frequencies = new List<double>();
                for (int i = 0; i < counts.Length; i++)
                {
                    frequencies.Add((double)counts[i] / (double)array.Length);
                    counts[i] = Convert.ToInt32(Math.Round(frequencies[i] * mesh.Vertices.Count()));
                }

                //Creating a list of vertex indices
                List<int> indices = new List<int>();
                for (int i = 0; i < mesh.Vertices.Count(); i++)
                    indices.Add(i);

                //Shuffling the list for random access
                indices.Shuffle();

                //Assigning a random value in the range of the selected bin to each vertex
                for (int i = 0; i < bins.Length - 1; i++)
                {
                    List<int> verticesIndices = indices.Take(counts[i]).ToList();

                    indices.RemoveRange(0, counts[i]);

                    for (int j = 0; j < counts[i]; j++)
                    {
                        double min = bins[i];
                        double max = bins[i + 1];
                        double r1 = Convert.ToDouble(rnd.NextDouble() * (max - min) + min);
                        mesh.Vertices[verticesIndices[j]].Value[0] = r1;
                    }
                }

                double mean = array.Average();

                //Assigning not-assigned values
                foreach (int index in indices)
                {
                    mesh.Vertices[index].Value[0] = mean;
                }

            }
            catch
            {

            }
        }

        /// <summary>
        /// Samples from a gaussian distribution with a mean and stdDeviation using the Box-Muller transformation
        /// </summary>
        /// <param name="mean">Mean</param>
        /// <param name="stdDev">Standard deviation</param>
        /// <returns></returns>
        public static double SampleFromGaussian(double mean, double stdDev)
        {
            double ret = 0;

            try
            {
                ret = mean + (SampleFromGaussian() * stdDev);
            }
            catch
            {

            }

            return ret;
        }

        /// <summary>
        /// Take a sample from the standard Gaussian distribution, i.e. with mean of 0 and standard deviation of 1.
        /// FROM http://svn.code.sf.net/p/sharpneat/code/branches/V2/src/SharpNeatLib/Utility/BoxMullerGaussianSampler.cs
        /// </summary>
        /// <returns>A random sample.</returns>
        public static double SampleFromGaussian()
        {
            double rand = 0;

            try
            {
                if (null != _spareValue)
                {
                    double tmp = _spareValue.Value;
                    _spareValue = null;
                    return tmp;
                }

                // Generate two new gaussian values.
                double x, y, sqr;

                int counter = 0;

                // We need a non-zero random point inside the unit circle.
                do
                {
                    x = 2.0 * rnd.NextDouble() - 1.0;
                    y = 2.0 * rnd.NextDouble() - 1.0;
                    sqr = x * x + y * y;
                    counter++;
                }
                while (counter < 10000 & (sqr > 1.0 || sqr == 0));

                // Make the Box-Muller transformation.
                double fac = Math.Sqrt(-2.0 * Math.Log(sqr) / sqr);

                _spareValue = x * fac;
                rand = y * fac;
            }
            catch
            {

            }

            return rand;
        }

        /// <summary>
        /// Scales a distribution to a range
        /// </summary>
        /// <param name="originalDistribution"></param>
        /// <param name="max"></param>
        /// <param name="min"></param>
        /// <returns></returns>
        public static double[] ScaleToArea(double[] originalDistribution, double max, double min)
        {
            double[] ret = new double[originalDistribution.Length];

            try
            {
                double zMax = originalDistribution.Max();
                double zMin = originalDistribution.Min();
                double zAverage = originalDistribution.Average();

                for (int i = 0; i < originalDistribution.Length; i++)
                {
                    ret[i] = ((originalDistribution[i] - zMin) / (zMax - zMin)) * (max - min) + min;
                }
            }
            catch
            {

            }

            return ret;
        }

        /// <summary>
        /// Performing a score transformation to the data set
        /// </summary>
        /// <param name="_data"></param>
        public static void ZScoreTransformation(ref Mesh _data)
        {
            int count = _data.Data.Rows.Count;

            if (count >= 0)
            {
                double avg = _data.Data.AsEnumerable().Average(r => r.Field<double>(0));
                double standardDeviation = _data.Data.AsEnumerable().Select(r => r.Field<double>(0)).StdDev();

                for (int i = 0; i < _data.Data.Rows.Count; i++)
                {
                    _data.Data.Rows[i][0] = ((double)_data.Data.Rows[i][0] - avg) / standardDeviation;

                    if (_data.Vertices.Count > 0)
                        _data.Vertices[i].Value[0] = (_data.Vertices[i].Value[0] - avg) / standardDeviation;
                }
            }
        }

        /// <summary>
        /// Transfers the z value to the value property
        /// </summary>
        /// <param name="_data"></param>
        public static void MakeZValue(ref Mesh _data)
        {
            //Transforming data set
            for (int i = 0; i < _data.Data.Rows.Count; i++)
            {
                _data.Data.Rows[i][0] = (double)_data.Data.Rows[i][3];
                if (_data.Vertices.Count > 0)
                    _data.Vertices[i].Value[0] = (double)_data.Vertices[i].Z;

            }
        }

        /// <summary>
        /// Produces an exponential transformation to the data set
        /// </summary>
        /// <param name="_data"></param>
        public static void ExponentialTransformation(ref Mesh _data)
        {
            //Transforming data set
            for (int i = 0; i < _data.Data.Rows.Count; i++)
            {
                _data.Data.Rows[i][0] = Math.Pow(10, (double)_data.Data.Rows[i][0]);
                if (_data.Vertices.Count > 0)
                    _data.Vertices[i].Value[0] = Math.Pow(10, (double)_data.Vertices[i].Value[0]);
            }
        }

        /// <summary>
        /// Performing a Mean transformation
        /// </summary>
        /// <param name="_data"></param>
        public static void MeanTransformation(ref Mesh _data)
        {
            int count = _data.Data.Rows.Count;

            if (count >= 0)
            {
                double avg = _data.Data.AsEnumerable().Average(r => r.Field<double>(0));
                double max = _data.Data.AsEnumerable().Select(r => r.Field<double>(0)).Max();
                double min = _data.Data.AsEnumerable().Select(r => r.Field<double>(0)).Min();

                for (int i = 0; i < _data.Data.Rows.Count; i++)
                {
                    _data.Data.Rows[i][0] = ((double)_data.Data.Rows[i][0] - avg) / (max - min);
                    if (_data.Vertices.Count > 0)
                        _data.Vertices[i].Value[0] = ((_data.Vertices[i].Value[0] - avg) / (max - min));
                }
            }
        }

        /// <summary>
        /// Performing a subtract mean transformation
        /// </summary>
        public static void SubtractMeanTransformation(ref Mesh _data)
        {
            int count = _data.Data.Rows.Count;

            if (count >= 0)
            {
                double avg = _data.Data.AsEnumerable().Average(r => r.Field<double>(0));

                for (int i = 0; i < _data.Data.Rows.Count; i++)
                {
                    _data.Data.Rows[i][0] = ((double)_data.Data.Rows[i][0] - avg);
                    if (_data.Vertices.Count > 0)
                        _data.Vertices[i].Value[0] = ((double)_data.Vertices[i].Value[0] - avg);
                }
            }
        }

        /// <summary>
        /// Performing a rescaling transformation
        /// </summary>
        /// <param name="_data"></param>
        public static void RescalingTransformation(ref Mesh _data)
        {
            int count = _data.Data.Rows.Count;

            if (count >= 0)
            {
                double avg = _data.Data.AsEnumerable().Average(r => r.Field<double>(0));
                double max = _data.Data.AsEnumerable().Select(r => r.Field<double>(0)).Max();
                double min = _data.Data.AsEnumerable().Select(r => r.Field<double>(0)).Min();

                for (int i = 0; i < _data.Data.Rows.Count; i++)
                {
                    _data.Data.Rows[i][0] = ((double)_data.Data.Rows[i][0] - min) / (max - min);
                    if (_data.Vertices.Count > 0)
                        _data.Vertices[i].Value[0] = ((double)_data.Vertices[i].Value[0] - min) / (max - min);
                }
            }
        }

        /// <summary>
        /// Performing a logarithmic transformation
        /// </summary>
        /// <param name="_data"></param>
        public static void LogarithmicTransformation(ref Mesh _data)
        {
            if (_data.Data.AsEnumerable().Where(y => y.Field<double>(0) <= 0).Count() > 0)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowError("Cannot log-transform negative values");
                return;
            }

            //Transforming data set
            for (int i = 0; i < _data.Data.Rows.Count; i++)
            {
                _data.Data.Rows[i][0] = Math.Log10((double)_data.Data.Rows[i][0]);
                if (_data.Vertices.Count > 0)
                    _data.Vertices[i].Value[0] = Math.Log10((double)_data.Vertices[i].Value[0]);
            }
        }

        /// <summary>
        /// Applying a quantile-quantile-normal score transform
        /// </summary>
        /// <param name="_data">Data set to be transformed</param>
        public static void QuantileQuantileNormalScoreTransformation(ref Mesh _data)
        {
            if (_data.Vertices.Count() == 0)
                _data.Vertices.AddRange(new BindableCollection<LocationTimeValue>(_data.Data.AsEnumerable()
                    .Select(x => new LocationTimeValue()
                    {
                        Value = new List<double>() { (x.Field<double?>(0) == -9999 || x.Field<double?>(0) == -999999 || x.Field<double?>(0) == 9999999) ? 0 : Convert.ToDouble(x.Field<double?>(0)) },
                        X = (x.Field<double?>(1) == -9999 || x.Field<double?>(1) == -999999 || x.Field<double?>(1) == 9999999) ? 0 : Convert.ToDouble(x.Field<double?>(1)),
                        Y = (x.Field<double?>(2) == -9999 || x.Field<double?>(2) == -999999 || x.Field<double?>(2) == 9999999) ? 0 : Convert.ToDouble(x.Field<double?>(2)),
                        Z = (x.Field<double?>(3) == -9999 || x.Field<double?>(3) == -999999 || x.Field<double?>(3) == 9999999) ? 0 : Convert.ToDouble(x.Field<double?>(3))
                    }).ToList()));
            else
            {
                _data.Vertices = new System.Collections.ObjectModel.ObservableCollection<LocationTimeValue>(_data.Vertices.Select(x =>
                new LocationTimeValue()
                {
                    X = x.X,
                    Y = x.Y,
                    Z = x.Z,
                    Value = x.Value,
                    MeshIndex = x.MeshIndex,
                    IsActive = x.IsActive,
                    IsDiscretized = x.IsDiscretized,
                    Brush = x.Brush,
                    Date = x.Date,
                    Name = x.Name,
                    Neighbors = x.Neighbors,
                    Geographic = x.Geographic,
                    IsExterior = x.IsExterior
                }).ToList());

            }

            _data.Vertices = new System.Collections.ObjectModel.ObservableCollection<LocationTimeValue>(_data.Vertices.OrderBy(x => x.Value[0]).ToList());

            DataView dv = _data.Data.DefaultView;
            try
            {
                dv.Sort = "Item1 asc";
            }
            catch
            {
                try
                {
                    dv.Sort = "Value asc";
                }
                catch
                {

                }
            }

            _data.Data = dv.ToTable();

            //Getting basic statistics
            double[] ccdf = CreateCumulativeRankStandardNormalDistribution(_data.Vertices.Count, -3, 3, 0, 1);

            //Assigning the new data
            for(int i = 0; i<_data.Vertices.Count();i++)
            {
                _data.Vertices[i].Value[0] = ccdf[i];
                if (_data.Vertices.Count > 0)
                    _data.Data.Rows[i][0] = ccdf[i];
            }
            
        }

        /// <summary>
        /// Simulate normal distribution and sample from it
        /// </summary>
        public static double GetFromNormalDistribution(int sampleSize, int index, double start, double end, double mean, double standardDeviation)
        {
            double x = start + ((end - start) * (double)index) / (double)sampleSize;
            return (double)((1/(Math.Sqrt(Math.Pow(standardDeviation,2)*2*Math.PI))) * Math.Exp(-Math.Pow((x - mean),2) / Math.Sqrt((2 * Math.Pow(standardDeviation,2)))));
        }

        /// <summary>
        /// Creating a rank-based cumulative standard normal distribution based 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="mean"></param>
        /// <param name="standardDeviation"></param>
        /// <returns></returns>
        public static double[] CreateCumulativeRankStandardNormalDistribution(int sampleSize, double start, double end, double mean, double standardDeviation)
        {
            double[] ret = new double[sampleSize];

            try
            {
                NormalDistribution normal = new NormalDistribution(mean, standardDeviation);
                ret[0] = start;
                ret[sampleSize-1] = end;

                for(int i = 1; i<sampleSize-1;i++)
                {
                    double x = (double)i / (double)sampleSize;
                    ret[i] = normal.InverseDistributionFunction(x);
                }
            }
            catch
            {

            }

            return ret;
        }


        /// <summary>
        /// Applying a quantile-quantile back transformation to a target distribution
        /// </summary>
        /// <param name="sampleSize"></param>
        /// <param name="_data"></param>
        public static void QuantileQuantileBackTransformation(ref Mesh _data, Mesh targetDistribution)
        {
            //Checking meshes for consistency
            CheckForNodeTableConsistency(ref _data);
            CheckForNodeTableConsistency(ref targetDistribution);

            SortByValue(ref _data);
            SortByValue(ref targetDistribution);

            //Deriving the complementary cumulative distribution function
            double[,] ccdf = CreateCumulativeRankDistribution(targetDistribution);
            
            //Interpolating the ccdf
            double[,] ccdfInterp = InterpolateCCDFLinear(ccdf, _data.Vertices.Count());

            //Transform the function according to the derived interpolated ccdf
            for(int i = 0; i< _data.Vertices.Count();i++)
            {
                _data.Vertices[i].Value[0] = ccdfInterp[i,1];
                if (_data.Vertices.Count > 0)
                    _data.Data.Rows[i][0] = ccdfInterp[i,1];
            }

            //Creating the mesh cells
            if (_data.MeshCellType == MeshCellType.Hexahedral || _data.MeshCellType == MeshCellType.Tetrahedal)
                _data.CellsFromPointCloud();
        }

        /// <summary>
        /// Creating a cumulative rank distribution
        /// </summary>
        /// <param name="targetDistribution"></param>
        /// <returns></returns>
        public static double[,] CreateCumulativeRankDistribution(Mesh targetDistribution)
        {
            double[,] ret = new double[targetDistribution.Vertices.Count,2];

            try
            {
                double start = targetDistribution.Vertices.Min(x => x.Value[0]);
                double end = targetDistribution.Vertices.Max(x => x.Value[0]);

                ret[0,0] = 0;
                ret[0,1] = start;

                ret[targetDistribution.Vertices.Count - 1,0] = 1;
                ret[targetDistribution.Vertices.Count - 1,1] = end;

                for (int i = 1; i < targetDistribution.Vertices.Count - 1; i++)
                {
                    double x = (double)i / (double)targetDistribution.Vertices.Count;
                    ret[i, 0] = x;
                    ret[i, 1] = (double)targetDistribution.Data.Rows[i][0];
                }
            }
            catch
            {

            }

            return ret;
        }

        /// <summary>
        /// Interpolates a CCDF linearly
        /// </summary>
        private static double[,] InterpolateCCDFLinear(double[,] ccdf, int targetSize)
        {
            double[,] ret = new double[targetSize,2];

            try
            {
                int length = ccdf.GetLength(0);
                //Items per lag
                int s = targetSize / (length-1);
                //Rest of the distribution
                int mod = targetSize % (length - 1);

                for(int j = 1;j < length; j++)
                {
                    for(int i = 0;i<= s; i++)
                    {
                        ret[(j-1) * s + i, 0] = ((double)i + ((double)j)*(double)s) / (double)targetSize;
                        ret[(j-1) * s + i, 1] = ccdf[j-1, 1] + (ccdf[j, 1] - ccdf[j-1, 1]) * ((double)i /(double)s);
                    }
                }

                for(int j = 0; j<mod;j++)
                {
                    ret[(s * (length-1) + j), 1] = ccdf.GetColumn(1).Max() + (ccdf[1,1] - ccdf[0, 1]) * (double)j/ (double)mod;
                }
            }
            catch
            {

            }

            return ret;
        }

        /// <summary>
        /// Checks a mesh for consistency between data table and nodes and adapts it if necessary
        /// </summary>
        /// <param name="_data"></param>
        public static void CheckForNodeTableConsistency(ref Mesh _data)
        {
            try
            {
                if (_data.Vertices.Count() == 0)
                    _data.Vertices.AddRange(new BindableCollection<LocationTimeValue>(_data.Data.AsEnumerable()
                        .Select(x => new LocationTimeValue()
                        {
                            Value = new List<double>() { (x.Field<double?>(0) == -9999 || x.Field<double?>(0) == -999999 || x.Field<double?>(0) == 9999999) ? 0 : Convert.ToDouble(x.Field<double?>(0)) },
                            X = (x.Field<double?>(1) == -9999 || x.Field<double?>(1) == -999999 || x.Field<double?>(1) == 9999999) ? 0 : Convert.ToDouble(x.Field<double?>(1)),
                            Y = (x.Field<double?>(2) == -9999 || x.Field<double?>(2) == -999999 || x.Field<double?>(2) == 9999999) ? 0 : Convert.ToDouble(x.Field<double?>(2)),
                            Z = (x.Field<double?>(3) == -9999 || x.Field<double?>(3) == -999999 || x.Field<double?>(3) == 9999999) ? 0 : Convert.ToDouble(x.Field<double?>(3))
                        }).ToList()));
                else
                {
                    _data.Vertices = new System.Collections.ObjectModel.ObservableCollection<LocationTimeValue>(_data.Vertices.Select(x =>
                    new LocationTimeValue()
                    {
                        X = x.X,
                        Y = x.Y,
                        Z = x.Z,
                        Value = x.Value,
                        MeshIndex = x.MeshIndex,
                        IsActive = x.IsActive,
                        IsDiscretized = x.IsDiscretized,
                        Brush = x.Brush,
                        Date = x.Date,
                        Name = x.Name,
                        Neighbors = x.Neighbors,
                        Geographic = x.Geographic,
                        IsExterior = x.IsExterior
                    }).ToList());

                }
            }
            catch
            {
                throw new Exception("Mesh could not be modified.");
            }
        }

        /// <summary>
        /// Sorts a mesh by its node values
        /// </summary>
        /// <param name="_data"></param>
        private static void SortByValue(ref Mesh _data)
        {
            _data.Vertices = new System.Collections.ObjectModel.ObservableCollection<LocationTimeValue>(_data.Vertices.OrderBy(x => x.Value[0]));
            DataView dv = _data.Data.DefaultView;

            try
            {
                dv.Sort = "Item1 asc";
            }
            catch
            {
                try
                {
                    dv.Sort = "Value asc";
                }
                catch
                {

                }
            }

            _data.Data = dv.ToTable();
        }
    }
    
    /// <summary>
    /// Distribution types
    /// </summary>
    public enum DistributionType
    {
        Normal = 0,
        Gaussian = 1
    }
}
