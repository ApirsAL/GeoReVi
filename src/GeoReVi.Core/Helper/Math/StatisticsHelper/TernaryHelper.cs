using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoReVi
{
    public static class TernaryHelper
    {
        /// <summary>
        /// Converts a list of three points into representative 2d coordinates
        /// Calculation based on http://mathworld.wolfram.com/TernaryDiagram.html
        /// </summary>
        /// <param name="threeDimensionalObject">The values have to be normalized beween 0 and 1</param>
        /// <returns></returns>
        public static List<LocationTimeValue> ConvertToTwoDimensionalCoordinates(List<LocationTimeValue> threeDimensionalObject)
        {
            //return object
            List<LocationTimeValue> ret = new List<LocationTimeValue>();

            //Normalize data set
            List<LocationTimeValue> norm = TransformationHelper.Normalization(threeDimensionalObject);

            //Calculating the ratio of each tuple entry
            norm = CompositionalHelper.CalculateRatio(norm);

            foreach (LocationTimeValue tup in norm)
            {
                //x coordinate of the 2 dimensional representation
                double x = (0.5 * (tup.X + 2 * tup.Y) / (tup.X + tup.Y + tup.Z));

                //y coordinate of the 2 dimensional representation
                double y = (tup.X / (tup.X + tup.Y + tup.Z));

                //Adding coordinates to the list
                ret.Add(new LocationTimeValue() { X = x, Y = y, Z=0 });
            }

            return ret;
        }
    }
}
