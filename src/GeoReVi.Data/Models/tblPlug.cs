namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblPlug")]
    public partial class tblPlug 
    {
        [Key, BsonId]
        public int plugIdPk { get; set; }

        [StringLength(255)]
        public string plugLabel { get; set; }

        public double? plugHeight { get; set; }

        public double? plugDiameter { get; set; }

        public bool? plugThinSection { get; set; }

        public bool? plugDestroyed { get; set; }

        public int? plugOrientationDipDirection { get; set; }

        public double? plugOrientationDipAngle { get; set; }

        [StringLength(255)]
        public string plugOrientationToBedding { get; set; }

        public double? plugBeddingDipDirection { get; set; }

        public double? plugBeddingDipAngle { get; set; }

        public int? plugStratificationDipDirection { get; set; }

        public int? plugStratificationDipAngle { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] SSMA_TimeStamp { get; set; }
    }

    /// <summary>
    /// A plug sample
    /// </summary>
    public class PlugSample
    {
        public tblRockSample RockSample { get; set; }
        public tblPlug Plug { get; set; }
    }
}
