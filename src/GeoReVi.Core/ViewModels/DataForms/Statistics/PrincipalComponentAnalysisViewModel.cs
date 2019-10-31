using Caliburn.Micro;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;

namespace GeoReVi
{
    /// <summary>
    /// A view model holding the information for a Principal Component Analysis
    /// </summary>
    public class PrincipalComponentAnalysisViewModel : PropertyChangedBase
    { 
        #region Public properties

        /// <summary>
        /// The Principal component helper
        /// </summary>
        private PrincipalComponentHelper principalComponentHelper = new PrincipalComponentHelper();
        public PrincipalComponentHelper PrincipalComponentHelper
        {
            get => this.principalComponentHelper;
            set
            {
                this.principalComponentHelper = value;
                NotifyOfPropertyChange(() => PrincipalComponentHelper);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        [ImportingConstructor]
        public PrincipalComponentAnalysisViewModel()
        {
            
        }

        /// <summary>
        /// Constructor with data set
        /// </summary>
        /// <param name="_dat"></param>
        [ImportingConstructor]
        public PrincipalComponentAnalysisViewModel(IEnumerable<KeyValuePair<string, DataTable>> _dat)
        {
            PrincipalComponentHelper = new PrincipalComponentHelper(_dat);
        }

        #endregion
    }
}
