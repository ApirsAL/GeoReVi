using LiteDB;
using System.ComponentModel.DataAnnotations;

namespace GeoReVi
{
    public partial class tblWater
    {
        [Key, BsonId]
        public int watIdPk { get; set; }

        [Required]
        [StringLength(255)]
        public string watName { get; set; }

        [StringLength(50)]
        public string watSubType { get; set; }

        public double watFlowVolume { get; set; }
    }
}
