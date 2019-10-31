namespace GeoReVi
{
    using LiteDB;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("tblHandpiece")]
    public partial class tblHandpiece 
    {
        [Key, BsonId]
        public int hpIdPk { get; set; }

        [StringLength(255)]
        public string hpLabelFk { get; set; }

        [StringLength(255)]
        public string hpGeometry { get; set; }

        [StringLength(255)]
        public string hpPurpose { get; set; }

        [StringLength(255)]
        public string hpDescription { get; set; }

        [StringLength(255)]
        public string hpOrientation { get; set; }

        public bool? hpThinSection { get; set; }
    }

    /// <summary>
    /// A handpiece sample
    /// </summary>
    public class HandpieceSample
    {
        public tblRockSample RockSample { get; set; }
        public tblHandpiece Handpiece { get; set; }
    }
}
