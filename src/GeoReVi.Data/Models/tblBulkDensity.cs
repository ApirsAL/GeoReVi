namespace GeoReVi
{
    using LiteDB;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [System.ComponentModel.DataAnnotations.Schema.Table("tblBulkDensity")]
    public partial class tblBulkDensity 
    {
        [Key, BsonId]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int bdIdFk { get; set; }

        public double? bdValue { get; set; }

        public double? bdAccuracy { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "timestamp")]
        [System.ComponentModel.DataAnnotations.MaxLength(8)]
        [Timestamp]
        public byte[] SSMA_TimeStamp { get; set; }

        public virtual tblMeasurement tblMeasurement { get; set; }
    }
}
