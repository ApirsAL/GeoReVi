namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tblLithofacy 
    {
        [Key, BsonId]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int facIdFk { get; set; }

        [StringLength(50)]
        public string lftCodeOptional { get; set; }

        [StringLength(50)]
        public string lftGrainsizeMax { get; set; }

        [StringLength(50)]
        public string lftGrainsizeMin { get; set; }

        [StringLength(50)]
        public string lftGrainsizeAverage { get; set; }

        [StringLength(50)]
        public string lftSorting { get; set; }

        [StringLength(50)]
        public string lftGrainForm { get; set; }

        [StringLength(50)]
        public string lftSphericity { get; set; }

        [StringLength(50)]
        public string lftCement { get; set; }

        [StringLength(255)]
        public string lftPrimarySedimentaryStructure { get; set; }

        [StringLength(255)]
        public string lftSecondarySedimentaryStructure { get; set; }

        [StringLength(255)]
        public string lftInterpretation { get; set; }

        [StringLength(255)]
        public string lftBasin { get; set; }

        public double? lftThicknessMax { get; set; }

        public double? lftThicknessMin { get; set; }

        public double? lftLateralRange { get; set; }

        [StringLength(50)]
        public string lftSetHeight { get; set; }

        public virtual tblFacy tblFacy { get; set; }
    }
}
