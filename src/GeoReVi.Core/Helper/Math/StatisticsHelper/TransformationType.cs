using System.ComponentModel;

namespace GeoReVi
{
    /// <summary>
    /// Types of univariate data transformations
    /// </summary>
    public enum TransformationType
    {
        [Description("Z-Score transformation")]
        ZScore=1,
        [Description("Rescaling")]
        Rescaling=2,
        [Description("Normal space")]
        NormalSpace=3,
        [Description("Mean normalization")]
        MeanNormalization=4,
        [Description("Subtract mean")]
        SubtractMean = 5,
        [Description("Exponential")]
        Exponential=6,
        [Description("Logarithmic")]
        Logarithmic=7,
        [Description("Make elevation to value")]
        Elevation = 8,
        [Description("Quantile-Quantile transformation")]
        QuantileQuantileTransform = 9
    }
}
