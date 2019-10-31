using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoReVi
{
    public static class MathExtensions
    {
        /// <summary>
        /// Calculated the geometric mean of a data set
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        public static double GeometricMean(this double[] dataSet)
        {
            double prod = 1;

            for(int i = 0; i<dataSet.Length; i++)
            {
                if (dataSet[i] != 0 && !Double.IsNaN(dataSet[i]))
                    prod *= dataSet[i];
            }

            return Math.Pow(prod, 1/Convert.ToDouble(dataSet.Length));
        }

        /// <summary>
        /// Calculated the harmonic mean of a data set
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        public static double HarmonicMean(this double[] dataSet)
        {
            double inverseSum = 0;

            for (int i = 0; i < dataSet.Length; i++)
            {
                if (dataSet[i] != 0 && !Double.IsNaN(dataSet[i]))
                    inverseSum += 1/dataSet[i];
            }

            return Convert.ToDouble(dataSet.Length/inverseSum);
        }

        /// <summary>
        /// Calculating the standard deviation
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static double StdDev(this IEnumerable<double> values)
        {
            double ret = 0;
            int count = values.Count();

            values = values.Where(x => x != 0 && !Double.IsNaN(x)).ToList();

            if (count > 1)
            {
                //Compute the Average
                    double avg = values.Average();

                //Perform the Sum of (value-avg)^2
                double sum = values.Sum(d => (d - avg) * (d - avg));

                //Put it all together
                ret = Math.Sqrt(sum / count);
            }

            return ret;
        }

        public static double[,] ZScore(this double[,] data)
        {
            int rows = data.GetLength(0);
            int cols = data.GetLength(1);
            double[,] matrix = new double[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                {        
                    // subtract mean and divide by standard deviation (convert to Z Scores)
                    try
                    {
                        matrix[i, j] = (data[i, j] - data.GetColumn(j).ToArray().Average()) / data.GetColumn(j).ToArray().StdDev();

                        if (Double.IsNaN(matrix[i, j]))
                            if(!Double.IsNaN(data[i,j]))
                                matrix[i, j] = 0;
                    }
                    catch
                    {
                        matrix[i, j] = 0;
                    }


                }

            return matrix;

        }

        /// <summary>
        /// Calculate the ZScore of an array
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static double[] ZScore(this double[] data)
        {
            int rows = data.Length;

            double[] matrix = new double[rows];

            for (int i = 0; i < rows; i++)
                    // subtract mean and divide by standard deviation (convert to Z Scores)
                    try
                    {
                        matrix[i] = (data[i] - data.Average()) / data.StdDev();

                        if (Double.IsNaN(matrix[i]))
                            if (!Double.IsNaN(data[i]))
                                matrix[i] = 0;
                    }
                    catch
                    {
                        matrix[i] = 0;
                    }

            return matrix;

        }

        public static double[,] Adjust(this double[,] data)
        {
            int rows = data.GetLength(0);
            int cols = data.GetLength(1);
            double[,] matrix = new double[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                {
                    // subtract mean and divide by standard deviation (convert to Z Scores)
                    matrix[i, j] = (data[i, j] - data.GetColumn(j).ToArray().Average());

                    if (Double.IsNaN(matrix[i, j]))
                        if (!Double.IsNaN(data[i, j]))
                            matrix[i, j] = 0;


                }

            return matrix;

        }

        /// <summary>
        /// Getting a column from a data matrix
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        public static double[] GetColumn(this double[,] matrix, int columnIndex)
        {
                return Enumerable.Range(0, matrix.GetLength(0))
                        .Select(x => matrix[x, columnIndex])
                        .ToArray();
        }

        /// <summary>
        /// Getting a row from a data matrix
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public static double[] GetRow(this double[,] matrix, int rowIndex)
        {
            return Enumerable.Range(0, matrix.GetLength(1))
                .Select(x => matrix[rowIndex, x])
                .ToArray();
        }

        /// <summary>
        /// Projects a value from degree to radians
        /// </summary>
        /// <param name="degree">value in degree</param>
        /// <returns></returns>
        public static double DegreeToRadians(this double degree)
        {
            return (degree / 180.0 * Math.PI);
        }

        /// <summary>
        /// Projects a value from radians to degree
        /// </summary>
        /// <param name="degree">value in degree</param>
        /// <returns></returns>
        public static double RadiansToDegree(this double radians)
        {
            return (radians / Math.PI * 180.0);
        }
    }
}
