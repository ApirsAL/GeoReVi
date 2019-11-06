using System.ComponentModel;

namespace GeoReVi
{
    public enum DistanceType
    {
        [Description("Euclidean (fastest)")]
        Euclidean = 1,
        [Description("Dijkstra (slowest)")]
        Dijkstra = 2,
        [Description("A* (slow)")]
        AStar = 3
    }
}
