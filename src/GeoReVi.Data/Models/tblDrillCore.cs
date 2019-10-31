namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblDrillCore")]
    public partial class tblDrillCore 
    {
        [Key, BsonId]
        public int dcIdPk { get; set; }

        [StringLength(255)]
        public string dcdrillNameFk { get; set; }

        public double? dcTopMeterBS { get; set; }

        public double? dcBottomMeterBS { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public double? dcLengthMeter { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [StringLength(276)]
        public string dcName { get; set; }

        public double? dcDiameterCm { get; set; }
    }
}
