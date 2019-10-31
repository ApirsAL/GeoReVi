namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tblLithofaciesArchitecturalElement 
    {
        public int lftIdFk { get; set; }

        public int archIdFk { get; set; }

        [Key, BsonId]
        public int litarIdPk { get; set; }

        public virtual tblArchitecturalElement tblArchitecturalElement { get; set; }

        public virtual tblFacy tblFacy { get; set; }
    }
}
