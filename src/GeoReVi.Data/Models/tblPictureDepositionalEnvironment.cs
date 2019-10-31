namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblPictureDepositionalEnvironment")]
    public partial class tblPictureDepositionalEnvironment 
    {
        [Key, BsonId]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int picIdFk { get; set; }

        [Key, BsonId]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int depenvIdFk { get; set; }

        [StringLength(255)]
        public string picName { get; set; }

        public Guid? picStreamIdFk { get; set; }

        public virtual tblDepositionalEnvironmentCatalogue tblDepositionalEnvironmentCatalogue { get; set; }
    }
}
