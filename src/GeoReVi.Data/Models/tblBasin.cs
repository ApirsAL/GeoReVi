namespace GeoReVi
{
    using SQLite;
    using LiteDB;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    [System.ComponentModel.DataAnnotations.Schema.Table("tblBasin")]
    public partial class tblBasin 
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblBasin()
        {
            tblBasinLithoUnits = new HashSet<tblBasinLithoUnit>();
        }

        [Key, BsonId]
        public int basIdPk { get; set; }

        [Required]
        [StringLength(255)]
        public string basName { get; set; }

        [StringLength(50)]
        public string basType { get; set; }

        public double? basNorthSouthExtensionMeter { get; set; }

        public double? basEastWestExtensionMeter { get; set; }

        public double? basStrike { get; set; }

        [StringLength(50)]
        public string basShape { get; set; }

        public double? basDepthMaxMeter { get; set; }

        [StringLength(255)]
        public string basLiterature { get; set; }

        public int? basUserIdFk { get; set; }

        [StringLength(50)]
        public string basUserName { get; set; }

        [Ignore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblBasinLithoUnit> tblBasinLithoUnits { get; set; }
    }
}
