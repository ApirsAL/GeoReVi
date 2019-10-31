using System;
using System.Windows.Media;

namespace GeoReVi
{
    /// <summary>
    /// A polygon object for a bar chart
    /// </summary>
    public class BarPolygon
    {
        #region Private members

        //Fill color for the rectangles
        private Brush fillColor = Brushes.Black;

        //Color of the border
        private Brush borderColor = Brushes.Black;

        //thickness of the border
        private double borderThickness = 2;

        //Name of the polygon
        private string name = "";

        //The value of the polygon
        private double value = 0;

        #endregion

        #region Public properties

        //fill color property
        public Brush FillColor
        {
            get { return this.fillColor; }
            set { this.fillColor = value; }
        }


        //Border color property
        public Brush BorderColor
        {
            get { return this.borderColor; }
            set { this.borderColor = value; }
        }

        //Border thickness property
        public double BorderThickness
        {
            get { return this.borderThickness; }
            set { this.borderThickness = value; }
        }

        //Border thickness property
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        //The value of the polygon
        public double Value
        {
            get => this.value;
            set => this.value = value;
        }

        //Points of the polygon
        public PointCollection PolygonPoints { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BarPolygon()
        {
            PolygonPoints = new PointCollection();
        }

        #endregion
    }
}
