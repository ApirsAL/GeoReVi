using Accord.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeoReVi
{
    public static class SpatialNeighborhoodHelper
    {
        #region Methods
        /// <summary>
        /// Builds a search ellipsoid, rotates the point set and determines, which points are contained in the search ellipsoid and which are not
        /// </summary>
        /// <param name="pointSet"></param>
        /// <param name="location"></param>
        /// <param name="distanceX"></param>
        /// <param name="distanceY"></param>
        /// <param name="distanceZ"></param>
        /// <param name="azimuth"></param>
        /// <param name="dip"></param>
        /// <param name="plunge"></param>
        /// <param name="maximumNumber"></param>
        /// <returns></returns>
        public static async Task<int[]> SearchByDistance(IEnumerable<LocationTimeValue> pointSet, 
            LocationTimeValue location,  
            double distanceX = 9999, 
            double distanceY = 9999, 
            double distanceZ = 9999, 
            double azimuth = 0, 
            double dip = 0, 
            double plunge = 0,
            int maximumNumber = 9999,
            InterpolationFeature interpolationFeature = InterpolationFeature.Value)
        {
            if (pointSet.Count() == 0)
                return new int[0];

            int[] ret = new int[0];
            Dictionary<int,double> distances = new Dictionary<int, double>();

            List<LocationTimeValue> locs = new List<LocationTimeValue>();
            lock (pointSet)
            {
                locs = new List<LocationTimeValue>(pointSet.ToList());
                ret = new int[pointSet.Count()];
            }

            //Initializing the transformation matrices
            double[,] transZ = GetTransformationMatrixZ(azimuth);
            double[,] transY = GetTransformationMatrixY(plunge);
            double[,] transX = GetTransformationMatrixX(dip);

            for (int i = 0; i<ret.Length; i++)
            {
                try
                {
                    await Task.Delay(0);

                    //Calculating the relative position of the point with regard to its rotation center
                    double[] vec = new double[3] { Math.Abs(location.X - locs[i].X), Math.Abs(location.Y - locs[i].Y), Math.Abs(location.Z - locs[i].Z) };

                    //Dependent on the feature which should be interpolated, the respective distance is set to 0
                    switch(interpolationFeature)
                    {
                        case InterpolationFeature.Elevation:
                            vec[2] = 0;
                            break;
                        case InterpolationFeature.Latitude:
                            vec[1] = 0;
                            break;
                        case InterpolationFeature.Longitude:
                            vec[0] = 0;
                            break;
                    }

                    //Calculating the new position of the point after rotation
                    vec = vec.Dot(transZ);
                    vec = vec.Dot(transX);
                    vec = vec.Dot(transY);

                    //Searching if the point is located in the defined search ellipsoid whose center is the target point
                    if(!IsInsideEllipse(vec, distanceX, distanceY, distanceZ) || (vec[0] == 0 && vec[1] == 0 && vec[2]== 0))
                    {
                        ret[i] = -1;
                    }
                    else
                    {
                        if(maximumNumber > distances.Count() || distances.Where(x => x.Value > vec.Euclidean())
                                                                         .Select(e => (KeyValuePair<int, double>?)e)
                                                                         .FirstOrDefault() != null)
                        {
                            ret[i] = i;
                            distances.Add(i, vec.Euclidean());

                            if(distances.Count() > maximumNumber)
                            {
                                //Getting the key of the maximum value
                                int a = distances.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
                                ret[a] = -1;
                                distances.Remove(a);
                            }
                        }
                        else
                            ret[i] = -1;
                    }
                }
                catch
                {

                }
            }

            //returning the points
            return ret.Where(x => x != -1).ToArray();
        }

        /// <summary>
        /// Transformation matrix in z direction
        /// </summary>
        /// <param name="angleZ"></param>
        /// <returns></returns>
        private static double[,] GetTransformationMatrixZ(double angleZ)
        {
            double[,] ret = new double[3, 3];

            try
            {
                ret[0, 0] = Math.Cos(angleZ);
                ret[0, 1] = 0;
                ret[0, 2] = -Math.Sin(angleZ);
                ret[1, 0] = 0;
                ret[1,1] = 1;
                ret[1, 2] = 0;
                ret[2, 0] = Math.Sin(angleZ);
                ret[2, 1] = 0;
                ret[2, 2] = Math.Cos(angleZ);
            }
            catch
            {

            }

            return ret;
        }

        /// <summary>
        /// Transformation matrix in z direction
        /// </summary>
        /// <param name="angleZ"></param>
        /// <returns></returns>
        private static double[,] GetTransformationMatrixX(double angleX)
        {
            double[,] ret = new double[3, 3];

            try
            {
                ret[0, 0] = 1;
                ret[0, 1] = 0;
                ret[0, 2] = 0;
                ret[1, 0] = 0;
                ret[1, 1] = Math.Cos(angleX);
                ret[1, 2] = Math.Sin(angleX);
                ret[2, 0] = 0;
                ret[2, 1] = -Math.Sin(angleX);
                ret[2, 2] = Math.Cos(angleX);
            }
            catch
            {

            }

            return ret;
        }

        /// <summary>
        /// Transformation matrix in z direction
        /// </summary>
        /// <param name="angleZ"></param>
        /// <returns></returns>
        private static double[,] GetTransformationMatrixY(double angleY)
        {
            double[,] ret = new double[3, 3];

            try
            {
                ret[0, 0] = Math.Cos(angleY);
                ret[0, 1] = Math.Sin(angleY);
                ret[0, 2] = 0;
                ret[1, 0] = -Math.Sin(angleY);
                ret[1, 1] = Math.Cos(angleY);
                ret[1, 2] = 0;
                ret[2, 0] = 0;
                ret[2, 1] = 0;
                ret[2, 2] = 1;
            }
            catch
            {

            }

            return ret;
        }

        public static bool IsInsideSearchEllipsoid()
        {
            return true;
        }

        /// <summary>
        /// Checks whether a point is inside an ellipse
        /// </summary>
        /// <param name="point"></param>
        /// <param name="rangeX"></param>
        /// <param name="rangeY"></param>
        /// <param name="rangeZ"></param>
        /// <returns></returns>
        public static bool IsInsideEllipse(double[] point, double rangeX, double rangeY, double rangeZ)
        {
            return Math.Pow(point[0] / rangeX, 2) + Math.Pow(point[1] / rangeY, 2) + Math.Pow(point[2] / rangeZ, 2) <= 1;
        }
        #endregion
    }
}
