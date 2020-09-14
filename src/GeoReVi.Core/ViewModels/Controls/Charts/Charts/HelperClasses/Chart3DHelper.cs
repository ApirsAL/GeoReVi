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
    }
}
