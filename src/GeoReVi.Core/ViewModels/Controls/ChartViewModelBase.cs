using Caliburn.Micro;

namespace GeoReVi
{
    public abstract class ChartViewModelBase<T> : PropertyChangedBase where T: class, new()
    {
        /// <summary>
        /// A single instance of the view model
        /// </summary>
        public static T Instance => new T();
    }
}
