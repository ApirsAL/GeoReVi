using LiteDB;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoReVi
{
    public class v_PetrophysicsFieldMeasurements
    {
        [Key, BsonId]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long RowID { get; set; }

        [Column("Local x (m)")]
        public double? Local_x { get; set; }

        [Column("Local y (m)")]
        public double? Local_y { get; set; }

        [Column("Local z (m)")]
        public double? Local_z { get; set; }

        [Column("Object of investigation")]
        public string Object_of_investigation { get; set; }

        [Column("Project ID")]
        public int? Project_ID { get; set; }

        public string Lithostratigraphy { get; set; }

        [Column("Facies type")]
        public string Lithofacies { get; set; }

        public string Chronostratigraphy { get; set; }

        //[Column("Architectural element")]
        //public string Architectural_element { get; set; }

        //[Column("Depositional environment")]
        //public string Depositional_environment { get; set; }

        [Column("Magnetic susceptibility maximum SI-4")]
        public double? Magnetic_susceptibility_maximum { get; set; }

        [Column("Potassium maximum %")]
        public double? Potassium_maximum_percent { get; set; }

        [Column("Uranium maximum ppm")]
        public double? Uranium_maximum_ppm { get; set; }

        [Column("Thorium maximum ppm")]
        public double? Thorium_maximum_ppm { get; set; }
        
        [Column("API maximum")]
        public double? API_maximum { get; set; }

        [Column("Dip angle °")]
        public int? Dip_angle_degree { get; set; }

        [Column("Dip direction °")]
        public int? Dip_direction_degree { get; set; }

    }
}
