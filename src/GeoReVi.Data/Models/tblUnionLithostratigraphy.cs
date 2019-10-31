namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblUnionLithostratigraphy")]
    public partial class tblUnionLithostratigraphy 
    {
        [BsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblUnionLithostratigraphy()
        {
            tblArchitecturalElementLithostrats = new HashSet<tblArchitecturalElementLithostrat>();
            tblBasinLithoUnits = new HashSet<tblBasinLithoUnit>();
            tblDepositionalEnvironmentLithostrats = new HashSet<tblDepositionalEnvironmentLithostrat>();
            tblFaciesLithostrats = new HashSet<tblFaciesLithostrat>();
        }

        [Required]
        [StringLength(255)]
        public string grName { get; set; }

        [BsonId]
        public int ID { get; set; }

        public int? unionLithUploaderIdFk { get; set; }

        [StringLength(255)]
        public string chronostratNameFk { get; set; }

        [BsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblArchitecturalElementLithostrat> tblArchitecturalElementLithostrats { get; set; }

        [BsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblBasinLithoUnit> tblBasinLithoUnits { get; set; }

        [BsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblDepositionalEnvironmentLithostrat> tblDepositionalEnvironmentLithostrats { get; set; }

        [BsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblFaciesLithostrat> tblFaciesLithostrats { get; set; }
    }
}
