namespace GeoReVi
{
    using LiteDB;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class v_FileStore 
    {
        [Key, BsonId]
        [Column(Order = 0)]
        public Guid stream_id { get; set; }

        public byte[] file_stream { get; set; }

        [Key, BsonId]
        [Column(Order = 1)]
        [StringLength(255)]
        public string name { get; set; }

        [Key, BsonId]
        [Column(Order = 2)]
        public bool is_directory { get; set; }

        public string unc_Path { get; set; }
    }
}
