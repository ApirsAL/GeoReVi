using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoReVi
{
    /// <summary>
    /// Static class for variogram calculations
    /// </summary>
    public static class VariogrammHelper
    {

        public static List<XY> ExperimentalVariogram(List<LocationValue<double>> dataSet, int steps = 10)
        {
            //Initialising the list of variogram values
            List<XY> variogramValues = new List<XY>();
            for (int a = 0; a < steps; a++)
                variogramValues.Add(new XY());

            if (dataSet != null)
            {
                if (dataSet.Count < 1)
                {
                    return variogramValues;
                }
            }
            else
            {
                return variogramValues;
            }

            List<XY> valuesDistance = new List<XY>();
            dataSet = dataSet.OrderBy(x => x.X).OrderBy(x => x.Y).OrderBy(x => x.Z).ToList();

            //Subdividing into bins


            //Converting coordinates of points to distances
            foreach (var pt in dataSet)
            {
                var begin = dataSet.First();
                var value = pt.Value;
                XY valDist = new XY();

                if (pt.Geographic)
                {
                    
                    //Calculating the distance between the first point and every other point
                    var distValue = EuclideanDistance(0, 0, 0,true,begin.Latitude, begin.Longitude, pt.Latitude, pt.Longitude);
                    valDist.Y = distValue;
                }
                else
                {
                    //Calculating the distance between the first point and every other point
                    var distValue = EuclideanDistance(pt.X-begin.X, pt.Y - begin.Y, pt.Z - begin.Z);
                    valDist.Y = distValue;
                }

                valDist.X = pt.Value;
                valuesDistance.Add(valDist);
            }

            double[] bins = DistributionHelper.Subdivide(valuesDistance.Select(x=>x.Y).ToArray(), steps);

            //Getting all values of a bin
            for(int i = 0;i<bins.Count();i++)
            {
                variogramValues[i].X = bins[i];

                List<XY> valuesInRange = new List<XY>((from valueDistance in valuesDistance
                                    where valueDistance.Y <= bins[i]
                                    select valueDistance).ToList());

                for (int j = 0; j<valuesInRange.Count(); j++)
                {
                    if (j == 0)
                        continue;

                    int n = valuesInRange.Count();
                    double val = 0;

                    for(int z = 0; z<=n-j; z++)
                    {
                         val += Math.Pow(valuesInRange[n-(z+1)].X - valuesInRange[j].X,2);
                    }

                    variogramValues[i].Y = val / n;
                }
            }

            return variogramValues;
        }


        #region Helper methods

        /// <summary>
        /// Calculating the range of an x,y,z vector or the distance of two geographic points
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        public static double EuclideanDistance(double x = 0, double y = 0, double z = 0, bool geographic = false, double latitude1 = 0, double longitude1 = 0, double latitude2 = 0, double longitude2 = 0)
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
        #endregion
    }


    /// <summary>
    /// A class for a location with a value (double or string)
    /// </summary>
    public class LocationValue<T>
    {
        #region Private members

        /// <summary>
        /// Location
        /// </summary>
        private double latitude;
        private double longitude;
        private double x;
        private double y;
        private double z;

        /// <summary>
        /// Value
        /// </summary>
        private T value;

        //Check if geographic or geometric
        private bool geographic;

        #endregion

        #region Public propeties
        public double Latitude
        {
            get { return this.latitude; }
            set { this.latitude = value; }
        }

        public double Longitude
        {
            get { return this.longitude; }
            set { this.longitude = value; }
        }

        public double X
        {
            get { return this.x; }
            set { this.x = value; }
        }

        public double Y
        {
            get { return this.y; }
            set { this.y = value; }
        }

        public double Z
        {
            get { return this.z; }
            set { this.z = value; }
        }

        public bool Geographic
        {
            get { return this.geographic; }
            set { this.geographic = value; }
        }

        public T Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        #endregion
    }
}
