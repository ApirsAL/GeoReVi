namespace GeoReVi
{
    using LiteDB;
    using System.ComponentModel.DataAnnotations;

    public partial class tblAffiliation 
    {
        [Key, BsonId]
        public int affIdPk { get; set; }

        [Required]
        [StringLength(50)]
        public string affname { get; set; }

        [StringLength(50)]
        public string affStreet { get; set; }

        [StringLength(10)]
        public string affHouseNumber { get; set; }

        [StringLength(50)]
        public string affCity { get; set; }

        public int? affPostCode { get; set; }

        [StringLength(255)]
        public string affURL { get; set; }

        [StringLength(50)]
        public string affCountry { get; set; }
    }
}
