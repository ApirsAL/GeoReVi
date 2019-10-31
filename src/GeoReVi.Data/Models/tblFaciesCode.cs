namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblFaciesCode")]
    public partial class tblFaciesCode 
    {
        [Key, BsonId]
        public int fcIdPk { get; set; }

        [Required]
        [StringLength(10)]
        public string fcCode { get; set; }

        [Required]
        [StringLength(50)]
        public string fcDecoding { get; set; }

        [StringLength(255)]
        public string fcDescription { get; set; }

        [StringLength(255)]
        public string fcFaciesType { get; set; }

        [StringLength(50)]
        public string fcHierarchy { get; set; }
    }
}
