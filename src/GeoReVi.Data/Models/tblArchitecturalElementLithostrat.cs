namespace GeoReVi
{
    using SQLite;
    using LiteDB;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [System.ComponentModel.DataAnnotations.Schema.Table("tblArchitecturalElementLithostrat")]
    public partial class tblArchitecturalElementLithostrat 
    {
        [Key, BsonId]
        public int aelithIdPk { get; set; }

        public int aeIdFk { get; set; }

        public int litIdFk { get; set; }

        [Ignore]
        public virtual tblArchitecturalElement tblArchitecturalElement { get; set; }

        [Ignore]
        public virtual tblUnionLithostratigraphy tblUnionLithostratigraphy { get; set; }
    }
}
