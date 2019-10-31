using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoReVi
{
    public static class TransformationHelper
    {
        /// <summary>
        /// A static class to calculate data normalization range of positive double values
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<double> Normalization(List<double> list)
        {
            //return object
            List<double> ret = new List<double>();

            try
            {
                //Maximum of the set
                double max = list.Max();

                foreach(double val in list)
                {
                    //calculating value
                    double x = val / max;

                    //adding to list
                    ret.Add(x);
                }
            }
            catch
            {

            }


            return ret;
        }

        /// <summary>
        /// A static class to calculate data normalization range of positive double values
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<LocationTimeValue> Normalization(List<LocationTimeValue> list)
        {
            //return object
            List<LocationTimeValue> ret = new List<LocationTimeValue>(list);

            try
            {
                //Maximum values
                double maxItem1 = list.Max(x => x.X);
                double maxItem2 = list.Max(x => x.Y);
                double maxItem3 = list.Max(x => x.Z);

                //Minimum values
                double minItem1 = list.Min(x => x.X);
                double minItem2 = list.Min(x => x.Y);
                double minItem3 = list.Min(x => x.Z);

                int i = 0;

                foreach (LocationTimeValue val in list)
                {
                    //calculating values
                    double x = (val.X-minItem1) / (maxItem1-minItem1);
                    double y = (val.Y-minItem2) / (maxItem2-minItem2);
                    double z = (val.Z-minItem3) / (maxItem3-minItem3);

                    //adding to list
                    ret[i] = new LocationTimeValue() { X=x, Y=y, Z=z };

                    i += 1;
                }
            }
            catch
            {
            }

            return ret;
        }
    }
}
