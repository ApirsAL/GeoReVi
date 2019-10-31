namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tblPetrography1 
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblPetrography1()
        {
            tblPetrography2 = new HashSet<tblPetrography2>();
        }

        [Key, BsonId]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int petr1PetrographyID { get; set; }

        [StringLength(255)]
        public string petr1PetrographicTerm { get; set; }

        [StringLength(255)]
        public string petr1Abbreviation { get; set; }

        public string petr1Definition { get; set; }

        [StringLength(255)]
        public string petr1AGI1987 { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] SSMA_TimeStamp { get; set; }

        public virtual tblUnionPetrography tblUnionPetrography { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblPetrography2> tblPetrography2 { get; set; }
    }
}
