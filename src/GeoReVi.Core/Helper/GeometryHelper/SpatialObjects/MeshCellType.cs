using System.ComponentModel;

namespace GeoReVi
{
    public enum MeshCellType
    {
        OneDimensional = 0,
        [Description("Triangular")]
        Triangular = 1,
        [Description("Rectangular")]
        Rectangular = 2,
        [Description("Tetrahedal")]
        Tetrahedal = 3,
        [Description("Hexahedral")]
        Hexahedral = 4,
        Combined = 5
    }
}
