namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblSubgroup")]
    public partial class tblSubgroup 
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblSubgroup()
        {
            tblFormations = new HashSet<tblFormation>();
        }

        [Key, BsonId]
        public int sgIdPk { get; set; }

        public int? sggrIdFk { get; set; }

        [StringLength(255)]
        public string sgName { get; set; }

        [StringLength(255)]
        public string sgNameValidity { get; set; }

        public string sgDescription { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] SSMA_TimeStamp { get; set; }

        public string sgLithologicDescriptionShort { get; set; }

        [StringLength(255)]
        public string sgBaseBoundary { get; set; }

        [StringLength(255)]
        public string sgTopBoundary { get; set; }

        public double? sgMeanThickness { get; set; }

        public double? sgMaxThickness { get; set; }

        [StringLength(255)]
        public string sgNotes { get; set; }

        [StringLength(255)]
        public string sgCountries { get; set; }

        [StringLength(255)]
        public string sgStates { get; set; }

        [StringLength(255)]
        public string sgLiterature { get; set; }

        [Column(TypeName = "date")]
        public DateTime? sgDateOfDocumentation { get; set; }

        [StringLength(255)]
        public string sgTypeLocality { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblFormation> tblFormations { get; set; }

        public virtual tblGroup tblGroup { get; set; }
    }
}
