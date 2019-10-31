namespace GeoReVi
{
    using LiteDB;
    using SQLite;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [System.ComponentModel.DataAnnotations.Schema.Table("tblFileLabMeasurement")]
    public partial class tblFileLabMeasurement 
    {
        [Key, BsonId]
        public int FilaIdPk { get; set; }

        [Required]
        [StringLength(255)]
        public string picName { get; set; }

        public int labmeIdFk { get; set; }

        public Guid picStreamIdFk { get; set; }

        [StringLength(255)]
        public string picPath { get; set; }
    }
}
