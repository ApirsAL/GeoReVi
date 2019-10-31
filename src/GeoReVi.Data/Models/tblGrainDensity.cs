namespace GeoReVi
{
    using LiteDB;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [System.ComponentModel.DataAnnotations.Schema.Table("tblGrainDensity")]
    public partial class tblGrainDensity 
    {
        [Key, BsonId]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int gdIdFk { get; set; }

        public double? gdTemperature { get; set; }

        public double? gdCellVolume { get; set; }

        public double? gdMeanVolume { get; set; }

        public double? gdMeanVolumeStdDev { get; set; }

        public double? gdMeanDensity { get; set; }

        public double? gdMeanDensityStdDev { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "timestamp")]
        [System.ComponentModel.DataAnnotations.MaxLength(8)]
        [Timestamp]
        public byte[] SSMA_TimeStamp { get; set; }

        public virtual tblMeasurement tblMeasurement { get; set; }

    }
}
