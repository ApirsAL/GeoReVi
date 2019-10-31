namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tblPicturesRockSample 
    {
        [Key, BsonId]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int picIdFk { get; set; }

        [Key, BsonId]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int sampIdFk { get; set; }

        [StringLength(255)]
        public string picName { get; set; }

        [StringLength(255)]
        public string picStreamIdFk { get; set; }

        public virtual tblRockSample tblRockSample { get; set; }
    }
}
