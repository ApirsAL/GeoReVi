using LiteDB;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoReVi
{
    [Table("tblAxialCompressionCurve")]
    public class tblAxialCompressionCurve
    {
        [Key, BsonId]
        public int accIdPk { get; set; }

        public int accAcIdFk { get; set; }

        public double accA1 { get; set; }

        public double accA2 { get; set; }

        public double accA3 { get; set; }

        public double accA4 { get; set; }

        public double accA5 { get; set; }

        public double accA6 { get; set; }

        public double accB1 { get; set; }
                         
        public double accB2 { get; set; }
                         
        public double accB3 { get; set; }
                         
        public double accB4 { get; set; }
                         
        public double accB5 { get; set; }
                         
        public double accB6 { get; set; }

        public virtual tblAxialCompression tblAxialCompression { get; set; }
    }
}
