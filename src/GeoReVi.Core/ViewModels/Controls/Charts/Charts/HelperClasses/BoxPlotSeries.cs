using Caliburn.Micro;
using System.Windows.Media;

namespace GeoReVi
{
    public class BoxPlotSeries : BarSeries
    {

        /// Point collection for the bar series
        private BoxPlotStatistics boxPlotStatistics;
        public BoxPlotStatistics BoxPlotStatisticsCollection
        {
            get => this.boxPlotStatistics;
            set
            {
                this.boxPlotStatistics = value;
            }
        }

        /// <summary>
        /// Outliers of the box plot series
        /// </summary>
        private BindableCollection<LocationTimeValue> outliers = new BindableCollection<LocationTimeValue>();
        public BindableCollection<LocationTimeValue> Outliers
        {
            get => this.outliers;
            set
            {
                this.outliers = value;
                NotifyOfPropertyChange(() => Outliers);
            }
        }

        /// <summary>
        /// X Gridlines of the chart object
        /// </summary>
        private BindableCollection<Gridline> whiskers = new BindableCollection<Gridline>();
        public BindableCollection<Gridline> Wiskers
        {
            get => this.whiskers;
            set
            {
                this.whiskers = value;
                NotifyOfPropertyChange(() => Wiskers);
            }
        }

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BoxPlotSeries()
        {

        }

        #endregion


    }
}
