using System;
using System.Windows.Media.Media3D;

namespace GeoReVi
{
    /// <summary>
    /// A static class for 3D transformations
    /// </summary>
    public static class Chart3DHelper
    {
        public static Matrix3D AzimuthElevation(double elevation, double azimuth)
        { 
            // Make sure elevation is in the range of [-90, 90]: 
            if (elevation > 90)
                elevation = 90;
            else if (elevation < -90)
                elevation = -90;
            // Make sure azimuth is in the range of [-180, 180]: 
            if (azimuth > 180)
                azimuth = 180;
            else if (azimuth < -180)
                azimuth = -180;

            elevation = elevation * Math.PI / 180;

            azimuth = azimuth * Math.PI / 180;

            double sne = Math.Sin(elevation);

            double cne = Math.Cos(elevation);

            double sna = Math.Sin(azimuth);
            double cna = Math.Cos(azimuth);

            return new Matrix3D(cna, -sne * sna, cne * sna, 0, sna, sne * cna, -cne * cna, 0, 0, cne, sne, 0, 0, 0, 0, 1);
        }

        //public static void Peak3D(DataSeries3D ds)
        //{
        //    double xmin = -3; double xmax = 3; double ymin = -3; double ymax = 3;
        //    ds.XLimitMin = xmin; ds.YLimitMin = ymin; ds.XSpacing = 0.2; ds.YSpacing = 0.2; ds.XNumber = Convert.ToInt16((xmax - xmin) / ds.XSpacing) + 1; ds.YNumber = Convert.ToInt16((ymax - ymin) / ds.YSpacing) + 1;
        //    Point3D[,] pts = new Point3D[ds.XNumber, ds.YNumber]; for (int i = 0; i < ds.XNumber; i++) { for (int j = 0; j < ds.YNumber; j++) { double x = ds.XLimitMin + i * ds.XSpacing; double y = ds.YLimitMin + j * ds.YSpacing; double z = 3 * Math.Pow((1 - x), 2) * Math.Exp(-x * x - (y + 1) * (y + 1)) - 10 * (0.2 * x - Math.Pow(x, 3) - Math.Pow(y, 5)) * Math.Exp(-x * x - y * y) - 1 / 3 * Math.Exp(-(x + 1) * (x + 1) - y * y); pts[i, j] = new Point3D(x, y, z); } }
        //    ds.PointArray = pts;
        //}
        //public static void Sinc3D(DataSeries3D ds)
        //{
        //    double xmin = -8; double xmax = 8; double ymin = -8; double ymax = 8;
        //    ds.XLimitMin = xmin; ds.YLimitMin = ymin; ds.XSpacing = 0.5; ds.YSpacing = 0.5; ds.XNumber = Convert.ToInt16((xmax - xmin) / ds.XSpacing) + 1; ds.YNumber = Convert.ToInt16((ymax - ymin) / ds.YSpacing) + 1;
        //    Point3D[,] pts = new Point3D[ds.XNumber, ds.YNumber]; for (int i = 0; i < ds.XNumber; i++) { for (int j = 0; j < ds.YNumber; j++) { double x = ds.XLimitMin + i * ds.XSpacing; double y = ds.YLimitMin + j * ds.YSpacing; double r = Math.Sqrt(x * x + y * y) + 0.000001; double z = Math.Sin(r) / r; pts[i, j] = new Point3D(x, y, z); } }
        //    ds.PointArray = pts;
        //}
    }
}
