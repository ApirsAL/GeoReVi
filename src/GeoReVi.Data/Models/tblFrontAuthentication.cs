using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoReVi
{
    [Table("tblFrontEndAuthentication")]
    public class tblFrontEndAuthentication
    {
        [Key]
        public int faIdPk { get; set; }

        [StringLength(50)]
        public string faRT { get; set; }

        [StringLength(int.MaxValue)]
        public string faRF { get; set; }
    }
}
