using LiteDB;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoReVi
{
    [Table("tblGrainSize")]
    public class tblGrainSize
    {
        [Key, BsonId]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int gslabmeIdFk { get; set; }

        [StringLength(50)]
        public string gsScale { get; set; }

        public double gsAverage { get; set; }

        public double gsMaximum { get; set; }

        public double gsMinimum { get; set; }

        public double gsSizeDomain { get; set; }

        public virtual tblMeasurement tblMeasurement { get; set; }
    }
}
