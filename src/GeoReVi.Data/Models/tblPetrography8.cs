namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tblPetrography8 
    {
        [Key, BsonId]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int petr8IdPk { get; set; }

        public int? petr8petr7IdFk { get; set; }

        [StringLength(255)]
        public string petr8PetrographicTerm { get; set; }

        [StringLength(255)]
        public string petr8Abbreviations { get; set; }

        [StringLength(255)]
        public string petr8Definition { get; set; }

        [StringLength(255)]
        public string petr8AGI1987 { get; set; }

        public virtual tblPetrography7 tblPetrography7 { get; set; }

        public virtual tblUnionPetrography tblUnionPetrography { get; set; }
    }
}
