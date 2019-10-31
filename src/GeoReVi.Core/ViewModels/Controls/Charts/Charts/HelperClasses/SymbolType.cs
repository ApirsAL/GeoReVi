using System.ComponentModel;

namespace GeoReVi
{
    /// <summary>
    /// Enumerator for the symbol type
    /// </summary>
    public enum SymbolTypeEnum
    {
        [Description("None")]
        None = 0,
        [Description("Χ")]
        Cross = 1,
        [Description("♦")]
        Diamond = 2,
        [Description("•")]
        Dot = 3,
        [Description("∇")]
        InvertedTriangle = 4,
        [Description("Box")]
        Box = 5,
        [Description("Star")]
        Star = 6,
        [Description("Δ (open)")]
        Triangle = 7,
    }
}
