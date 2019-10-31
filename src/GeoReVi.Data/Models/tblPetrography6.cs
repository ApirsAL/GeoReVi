namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tblPetrography6 
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblPetrography6()
        {
            tblPetrography7 = new HashSet<tblPetrography7>();
        }

        [Key, BsonId]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int petr6IdPk { get; set; }

        public int? petr6petr5IdFk { get; set; }

        [StringLength(255)]
        public string petr6PetrographicTerm { get; set; }

        [StringLength(255)]
        public string petr6Abbreviations { get; set; }

        [StringLength(255)]
        public string petr6Definition { get; set; }

        [StringLength(255)]
        public string petr6AGI1987 { get; set; }

        public virtual tblPetrography5 tblPetrography5 { get; set; }

        public virtual tblUnionPetrography tblUnionPetrography { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblPetrography7> tblPetrography7 { get; set; }
    }
}
