using System.ComponentModel;

namespace GeoReVi
{
    /// <summary>
    /// Query operators for number comparison
    /// </summary>
    public enum NumberQueryOperator
    {
        [Description("<")]
        LowerThan = 1,
        [Description(">")]
        BiggerThan = 2,
        [Description("<=")]
        LowerOrSimilar = 3,
        [Description(">=")]
        BiggerOrSimilar = 4,
        [Description("=")]
        Similar = 5,
    }

    /// <summary>
    /// Query operators for text comparison
    /// </summary>
    public enum TextQueryOperator
    {
        [Description("contains")]
        Contains = 1,
        [Description("begins with")]
        BeginsWith = 2,
        [Description("ends with")]
        EndsWith = 3
    }
}
