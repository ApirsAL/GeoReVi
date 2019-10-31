using Caliburn.Micro;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;

namespace GeoReVi
{
    public class HeterogeneityStatisticsViewModel : PropertyChangedBase
    {
        #region Public properties

        /// <summary>
        /// A descriptive statistics object
        /// </summary>
        private BindableCollection<KeyValuePair<string, UnivariateHeterogeneityMeasuresHelper>> univariateHeterogeneityMeasuresHelper;
        public BindableCollection<KeyValuePair<string, UnivariateHeterogeneityMeasuresHelper>> UnivariateHeterogeneityMeasuresHelper
        {
            get => this.univariateHeterogeneityMeasuresHelper;
            set
            {
                this.univariateHeterogeneityMeasuresHelper = value;
                NotifyOfPropertyChange(() => UnivariateHeterogeneityMeasuresHelper);
            }
        }

        #endregion

        #region Constructor

        public HeterogeneityStatisticsViewModel()
        {

        }

        #endregion

        #region Public methods

        //Exporting the actually selected list of objects to a csv file
        public void ExportStatistics()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV (*.csv)|*.csv";
            saveFileDialog.RestoreDirectory = true;

            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != "")
            {
                //Exporting the list dependent on the sample type and the actual selection
                ExportHelper.ExportList<UnivariateHeterogeneityMeasuresHelper>(new List<UnivariateHeterogeneityMeasuresHelper>(UnivariateHeterogeneityMeasuresHelper.Select(x => x.Value).ToList()), saveFileDialog.FileName, "");
            }
        }

        #endregion
    }
}
