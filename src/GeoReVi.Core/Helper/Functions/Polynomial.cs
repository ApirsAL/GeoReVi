using Caliburn.Micro;
using System;

namespace GeoReVi
{
    /// <summary>
    /// A class representing a polynomial function of nth grade
    /// </summary>
    public class Polynomial : PropertyChangedBase
    {
        #region Properties

        /// <summary>
        /// Coefficients 
        /// </summary>
        private BindableCollection<Number> coefficients = new BindableCollection<Number>()
        {    new Number(1),
             new Number(1),
             new Number(1)
        };
        public BindableCollection<Number> Coefficients
        {
            get => this.coefficients;
            set
            {
                this.coefficients = value;
                NotifyOfPropertyChange(() => Coefficients);
            }
        }

        /// <summary>
        /// Exponents
        /// </summary>
        private BindableCollection<Number> exponents = new BindableCollection<Number>()
        {    new Number(1),
             new Number(1),
             new Number(1)
        };
        public BindableCollection<Number> Exponents
        {
            get => this.exponents;
            set
            {
                this.exponents = value;
                NotifyOfPropertyChange(() => Exponents);
            }
        }

        /// <summary>
        /// Degree 
        /// </summary>
        private int degree = 3;
        public int Degree
        {
            get => this.degree;
            set
            {
                this.degree = value;
                NotifyOfPropertyChange(() => Exponents);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="degree"></param>
        public Polynomial()
        {
            Coefficients = new BindableCollection<Number>()
                            {    new Number(1),
                                 new Number(1),
                                 new Number(1)
                            };
            Exponents = new BindableCollection<Number>()
                            {    new Number(1),
                                 new Number(1),
                                 new Number(1)
                            };

        }

        #endregion

        /// <summary>
        /// Solving a polynomial of degree 3
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public double Solve(LocationTimeValue point)
        {
            if (Degree != 3)
                return Double.NaN;

            return Coefficients[0].Num * Math.Pow(point.X, Exponents[0].Num) * Coefficients[1].Num * Math.Pow(point.Y, Exponents[1].Num) * Coefficients[2].Num * Math.Pow(point.Z, Exponents[2].Num);
        }


    }
}
