using Caliburn.Micro;
using System.Collections.ObjectModel;

namespace GeoReVi
{
    public class LithologiesWrapViewModel : Screen
    {
        #region private members
        //Observable collection of children
        ObservableCollection<object> _children;

        /// <summary>
        /// Event aggregator for event subscription to communicate with other viewmodels
        /// </summary>
        private IEventAggregator _events;
        #endregion

        #region Public properties

        /// <summary>
        /// observable collection of the view models children
        /// </summary>
        public ObservableCollection<object> Children { get { return _children; } }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public LithologiesWrapViewModel(IEventAggregator events)
        {
            _events = events;
            _children = new ObservableCollection<object>();
            _children.Add(new LithologyDetailsViewModel(_events));
            _children.Add(new ArchitecturalElementsDetailsViewModel(_events));
        }
        #endregion
    }
}
