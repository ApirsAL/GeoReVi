namespace GeoReVi
{
    using LiteDB;
    using SQLite;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [System.ComponentModel.DataAnnotations.Schema.Table("tblErathem")]
    public partial class tblErathem 
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblErathem()
        {
            tblPeriods = new HashSet<tblPeriod>();
        }

        [Key, BsonId]
        public int eraIdPk { get; set; }

        public int? eraeonIdFk { get; set; }

        [StringLength(255)]
        public string eraName { get; set; }

        public double? eraNumericalAgeLowerBoundary { get; set; }

        public int? eraPlusMinus { get; set; }

        public int? erachronIdFk { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "timestamp")]
        [System.ComponentModel.DataAnnotations.MaxLength(8)]
        [Timestamp, BsonIgnore]
        public byte[] SSMA_TimeStamp { get; set; }

        [Ignore, BsonIgnore]
        public virtual tblEonothem tblEonothem { get; set; }
        [Ignore, BsonIgnore]
        public virtual tblUnionChronostratigraphy tblUnionChronostratigraphy { get; set; }
        [Ignore, BsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblPeriod> tblPeriods { get; set; }
    }
}
