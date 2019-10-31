namespace GeoReVi
{
    using SQLite;
    using LiteDB;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tblBoundingSurface 
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblBoundingSurface()
        {
            tblArchEleOccurences = new HashSet<tblArchEleOccurence>();
            tblArchEleOccurences1 = new HashSet<tblArchEleOccurence>();
            tblBoundingSurfaceArchEles = new HashSet<tblBoundingSurfaceArchEle>();
        }

        [Key, BsonId]
        public int bsIdPk { get; set; }

        public int? bsType { get; set; }

        public string bsDescription { get; set; }

        [StringLength(255)]
        public string bsHorizontalRange { get; set; }

        [StringLength(255)]
        public string bsVerticalRange { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "timestamp")]
        [System.ComponentModel.DataAnnotations.MaxLength(8)]
        [Timestamp]
        public byte[] SSMA_TimeStamp { get; set; }

        public int? bsLithostratigraphyIdFk { get; set; }

        [Ignore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblArchEleOccurence> tblArchEleOccurences { get; set; }
        [Ignore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblArchEleOccurence> tblArchEleOccurences1 { get; set; }
        [Ignore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblBoundingSurfaceArchEle> tblBoundingSurfaceArchEles { get; set; }
    }
}
