namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tblSery 
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblSery()
        {
            tblStages = new HashSet<tblStage>();
        }

        [Key, BsonId]
        public int serIdPk { get; set; }

        public int? sereonIdFk { get; set; }

        public int? sereraIdFk { get; set; }

        public int? serperIdFk { get; set; }

        public int? sersysIdFk { get; set; }

        [StringLength(255)]
        public string serName { get; set; }

        public double? serNumericalAgeLowerBoundary { get; set; }

        public double? serPlusMinus { get; set; }

        public int? serchronIdFk { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp, BsonIgnore]
        public byte[] SSMA_TimeStamp { get; set; }

        [BsonIgnore]
        public virtual tblPeriod tblPeriod { get; set; }
        [BsonIgnore]
        public virtual tblSystem tblSystem { get; set; }
        [BsonIgnore]
        public virtual tblUnionChronostratigraphy tblUnionChronostratigraphy { get; set; }
        [BsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblStage> tblStages { get; set; }
    }
}
