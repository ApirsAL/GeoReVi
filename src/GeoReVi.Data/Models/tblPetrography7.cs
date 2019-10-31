namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tblPetrography7 
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblPetrography7()
        {
            tblPetrography8 = new HashSet<tblPetrography8>();
        }

        [Key, BsonId]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int petr7IdPk { get; set; }

        public int? petr7petr6IdFk { get; set; }

        [StringLength(255)]
        public string petr7PetrographicTerm { get; set; }

        [StringLength(255)]
        public string petr7Abbreviations { get; set; }

        [StringLength(255)]
        public string petr7Definition { get; set; }

        [StringLength(255)]
        public string petr7AGI1987 { get; set; }

        public virtual tblPetrography6 tblPetrography6 { get; set; }

        public virtual tblUnionPetrography tblUnionPetrography { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblPetrography8> tblPetrography8 { get; set; }
    }
}
