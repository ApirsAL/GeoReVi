namespace GeoReVi
{
    using SQLite;
    using LiteDB;
    using System.ComponentModel.DataAnnotations;

    [System.ComponentModel.DataAnnotations.Schema.Table("tblBasinLithoUnit")]
    public partial class tblBasinLithoUnit 
    {
        [Key, BsonId]
        public int baslitIdPk { get; set; }

        public int basIdFk { get; set; }

        public int lithID { get; set; }

        [Ignore]
        public virtual tblBasin tblBasin { get; set; }
        [Ignore]
        public virtual tblUnionLithostratigraphy tblUnionLithostratigraphy { get; set; }
    }
}
