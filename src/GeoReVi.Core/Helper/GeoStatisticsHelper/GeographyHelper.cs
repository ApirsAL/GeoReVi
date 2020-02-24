using Caliburn.Micro;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;

namespace GeoReVi
{
    public static class GeographyHelper
    {
        //Calculating the distance matrix of a set of points
        public static List<XY> DistanceMatrix(BindableCollection<LocationTimeValue> Points, VariogramHelper vh)
        {
            List<XY> valList = new List<XY>();

            int f = 1;

            if (Points.Count() > 10000)
                f = 500;

            for (int i = 0; i < Points.Count(); i+=f)
            {
                //Searching the neighborhood according to a search ellipsoid
                List<LocationTimeValue> neighborhood = SpatialNeighborhoodHelper.SearchByDistance(Points, Points[i], vh.RangeX, vh.RangeY, vh.RangeZ, vh.Azimuth, vh.Dip, vh.Plunge).ToList();

                for (int j = 0; j < neighborhood.Count; j++)
                {
                    try
                    {
                            XY valDist = new XY();

                            valDist.X = neighborhood[j].Value[0] - Points[i].Value[0];

                            valDist.Y = EuclideanDistance(neighborhood[j].X - Points[i].X, neighborhood[j].Y - Points[i].Y, neighborhood[j].Z - Points[i].Z);

                            valList.Add(valDist);
                    }
                    catch
                    {
                        continue;
                    }

                }
            }

            return valList;
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
            return Math.Sqrt((point2.X - point1.X)*(point2.X - point1.X) + (point2.Y - point1.Y)*(point2.Y - point1.Y) + (point2.Z - point1.Z)*(point2.Z - point1.Z));
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
