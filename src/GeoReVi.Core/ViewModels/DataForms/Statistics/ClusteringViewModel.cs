using Caliburn.Micro;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;

namespace GeoReVi
{
    /// <summary>
    /// A view model for clustering analysis
    /// </summary>
    public class ClusteringViewModel : PropertyChangedBase
    {
        #region Public properties

        /// <summary>
        /// The clustering helper
        /// </summary>
        private ClusteringHelper clusteringHelper = new ClusteringHelper();
        public ClusteringHelper ClusteringHelper
        {
            get => this.clusteringHelper;
            set
            {
                this.clusteringHelper = value;
                NotifyOfPropertyChange(() => ClusteringHelper);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        [ImportingConstructor]
        public ClusteringViewModel()
        {

        }

        /// <summary>
        /// Constructor with data set
        /// </summary>
        /// <param name="_dat"></param>
        [ImportingConstructor]
        public ClusteringViewModel(IEnumerable<KeyValuePair<string, DataTable>> _dat)
        {
            ClusteringHelper = new ClusteringHelper(_dat);
        }

        #endregion
    }
}
