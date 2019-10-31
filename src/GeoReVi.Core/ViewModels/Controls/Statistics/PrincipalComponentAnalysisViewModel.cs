using Caliburn.Micro;
using System.ComponentModel.Composition;

namespace GeoReVi
{
    /// <summary>
    /// A view model holding the logic for a Principal Component Analysis
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

        #endregion
    }
}
