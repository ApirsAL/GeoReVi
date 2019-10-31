namespace GeoReVi
{
    using LiteDB;
    using SQLite;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [System.ComponentModel.DataAnnotations.Schema.Table("tblEffectivePorosity")]
    public partial class tblEffectivePorosity 
    {
        [Key, BsonId]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int porIdFk { get; set; }

        public double? porValuePerc { get; set; }

        public double? porValueDec { get; set; }

        public double? porStandardDeviationDec { get; set; }

        public double? porAccuracy { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "timestamp")]
        [System.ComponentModel.DataAnnotations.MaxLength(8)]
        [Timestamp]
        public byte[] SSMA_TimeStamp { get; set; }

        [StringLength(50)]
        public string porType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "date")]
        public DateTime? porDateCreated { get; set; }

        public virtual tblMeasurement tblMeasurement { get; set; }
    }
}
