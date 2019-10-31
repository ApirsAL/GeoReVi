using Caliburn.Micro;
using System;
using System.Windows.Media;

namespace GeoReVi
{
    /// <summary>
    /// Box Whisker Object
    /// </summary>
    public class BoxWhiskerPlot : BarPolygon
    {
        //Point collection for whisker lines
        public BindableCollection<LineSeries> WhiskerLines { get; set; }

        //Outlier points
        public LineSeries Outliers { get; set; }

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BoxWhiskerPlot()
        {
            PolygonPoints = new PointCollection();
            WhiskerLines = new BindableCollection<LineSeries>();
            Outliers = new LineSeries();
        }

        #endregion
    }
}
