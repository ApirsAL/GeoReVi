namespace GeoReVi
{
    using LiteDB;
    using SQLite;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class tblIgneousFacy 
    {
        [Key, BsonId]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int iffacIdFk { get; set; }

        [StringLength(10)]
        public string ifOptionalCode { get; set; }

        [Required]
        [StringLength(50)]
        public string ifIgneousRockType { get; set; }

        [Required]
        [StringLength(50)]
        public string ifName { get; set; }

        [StringLength(50)]
        public string ifGeologicalObject { get; set; }

        [StringLength(50)]
        public string ifMineralSizePhenocrysts { get; set; }

        [StringLength(50)]
        public string ifMineralSizeMatrix { get; set; }

        [StringLength(50)]
        public string ifFabric { get; set; }

        [StringLength(255)]
        public string ifInterpretation { get; set; }

        public bool? ifExtrusive { get; set; }

        public double? ifAverageThicknessCentimeters { get; set; }

        public double? ifAverageLateralExtendCentimeters { get; set; }

        [StringLength(50)]
        public string ifTexture { get; set; }

        [Ignore]
        public virtual tblFacy tblFacy { get; set; }
    }
}
