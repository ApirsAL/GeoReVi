namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblPictureArchitecturalElement")]
    public partial class tblPictureArchitecturalElement 
    {
        [Required]
        [StringLength(255)]
        public string picName { get; set; }

        [Key, BsonId]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int aeIdFk { get; set; }

        [Key, BsonId]
        [Column(Order = 1)]
        public Guid picStreamIdFk { get; set; }

        [StringLength(255)]
        public string picPath { get; set; }
    }
}
