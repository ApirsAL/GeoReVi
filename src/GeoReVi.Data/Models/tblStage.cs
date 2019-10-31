namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblStage")]
    public partial class tblStage 
    {
        [Key, BsonId]
        public int stageIdPk { get; set; }

        public int? stageeonIdFk { get; set; }

        public int? stageeraIdFk { get; set; }

        public int? stageperIdFk { get; set; }

        public int? stagesysIdFk { get; set; }

        public int? stageserIdFk { get; set; }

        [StringLength(255)]
        public string stageName { get; set; }

        public double? stageNumericalAgeLowerBoundary { get; set; }

        public double? stagePlusMinus { get; set; }

        public int? stagechronIdFk { get; set; }

        [BsonIgnore]
        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] SSMA_TimeStamp { get; set; }
        [BsonIgnore]
        public virtual tblSery tblSery { get; set; }
        [BsonIgnore]
        public virtual tblUnionChronostratigraphy tblUnionChronostratigraphy { get; set; }
    }
}
