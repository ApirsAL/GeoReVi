namespace GeoReVi
{
    using LiteDB;
    using SQLite;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [System.ComponentModel.DataAnnotations.Schema.Table("tblFileSection")]
    public partial class tblFileSection 
    {
        [Key, BsonId]
        public int filaIdPk { get; set; }

        [Required]
        [StringLength(255)]
        public string filName { get; set; }

        public int secIdFk { get; set; }

        public Guid filStreamIdFk { get; set; }

        [StringLength(255)]
        public string filPath { get; set; }

        [Ignore]
        public virtual tblSection tblSection { get; set; }
    }
}
