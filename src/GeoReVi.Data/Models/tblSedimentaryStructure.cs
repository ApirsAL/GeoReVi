namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tblSedimentaryStructure 
    {
        [Key, BsonId]
        public int sedIdPk { get; set; }

        [Required]
        [StringLength(255)]
        public string sedName { get; set; }

        public string sedRockType { get; set; }

        public string sedFluidDynamics { get; set; }

        public double? sedFluidVelocityIn { get; set; }

        public string sedDescription { get; set; }

        public bool? sedPrimary { get; set; }
    }
}
