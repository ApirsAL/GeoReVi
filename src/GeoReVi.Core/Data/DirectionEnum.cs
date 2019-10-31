using System;
using System.ComponentModel;
namespace GeoReVi
{
    /// <summary>
    /// Provides the direction/orientation of an object
    /// </summary>
    [Serializable]
    public enum DirectionEnum
    {
        [Description("X-direction")]
        X = 1,
        [Description("Y-direction")]
        Y = 2,
        [Description("Z-direction")]
        Z = 3,
        [Description("XY-direction")]
        XY = 4,
        [Description("XZ-direction")]
        XZ = 5,
        [Description("YZ-direction")]
        YZ = 6,
        [Description("XYZ-direction")]
        XYZ = 7,
        [Description("Directionless")]
        Directionless = 8
    }
}
