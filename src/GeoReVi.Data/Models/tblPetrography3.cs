namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tblPetrography3 
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblPetrography3()
        {
            tblPetrography4 = new HashSet<tblPetrography4>();
        }

        [Key, BsonId]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int petr3PetrographyID { get; set; }

        public int? petr3petr2IdFk { get; set; }

        [StringLength(255)]
        public string petr3PetrographicTerm { get; set; }

        [StringLength(255)]
        public string petr3Abbreviation { get; set; }

        [StringLength(255)]
        public string petr3Definition { get; set; }

        [StringLength(255)]
        public string petr3AGI1987 { get; set; }

        public virtual tblPetrography2 tblPetrography2 { get; set; }

        public virtual tblUnionPetrography tblUnionPetrography { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblPetrography4> tblPetrography4 { get; set; }
    }
}
