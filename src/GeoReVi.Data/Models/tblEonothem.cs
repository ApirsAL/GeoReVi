namespace GeoReVi
{
    using LiteDB;
    using SQLite;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [System.ComponentModel.DataAnnotations.Schema.Table("tblEonothem")]
    public partial class tblEonothem 
    {
        [BsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblEonothem()
        {
            tblErathems = new HashSet<tblErathem>();
        }

        [Key, BsonId]
        public int eonIdPk { get; set; }

        [StringLength(255)]
        public string eonName { get; set; }

        public int? eonNumericalAgeLowerBoundary { get; set; }

        public int? eonPlusMinus { get; set; }

        public int? eonchronIdFk { get; set; }

        [Ignore, BsonIgnore]
        public virtual tblUnionChronostratigraphy tblUnionChronostratigraphy { get; set; }

        [Ignore, BsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblErathem> tblErathems { get; set; }
    }
}
