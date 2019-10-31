namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    [Table("tblProject")]
    public partial class tblProject 
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblProject()
        {
            tblRockSamples = new HashSet<tblRockSample>();
        }

        [Key, BsonId]
        public int prjIdPk { get; set; }

        [Required]
        [StringLength(50)]
        public string prjName { get; set; }

        public int prjCreatorIdFk { get; set; }

        [Column(TypeName = "date")]
        public DateTime? prjBeginDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? prjEndDate { get; set; }

        public double? prjBudget { get; set; }

        [StringLength(255)]
        public string prjSponsors { get; set; }

        [StringLength(255)]
        public string prjPartners { get; set; }

        public double? prjLat { get; set; }

        public double? prjLong { get; set; }

        [StringLength(50)]
        public string prjCountry { get; set; }

        public virtual tblPerson tblPerson { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblRockSample> tblRockSamples { get; set; }

        [NotMapped, BsonIgnore]
        /// <summary>
        /// Initials of the project
        /// </summary>
        public string Initials
        {
            get
            {
                return string.Concat(prjName.Where(c => c >= 'A' && c <= 'Z').Take(2));
            }
        }
    }
}
