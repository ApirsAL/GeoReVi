namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tblVolcanicFacy 
    {
        [Key, BsonId]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int vffacIdFk { get; set; }

        [StringLength(10)]
        public string vfOptionalCode { get; set; }

        [Required]
        [StringLength(50)]
        public string vfVolcanicRockType { get; set; }

        [Required]
        [StringLength(50)]
        public string vfName { get; set; }

        [StringLength(255)]
        public string vfDescription { get; set; }

        [StringLength(50)]
        public string vfGeologicalObject { get; set; }

        [StringLength(255)]
        public string vfMineralogyMacroscopic { get; set; }

        [StringLength(50)]
        public string vfMineralSizePhenocrysts { get; set; }

        [StringLength(50)]
        public string vfMineralSizeMatrix { get; set; }

        [StringLength(50)]
        public string vfFabric { get; set; }

        [StringLength(255)]
        public string vfInterpretation { get; set; }

        public bool? vfExtrusive { get; set; }

        public double? vfAverageThicknessCentimeters { get; set; }

        public double? vfAverageLateralExtendCentimeters { get; set; }

        [StringLength(50)]
        public string vfTexture { get; set; }

        public virtual tblFacy tblFacy { get; set; }
    }
}
