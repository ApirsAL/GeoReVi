using LiteDB;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoReVi
{
    [Table("tblRockQualityDesignationIndex")]
    public class tblRockQualityDesignationIndex
    {
        [Key, BsonId]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int rqdfimeIdFk { get; set; }

        public double rqdValue { get; set; }

        [StringLength(100)]
        public string rqdType { get; set; }

        public double rqdZfrom { get; set; }
        public double rqdZto { get; set; }
        public double rqdXfrom { get; set; }
        public double rqdXto { get; set; }
        public double rqdYfrom { get; set; }
        public double rqdYto { get; set; }

        public virtual tblMeasurement tblMeasurement { get; set; }
    }
}
