using Caliburn.Micro;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;

namespace GeoReVi
{
    public class MeshStatisticsViewModel : PropertyChangedBase
    {
        #region Public properties

        /// <summary>
        /// A descriptive statistics object
        /// </summary>
        private BindableCollection<MeshStatisticsHelper> meshStatisticsHelper;
        public BindableCollection<MeshStatisticsHelper> MeshStatisticsHelper
        {
            get => this.meshStatisticsHelper;
            set
            {
                this.meshStatisticsHelper = value;
                NotifyOfPropertyChange(() => MeshStatisticsHelper);
            }
        }

        #endregion

        #region Constructor

        public MeshStatisticsViewModel()
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
                ExportHelper.ExportList<MeshStatisticsHelper>(new List<MeshStatisticsHelper>(MeshStatisticsHelper), saveFileDialog.FileName, "");
            }
        }

        #endregion
    }
}
