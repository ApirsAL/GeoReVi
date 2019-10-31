namespace GeoReVi
{
    using LiteDB;
    using System.ComponentModel.DataAnnotations;

    public partial class tblAlia 
    {
        [Key, BsonId]
        public int alIdPk { get; set; }

        [StringLength(50)]
        public string alColumnName { get; set; }

        [StringLength(50)]
        public string alAlias { get; set; }

        [StringLength(50)]
        public string alTableName { get; set; }
        
        [StringLength(50)]
        public string alTableAlias { get; set; }

        public bool alImport { get; set; }

        [StringLength(50)]
        public string alDataType { get; set; }
    }
}
