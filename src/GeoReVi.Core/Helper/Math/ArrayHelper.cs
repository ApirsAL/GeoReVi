using System;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;

namespace GeoReVi
{
    /// <summary>
    /// A helper class for array operations
    /// </summary>
    public static class ArrayHelper
    {
        /// <summary>
        /// A method to delete a row from a two-dimensional array
        /// </summary>
        /// <param name="rowToRemove"></param>
        /// <param name="columnToRemove"></param>
        /// <param name="originalArray"></param>
        /// <returns></returns>
        public static double[,] TrimArray(int rowToRemove, int columnToRemove, double[,] originalArray)
        {
            double[,] result = new double[originalArray.GetLength(0) - 1, originalArray.GetLength(1) - 1];

            for (int i = 0, j = 0; i < originalArray.GetLength(0); i++)
            {
                if (i == rowToRemove)
                    continue;

                for (int k = 0, u = 0; k < originalArray.GetLength(1); k++)
                {
                    if (k == columnToRemove)
                        continue;

                    result[j, u] = originalArray[i, k];
                    u++;
                }
                j++;
            }

            return result;
        }

        /// <summary>
        /// Gauss summation ( 1 + 2 + 3 + n)
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static int CalculateGaussSumation(int count)
        {
            if (count <= 0)
                return 0;

            // https://de.wikipedia.org/wiki/Gau%C3%9Fsche_Summenformel
            return (count * count + count) / 2;
        }

        /// <summary>
        /// Transforming a jagged array to a datatable
        /// </summary>
        /// <param name="array"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static DataTable JaggedArrayToSymmetricDataTable(this double[][] array, string[] headers)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Parameter");

            // create columns
            for (int i = 0; i < headers.GetLength(0); i++)
            {
                dt.Columns.Add(headers[i]);
            }

            for (int i = 0; i < array[0].GetLength(0); i++)
            {
                try
                {
                    // create a DataRow using .NewRow()
                    DataRow row = dt.NewRow();
                    dt.Rows.Add(row);

                    dt.Rows[i][0] = dt.Columns[i + 1].ColumnName;

                    // iterate over all columns to fill the row
                    for (int j = 1; j < dt.Columns.Count; j++)
                    {
                        dt.Rows[i][j] = array[i][j - 1];
                    }

                    // add the current row to the DataTable
                }
                catch
                {
                    return null;
                }
            }

            return dt;
        }

        /// <summary>
        /// Transforming a jagged array to a datatable
        /// </summary>
        /// <param name="array"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static DataTable JaggedArrayToDataTable(this double[][] array, string[] headers)
        {
            DataTable dt = new DataTable();

            // create columns
            for (int i = 0; i < headers.GetLength(0); i++)
            {
                dt.Columns.Add(headers[i]);
            }

            for (int i = 0; i < array[0].GetLength(0); i++)
            {
                try
                {
                    // create a DataRow using .NewRow()
                    DataRow row = dt.NewRow();
                    dt.Rows.Add(row);

                    // iterate over all columns to fill the row
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        double a = array[i][j];
                        dt.Rows[i][j] = array[i][j];
                    }

                    // add the current row to the DataTable
                }
                catch
                {
                    return null;
                }
            }

            return dt;
        }

        /// <summary>
        /// Getting the row of a multidimensional array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public static T[] GetRow<T>(this T[,] array, int row)
        {
            if (!typeof(T).IsPrimitive)
                throw new InvalidOperationException("Not supported for managed types.");

            if (array == null)
                throw new ArgumentNullException("array");

            int cols = array.GetUpperBound(1) + 1;
            T[] result = new T[cols];

            int size;

            if (typeof(T) == typeof(bool))
                size = 1;
            else if (typeof(T) == typeof(char))
                size = 2;
            else
                size = Marshal.SizeOf<T>();

            Buffer.BlockCopy(array, row * cols * size, result, 0, cols * size);

            return result;
        }

        public static T[] GetColumn<T>(this T[,] matrix, int columnNumber)
        {
            if (!typeof(T).IsPrimitive)
                throw new InvalidOperationException("Not supported for managed types.");

            if (matrix == null)
                throw new ArgumentNullException("array");

            return Enumerable.Range(0, matrix.GetLength(0))
                    .Select(x => matrix[x, columnNumber])
                    .ToArray();
        }

        /// <summary>
        /// Copies a subset of an array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="fromX"></param>
        /// <param name="fromY"></param>
        /// <param name="lengthX"></param>
        /// <param name="lengthY"></param>
        /// <returns></returns>
        public static T[,] CopyArray<T>(this T[,] array, int fromX, int fromY, int lengthX, int lengthY)
        {
            T[,] result = new T[lengthX, lengthY];
            for (int x = 0; x < result.GetLength(0); x++)
            {
                for (int y = 0; y < result.GetLength(1); y++)
                {
                    result[x, y] = array[x + fromX, y + fromY];
                }
            }
            return result;
        }

        /// <summary>
        /// Computes a FisherYatesShuffle
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        internal static void FisherYatesShuffle<T>(this T[] array)
        {
            Random _rnd = new Random();

            for (int i = array.Length - 1; i > 0; i--)
            {
                // Pick random position:
                int pos = _rnd.Next(i + 1);

                // Swap:
                T tmp = array[i];
                array[i] = array[pos];
                array[pos] = tmp;
            }
        }
    }
}
