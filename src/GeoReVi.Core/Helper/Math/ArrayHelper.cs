using System;
using System.Data;
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
