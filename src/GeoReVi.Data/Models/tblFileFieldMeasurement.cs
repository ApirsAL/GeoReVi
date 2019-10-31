namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.ComponentModel.DataAnnotations;

    [System.ComponentModel.DataAnnotations.Schema.Table("tblFileFieldMeasurement")]
    public partial class tblFileFieldMeasurement 
    {
        [Key, BsonId]
        public int filaIdPk { get; set; }

        [Required]
        [StringLength(255)]
        public string filName { get; set; }

        public int fimeIdFk { get; set; }

        public Guid filStreamIdFk { get; set; }

        [StringLength(255)]
        public string filPath { get; set; }
    }
}
