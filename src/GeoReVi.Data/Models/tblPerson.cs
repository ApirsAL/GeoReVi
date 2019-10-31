namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tblPerson 
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblPerson()
        {
            tblFaciesObservations = new HashSet<tblFaciesObservation>();
            tblMessages = new HashSet<tblMessage>();
            tblMessages1 = new HashSet<tblMessage>();
            tblProjects = new HashSet<tblProject>();
        }

        [Key, BsonId]
        public int persIdPk { get; set; }

        [StringLength(255)]
        public string persName { get; set; }

        [StringLength(255)]
        public string persVorname { get; set; }

        [StringLength(255)]
        public string perStatus { get; set; }

        [Required]
        [StringLength(255)]
        public string persUserName { get; set; }

        [StringLength(50)]
        public string persAffiliation { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [StringLength(511)]
        public string persFullName { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblFaciesObservation> tblFaciesObservations { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblMessage> tblMessages { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblMessage> tblMessages1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblProject> tblProjects { get; set; }
    }
}
