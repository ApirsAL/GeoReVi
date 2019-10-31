namespace GeoReVi
{
    using LiteDB;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [System.ComponentModel.DataAnnotations.Schema.Table("tblCuboid")]
    public partial class tblCuboid 
    {
        [Key, BsonId]
        public int cubIdPk { get; set; }

        [StringLength(255)]
        public string cubLabel { get; set; }

        public double? cubHeigth { get; set; }

        public double? cubWidth { get; set; }

        public double? cubLength { get; set; }

        public bool? cubThinSection { get; set; }

        public bool? cubDestroyed { get; set; }

        public double? cubXAxisDipDirection { get; set; }

        public double? cubXAxisDipAngle { get; set; }

        public double? cubYAxisDipDirection { get; set; }

        public double? cubYAxisDipAngle { get; set; }

        public double? cubStratificationDipDirection { get; set; }

        public double? cubStratificationDipAngle { get; set; }

        public double? cubBeddingDipDirection { get; set; }

        public double? cubBeddingDipAngle { get; set; }

        [StringLength(50)]
        public string cubOrientationToBedding { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "timestamp")]
        [System.ComponentModel.DataAnnotations.MaxLength(8)]
        [Timestamp]
        public byte[] SSMA_TimeStamp { get; set; }
    }

    /// <summary>
    /// A cuboid sample
    /// </summary>
    public class CuboidSample
    {
        public tblRockSample RockSample { get; set; }
        public tblCuboid Cuboid {get;set;}
    }

}
