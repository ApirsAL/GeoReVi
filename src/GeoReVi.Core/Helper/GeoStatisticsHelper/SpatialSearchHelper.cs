using Accord.Math;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoReVi
{
    public static class SpatialNeighborhoodHelper
    {
        #region Methods

        public static async Task<IEnumerable<LocationTimeValue>> SearchByDistance(IEnumerable<LocationTimeValue> pointSet, 
            LocationTimeValue location,  
            double distanceX = 9999, 
            double distanceY = 9999, 
            double distanceZ = 9999, 
            double azimuth = 0, 
            double dip = 0, 
            double plunge = 0)
        {
            if (pointSet.Count() == 0)
                return new List<LocationTimeValue>();

            List<LocationTimeValue> locs = new List<LocationTimeValue>();

            lock (pointSet)
            {
                locs = new List<LocationTimeValue>(pointSet.ToList());
            }

            for(int i = 0; i<locs.Count(); i++)
            {
                try
                {
                    await Task.Delay(0);

                    double[] vec = new double[3] { Math.Abs(location.X - locs[i].X), Math.Abs(location.Y - locs[i].Y), Math.Abs(location.Z - locs[i].Z) };

                    vec = vec.Dot(GetTransformationMatrixZ(azimuth));
                    vec = vec.Dot(GetTransformationMatrixX(dip));
                    vec = vec.Dot(GetTransformationMatrixY(plunge));

                    if(!IsInsideEllipse(vec, distanceX, distanceY, distanceZ) || (vec[0] == 0 && vec[1] == 0 && vec[2]== 0))
                    {
                        locs.RemoveAt(i);
                        i -= 1;
                    }
                }
                catch
                {

                }
            }

            return locs;
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
