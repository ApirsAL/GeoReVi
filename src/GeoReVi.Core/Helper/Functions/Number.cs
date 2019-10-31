using Caliburn.Micro;

namespace GeoReVi
{
    /// <summary>
    /// A simple class to enable NotifyPropertyChanged for doubles
    /// </summary>
    public class Number : PropertyChangedBase
    {
        private double num = 1;
        public double Num
        {
            get => this.num;
            set
            {
                this.num = value;
                NotifyOfPropertyChange(() => Num);
            }
        }

        /// <summary>
        /// Specific constructor
        /// </summary>
        /// <param name="_num"></param>
        public Number(double _num)
        {
            Num = _num;
        }
    }
}
