namespace GeoReVi
{
    using LiteDB;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("tblSonicWave")]
    public partial class tblSonicWave 
    {
        [Key, BsonId]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int swIdFk { get; set; }

        [StringLength(255)]
        public string swWavetype { get; set; }

        public double? swSampleSaturation { get; set; }

        public double? swFrequency { get; set; }

        public double? swAmplitude { get; set; }

        public double? swPeriod { get; set; }

        public double? swVelocity { get; set; }

        [StringLength(255)]
        public string swMeasurementDirection { get; set; }

        public double? swLocalCoordinateX { get; set; }

        public double? swLocalCoordinateY { get; set; }

        public double? swLocalCoordinateZ { get; set; }

        public double? swMeasurementAngle { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] SSMA_TimeStamp { get; set; }

        public double? swMeasurementDisplacement { get; set; }

        public double? swMeasuringTime { get; set; }

        public virtual tblMeasurement tblMeasurement { get; set; }
    }

}
