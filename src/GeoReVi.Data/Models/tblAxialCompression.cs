using LiteDB;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoReVi
{
    [Table("tblAxialCompression")]
    public class tblAxialCompression
    {
        [Key, BsonId]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int aclabmeIdFk { get; set; }

        [StringLength(50)]
        public string acType { get; set; }

        public double acAxialStressMax { get; set; }

        public double acAxialStrainAtStressMax { get; set; }

        public double acAxialStrainMax { get; set; }

        public double acLateralStress { get; set; }

        [StringLength(50)]
        public string acStrainMeasuringMethod { get; set; }

        [StringLength(50)]
        public string acLoadControl { get; set; }

        public double acYoungsModulus { get; set; }

        public double acPoissonRatio { get; set; }

        public virtual tblMeasurement tblMeasurement { get; set; }

        public virtual tblAxialCompressionCurve tblAxialCompressionCurve { get; set; }
    }
}
