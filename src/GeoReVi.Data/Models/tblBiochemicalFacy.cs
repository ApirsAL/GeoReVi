namespace GeoReVi
{
    using SQLite;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using LiteDB;

    public partial class tblBiochemicalFacy 
    {
        [Key, BsonId]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int bffacIdFk { get; set; }

        [Required]
        [StringLength(50)]
        public string bfType { get; set; }

        [Required]
        [StringLength(50)]
        public string bfName { get; set; }

        [StringLength(50)]
        public string bfGrainsizeComponents { get; set; }

        [StringLength(255)]
        public string bfTypeOfComponents { get; set; }

        public bool? bfComponentSupported { get; set; }

        [StringLength(50)]
        public string bfPrimarySedimentaryStructure { get; set; }

        [StringLength(50)]
        public string bfSecondarySedimentaryStructure { get; set; }

        [StringLength(50)]
        public string bfGrainsizeMatrix { get; set; }

        [StringLength(50)]
        public string bfRounding { get; set; }

        public double? bfRatioComponentsPerc { get; set; }

        public bool? bfInSitu { get; set; }

        [StringLength(50)]
        public string bfFossilContent { get; set; }

        [StringLength(255)]
        public string bfDescription { get; set; }

        public double? bfThicknessMaxCentimeter { get; set; }

        public double? bfLateralRangeMaxCentimeter { get; set; }

        [StringLength(50)]
        public string bfSetHeight { get; set; }

        [StringLength(50)]
        public string bfCement { get; set; }
        
        [Ignore]
        public virtual tblFacy tblFacy { get; set; }
    }
}
