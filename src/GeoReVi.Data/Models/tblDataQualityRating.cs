namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblDataQualityRating")]
    public partial class tblDataQualityRating 
    {
        [Key, BsonId]
        public int id { get; set; }

        public int dqrNumber { get; set; }

        [Required]
        [StringLength(10)]
        public string dqrExpression { get; set; }

        [Required]
        [StringLength(255)]
        public string dqrDescription { get; set; }

        [StringLength(50)]
        public string dqrTableName { get; set; }

        public int? dqrTableIdFk { get; set; }
    }
}
