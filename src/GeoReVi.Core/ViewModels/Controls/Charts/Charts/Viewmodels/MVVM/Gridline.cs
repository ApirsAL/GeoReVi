using Caliburn.Micro;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;

namespace GeoReVi
{
    public class Gridline : PropertyChangedBase
    {

        #region Public properties

        /// <summary>
        /// The points of the gridline
        /// </summary>
        private PointCollection points = new PointCollection();
        [XmlIgnore()]
        public PointCollection Points
        {
            get
            {
                return new PointCollection(new List<Point>() { new Point(X1, Y1), new Point(X2, Y2) });
            }
        }

        /// <summary>
        /// X1 coordinate
        /// </summary>
        private double x1 = 0;
        public double X1
        {
            get => this.x1;
            set
            {
                this.x1 = value;
                NotifyOfPropertyChange(() => X1);
                NotifyOfPropertyChange(() => Points);
            }
        }

        /// <summary>
        /// Y coordinate
        /// </summary>
        private double y1 = 0;
        public double Y1
        {
            get => this.y1;
            set
            {
                this.y1 = value;
                NotifyOfPropertyChange(() => Y1);
                NotifyOfPropertyChange(() => Points);
            }
        }

        /// <summary>
        /// X2 coordinate
        /// </summary>
        private double x2 = 0;
        public double X2
        {
            get => this.x2;
            set
            {
                this.x2 = value;
                NotifyOfPropertyChange(() => X2);
                NotifyOfPropertyChange(() => Points);
            }
        }

        /// <summary>
        /// Y coordinate
        /// </summary>
        private double y2 = 0;
        public double Y2
        {
            get => this.y2;
            set
            {
                this.y2 = value;
                NotifyOfPropertyChange(() => Y2);
                NotifyOfPropertyChange(() => Points);
            }
        }

        /// <summary>
        /// Dash array of the gridline
        /// </summary>
        private DoubleCollection strokeDashArray = new DoubleCollection(new double[4] { 4, 2, 1, 2 });
        public DoubleCollection StrokeDashArray
        {
            get => this.strokeDashArray;
            set
            {
                this.strokeDashArray = value;
                NotifyOfPropertyChange(() => StrokeDashArray);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public Gridline()
        {

        }

        #endregion
    }
}
