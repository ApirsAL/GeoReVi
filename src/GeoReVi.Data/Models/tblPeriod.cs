namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblPeriod")]
    public partial class tblPeriod 
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblPeriod()
        {
            tblSeries = new HashSet<tblSery>();
            tblSystems = new HashSet<tblSystem>();
        }

        [Key, BsonId]
        public int perIdPk { get; set; }

        public int? pereonIdFk { get; set; }

        public int? pereraIdFk { get; set; }

        [StringLength(255)]
        public string perName { get; set; }

        public double? perNumericalAgeLowerBoundary { get; set; }

        public double? perPlusMinus { get; set; }

        public int? perchronIdFk { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] SSMA_TimeStamp { get; set; }

        public virtual tblErathem tblErathem { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblSery> tblSeries { get; set; }

        public virtual tblUnionChronostratigraphy tblUnionChronostratigraphy { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblSystem> tblSystems { get; set; }
    }
}
