using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoReVi
{
    public static class CompositionalHelper
    {
        /// <summary>
        /// Calculating the ratios of single tuple entries out of values
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static List<LocationTimeValue> CalculateRatio(List<LocationTimeValue> values)
        {
            //return object
            List<LocationTimeValue> ret = new List<LocationTimeValue>();

            foreach(LocationTimeValue tup in values)
            {
                //sum of the tuple entries
                double sum = tup.X + tup.Y + tup.Z;

                double x=0;
                double y=0;
                double z=0;

                //calculating ratio of each entry
                if (sum !=0)
                {
                    x = (tup.X / sum);
                    y = (tup.Y / sum);
                    z = (tup.Z / sum);
                }

                ret.Add(new LocationTimeValue() { X = x, Y = y, Z = z });
            }

            return ret;
        }
    }
}
