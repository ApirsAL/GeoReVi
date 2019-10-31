using System.Runtime.Serialization;

namespace GeoReVi.Helper.Math.GeometryHelper.DataContracts
{
    [DataContract]
    public class OpenElevationResult
    {
        public OpenElevationPoint[] results;
    }

    [DataContract]
    public class OpenElevationPoint
    {
        public double latitude;
        public double longitude;
        public double elevation;
    }

    [DataContract]
    public class OpenElevationRequest
    {
        public OpenElevationRequestPoint[] locations;
    }

    [DataContract]
    public class OpenElevationRequestPoint
    {
        public double latitude;
        public double longitude;
    }
}
