namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblSection")]
    public partial class tblSection 
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblSection()
        {
            tblFileSections = new HashSet<tblFileSection>();
            tblSectionLithofacies = new HashSet<tblSectionLithofacy>();
        }

        [Key, BsonId]
        public int secIdPk { get; set; }

        [Required]
        [StringLength(50)]
        public string secName { get; set; }

        [Required]
        [StringLength(50)]
        public string secType { get; set; }

        public int secInterpreter { get; set; }

        public double? secLatitude { get; set; }

        public double? secLongitude { get; set; }

        [StringLength(255)]
        public string secOoiName { get; set; }

        public int? secInterpreterIdFk { get; set; }

        public int? secprjIdFk { get; set; }

        [StringLength(255)]
        public string secInterpreterName { get; set; }

        [StringLength(50)]
        public string secProjectName { get; set; }

        public double? secAltitude { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblFileSection> tblFileSections { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblSectionLithofacy> tblSectionLithofacies { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblLithoStratigraphySection> tblLithoStratigraphySection { get; set; }
    }
}
