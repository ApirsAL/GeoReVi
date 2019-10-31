namespace GeoReVi
{
    using SQLite;
    using LiteDB;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [System.ComponentModel.DataAnnotations.Schema.Table("tblArchitecturalElement")]
    public partial class tblArchitecturalElement 
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblArchitecturalElement()
        {
            tblArchEleOccurences = new HashSet<tblArchEleOccurence>();
            tblArchitecturalElementLithostrats = new HashSet<tblArchitecturalElementLithostrat>();
            tblArchitecturalElementsDepositionalEnvironments = new HashSet<tblArchitecturalElementsDepositionalEnvironment>();
            tblBoundingSurfaceArchEles = new HashSet<tblBoundingSurfaceArchEle>();
            tblLithofaciesArchitecturalElements = new HashSet<tblLithofaciesArchitecturalElement>();
        }

        [Key, BsonId]
        public int aeIdPk { get; set; }

        [Required]
        [StringLength(255)]
        public string aeLabelEnglish { get; set; }

        [StringLength(255)]
        public string aeCode { get; set; }

        public string aeDescription { get; set; }

        [StringLength(255)]
        public string aeBoundaryTop { get; set; }

        [StringLength(255)]
        public string aeBoundaryBase { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "timestamp")]
        [System.ComponentModel.DataAnnotations.MaxLength(8)]
        [Timestamp]
        public byte[] SSMA_TimeStamp { get; set; }

        public int? aeprjIdFk { get; set; }

        public int? aeUserIdFk { get; set; }

        [StringLength(250)]
        public string aeProject { get; set; }

        [StringLength(250)]
        public string aeUser { get; set; }

        [StringLength(50)]
        public string aeType { get; set; }

        [Ignore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblArchEleOccurence> tblArchEleOccurences { get; set; }

        [Ignore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblArchitecturalElementLithostrat> tblArchitecturalElementLithostrats { get; set; }

        [Ignore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblArchitecturalElementsDepositionalEnvironment> tblArchitecturalElementsDepositionalEnvironments { get; set; }

        [Ignore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblBoundingSurfaceArchEle> tblBoundingSurfaceArchEles { get; set; }

        [Ignore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblLithofaciesArchitecturalElement> tblLithofaciesArchitecturalElements { get; set; }
    }
}
