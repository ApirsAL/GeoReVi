namespace GeoReVi
{
    using LiteDB;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class tblSectionLithofacy 
    {
        [Key, BsonId]
        public int litsecIdPk { get; set; }

        public int litsecIdFk { get; set; }

        public int? litseclftId { get; set; }

        [StringLength(10)]
        public string litseclftCode { get; set; }

        public double? litsecTop { get; set; }

        public double? litsecBase { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public double? litsecThickness { get; set; }

        [StringLength(255)]
        public string litsecGrainSizeTop { get; set; }

        [StringLength(255)]
        public string litsecGrainSizeBase { get; set; }

        public string litsecFossils { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] SSMA_TimeStamp { get; set; }

        [StringLength(50)]
        public string litsecBaseType { get; set; }

        [StringLength(255)]
        public string litsecGrainSizeMin { get; set; }

        public virtual tblFacy tblFacy { get; set; }

        public virtual tblSection tblSection { get; set; }
    }
}
