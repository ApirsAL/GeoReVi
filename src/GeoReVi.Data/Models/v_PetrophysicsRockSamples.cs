namespace GeoReVi
{
    using LiteDB;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class v_PetrophysicsRockSamples 
    {
        [StringLength(255)]
        public string labmeSampleName { get; set; }

        [Key, BsonId]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long RowID { get; set; }

        public string Lithostratigraphy { get; set; }

        public string Petrography { get; set; }

        public string Lithofacies { get; set; }

        public string Chronostratigraphy { get; set; }

        [Column("Object of investigation")]
        public string Object_of_investigation { get; set; }

        [Column("Sample type")]
        public string Sample_type { get; set; }

        [Column("Architectural element")]
        public string Architectural_element { get; set; }

        [Column("Depositional environment")]
        public string Depositional_environment { get; set; }

        [Column("Local x")]
        public double? Local_x { get; set; }

        public double? Longitude { get; set; }

        public double? Latitude { get; set; }

        [Column("Local y")]
        public double? Local_y { get; set; }

        [Column("Local z")]
        public double? Local_z { get; set; }

        [Column("Project ID")]
        public int? Project_ID { get; set; }
        
        [Column("Grain density")]
        public double? Grain_density { get; set; }

        [Column("Bulk density")]
        public double? Bulk_density { get; set; }

        [Column("Porosity")]
        public double? Porosity { get; set; }

        [Column("Intrinsic permeability")]
        public double? Intrinsic_permeability { get; set; }

        [Column("Apparent permeability")]
        public double? Apparent_permeability { get; set; }

        [Column("Thermal conductivity")]
        public double? Thermal_conductivity { get; set; }

        [Column("Thermal diffusivity")]
        public double? Thermal_diffusivity { get; set; }

        [Column("P sonic wave velocity")]
        public double? P_sonic_wave_velocity { get; set; }

        [Column("S sonic wave velocity")]
        public double? S_sonic_wave_velocity { get; set; }

        [Column("Resistivity")]
        public double? Resistivity { get; set; }

        public double? C13{ get; set; }

        public double? C14 { get; set; }

        public double? O16 { get; set; }

        public double? O18 { get; set; }

        public double? SiO2 { get; set; }

        public double? Al2O3 { get; set; }

        public double? Fe2O3 { get; set; }

        public double? CaO { get; set; }

        public double? Mgo { get; set; }

        public double? K2O { get; set; }

        public double? Na2O { get; set; }

        public double? MnO { get; set; }

        public double? TiO2 { get; set; }

        public double? P2O5 { get; set; }
    }
}
