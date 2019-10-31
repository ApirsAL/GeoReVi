namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tblPetrography5 
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblPetrography5()
        {
            tblPetrography6 = new HashSet<tblPetrography6>();
        }

        [Key, BsonId]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int petr5IdPk { get; set; }

        public int? petr5petr4IdFk { get; set; }

        [StringLength(255)]
        public string petr5PetrographicTerm { get; set; }

        [StringLength(255)]
        public string petr5Abbreviations { get; set; }

        [StringLength(255)]
        public string petr5Definition { get; set; }

        [StringLength(255)]
        public string petr5AGI1987 { get; set; }

        public virtual tblPetrography4 tblPetrography4 { get; set; }

        public virtual tblUnionPetrography tblUnionPetrography { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblPetrography6> tblPetrography6 { get; set; }
    }
}
