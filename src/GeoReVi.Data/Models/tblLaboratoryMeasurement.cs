namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class tblLaboratoryMeasurement 
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblLaboratoryMeasurement()
        {
            tblFileLabMeasurements = new HashSet<tblFileLabMeasurement>();
        }

        [Key, BsonId]
        public int labmeIdPk { get; set; }

        [StringLength(255)]
        public string labmeParameter { get; set; }

        [StringLength(255)]
        public string labmeSampleName { get; set; }

        [StringLength(255)]
        public string labmeDevice { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? labmeDate { get; set; }

        [StringLength(255)]
        public string labmeUploader { get; set; }

        [StringLength(255)]
        public string labmeNotes { get; set; }

        public int? labmeprjIdFk { get; set; }

        public bool? labmeConfidential { get; set; }

        [StringLength(50)]
        public string labmeProjectName { get; set; }

        public int? labmeUploaderId { get; set; }

        [Column(TypeName = "date")]
        public DateTime? labmeUploadDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? labmeChangeDate { get; set; }

        public virtual tblApparentPermeability tblApparentPermeability { get; set; }

        public virtual tblAxialCompression tblAxialCompression  { get; set; }

        public virtual tblBulkDensity tblBulkDensity { get; set; }

        public virtual tblEffectivePorosity tblEffectivePorosity { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblFileLabMeasurement> tblFileLabMeasurements { get; set; }

        public virtual tblGrainDensity tblGrainDensity { get; set; }

        public virtual tblGrainSize tblGrainSize { get; set; }

        public virtual tblIntrinsicPermeability tblIntrinsicPermeability { get; set; }

        public virtual tblIsotopes tblIsotopes { get; set; }

        public virtual tblProject tblProject { get; set; }

        public virtual tblResistivity tblResistivity { get; set; }

        public virtual tblSonicWave tblSonicWave { get; set; }

        public virtual tblThermalConductivity tblThermalConductivity { get; set; }

        public virtual tblThermalDiffusivity tblThermalDiffusivity { get; set; }

        public virtual tblXRayFluorescenceSpectroscopy tblXRayFluorescenceSpectroscopy { get; set; }

        [NotMapped]
        public double Value { get; set; }
    }
}
