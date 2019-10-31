namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblThermalDiffusivity")]
    public partial class tblThermalDiffusivity
    {
        [Key, BsonId]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int tdIdFk { get; set; }

        public int? tdtcIdFk { get; set; }

        [StringLength(255)]
        public string tcMeasurementDirection { get; set; }

        public double? tdtcAvValue { get; set; }

        public double? tdtcMinValue { get; set; }

        public double? tdtcMaxValue { get; set; }

        public double? tdtcStandardDev { get; set; }

        public double? tdtcHeterogeneity { get; set; }

        public double? tdAvValue { get; set; }

        public double? tdMinValue { get; set; }

        public double? tdMaxValue { get; set; }

        [StringLength(255)]
        public string tdFile { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] SSMA_TimeStamp { get; set; }

        public double? tdSampleSaturation { get; set; }

        [StringLength(255)]
        public string tdUsedReferenceSample { get; set; }

        public virtual tblMeasurement tblMeasurement { get; set; }
    }
}
