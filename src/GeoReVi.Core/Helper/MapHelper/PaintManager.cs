using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GeoReVi
{
    public class PaintManager
    {
        #region Private Properties

        private Map _map;
        private MapLayer shapeLayer;
        private MapLayer pinLayer;
        private MapPolygon currentShape;
        private Location center;
        private Location currentLocation;
        private int coordIdx;
        private bool isDrawing;
        private string _drawingMode;

        #endregion

        #region Public properties

        public Color Color { get; set; }
        public double LineWidth { get; set; }
        public MapLayer ShapeLayer { get { return shapeLayer; } }
        public MapLayer PinLayer { get { return pinLayer; } }

        #endregion

        #region Constructor


        #endregion

        #region Public methods

        #endregion
        }
}
