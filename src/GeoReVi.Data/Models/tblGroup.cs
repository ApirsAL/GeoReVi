namespace GeoReVi
{
    using LiteDB;
    using SQLite;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [System.ComponentModel.DataAnnotations.Schema.Table("tblGroup")]
    public partial class tblGroup 
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblGroup()
        {
            tblSubgroups = new HashSet<tblSubgroup>();
        }

        [Key, BsonId]
        public int grIdPk { get; set; }

        [StringLength(255)]
        public string grName { get; set; }

        [StringLength(255)]
        public string grNameValidity { get; set; }

        [StringLength(255)]
        public string grLithologicDescriptionShort { get; set; }

        [StringLength(255)]
        public string grBaseBoundary { get; set; }

        [StringLength(255)]
        public string grTopBoundary { get; set; }

        public double? grMeanThickness { get; set; }

        public double? grMaxThickness { get; set; }

        [StringLength(255)]
        public string grTypeLocality { get; set; }

        [StringLength(255)]
        public string grCountries { get; set; }

        [StringLength(255)]
        public string grStates { get; set; }

        public string grNotes { get; set; }

        [StringLength(255)]
        public string grLiterature { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "date")]
        public DateTime? grDateDocumentation { get; set; }

        [Ignore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblSubgroup> tblSubgroups { get; set; }
    }
}
