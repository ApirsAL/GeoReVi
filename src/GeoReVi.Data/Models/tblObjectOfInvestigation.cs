namespace GeoReVi
{
    using LiteDB;
    using SQLite;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity.Spatial;

    [System.ComponentModel.DataAnnotations.Schema.Table("tblObjectOfInvestigation")]
    [SQLite.Table("tblObjectOfInvestigation")]
    public partial class tblObjectOfInvestigation
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblObjectOfInvestigation()
        {
            tblArchEleOccurences = new HashSet<tblArchEleOccurence>();
        }

        [Key, PrimaryKey, AutoIncrement, Ignore, BsonId]
        public int ooiIdPk { get; set; }

        [StringLength(255)]
        public string ooiName { get; set; }

        [StringLength(255)]
        public string ooiType { get; set; }

        public int? ooiOriginHeight { get; set; }

        [StringLength(255)]
        public string ooiHeightReferenceSystem { get; set; }

        [StringLength(255)]
        public string ooiBasin { get; set; }

        public int? ooiGeologicalMapNumber { get; set; }

        public double? ooiLatitude { get; set; }

        [Range(-180, 180)]
        public double? ooiLongitude { get; set; }

        [StringLength(255)]
        public string ooiGeolocationPointWKT { get; set; }

        public DateTime? ooiDateUpload { get; set; }

        [StringLength(50)]
        public string ooiUploadersName { get; set; }

        [StringLength(255)]
        public string ooiOwner { get; set; }

        [StringLength(255)]
        public string ooiNotes { get; set; }

        public int? ooiUploaderIdFk { get; set; }

        [Ignore]
        [BsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblArchEleOccurence> tblArchEleOccurences { get; set; }


    }
}
