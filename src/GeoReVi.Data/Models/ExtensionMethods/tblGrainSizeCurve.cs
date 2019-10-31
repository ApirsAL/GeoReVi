using LiteDB;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoReVi
{
    [Table("tblGrainSizeCurve")]
    public class tblGrainSizeCurve
    {
        [Key, BsonId]
        public int gscIdPk { get; set; }

        public int gscGsIdFk { get; set; }

        public double gsGrainSizeMM { get; set; }

        public double gsPercentage { get; set; }

        public tblGrainSize tblGrainSize { get; set; }
    }
}
