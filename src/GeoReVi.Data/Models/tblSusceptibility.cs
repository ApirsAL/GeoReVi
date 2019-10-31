namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblSusceptibility")]
    public partial class tblSusceptibility 
    {
        [Key, BsonId]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int susfimeIdPk { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? susDate { get; set; }

        public double susValue { get; set; }

        public int? susAccuracy { get; set; }

        public string susNotes { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] SSMA_TimeStamp { get; set; }

        public virtual tblMeasurement tblMeasurement { get; set; }
    }
}
