using LiteDB;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoReVi
{
    [System.ComponentModel.DataAnnotations.Schema.Table("tblHydraulicHead")]
    public partial class tblHydraulicHead
    {
        [Key, BsonId]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int hhmeasIdFk { get; set; }

        public double? hhHead { get; set; }

        public double? hhAccuracy { get; set; }

        public double? hhMeasurementTime { get; set; }

        public virtual tblMeasurement tblMeasurement { get; set; }
    }
}
