using System.ComponentModel;

namespace GeoReVi
{
    /// <summary>
    /// The type of drift
    /// </summary>
    public enum Drift
    {
        [Description("Spatial function")]
        SpatialFunction = 0,
        [Description("Drift parameter")]
        DriftParameter = 1,
    }
}
