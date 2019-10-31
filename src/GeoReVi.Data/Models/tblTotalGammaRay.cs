namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("tblTotalGammaRay")]
    public partial class tblTotalGammaRay 
    {
        [Key, BsonId]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int tgrfimeIdPk { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? tgrDate { get; set; }

        public int? tgrMeasuringTime { get; set; }

        public double? tgrTotalCounts { get; set; }

        public double? tgrValueCPS { get; set; }

        public double? tgrValueAPI { get; set; }

        public double? tgrAccuracy { get; set; }

        public string tgrNotes { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] SSMA_TimeStamp { get; set; }

        public virtual tblMeasurement tblMeasurement { get; set; }
    }
}
