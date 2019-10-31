namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblObjectLithostratigraphy")]
    public partial class tblObjectLithostratigraphy 
    {
        [Key, BsonId]
        public int ooilitIdPk { get; set; }

        public int ooiIdFk { get; set; }

        public int lithID { get; set; }
    }
}
