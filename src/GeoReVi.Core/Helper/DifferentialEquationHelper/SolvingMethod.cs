using System.ComponentModel;

namespace GeoReVi
{
    public enum SolvingMethod
    {
        [Description("Finite differences method")]
        FiniteDifferenceMethod = 0,
        [Description("Finite volume method")]
        FiniteVolumeMethod = 1,
        [Description("Finite element method")]
        FiniteElementsMethod=2
    }
}
