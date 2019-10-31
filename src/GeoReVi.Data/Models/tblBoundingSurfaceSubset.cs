namespace GeoReVi
{
    using SQLite;
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [System.ComponentModel.DataAnnotations.Schema.Table("tblBoundingSurfaceSubset")]
    public partial class tblBoundingSurfaceSubset 
    {
        [Key, BsonId]
        public int susuIdPk { get; set; }

        [Ignore]
        public Guid? susuGuid { get; set; }

        public DateTime? susuInsertDate { get; set; }

        [BsonIgnore]
        public DbGeography susuPointCollection { get; set; }

        public int? susuProjectIdFk { get; set; }

        [StringLength(255)]
        public string susuProjectName { get; set; }

        public int? susuUserIdFk { get; set; }

        [StringLength(255)]
        public string susuUserName { get; set; }
    }
}
