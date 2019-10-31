namespace GeoReVi
{
    using LiteDB;
    using SQLite;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [System.ComponentModel.DataAnnotations.Schema.Table("tblFormation")]
    public partial class tblFormation 
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblFormation()
        {
            tblSubformations = new HashSet<tblSubformation>();
        }

        [Key, BsonId]
        public int fmIdPk { get; set; }

        public int fmgrId { get; set; }

        public int? fmsgIdFk { get; set; }

        [StringLength(255)]
        public string fmName { get; set; }

        public string fmDescription { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "timestamp")]
        [System.ComponentModel.DataAnnotations.MaxLength(8)]
        [Timestamp]
        public byte[] SSMA_TimeStamp { get; set; }

        [StringLength(50)]
        public string fmNameValidity { get; set; }

        [StringLength(255)]
        public string fmBaseBoundary { get; set; }

        [StringLength(255)]
        public string fmTopBoundary { get; set; }

        public double? fmMeanThickness { get; set; }

        public double? fmMaxThickness { get; set; }

        [StringLength(255)]
        public string fmTypeLocality { get; set; }

        [StringLength(255)]
        public string fmStates { get; set; }

        [StringLength(255)]
        public string fmCountries { get; set; }

        [StringLength(255)]
        public string fmNotes { get; set; }

        [StringLength(255)]
        public string fmLiterature { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "date")]
        public DateTime? fmDateOfDocumentation { get; set; }

        [Ignore]
        public virtual tblSubgroup tblSubgroup { get; set; }

        [Ignore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblSubformation> tblSubformations { get; set; }
    }
}
