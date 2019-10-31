namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblPictureRockSample")]
    public partial class tblPictureRockSample 
    {
        [Required]
        [StringLength(255)]
        public string picName { get; set; }

        [Key, BsonId]
        public Guid picStreamIdFk { get; set; }

        [StringLength(255)]
        public string picPath { get; set; }

        public int? sampIdFk { get; set; }
    }
}
