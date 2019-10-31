namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tblMeasuringDevice 
    {
        [Key, BsonId]
        public int mdIdPk { get; set; }

        [Required]
        [StringLength(255)]
        public string mdName { get; set; }

        [StringLength(255)]
        public string mdlCompany { get; set; }

        [StringLength(255)]
        public string mdInputParameter { get; set; }

        [StringLength(255)]
        public string mdOutputParameter { get; set; }

        [StringLength(255)]
        public string mdDescription { get; set; }

        [StringLength(255)]
        public string mdLocation { get; set; }

        [StringLength(255)]
        public string mdLaboratoryRoom { get; set; }

        public int? mdUploaderId { get; set; }

        [StringLength(100)]
        public string mdUploaderName { get; set; }
    }
}
