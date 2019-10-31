namespace GeoReVi
{
    using SQLite;
    using LiteDB;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [System.ComponentModel.DataAnnotations.Schema.Table("tblDepositionalEnvironmentCatalogue")]
    public partial class tblDepositionalEnvironmentCatalogue 
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblDepositionalEnvironmentCatalogue()
        {
            tblArchitecturalElementsDepositionalEnvironments = new HashSet<tblArchitecturalElementsDepositionalEnvironment>();
            tblDepositionalEnvironmentLithostrats = new HashSet<tblDepositionalEnvironmentLithostrat>();
            tblPictureDepositionalEnvironments = new HashSet<tblPictureDepositionalEnvironment>();
        }

        [Key, BsonId]
        public int depenvIdPk { get; set; }

        [Required]
        [StringLength(255)]
        public string depenvLabelEnglish { get; set; }

        [StringLength(255)]
        public string depenvBasin { get; set; }

        [StringLength(50)]
        public string depenvSuperSetting { get; set; }

        [StringLength(50)]
        public string depenvSubSetting { get; set; }

        public int depenvfrmIdFk { get; set; }

        [StringLength(255)]
        public string depenvFormation { get; set; }

        [StringLength(255)]
        public string depenvCode { get; set; }

        public string depenvDescription { get; set; }

        public string depenvFossils { get; set; }

        public int? depenvLateralRange { get; set; }

        public int? depenvLateralRangeCorrected { get; set; }

        public int? depenvVerticalRange { get; set; }

        public int? depenvVerticalRangeCorrected { get; set; }

        [StringLength(255)]
        public string depenvBoundaryTop { get; set; }

        public int depenvBoundingSurfaceTop { get; set; }

        [StringLength(255)]
        public string depenvBoundaryBottom { get; set; }

        public int? depenvBoundingSurfaceBottom { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "timestamp")]
        [System.ComponentModel.DataAnnotations.MaxLength(8)]
        [Timestamp]
        public byte[] SSMA_TimeStamp { get; set; }

        public int? depenvbasIdFk { get; set; }

        public int? depenvUserIdFk { get; set; }

        public int? depenvProjectIdFk { get; set; }

        [StringLength(250)]
        public string depenvUser { get; set; }

        [StringLength(250)]
        public string depenvProject { get; set; }

        [StringLength(50)]
        public string depenvType { get; set; }

        [Ignore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblArchitecturalElementsDepositionalEnvironment> tblArchitecturalElementsDepositionalEnvironments { get; set; }
        [Ignore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblDepositionalEnvironmentLithostrat> tblDepositionalEnvironmentLithostrats { get; set; }
        [Ignore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblPictureDepositionalEnvironment> tblPictureDepositionalEnvironments { get; set; }
    }
}
