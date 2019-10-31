using LiteDB;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoReVi
{
    [Table("tblIsotopes")]
    public class tblIsotopes
    {
        [Key, BsonId]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int islabmeIdFk { get; set; }

        [StringLength(255)]
        public string isIsotope { get; set; }

        public double isValue { get; set; }

        public double isMeasurementTime { get; set; }

        public virtual tblMeasurement tblMeasurement { get; set; }
    }

    public enum Isotope
    {
        [Description("14C")]
        C13 = 1,
        [Description("13C")]
        C14 = 2,
        [Description("16O")]
        O16 = 3,
        [Description("18O")]
        O18 = 4
    }
}
