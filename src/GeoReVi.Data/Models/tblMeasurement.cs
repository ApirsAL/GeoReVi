using LiteDB;
using System;
using System.ComponentModel.DataAnnotations;

namespace GeoReVi
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public partial class tblMeasurement
    {
        [Key, BsonId]
        public int measIdPk { get; set; }

        [StringLength(255)]
        public string measType { get; set; }

        [StringLength(255)]
        public string measParameter { get; set; }

        [StringLength(50)]
        public string measResultType { get; set; }

        [StringLength(50)]
        public string measRockSampleIdFk { get; set; }

        [StringLength(255)]
        public string measObjectOfInvestigationIdFk { get; set; }

        [StringLength(255)]
        public string measLithostratigraphy { get; set; }

        [StringLength(255)]
        public string measChronostratigraphy { get; set; }

        [StringLength(255)]
        public string measDevice { get; set; }

        [StringLength(255)]
        public string measPetrography { get; set; }

        [StringLength(50)]
        public string measFacies { get; set; }

        [StringLength(50)]
        public string measArchitecturalElement { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "datetime")]
        public DateTime? measDate { get; set; }

        public double? measLocalCoordinateX { get; set; }

        public double? measLocalCoordinateY { get; set; }

        public double? measLocalCoordinateZ { get; set; }

        public double? measLatitude { get; set; }

        public double? measLongitude { get; set; }

        public double? measElevation { get; set; }

        [StringLength(255)]
        public string measDirection { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "timestamp")]
        [System.ComponentModel.DataAnnotations.MaxLength(8)]
        [Timestamp]
        public byte[] SSMA_TimeStamp { get; set; }

        [StringLength(255)]
        public string measProject { get; set; }

        public int? measprjIdFk { get; set; }

        public int? measUploaderId { get; set; }

        public int? measFaciesIdFk { get; set; }

        public int? measArchitecturalElementIdFk { get; set; }

        public int? measDepositionalEnvironmentIdFk { get; set; }

        [StringLength(50)]
        public string measUploader { get; set; }

        public int? measAnalyticalDeviceId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "date")]
        public DateTime? measUploadDate { get; set; }

        public virtual tblStructureOrientation tblStructureOrientation { get; set; }

        public virtual tblSpectralGammaRay tblSpectralGammaRay { get; set; }

        public virtual tblSusceptibility tblSusceptibility { get; set; }

        public virtual tblTotalGammaRay tblTotalGammaRay { get; set; }

        public virtual tblSonicLog tblSonicLog { get; set; }

        public virtual tblRockQualityDesignationIndex tblRockQualityDesignationIndex { get; set; }

        public virtual tblBoreholeTemperature tblBoreholeTemperature { get; set; }

        public virtual tblApparentPermeability tblApparentPermeability { get; set; }

        public virtual tblAxialCompression tblAxialCompression { get; set; }

        public virtual tblBulkDensity tblBulkDensity { get; set; }

        public virtual tblEffectivePorosity tblEffectivePorosity { get; set; }

        public virtual tblGrainDensity tblGrainDensity { get; set; }

        public virtual tblGrainSize tblGrainSize { get; set; }

        public virtual tblHydraulicHead tblHydraulicHead { get; set; }

        public virtual tblIntrinsicPermeability tblIntrinsicPermeability { get; set; }

        public virtual tblIsotopes tblIsotopes { get; set; }

        public virtual tblResistivity tblResistivity { get; set; }

        public virtual tblSonicWave tblSonicWave { get; set; }

        public virtual tblThermalConductivity tblThermalConductivity { get; set; }

        public virtual tblThermalDiffusivity tblThermalDiffusivity { get; set; }

        public virtual tblXRayFluorescenceSpectroscopy tblXRayFluorescenceSpectroscopy { get; set; }
    }
}
