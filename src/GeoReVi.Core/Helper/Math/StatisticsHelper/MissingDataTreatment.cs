using System.ComponentModel;

namespace GeoReVi
{
    /// <summary>
    /// A set of methods how to handle missing data values
    /// </summary>
    public enum MissingDataTreatment
    {
        [Description("Remove row")]
        RemoveRow = 1,
        [Description("Remove column")]
        RemoveColumn = 2,
        [Description("Arithmetic mean")]
        ArithmeticAverage = 3
    }
}
