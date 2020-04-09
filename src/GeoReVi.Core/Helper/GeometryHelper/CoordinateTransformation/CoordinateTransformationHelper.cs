using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;

namespace GeoReVi
{
    /// <summary>
    /// A class to perform coordinate transformations and projections
    /// </summary>
    public class CoordinateTransformationHelper : PropertyChangedBase
    {
        
        //Ellipsoid model constants (actual values here are for WGS84) 
        private const double R = 6378137.0; //Earth radius in meters
        private SRIDReader rd = new SRIDReader();

        ///Source SRID (WGS84)
        private int sridSource = 4326;
        public int SRIDSource
        {
            get => sridSource;
            set
            {
                sridSource = value;
                NotifyOfPropertyChange(() => SRIDSource);
            }
        }

        ///Source SRID (WGS84)
        private int sridTarget= 31467;
        public int SRIDTarget
        {
            get => sridTarget;
            set
            {
                sridTarget = value;
                NotifyOfPropertyChange(() => SRIDTarget);
            }
        }

        /// <summary>
        /// Converts a latitude, longitude, height WGS84 point to projected UTM WGS84 coordinates
        /// </summary>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Longitude</param>
        /// <param name="elevation">Elevation</param>
        /// <param name="utmZone"></param>
        /// <returns></returns>
        public LocationTimeValue GeometryToGeography(double x, double y, double z, int utmZone = 32)
        {
            var cs1 = rd.GetCSbyID(SRIDSource);
            var cs2 = rd.GetCSbyID(SRIDTarget);                                          

            CoordinateTransformationFactory ctf = new CoordinateTransformationFactory();
            var ct = ctf.CreateFromCoordinateSystems(cs2, cs1);
            double[] point = ct.MathTransform.Transform(new double[] { x, y, z});
            return new LocationTimeValue(point[0], point[1], z);

        }

        /// <summary>
        /// Converts a location time value
        /// </summary>
        /// <param name="loc"></param>
        public async Task ConvertCoordinate(LocationTimeValue loc)
        {
            var cs1 = rd.GetCSbyID(SRIDSource);
            var cs2 = rd.GetCSbyID(SRIDTarget);

            CoordinateTransformationFactory ctf = new CoordinateTransformationFactory();
            var ct = ctf.CreateFromCoordinateSystems(cs1, cs2);
            double[] point = ct.MathTransform.Transform(new double[] { loc.X, loc.Y, loc.Z });
            loc.X = point[0];
            loc.Y = point[1];
        }

        /// <summary>
        /// Converts a location time value
        /// </summary>
        /// <param name="loc"></param>
        public void ConvertBack(ref LocationTimeValue loc)
        {
            var cs2 = rd.GetCSbyID(SRIDSource);
            var cs1 = rd.GetCSbyID(SRIDTarget);

            CoordinateTransformationFactory ctf = new CoordinateTransformationFactory();
            var ct = ctf.CreateFromCoordinateSystems(cs1, cs2);
            double[] point = ct.MathTransform.Transform(new double[] { loc.X, loc.Y, loc.Z });
            loc.X = point[0];
            loc.Y = point[1];
        }


        /// <summary>
        /// LatLonToWorldNormalMercator
        /// </summary>
        /// <param name="WGS84coordinate">Longitude/Latitude of the point.</param>
        /// <returns>A 2-element class with the World Mercator x and y values</returns>
        public static XY NormalMercatorProjection(double latitude, double longitude)
        {
            try
            {
                //Formula for x and y values according to https://web.archive.org/web/20130930144834/http://www.mercator99.webspace.virginmedia.com/mercator.pdf
                return new XY() { X = R * longitude, Y = R * Math.Log(Math.Tan(latitude/2+Math.PI/4)) };
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Automatically calculates the UTM zone of a given longitude
        /// </summary>
        /// <param name="longitude"></param>
        /// <returns></returns>
        private static int DetermineUTMZone(double longitude)
        {
            return Convert.ToInt32(longitude + 180 / 6) + 1;
        }

        /// <summary>
        /// Opening the list with all SRIDs
        /// </summary>
        public void OpenSRIDList()
        {
            try
            {
                System.Diagnostics.Process.Start(@"Media\CoordinateSystems\SRID.csv");
            }
            catch (Exception e)
            {
                ((ShellViewModel)IoC.Get<IShell>()).LogError(e);
            }
        }

        /// <summary>
        /// Converts a set of coordinates in a mesh
        /// </summary>
        /// <param name="mesh"></param>
        public async Task ConvertCoordinates(Mesh mesh)
        {
            try
            {
                for (int i = 0; i<mesh.Vertices.Count();i++)
                {
                    LocationTimeValue loc = mesh.Vertices[i];
                    await Task.WhenAll(ConvertCoordinate(loc));
                }
            }
            catch
            {

            }
        }
    }
}
