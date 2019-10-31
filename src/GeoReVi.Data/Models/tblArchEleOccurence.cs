namespace GeoReVi
{
    using LiteDB;
    using SQLite;
    using System.ComponentModel.DataAnnotations;

    public partial class tblArchEleOccurence 
    {
        [Key, BsonId]
        public int aoIdPk { get; set; }

        public int aeIdFk { get; set; }

        public int aoOoiIdFk { get; set; }

        [Required]
        [StringLength(255)]
        public string aoInterpreter { get; set; }

        public double? aoLatBegin { get; set; }

        public double? aoLongBegin { get; set; }

        public double? aoLatEnd { get; set; }

        public double? aoLongEnd { get; set; }

        public double? aoLatCenter { get; set; }

        public double? aoLongCenter { get; set; }

        public double? aoWidth { get; set; }

        public double? aoHeight { get; set; }

        public int? aoBoundingSurfaceIdTop { get; set; }

        public int? aoBoundingSurfaceIdBottom { get; set; }

        [Ignore]
        public virtual tblArchitecturalElement tblArchitecturalElement { get; set; }
        [Ignore]
        public virtual tblBoundingSurface tblBoundingSurface { get; set; }
        [Ignore]
        public virtual tblBoundingSurface tblBoundingSurface1 { get; set; }
        [Ignore]
        public virtual tblObjectOfInvestigation tblObjectOfInvestigation { get; set; }
    }
}
