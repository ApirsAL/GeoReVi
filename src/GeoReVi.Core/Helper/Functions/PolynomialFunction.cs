using Caliburn.Micro;

namespace GeoReVi
{
    /// <summary>
    /// A function that contains a set of polynomials
    /// </summary>
    public class PolynomialFunction : PropertyChangedBase
    {

        #region Public properties
        
        /// <summary>
        /// Set of polynomials
        /// </summary>
        private BindableCollection<Polynomial> polynomials = new BindableCollection<Polynomial>();
        public BindableCollection<Polynomial> Polynomials
        {
            get => this.polynomials;
            set
            {
                this.polynomials = value;
                NotifyOfPropertyChange(() => Polynomials);
            }
        }
        
        #endregion

        #region Constructor

        /// <summary>
        /// Default contructor
        /// </summary>
        public PolynomialFunction()
        {

        }

        #endregion
    }
}
