using System.ComponentModel;

namespace GeoReVi
{
    /// <summary>
    /// Boundary types for grid creation
    /// </summary>
    public enum BoundaryType
    {
        [Description("3D Bounding box")]
        Rectangular = 1,
        [Description("X-Y convex hull")]
        ConvexHullXY = 2,
        [Description("Two bounding surfaces")]
        TwoBoundingSurfaces = 4
    }
}
