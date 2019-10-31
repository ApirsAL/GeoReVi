using System.ComponentModel;

namespace GeoReVi
{
    public enum CategorizationMethod
    {
        [Description("Nearest Neighbor")]
        NearestNeighbor = 1,
        [Description("k-Means cluster analysis")]
        kMeansCluster = 2,
        [Description("Indicator kriging")]
        IndicatorKriging = 3
    }
}
