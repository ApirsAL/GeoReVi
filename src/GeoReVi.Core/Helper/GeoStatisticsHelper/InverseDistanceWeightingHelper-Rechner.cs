using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeoReVi
{
    /// <summary>
    /// The inverse distance weighting is a non-statistical interpolation procedure
    /// </summary>
    public class InverseDistanceWeightingHelper : SpatialInterpolationHelper<double>
    {

        #region Constructor

        public InverseDistanceWeightingHelper(BindableCollection<LocationTimeValue<double>> locationValues,
                int binsX = 20,
                int binsY = 20,
                int binsZ = 20,
                DiscretizationMethod _discretizationMethod = DiscretizationMethod.RegularGrid,
                DirectionEnum _direction = DirectionEnum.XYZ,
                double power = 2)
        {
            DiscretizedLocationValues = new List<LocationTimeValue<double>>();
            OriginalLocationValues = locationValues;
            BinsX = binsX;
            BinsY = binsY;
            BinsZ = binsZ;
            DiscretizationMethod = _discretizationMethod;
            Direction = _direction;
            Power = power;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Computing the IDW data matrix
        /// </summary>
        public void ComputeIDW()
        {
            if (OriginalLocationValues.Count < 1)
            {
                DiscretizedLocationValues = new List<LocationTimeValue<double>>();
                DiscretizedLocationVariance = new List<LocationTimeValue<double>>();
            }

            List<LocationTimeValue<double>> ret = new List<LocationTimeValue<double>>();
            double rmseSum = 0;

            try
            {
                //Doing discretization if not done yet
                if (!OriginalLocationValues.First().IsDiscretized)
                {
                    DiscretizedLocationValues = new List<LocationTimeValue<double>>();
                    DiscretizedLocationVariance = new List<LocationTimeValue<double>>();
                    DiscretizeOriginalLocalLocationValuesIntoRegularGrid();
                }

                for (int i = 0; i < DiscretizedLocationValues.Count(); i++)
                {
                    double value = 0;
                    double weightSum = 0;

                    //Calculating IDW values
                    for (int j = 0; j < OriginalLocationValues.Count(); j++)
                    {
                        double dist = GeographyHelper.EuclideanDistance(DiscretizedLocationValues[i].X - OriginalLocationValues[j].X, DiscretizedLocationValues[i].Y - OriginalLocationValues[j].Y, DiscretizedLocationValues[i].Z - OriginalLocationValues[j].Z, false);
                        if (dist != 0)
                        {
                            //Summing up the weights
                            weightSum += 1 / Math.Pow(dist, Power);
                            //Increasing the value by the constrain value and it's weight
                            value += Convert.ToDouble(OriginalLocationValues[j].Value) / Math.Pow(dist, Power);
                        }
                        else
                        {
                            value = OriginalLocationValues[j].Value;
                            break;
                        }
                    }

                    DiscretizedLocationValues[i].Value = value / weightSum;

                }

                //Calculating the RMSE
                for (int i = 0; i < OriginalLocationValues.Count(); i++)
                {
                    double weightSum = 0;
                    double value = 0;

                    for (int j = 0; j < OriginalLocationValues.Count(); j++)
                    {
                        if (i != j)
                        {
                            double dist = GeographyHelper.EuclideanDistance(DiscretizedLocationValues[i].X - OriginalLocationValues[j].X, DiscretizedLocationValues[i].Y - OriginalLocationValues[j].Y, DiscretizedLocationValues[i].Z - OriginalLocationValues[j].Z, false);
                            if (dist != 0)
                            {
                                weightSum += 1 / Math.Pow(dist, Power);
                                value += Convert.ToDouble(OriginalLocationValues[j].Value) / Math.Pow(dist, Power);
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }

                    rmseSum += Math.Abs(Math.Pow(OriginalLocationValues[i].Value - (value / weightSum),2));
                }

                RMSE = Math.Sqrt(rmseSum / OriginalLocationValues.Count());
            }
            catch
            {

            }
        }

        #endregion
    }
}
