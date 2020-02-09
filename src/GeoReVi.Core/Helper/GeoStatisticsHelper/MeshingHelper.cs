using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoReVi
{
    public static class MeshingHelper
    {
        #region Methods

        /// <summary>
        /// Interpolating a line in 2D
        /// </summary>
        /// <param name="startX">X coordinate of starting point</param>
        /// <param name="startY">Y coordinate of starting point</param>
        /// <param name="endX">X coordinate of ending point</param>
        /// <param name="endY">X coordinate of ending point</param>
        /// <param name="count">Count of points</param>
        /// <returns></returns>
        public static double[,] InterpolateLine2D(double startX, double startY, double endX, double endY, int count)
        {
            double[,] ret = new double[count, 2];

            try
            {
                double[] xCoordinates = DistributionHelper.Subdivide(new double[2] { startX, endX }, count + 1);
                double[] yCoordinates = DistributionHelper.Subdivide(new double[2] { startY, endY }, count + 1);

                for(int i = 0; i<count; i++)
                {
                    ret[i, 0] = xCoordinates[i];
                    ret[i, 1] = yCoordinates[i];
                }
            }
            catch
            {

            }

            return ret;
        }

        #endregion
    }
}
