using System.ComponentModel;

namespace GeoReVi
{
    public enum PhysicalProblem
    {
        [Description("Heat conduction")]
        HeatConduction = 1,
        [Description("Heat conduction and convection")]
        HeadConductionAndConvection = 2,
        [Description("Diffusive mass transport")]
        DiffusiveMassTransport = 3
    }
}
