using Caliburn.Micro;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;

namespace GeoReVi
{
    public static class GeographyHelper
    {

        /// <summary>
        /// Calculates the value difference and distance nx2 matrix of a mesh 
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static async Task<double[]> GetDistances(List<LocationTimeValue> vertices, VariogramHelper vh)
        {
            if (vertices?.Count == 0)
                return null;

            long count = ArrayHelper.CalculateGaussSumation(vertices.Count) - vertices.Count;

            if (count > int.MaxValue)
                count = int.MaxValue;

            double[] distances = new double[Convert.ToInt32(count)];

            int counter = 0; 

            for (int i = 0; i < vertices.Count(); i++)
                for (int j = 1; j < vertices.Count() - i; j++)
                {
                    try
                    {
                        if (counter == int.MaxValue)
                            break;

                        //Searching the neighborhood according to a search ellipsoid
                        int[] neighborhood = await SpatialNeighborhoodHelper.SearchByDistance(new List<LocationTimeValue>() { vertices[i] }, vertices[j], vh.RangeX, vh.RangeY, vh.RangeZ, vh.Azimuth, vh.Dip, vh.Plunge);

                        // Checking if point is in neighborhood
                        if (neighborhood?.Length == 0)
                            continue;

                        // Calculating the distance
                        distances[counter] = vertices[i].GetEuclideanDistance(vertices[j]);

                        counter++;
                    }
                    catch
                    {
                        continue;
                    }

                }

            return distances.Take(counter).ToArray();
        }

        /// <summary>
        /// Calculates the value difference and distance nx2 matrix of a mesh 
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static async Task<double[]> GetDifferences(List<LocationTimeValue> vertices, VariogramHelper vh)
        {
            if (vertices?.Count == 0)
                return null;

            long count = ArrayHelper.CalculateGaussSumation(vertices.Count) - vertices.Count;

            if (count > int.MaxValue)
                count = int.MaxValue / 2;

            double[] differences = new double[count];

            int counter = 0;

            for (int i = 0; i < vertices.Count(); i++)
                for (int j = 1; j < vertices.Count() - i; j++)
                {
                    try
                    {
                        if (counter == int.MaxValue)
                            break;

                        //Searching the neighborhood according to a search ellipsoid
                        int[] neighborhood = await SpatialNeighborhoodHelper.SearchByDistance(new List<LocationTimeValue>() { vertices[i] }, vertices[j], vh.RangeX, vh.RangeY, vh.RangeZ, vh.Azimuth, vh.Dip, vh.Plunge);

                        // Checking if point is in neighborhood
                        if (neighborhood?.Length == 0)
                            continue;

                        // Calculating the values' difference
                        differences[counter] = Math.Abs(vertices[i].Value[0] - vertices[j].Value[0]);

                        counter++;
                    }
                    catch
                    {
                        continue;
                    }

                }

            return differences.Take(counter).ToArray();
        }

        /// <summary>
        /// Calculating the range of an x,y,z vector or the distance of two geographic points
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        public static double EuclideanDistance(double x = 0,
            double y = 0,
            double z = 0,
            bool geographic =
            false,
            double latitude1 = 0,
            double longitude1 = 0,
            double latitude2 = 0,
            double longitude2 = 0)
        {
            if (!geographic)
            {
                return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2));
            }
            else
            {
                var sCoord = new GeoCoordinate(latitude1, longitude1, z);
                var eCoord = new GeoCoordinate(latitude2, longitude2, z);

                return sCoord.GetDistanceTo(eCoord);
            }
        }

        /// <summary>
        /// Calculating the range of an x,y,z vector or the distance of two geographic points
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        public static double EuclideanDistance(LocationTimeValue point1, LocationTimeValue point2)
        {
            return Math.Sqrt((point2.X - point1.X) * (point2.X - point1.X) + (point2.Y - point1.Y) * (point2.Y - point1.Y) + (point2.Z - point1.Z) * (point2.Z - point1.Z));
        }

        #region Public methods

        public static void EquirectangularProjection(this List<LocationTimeValue> locationValues)
        {
            if (locationValues == null)
                if (locationValues.Count() == 0)
                    return;

            if (locationValues.First().Geographic)
            {
            }
            else
            {

            }
        }

        #endregion
    }
}
