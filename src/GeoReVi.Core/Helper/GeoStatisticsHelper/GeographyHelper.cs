using Caliburn.Micro;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Threading.Tasks;

namespace GeoReVi
{
    public static class GeographyHelper
    {
        //Calculating the distance matrix of a set of points
        public static async Task<Tuple<alglib.sparsematrix, alglib.sparsematrix>> DistanceMatrix(Mesh Points, VariogramHelper vh, int maximumNumberOfPointPairsPerPoint = 50)
        {
            Points.Vertices = new System.Collections.ObjectModel.ObservableCollection<LocationTimeValue>(Points.Vertices.PickRandom(maximumNumberOfPointPairsPerPoint).ToList());

            alglib.sparsematrix distanceMatrix;
            alglib.sparsematrix diffMatrix;
            alglib.sparsecreate(Points.Vertices.Count(), Points.Vertices.Count(), out distanceMatrix);
            alglib.sparsecreate(Points.Vertices.Count(), Points.Vertices.Count(), out diffMatrix);

            for (int i = 0; i < Points.Vertices.Count(); i += 1)
            {
                //Searching the neighborhood according to a search ellipsoid
                int[] neighborhood = await SpatialNeighborhoodHelper.SearchByDistance(Points.Vertices, Points.Vertices[i], vh.RangeX, vh.RangeY, vh.RangeZ, vh.Azimuth, vh.Dip, vh.Plunge, maximumNumberOfPointPairsPerPoint);

                await Task.Delay(0);

                for (int j = 0; j < neighborhood.Length; j++)
                {
                    try
                    {
                        if (neighborhood[j] == i)
                            continue;

                        double diff = Points.Vertices[neighborhood[j]].Value[0] - Points.Vertices[i].Value[0];

                        double dist = EuclideanDistance(Points.Vertices[neighborhood[j]].X - Points.Vertices[i].X, Points.Vertices[neighborhood[j]].Y - Points.Vertices[i].Y, Points.Vertices[neighborhood[j]].Z - Points.Vertices[i].Z);

                        alglib.sparseset(diffMatrix, i, neighborhood[j], diff);
                        alglib.sparseset(distanceMatrix, i, neighborhood[j], dist); 
                    }
                    catch
                    {
                        continue;
                    }

                }

            }

            return new Tuple<alglib.sparsematrix, alglib.sparsematrix>(distanceMatrix,diffMatrix);
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
