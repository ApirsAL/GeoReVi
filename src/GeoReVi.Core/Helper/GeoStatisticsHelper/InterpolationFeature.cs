using System.ComponentModel;

namespace GeoReVi
{
    /// <summary>
    /// The interpolation feature
    /// </summary>
    public enum InterpolationFeature
    {
        [Description("Value")]
        Value = 1,
        [Description("Longitude/x")]
        Longitude = 2,
        [Description("Latitude/y")]
        Latitude = 3,
        [Description("Elevation/z")]
        Elevation = 4
    }
}
