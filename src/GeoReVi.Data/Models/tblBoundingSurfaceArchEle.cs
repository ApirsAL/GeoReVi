namespace GeoReVi
{
    using SQLite;
    using LiteDB;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [System.ComponentModel.DataAnnotations.Schema.Table("tblBoundingSurfaceArchEle")]
    public partial class tblBoundingSurfaceArchEle 
    {
        [Key, BsonId]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int aeIdFk { get; set; }

        [Key, BsonId]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int bsidFk { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        [StringLength(10)]
        public string topbottom { get; set; }

        [Ignore]
        public virtual tblArchitecturalElement tblArchitecturalElement { get; set; }
        [Ignore]
        public virtual tblBoundingSurface tblBoundingSurface { get; set; }
    }
}
