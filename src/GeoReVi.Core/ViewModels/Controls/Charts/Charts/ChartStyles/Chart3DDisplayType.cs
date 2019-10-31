using System.ComponentModel;

namespace GeoReVi
{
    /// <summary>
    /// How a set of 3D points should be displayed
    /// </summary>
    public enum Chart3DDisplayType
    {
        [Description("Scatter chart")]
        Scatter = 1,
        [Description("Gradient chart")]
        Gradient = 2,
        [Description("Line chart")]
        Line = 3,
        [Description("Surface chart")]
        Surface = 4,
        [Description("Image")]
        Image = 5,
        [Description("Volumetric")]
        Volumetric = 6,
        [Description("Loaded model")]
        Model = 7
    }
}
