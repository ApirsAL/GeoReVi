using Caliburn.Micro;
using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace GeoReVi
{
    public class RegressionHelper : PropertyChangedBase
    {

        #region Public properties

        /// <summary>
        /// RSquare
        /// </summary>
        private double rSquare = 0;
        public double RSquare
        {
            get => this.rSquare;
            set
            {
                this.rSquare = value;
                NotifyOfPropertyChange(() => RSquare);
            }
        }

        /// <summary>
        /// Fit polynomials
        /// </summary>
        private double[] fit = new double[] { };
        public double[] FitPolynomials
        {
            get => this.fit;
            set
            {
                this.fit = value;
            }
        }

        /// <summary>
        /// Degree
        /// </summary>
        private int degree = 2;
        public int Degree
        {
            get => this.degree;
            set
            {
                this.degree = value;
                NotifyOfPropertyChange(() => degree);
            }
        }

        /// <summary>
        /// The function applied
        /// </summary>
        private string function = "";
        public string Function
        {
            get => this.function;
            set
            {
                this.function = value;
                NotifyOfPropertyChange(() => Function);
            }
        }

        /// <summary>
        /// Interception point of the regression function
        /// </summary>
        private double axisInterceptionPoint = 0;
        public double AxisInterceptionPoint
        {
            get => this.axisInterceptionPoint;
            set
            {
                this.axisInterceptionPoint = value;
                NotifyOfPropertyChange(() => AxisInterceptionPoint);
            }
        }

        /// <summary>
        /// Regression type for the function
        /// </summary>
        private RegressionFunctionBiVariate regType = RegressionFunctionBiVariate.linear;
        public RegressionFunctionBiVariate RegType
        {
            get => this.regType;
            set
            {
                this.regType = value;
                NotifyOfPropertyChange(() => RegType);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public RegressionHelper()
        {

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Computing a regression analysis
        /// </summary>
        public PointCollection Compute(List<LocationTimeValue> data)
        {
            PointCollection ret = new PointCollection();
            RSquare = 0;
            Function = "";

            try
            {
                double minX = data.Min(x => x.X);
                double maxX = data.Max(x => x.X);
                double minY = data.Min(x => x.Y);
                double maxY = data.Max(x => x.Y);

                double meanX = data.Average(x => x.X);
                double meanY = data.Average(x => x.Y);

                double[] xValues = data.Select(z => z.X).ToArray();
                double[] yValues = data.Select(z => z.Y).ToArray();

                double errorSum = 0;
                double totalSumSquares = 0;

                switch (RegType)
                {
                    //Linear regression
                    case RegressionFunctionBiVariate.linear:

                        double b1 = data.Sum(x => (x.X - meanX) * (x.Y - meanY)) / data.Sum(x => Math.Pow(x.X - meanX, 2));
                        double b0 = meanY - b1 * meanX;

                        FitPolynomials = new double[2] { b0, b1 };

                        //Calculate polynomials
                        for (double i = minX; i < maxX; i += (maxX - minX) / 200)
                        {
                            Point point = new Point()
                            {
                                X = i,
                                Y = b0 + b1 * i
                            };

                            ret.Add(point);
                        }

                        for(int i =0;i<data.Count();i++)
                        {
                            xValues[i] = b0 + b1 * data[i].X;
                        }

                        break;

                    case RegressionFunctionBiVariate.curvilinear:


                        FitPolynomials = Fit.Polynomial(xValues, yValues, Degree);

                        //Calculate polynomials
                        for (double i = minX; i < maxX; i += (maxX - minX) / 200)
                        {
                            Point point = new Point()
                            {
                                X = i,
                                Y = 0
                            };

                            ///Drawing polynomial fit
                            for (int j = 0; j < FitPolynomials.Length; j++)
                            {
                                point.Y += FitPolynomials[j] * Math.Pow(i, j);
                            }

                            ret.Add(point);
                        }

                        //Calculate R squared
                        for (int i = 0; i < data.Count(); i++)
                        {
                            double value = 0;

                            ///Drawing polynomial fit
                            for (int j = 0; j < FitPolynomials.Length; j++)
                            {
                                value += FitPolynomials[j] * Math.Pow(data[i].X, j);
                            }

                            xValues[i] = value;
                        }
                        break;

                }

                RSquare = GoodnessOfFit.RSquared(xValues, yValues);
            }
            catch
            {

            }

            return ret;

        }

        /// <summary>
        /// Writes the function to a string
        /// </summary>
        public void CreateFunction()
        {
            try
            {
                for(int i = 0; i< FitPolynomials.Length; i++)
                {
                    if (i == 0)
                        Function = FitPolynomials[0].ToString().Substring(0, 4);
                    else
                        Function = Function + " + " + FitPolynomials[i].ToString().Substring(0, 4) + "xE+" + i.ToString();
                }
            }
            catch
            {

            }
        }

        #endregion
    }
}
