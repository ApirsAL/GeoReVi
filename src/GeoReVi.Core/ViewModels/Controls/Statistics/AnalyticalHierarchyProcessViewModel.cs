using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoReVi
{
    public class AnalyticalHierarchyProcessViewModel : Screen
    {
        #region Properties

        /// <summary>
        /// The AHP helper
        /// </summary>
        private AnalyticalHierarchyProcessHelper analyticalHierarchyProcessHelper = new AnalyticalHierarchyProcessHelper();
        public AnalyticalHierarchyProcessHelper AnalyticalHierarchyProcessHelper
        {
            get => this.analyticalHierarchyProcessHelper;
            set
            {
                this.analyticalHierarchyProcessHelper = value;
                NotifyOfPropertyChange(() => AnalyticalHierarchyProcessHelper);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        [ImportingConstructor]
        public AnalyticalHierarchyProcessViewModel()
        {

        }

        #endregion
    }
}
