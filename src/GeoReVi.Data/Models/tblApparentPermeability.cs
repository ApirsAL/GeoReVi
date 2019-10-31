namespace GeoReVi
{
    using LiteDB;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [System.ComponentModel.DataAnnotations.Schema.Table("tblApparentPermeability")]
    public partial class tblApparentPermeability 
    {
        [Key, BsonId]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int apermIdFk { get; set; }

        [StringLength(255)]
        public string apermMeasurementDirection { get; set; }

        public double? apermTemperature { get; set; }

        public double? apermAirPressure { get; set; }

        public double? apermDifferentialPressure { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public double? apermInjectionPressure { get; set; }

        public double? apermValueM2 { get; set; }

        public double? apermLogM2 { get; set; }

        public double? apermValueMD { get; set; }

        [StringLength(4000)]
        public string apermData { get; set; }

        public double? apermLocalCoordinateX { get; set; }

        public double? apermLocalCoordinateY { get; set; }

        public double? apermLocalCoordinateZ { get; set; }

        public double? apermMeasurementAngle { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "timestamp")]
        [System.ComponentModel.DataAnnotations.MaxLength(8)]
        [Timestamp]
        public byte[] SSMA_TimeStamp { get; set; }

        public virtual tblMeasurement tblMeasurement { get; set; }
    }
}
