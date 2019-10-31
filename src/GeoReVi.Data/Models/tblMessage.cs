namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblMessage")]
    public partial class tblMessage 
    {
        [Key, BsonId]
        public int messIdPk { get; set; }

        public int messFromPersonIdFk { get; set; }

        public int messToPersonIdFk { get; set; }

        [Required]
        [StringLength(100)]
        public string messHeader { get; set; }

        public string messPlainText { get; set; }

        public bool? messRead { get; set; }

        public DateTime? messDate { get; set; }

        [StringLength(50)]
        public string messFromName { get; set; }

        public virtual tblPerson tblPerson { get; set; }

        public virtual tblPerson tblPerson1 { get; set; }
    }
}
