namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tblTransect 
    {
        [Key, BsonId]
        public int traIdPk { get; set; }

        [StringLength(255)]
        public string traName { get; set; }

        [Required]
        [StringLength(50)]
        public string traType { get; set; }

        public double? traLength { get; set; }

        public double? traDepthMeter { get; set; }

        public double? traDepthTWT { get; set; }

        [Column(TypeName = "date")]
        public DateTime? traProductionDate { get; set; }

        [StringLength(255)]
        public string traMeasurementDevice { get; set; }

        [StringLength(255)]
        public string traMeasurementCompany { get; set; }

        public double? traLatNorthEnd { get; set; }

        public double? traLongNorthEnd { get; set; }

        public double? traLatSouthEnd { get; set; }

        public double? traLongSouthEnd { get; set; }
    }
}
