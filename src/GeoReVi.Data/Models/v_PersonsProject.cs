namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class v_PersonsProject 
    {
        [Key, BsonId]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int prjIdFk { get; set; }

        [Key, BsonId]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int persIdFk { get; set; }

        [Key, BsonId]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int persIdPk { get; set; }

        [StringLength(255)]
        public string persName { get; set; }

        [StringLength(255)]
        public string persVorname { get; set; }

        [StringLength(50)]
        public string persAffiliation { get; set; }

        [StringLength(511)]
        public string persFullName { get; set; }
    }
}
