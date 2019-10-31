using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace GeoReVi
{
    public static class ColorMapHelper
    {
        /// <summary>
        /// Returning a brush based on a value
        /// </summary>
        /// <param name="z">Actual value</param>
        /// <param name="zmin">Minimum</param>
        /// <param name="zmax">Maximum</param>
        /// <param name="cm">A colormap object</param>
        /// <returns></returns>
        public static SolidColorBrush GetBrush(double z, double zmin, double zmax, ColormapBrush cm, double opacity = 1)
        {
            SolidColorBrush brush = new SolidColorBrush();

            int colorIndex = (int)(((cm.ColormapLength - 1) * (z - zmin) + zmax - z) / (zmax - zmin));

            if (colorIndex < 0)
                colorIndex = 0;

            if (colorIndex >= cm.ColormapLength)
                colorIndex = cm.ColormapLength - 1;

            if (cm.ColormapBrushes == null || cm.ColormapBrushes.Count == 0)
                cm.CalculateColormapBrushes(opacity);

            brush = cm.ColormapBrushes[colorIndex].Brush;

            //brush.Freeze();

            return brush;
        }
    }
}