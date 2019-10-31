using System.Windows.Controls;

namespace GeoReVi
{
    public class ChartStyleBase
    {
        #region Private members

        private double xmin = -1.1;

        private double xmax = 1.1;

        private double ymin = -1.1;
        private double y2min = -1.1;

        private double ymax = 1.1;
        private double y2max = 1.1;

        private double zmin = -1.1;

        private double zmax = 1.1;

        #endregion

        #region Public properties

        //Chart canvas object
        public Canvas ChartCanvas { get; set; }
        //Xmin
        public double Xmin { get { return xmin; } set { xmin = value; } }
        //Xmax
        public double Xmax { get { return xmax; } set { xmax = value; } }
        //Ymin
        public double Ymin { get { return ymin; } set { ymin = value; } }
        //Ymin
        public double Y2min { get { return y2min; } set { y2min = value; } }
        //Ymax
        public double Ymax { get { return ymax; } set { ymax = value; } }
        //Ymax
        public double Y2max { get { return y2max; } set { y2max = value; } }
        //Zmin
        public double Zmin { get { return zmin; } set { zmin = value; } }
        //Zmax
        public double Zmax { get { return zmax; } set { zmax = value; } }

        #endregion

        #region Constructor

        #endregion
    }
}