using System.ComponentModel;
using Caliburn.Micro;

namespace GeoReVi
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    public class GenericPair<T, U> : PropertyChangedBase
        where T : class, new()
        where U : class, new()
    {
        private T first;
        public T First { get => this.first; set { this.first = value; NotifyOfPropertyChange(() => First); } }
        private U second;
        public U Second { get => this.second; set { this.second = value; NotifyOfPropertyChange(() => Second); } }
    }
}
