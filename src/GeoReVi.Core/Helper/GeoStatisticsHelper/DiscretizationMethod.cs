using System.ComponentModel;

namespace GeoReVi
{
    /// <summary>
    /// The discretization method that should be applied on a set of discrete points
    /// </summary>
    public enum DiscretizationMethod
    {
        [Description("Hexahedral regular grid")]
        Hexahedral = 1,
        [Description("Random grid")]
        Random = 2,
        [Description("Octree grid")]
        Octree = 3,
    }

    /// <summary>
    /// How refinement should be done on constraints
    /// </summary>
    public enum Refinement
    {
        [Description("No refinement")]
        None = 1,
        [Description("Refine at constraints")]
        AtConstraints = 2,
        [Description("Refine at all points in the model")]
        AtAllPoints = 3
    }
}
