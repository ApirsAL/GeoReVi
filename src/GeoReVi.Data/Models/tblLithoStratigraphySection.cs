using LiteDB;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoReVi
{
    public partial class tblLithoStratigraphySection
    {
        [Key, BsonId]
        public int lithosecIdPk { get; set; }

        public int lithosecIdFk { get; set; }

        public double? lithosecTop { get; set; }

        public double? lithosecBase { get; set; }

        [StringLength(255)]
        public string lithosecLithostratigraphy { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public double? lithosecThickness { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] SSMA_TimeStamp { get; set; }


        public virtual tblSection tblSection { get; set; }
    }
}
