using System.ComponentModel;

namespace GeoReVi
{
    /// <summary>
    /// Geostatistical interpolation method
    /// </summary>
    public enum GeostatisticalInterpolationMethod
    {
        [Description("Inverse Distance Weighting")]
        IDW = 0,
        [Description("Ordinary Kriging")]
        OrdinaryKriging = 1,
        [Description("Simple Kriging")]
        SimpleKriging = 2,
        [Description("Universal Kriging")]
        UniversalKriging = 3,
        [Description("Kriging w. external drift")]
        KrigingWithExternalDrift = 4,
        [Description("Sequential Simulation")]
        SequentialGaussianSimulation = 5,
        [Description("Simulated Annealing")]
        SimulatedAnnealing = 6
    }
}
