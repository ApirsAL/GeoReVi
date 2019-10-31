namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblPowder")]
    public partial class tblPowder 
    {
        [Key, BsonId]
        public int powIdPk { get; set; }

        [StringLength(255)]
        public string powSampleName { get; set; }

        [StringLength(255)]
        public string powFromSampleName { get; set; }

        public double? powGrainSize { get; set; }

        [StringLength(255)]
        public string powColor { get; set; }

        [StringLength(50)]
        public string powStorage { get; set; }

        [StringLength(50)]
        public string powPreparation { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] SSMA_TimeStamp { get; set; }
    }

    /// <summary>
    /// A powder sample
    /// </summary>
    public class PowderSample
    {
        public tblRockSample RockSample { get; set; }
        public tblPowder Powder { get; set; }
    }
}
