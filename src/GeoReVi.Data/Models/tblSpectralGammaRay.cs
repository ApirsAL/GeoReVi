namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("tblSpectralGammaRay")]
    public partial class tblSpectralGammaRay 
    {
        [Key, BsonId]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int sgrIdPk { get; set; }

        [StringLength(50)]
        public string sgType { get; set; }

        [Column(TypeName = "date")]
        public DateTime? sgDate { get; set; }

        public int? sgMeasuringTime { get; set; }

        public int? sgTotalCounts { get; set; }

        public double? sgPotassiumCPS { get; set; }

        public double? sgPotassiumPercent { get; set; }

        public double? sgUraniumCPS { get; set; }

        public double? sgUraniumPPM { get; set; }

        public double? sgThoriumCPS { get; set; }

        public double? sgThoriumPPM { get; set; }

        public int? sgAccuracy { get; set; }

        public string sgNotes { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] SSMA_TimeStamp { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public double? sgAPI { get; set; }

        public virtual tblMeasurement tblMeasurement { get; set; }
    }

    public enum Radionuclide
    {
        [Description("K [%]")]
        K = 1,
        [Description("U [ppm]")]
        U = 2,
        [Description("Th [ppm]")]
        Th = 3,
        [Description("Total radiation [API]")]
        API = 4
    }
}
