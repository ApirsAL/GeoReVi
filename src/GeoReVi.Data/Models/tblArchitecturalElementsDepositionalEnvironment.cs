namespace GeoReVi
{
    using SQLite;
    using System;
    using LiteDB;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tblArchitecturalElementsDepositionalEnvironment 
    {
        public int archIdFk { get; set; }

        public int depenvIdFk { get; set; }

        [Key, BsonId]
        public int aedepenvIdPk { get; set; }

        [Ignore]
        public virtual tblArchitecturalElement tblArchitecturalElement { get; set; }
        [Ignore]
        public virtual tblDepositionalEnvironmentCatalogue tblDepositionalEnvironmentCatalogue { get; set; }
    }
}
