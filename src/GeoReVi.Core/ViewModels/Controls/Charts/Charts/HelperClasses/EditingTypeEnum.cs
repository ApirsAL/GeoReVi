using System.ComponentModel;

namespace GeoReVi
{
    /// <summary>
    /// An enum that defines the type of editing that should be applied
    /// </summary>
    public enum EditingTypeEnum
    {
        [Description("Select points")]
        SelectPoints = 0,
        [Description("Add points")]
        AddPoints = 1,
        [Description("Extract vertical section")]
        ExtractVerticalSection = 2,
        [Description("Extract horizontal section")]
        ExtractHorizontalSection = 3,
        [Description("Extract vertical profile")]
        ExtractVerticalProfile = 4,
        [Description("Extract horizontal profile")]
        ExtractHorizontalProfile = 5
    }
}
