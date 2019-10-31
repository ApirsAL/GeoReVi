namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblPictureBasin")]
    public partial class tblPictureBasin 
    {
        [Required]
        [StringLength(255)]
        public string picName { get; set; }

        [Key, BsonId]
        [Column(Order = 0)]
        [StringLength(50)]
        public string basLabelFk { get; set; }

        [Key, BsonId]
        [Column(Order = 1)]
        public Guid picStreamIdFk { get; set; }
    }
}
