namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tblPetrography4 
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblPetrography4()
        {
            tblPetrography5 = new HashSet<tblPetrography5>();
        }

        [Key, BsonId]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int petr4IdPk { get; set; }

        public int? petr4petr3IdFk { get; set; }

        [StringLength(255)]
        public string petr4PetrographicTerm { get; set; }

        [StringLength(255)]
        public string petr4Abbreviation { get; set; }

        [StringLength(255)]
        public string petr4Definition { get; set; }

        [StringLength(255)]
        public string petr4AGI1987 { get; set; }

        public virtual tblPetrography3 tblPetrography3 { get; set; }

        public virtual tblUnionPetrography tblUnionPetrography { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblPetrography5> tblPetrography5 { get; set; }
    }
}
