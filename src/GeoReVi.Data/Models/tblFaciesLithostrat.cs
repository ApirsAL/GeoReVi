namespace GeoReVi
{
    using LiteDB;
    using SQLite;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [System.ComponentModel.DataAnnotations.Schema.Table("tblFaciesLithostrat")]
    public partial class tblFaciesLithostrat 
    {
        [Key, BsonId]
        public int faclithIdPk { get; set; }

        public int facIdFk { get; set; }

        public int litIdFk { get; set; }

        [Ignore]
        public virtual tblFacy tblFacy { get; set; }
        [Ignore]
        public virtual tblUnionLithostratigraphy tblUnionLithostratigraphy { get; set; }
    }
}
