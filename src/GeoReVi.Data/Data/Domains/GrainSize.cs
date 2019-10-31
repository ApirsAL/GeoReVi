using System.ComponentModel;

namespace GeoReVi.Data.Data.Domains
{
    public enum GrainSize
    {
        [Description("Unknown")]
        Unknown = 0,
        [Description("Mud")]
        Mud = 1,
        [Description("Silt")]
        Silt = 2,
        [Description("Very fine sand")]
        VeryFineSand = 3,
        [Description("Fine sand")]
        FineSand = 4,
        [Description("Medium sand")]
        MediumSand = 5,
        [Description("Coarse sand")]
        CoarseSand = 6,
        [Description("Very coarse sand")]
        VeryCoarseSand = 7,
        [Description("Granule")]
        Granule = 8,
        [Description("Pepple")]
        Pebble = 9,
        [Description("Cobble")]
        Cobble = 10,
        [Description("Boulder")]
        Boulder = 11
    }
}