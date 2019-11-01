using System;
using System.Collections.Generic;
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
                for (int j = 0; j < bins.Count() - 1; j++)
                {
                    if (j == bins.Count() - 1)
                        continue;

                    if (distribution[i] > bins[j] && distribution[i] <= bins[j + 1])
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

                // We need a non-zero random point inside the unit circle.
                do
                {
                    x = 2.0 * rnd.NextDouble() - 1.0;
                    y = 2.0 * rnd.NextDouble() - 1.0;
                    sqr = x * x + y * y;
                }
                while (sqr > 1.0 || sqr == 0);

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
