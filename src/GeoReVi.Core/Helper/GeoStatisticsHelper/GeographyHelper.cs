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
        public static async Task<double[,]> GetDifferenceDistanceMatrix(Mesh mesh, VariogramHelper vh)
        {
            if (mesh?.Vertices?.Count == 0)
                return null;

            int count = ArrayHelper.CalculateGaussSumation(mesh.Vertices.Count) - mesh.Vertices.Count;

            double[,] ret = new double[count, 2];

            int counter = 0; 

            for (int i = 0; i < mesh.Vertices.Count(); i++)
                for (int j = 1; j < mesh.Vertices.Count() - i; j++)
                {
                    try
                    {
                        //Searching the neighborhood according to a search ellipsoid
                        int[] neighborhood = await SpatialNeighborhoodHelper.SearchByDistance(new List<LocationTimeValue>() { mesh.Vertices[i] }, mesh.Vertices[j], vh.RangeX, vh.RangeY, vh.RangeZ, vh.Azimuth, vh.Dip, vh.Plunge);

                        // Checking if point is in neighborhood
                        if (neighborhood?.Length == 0)
                            continue;

                        // Calculating the values' difference
                        ret[counter, 0] = Math.Abs(mesh.Vertices[i].Value[0] - mesh.Vertices[j].Value[0]);

                        // Calculating the distance
                        ret[counter, 1] = mesh.Vertices[i].GetEuclideanDistance(mesh.Vertices[j]);

                        counter++;
                    }
                    catch
                    {
                        continue;
                    }

                }

            ret = ArrayHelper.CopyArray(ret, 0, 0, counter, 2);

            return ret;
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
