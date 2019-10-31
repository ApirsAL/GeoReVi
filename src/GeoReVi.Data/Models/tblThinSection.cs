namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblThinSection")]
    public partial class tblThinSection 
    {
        [Key, BsonId]
        public int tsIdPk { get; set; }

        [Required]
        [StringLength(255)]
        public string tsLabel { get; set; }

        [StringLength(255)]
        public string tsPetrography { get; set; }

        [StringLength(255)]
        public string tsFromSample { get; set; }

        [StringLength(255)]
        public string tsStorage { get; set; }

        public bool? tsPorespace { get; set; }

        public bool? tsOrientation { get; set; }

        public bool? tsDestroyed { get; set; }

        [StringLength(50)]
        public string tsPreparation { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] SSMA_TimeStamp { get; set; }
    }

    /// <summary>
    /// A Thin section sample
    /// </summary>
    public class ThinSectionSample
    {
        public tblRockSample RockSample { get; set; }
        public tblThinSection ThinSection { get; set; }
    }
}
