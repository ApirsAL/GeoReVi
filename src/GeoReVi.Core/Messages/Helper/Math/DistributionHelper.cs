using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoReVi
{
    public static class DistributionHelper
    {
        /// <summary>
        /// A static method to subdivide a distribution into bins
        /// </summary>
        /// <param name="array"></param>
        /// <param name="countBins"></param>
        /// <returns></returns>
        public static double[] Subdivide(double[] array, int countBins = 0)
        {
            //Getting meta properties of the array
            int length = array.Length;
            double max = array.Max();
            double min = array.Min();
            double mean = array.Sum() / Convert.ToDouble(length);

            double[] ret = new double[] { };

            if(min>=0 && max>0)
            {
                //probably normal distribution
                if((min/max)>0.01)
                {
                    double step = Math.Round((max - min) / countBins, 0);

                    if(step!=0)
                    {
                        min = Math.Round(min, 0);
                        max = Math.Round(max, 0);
                    }
                    else
                    {
                        step = Math.Round((max - min) / countBins, 1);

                        if (step != 0)
                        {
                            min = Math.Round(min, 1);
                            max = Math.Round(max, 1);
                        }
                        else
                        {
                            step = Math.Round((max - min) / countBins, 2);

                            if (step != 0)
                            {
                                min = Math.Round(min, 2);
                                max = Math.Round(max, 2);
                            }
                        }
                    }

                    Array.Resize(ref ret, countBins);

                    for (int i = 0; i<countBins;i++)
                    {
                        if (i == 0)
                        {
                            ret[i] = min;
                        }
                        else
                        {
                            ret[i] = ret[i - 1] + step;
                        }
                    }
                }
                else if(min/max==0)
                {
                    double step = Math.Round((max - min) / countBins, 0);

                    if (step != 0)
                    {
                        min = Math.Round(min, 0);
                        max = Math.Round(max, 0);
                    }

                    Array.Resize(ref ret, countBins);

                    for (int i = 0; i < countBins; i++)
                    {
                        if (i == 0)
                        {
                            ret[i] = min;
                        }
                        else
                        {
                            ret[i] = ret[i - 1] + step;
                        }
                    }
                }
            }

            return ret;
        }

        //Counting occurences of doubles withing a range of bins
        public static double[] Counts(double[] distribution, double[] bins)
        {
            double[] y = new double[bins.Count()];

            //Counting based on the bin
            for (int i = 0; i < distribution.Count(); i++)
            {
                for (int j = 0; j < bins.Count() - 1; j++)
                {
                    if (j == bins.Count() - 1)
                        continue;

                    if (distribution[i] >= bins[j] && distribution[i] <= bins[j + 1])
                    {
                        y[j] += 1;
                    }
                }
            }

            return y;
        }
    }
}
