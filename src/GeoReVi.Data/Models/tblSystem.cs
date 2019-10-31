namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblSystem")]
    public partial class tblSystem 
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblSystem()
        {
            tblSeries = new HashSet<tblSery>();
        }

        [Key, BsonId]
        public int sysIdPk { get; set; }

        public int? syseonIdFk { get; set; }

        public int? syseraIdFk { get; set; }

        public int? sysperIdFk { get; set; }

        [StringLength(255)]
        public string sysName { get; set; }

        public double? sysNumericalAgeLowerBoundary { get; set; }

        public double? sysPlusMinus { get; set; }

        public int? syschronIdFk { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp, BsonIgnore]
        public byte[] SSMA_TimeStamp { get; set; }

        [BsonIgnore]
        public virtual tblPeriod tblPeriod { get; set; }

        [BsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblSery> tblSeries { get; set; }

        [BsonIgnore]
        public virtual tblUnionChronostratigraphy tblUnionChronostratigraphy { get; set; }
    }
}
