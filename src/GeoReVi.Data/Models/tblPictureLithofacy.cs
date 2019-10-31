namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class tblPictureLithofacy 
    {
        [Required]
        [StringLength(255)]
        public string picName { get; set; }

        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int lftIdFk { get; set; }

        [Key, BsonId]
        [Column(Order = 1)]
        public Guid picStreamIdFk { get; set; }
    }
}
