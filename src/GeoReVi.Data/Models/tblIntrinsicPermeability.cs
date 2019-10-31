namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblIntrinsicPermeability")]
    public partial class tblIntrinsicPermeability 
    {
        [Key, BsonId]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int inpeIdFk { get; set; }

        public decimal? inpeLength { get; set; }

        public int? inpeDifferentialPressure { get; set; }

        public decimal? inpeRadius { get; set; }

        public double? inpeValuem2 { get; set; }

        public double? inpeValuemD { get; set; }

        public string inpeArrayPressure { get; set; }

        public string inpeArrayDifferentialPressure { get; set; }

        public string inpeArrayApparentPermm2 { get; set; }

        public double? inpeAccuracym2 { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] SSMA_TimeStamp { get; set; }

        public double? inpeTemperature { get; set; }

        public string inpeArrayDischarge { get; set; }

        public virtual tblMeasurement tblMeasurement { get; set; }
    }

}
