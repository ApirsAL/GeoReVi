namespace GeoReVi
{
    using SQLite;
    using LiteDB;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [System.ComponentModel.DataAnnotations.Schema.Table("tblCountry")]
    public partial class tblCountry 
    {
        [StringLength(40)]
        public string NAME { get; set; }

        [StringLength(64)]
        public string CAPITAL { get; set; }

        [Key, BsonId]
        public int countrIdPk { get; set; }
    }
}
