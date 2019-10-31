namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblThermalConductivity")]
    public partial class tblThermalConductivity 
    {
        [Key, BsonId]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int tcIdFk { get; set; }

        [StringLength(255)]
        public string tcMeasurementDirection { get; set; }

        public double? tcAvValue { get; set; }

        public double? tcMinValue { get; set; }

        public double? tcMaxValue { get; set; }

        public double? tcStandardDev { get; set; }

        public double? tcHeterogeneity { get; set; }

        [StringLength(255)]
        public string tcFileName { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] SSMA_TimeStamp { get; set; }

        public double? tcSampleSaturation { get; set; }

        [StringLength(255)]
        public string tcUsedReferenceSample { get; set; }

        public virtual tblMeasurement tblMeasurement { get; set; }
    }
}
