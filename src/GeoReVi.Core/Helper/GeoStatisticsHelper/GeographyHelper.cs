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
        public static async Task<List<XY>> DistanceMatrix(BindableCollection<LocationTimeValue> Points, VariogramHelper vh)
        {
            List<XY> valList = new List<XY>();

            int f = 1;

            BindableCollection<LocationTimeValue> subset = new BindableCollection<LocationTimeValue>();

            if (Points.Count() > 500)
            {
                Points.Shuffle();
                subset.AddRange(Points.Take(500).ToList());
            }
            else
            {
                subset.AddRange(Points);
            }

            for (int i = 0; i < subset.Count(); i+=1)
            {
                await Task.Delay(0);

                //Searching the neighborhood according to a search ellipsoid
                List<LocationTimeValue> neighborhood = (await SpatialNeighborhoodHelper.SearchByDistance(Points, subset[i], vh.RangeX, vh.RangeY, vh.RangeZ, vh.Azimuth, vh.Dip, vh.Plunge)).ToList();

                BindableCollection<LocationTimeValue> subsetNeighborhood = new BindableCollection<LocationTimeValue>();

                if (neighborhood.Count() > 500)
                {
                    neighborhood.Shuffle();
                    subsetNeighborhood.AddRange(Points.Take(500).ToList());
                }
                else
                {
                    subsetNeighborhood.AddRange(Points);
                }

                for (int j = 0; j < subsetNeighborhood.Count; j++)
                {
                    try
                    {
                        await Task.Delay(0);

                        XY valDist = new XY();

                        valDist.X = subsetNeighborhood[j].Value[0] - Points[i].Value[0];

                        valDist.Y = EuclideanDistance(subsetNeighborhood[j].X - subset[i].X, subsetNeighborhood[j].Y - subset[i].Y, subsetNeighborhood[j].Z - subset[i].Z);

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
