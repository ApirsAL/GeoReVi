using Caliburn.Micro;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;

namespace GeoReVi
{
    /// <summary>
    /// A view model holding the logic for a Sammon Projection
    /// </summary>
    public class SammonProjectionViewModel : PropertyChangedBase
    {
        #region Public properties

        /// <summary>
        /// The Principal component helper
        /// </summary>
        private SammonProjectionHelper sammonProjectionHelper = new SammonProjectionHelper();
        public SammonProjectionHelper SammonProjectionHelper
        {
            get => this.sammonProjectionHelper;
            set
            {
                this.sammonProjectionHelper = value;
                NotifyOfPropertyChange(() => SammonProjectionHelper);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        [ImportingConstructor]
        public SammonProjectionViewModel()
        {

        }

        /// <summary>
        /// Constructor with data set
        /// </summary>
        /// <param name="_dat"></param>
        [ImportingConstructor]
        public SammonProjectionViewModel(IEnumerable<Mesh> _dat)
        {
            SammonProjectionHelper = new SammonProjectionHelper(_dat);
        }

        #endregion
    }
}
