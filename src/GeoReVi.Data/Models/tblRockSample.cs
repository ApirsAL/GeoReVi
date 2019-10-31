namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblRockSample")]
    public partial class tblRockSample 
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblRockSample()
        {
            tblPicturesRockSamples = new HashSet<tblPicturesRockSample>();
        }

        [Key, BsonId]
        public int sampIdPk { get; set; }

        public int? sampLithofacies { get; set; }

        public int? sampArchitecturalElementIdFk { get; set; }

        public int? sampDepositionalEnvironmentIdFk { get; set; }

        [StringLength(255)]
        public string sampLabel { get; set; }

        [StringLength(255)]
        public string sampType { get; set; }

        [StringLength(50)]
        public string sampSamplingMethod { get; set; }

        [StringLength(255)]
        public string sampooiName { get; set; }

        [StringLength(255)]
        public string sampLithostratigraphyName { get; set; }

        [StringLength(255)]
        public string sampChronStratName { get; set; }

        [StringLength(255)]
        public string sampPetrographicTerm { get; set; }

        [StringLength(255)]
        public string sampGeolocationWKT { get; set; }

        public double? sampLocalXCoordinates { get; set; }

        public double? sampLocalYCoordinates { get; set; }

        public double? sampLocalZCoordinates { get; set; }

        public double? sampMass { get; set; }

        [StringLength(255)]
        public string sampSampler { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? sampDate { get; set; }

        public string sampNotes { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] SSMA_TimeStamp { get; set; }

        public int? sampprjIdFk { get; set; }

        public double? sampLatitude { get; set; }

        public double? sampLongitude { get; set; }

        public double? sampElevation { get; set; }

        public bool? sampConfidential { get; set; }

        [StringLength(255)]
        public string sampProject { get; set; }

        [StringLength(50)]
        public string sampFaciesFk { get; set; }

        [StringLength(50)]
        public string sampArchitecturalElement { get; set; }

        [StringLength(50)]
        public string sampDepositionalEnvironment { get; set; }

        public bool? sampThinSectionAvailable { get; set; }

        public int? sampUploaderIdFk { get; set; }

        [StringLength(100)]
        public string sampUploaderName { get; set; }

        public virtual tblFacy tblFacy { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblPicturesRockSample> tblPicturesRockSamples { get; set; }

        public virtual tblProject tblProject { get; set; }

        public virtual tblUnionChronostratigraphy tblUnionChronostratigraphy { get; set; }

        public virtual tblUnionPetrography tblUnionPetrography { get; set; }
    }
}
