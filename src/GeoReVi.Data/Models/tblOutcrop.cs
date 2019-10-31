namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tblOutcrop 
    {
        [Key, BsonId]
        public int outIdPk { get; set; }

        [StringLength(255)]
        public string outLocalName { get; set; }

        [StringLength(255)]
        public string outArea { get; set; }

        [StringLength(255)]
        public string outCity { get; set; }

        public string outDescription { get; set; }

        public string outLocalityJourney { get; set; }

        public bool? outActive { get; set; }

        [StringLength(255)]
        public string outLiterature { get; set; }

        public int? outWidth { get; set; }

        public int? outHeight { get; set; }

        [StringLength(255)]
        public string outURL { get; set; }

        [StringLength(255)]
        public string outLastCondition { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? outLastJourney { get; set; }

        public bool? outWorkingPermission { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] SSMA_TimeStamp { get; set; }

        public bool? outOutcropArea { get; set; }
    }
}
