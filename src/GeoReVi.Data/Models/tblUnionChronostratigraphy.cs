namespace GeoReVi
{
    using LiteDB;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("tblUnionChronostratigraphy")]
    public partial class tblUnionChronostratigraphy 
    {
        [BsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblUnionChronostratigraphy()
        {
            tblEonothems = new HashSet<tblEonothem>();
            tblErathems = new HashSet<tblErathem>();
            tblPeriods = new HashSet<tblPeriod>();
            tblRockSamples = new HashSet<tblRockSample>();
            tblSeries = new HashSet<tblSery>();
            tblStages = new HashSet<tblStage>();
            tblSystems = new HashSet<tblSystem>();
        }

        [Key, BsonId]
        [StringLength(255)]
        public string eonName { get; set; }

        [BsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblEonothem> tblEonothems { get; set; }

        [BsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblErathem> tblErathems { get; set; }

        [BsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblPeriod> tblPeriods { get; set; }

        [BsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblRockSample> tblRockSamples { get; set; }

        [BsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblSery> tblSeries { get; set; }

        [BsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblStage> tblStages { get; set; }

        [BsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblSystem> tblSystems { get; set; }
    }
}
