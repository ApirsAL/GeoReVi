namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblSectionsPetrography")]
    public partial class tblSectionsPetrography 
    {
        [Key, BsonId]
        public int petsecIdPk { get; set; }

        [StringLength(255)]
        public string petsecooiName { get; set; }

        public int? petsecLat { get; set; }

        public int? petsecLong { get; set; }

        [StringLength(255)]
        public string petsecPetrgraphy { get; set; }

        public double? petsecTop { get; set; }

        public double? petsecBase { get; set; }

        public double? petsecThickness { get; set; }

        [StringLength(255)]
        public string petsecFossils { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] SSMA_TimeStamp { get; set; }

        public virtual tblUnionPetrography tblUnionPetrography { get; set; }
    }
}
