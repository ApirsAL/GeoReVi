using LiteDB;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoReVi
{
    [Table("tblResistivity")]
    public partial class tblResistivity
    {
        [Key, BsonId]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int reslabmeIdFk { get; set; }

        public double? resValue { get; set; }

        [StringLength(50)]
        public string resDirection { get; set; }

        public double ? resSaturation { get; set; }

        public virtual tblMeasurement tblMeasurement { get; set; }
    }
}
