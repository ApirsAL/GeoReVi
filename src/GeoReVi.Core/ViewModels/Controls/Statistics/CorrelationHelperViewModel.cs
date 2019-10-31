using Caliburn.Micro;
using System.Collections.Generic;

namespace GeoReVi
{
    public class CorrelationHelperViewModel : PropertyChangedBase
    {
        #region Public properties

        /// <summary>
        /// A correlation analysis object
        /// </summary>
        private CorrelationHelper correlationHelper = new CorrelationHelper();
        public CorrelationHelper CorrelationHelper
        {
            get => this.correlationHelper;
            set
            {
                this.correlationHelper = value;
                NotifyOfPropertyChange(() => CorrelationHelper);
            }
        }

        #endregion

        #region Constructor

        public CorrelationHelperViewModel()
        {

        }

        #endregion

    }
}
