namespace GeoReVi
{
    using LiteDB;
    using SQLite;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class tblFacy 
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblFacy()
        {
            tblFaciesLithostrats = new HashSet<tblFaciesLithostrat>();
            tblFaciesObservations = new HashSet<tblFaciesObservation>();
            tblLithofaciesArchitecturalElements = new HashSet<tblLithofaciesArchitecturalElement>();
            tblRockSamples = new HashSet<tblRockSample>();
            tblSectionLithofacies = new HashSet<tblSectionLithofacy>();
        }

        [Key, BsonId]
        public int facIdPk { get; set; }

        [Required]
        [StringLength(10)]
        public string facCode { get; set; }

        [StringLength(50)]
        public string facChronostratigraphy { get; set; }

        [StringLength(50)]
        public string facColor { get; set; }

        [StringLength(50)]
        public string facType { get; set; }

        [StringLength(50)]
        public string facMineralContentMacroscopic { get; set; }

        [StringLength(50)]
        public string facTexture { get; set; }

        [StringLength(255)]
        public string facNotes { get; set; }

        [StringLength(50)]
        public string facInterpretersName { get; set; }

        [StringLength(255)]
        public string facPetrographicTerm { get; set; }

        public int? facInterpreterId { get; set; }

        public int? facprjIdFk { get; set; }

        [StringLength(50)]
        public string facProjectName { get; set; }

        [Ignore]
        public virtual tblBiochemicalFacy tblBiochemicalFacy { get; set; }

        [Ignore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblFaciesLithostrat> tblFaciesLithostrats { get; set; }

        [Ignore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblFaciesObservation> tblFaciesObservations { get; set; }

        [Ignore]
        public virtual tblIgneousFacy tblIgneousFacy { get; set; }

        [Ignore]
        public virtual tblVolcanicFacy tblVolcanicFacy { get; set; }

        [Ignore]
        public virtual tblUnionPetrography tblUnionPetrography { get; set; }

        [Ignore]
        public virtual tblLithofacy tblLithofacy { get; set; }

        [Ignore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblLithofaciesArchitecturalElement> tblLithofaciesArchitecturalElements { get; set; }

        [Ignore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblRockSample> tblRockSamples { get; set; }

        [Ignore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblSectionLithofacy> tblSectionLithofacies { get; set; }
    }
}
