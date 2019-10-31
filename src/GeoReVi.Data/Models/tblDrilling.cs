namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tblDrilling 
    {
        [Key, BsonId]
        public int drillIdPk { get; set; }

        [StringLength(255)]
        public string drillName { get; set; }

        [StringLength(255)]
        public string drillDate { get; set; }

        public double? drillLength { get; set; }

        [StringLength(255)]
        public string drillSampleMaterial { get; set; }

        [StringLength(255)]
        public string drillLiterature { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] SSMA_TimeStamp { get; set; }

        [StringLength(255)]
        public string drillUsage { get; set; }

        [StringLength(255)]
        public string drillDrillingProcess { get; set; }

        [StringLength(255)]
        public string drillDrillingFluid { get; set; }

        public double? drillDip { get; set; }

        public double? drillDipDirection { get; set; }

        public bool? drillCored { get; set; }

        [StringLength(50)]
        public string drillDatabase { get; set; }

        [StringLength(50)]
        public string drillContact { get; set; }
    }
}
