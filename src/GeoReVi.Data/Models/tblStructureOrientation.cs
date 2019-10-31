namespace GeoReVi
{
    using LiteDB;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("tblStructureOrientation")]
    public partial class tblStructureOrientation 
    {
        [Key, BsonId]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int sofimeIdFk { get; set; }

        public int? soDipDirection { get; set; }

        public int? soDipAngle { get; set; }

        public string soNotes { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] SSMA_TimeStamp { get; set; }

        [StringLength(50)]
        public string soMeasuredGeometricalStructure { get; set; }

        [StringLength(50)]
        public string soMeasuredGeologicalStructure { get; set; }

        public double? soAccuracy { get; set; }

        public virtual tblMeasurement tblMeasurement { get; set; }
    }
}
