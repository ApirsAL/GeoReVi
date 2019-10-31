namespace GeoReVi
{
    using LiteDB;
    using SQLite;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [System.ComponentModel.DataAnnotations.Schema.Table("tblDepositionalEnvironmentLithostrat")]
    public partial class tblDepositionalEnvironmentLithostrat 
    {
        [Key, BsonId]
        public int depenvlithIdPk { get; set; }

        public int depenvIdFk { get; set; }

        public int litIdFk { get; set; }

        [Ignore]
        public virtual tblDepositionalEnvironmentCatalogue tblDepositionalEnvironmentCatalogue { get; set; }

        [Ignore]
        public virtual tblUnionLithostratigraphy tblUnionLithostratigraphy { get; set; }
    }
}
