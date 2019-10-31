namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblSubformation")]
    public partial class tblSubformation 
    {
        [Key, BsonId]
        public int sfIdPk { get; set; }

        public int? sfgrId { get; set; }

        public int? sfsgId { get; set; }

        public int? sffmId { get; set; }

        [StringLength(255)]
        public string sfName { get; set; }

        [StringLength(255)]
        public string sfBaseBoundary { get; set; }

        [StringLength(255)]
        public string sfTopBoundary { get; set; }

        [StringLength(255)]
        public string sfTypeLocality { get; set; }

        public double? sfMeanThickness { get; set; }

        public double? sfMaxThickness { get; set; }

        [StringLength(255)]
        public string sfLiterature { get; set; }

        [StringLength(255)]
        public string sfNotes { get; set; }

        public string sfDescription { get; set; }

        [StringLength(255)]
        public string sfCountries { get; set; }

        [StringLength(255)]
        public string sfStates { get; set; }

        public DateTime? sfDateOfDocumentation { get; set; }

        public virtual tblFormation tblFormation { get; set; }
    }
}
