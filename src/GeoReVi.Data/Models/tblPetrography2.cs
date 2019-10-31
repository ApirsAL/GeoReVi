namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tblPetrography2 
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblPetrography2()
        {
            tblPetrography3 = new HashSet<tblPetrography3>();
        }

        [Key, BsonId]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int petr2IdPk { get; set; }

        public int petr2petr1IdFk { get; set; }

        [StringLength(255)]
        public string petr2PetrographicTerm { get; set; }

        [StringLength(255)]
        public string petr2Abbreviation { get; set; }

        [StringLength(255)]
        public string petr2Definition { get; set; }

        [StringLength(255)]
        public string petr2AGI1987 { get; set; }

        public virtual tblPetrography1 tblPetrography1 { get; set; }

        public virtual tblUnionPetrography tblUnionPetrography { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblPetrography3> tblPetrography3 { get; set; }
    }
}
