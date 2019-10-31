namespace GeoReVi
{
    using LiteDB;
    using SQLite;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class tblFieldMeasurement 
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblFieldMeasurement()
        {
            tblFileFieldMeasurements = new HashSet<tblFileFieldMeasurement>();
        }

        [Key, BsonId]
        public int fimeIdPk { get; set; }

        [StringLength(255)]
        public string fimType { get; set; }

        [StringLength(255)]
        public string fimeObjectOfInvestigation { get; set; }

        [StringLength(255)]
        public string fimeLithostratigraphy { get; set; }

        [StringLength(255)]
        public string fimeChronostratigraphy { get; set; }

        [StringLength(255)]
        public string fimeDevice { get; set; }

        [StringLength(50)]
        public string fimeFacies { get; set; }

        [StringLength(50)]
        public string fimeArchitecturalElement { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "datetime")]
        public DateTime? fimeDate { get; set; }

        public double? fimeLocalCoordinateX { get; set; }

        public double? fimeLocalCoordinateY { get; set; }

        public double? fimeLocalCoordinateZ { get; set; }

        public double? fimeLatitude { get; set; }

        public double? fimeLongitude { get; set; }

        public double? fimeElevation { get; set; }

        [StringLength(255)]
        public string fimeDirection { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "timestamp")]
        [System.ComponentModel.DataAnnotations.MaxLength(8)]
        [Timestamp]
        public byte[] SSMA_TimeStamp { get; set; }

        [StringLength(255)]
        public string fimeProject { get; set; }

        public int? fimeprjIdFk { get; set; }

        public int? fimeUploaderId { get; set; }

        public int? fimeFaciesIdFk { get; set; }

        public int? fimeArchitecturalElementIdFk { get; set; }

        public int? fimeDepositionalEnvironmentIdFk { get; set; }

        [StringLength(50)]
        public string fimeUploader { get; set; }

        public int? fimeAnalyticalDeviceId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "date")]
        public DateTime? fimeUploadDate { get; set; }

        [Ignore]
        public virtual tblProject tblProject { get; set; }

        [Ignore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblFileFieldMeasurement> tblFileFieldMeasurements { get; set; }

        [Ignore]
        public virtual tblStructureOrientation tblStructureOrientation { get; set; }

        [Ignore]
        public virtual tblSpectralGammaRay tblSpectralGammaRay { get; set; }

        [Ignore]
        public virtual tblSusceptibility tblSusceptibility { get; set; }

        [Ignore]
        public virtual tblTotalGammaRay tblTotalGammaRay { get; set; }

        [Ignore]
        public virtual tblSonicLog tblSonicLog { get; set; }

        [Ignore]
        public virtual tblRockQualityDesignationIndex tblRockQualityDesignationIndex { get; set; }

        [Ignore]
        public virtual tblBoreholeTemperature tblBoreholeTemperature { get; set; }

    }
}
