namespace GeoReVi
{
    using LiteDB;
    using SQLite;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [System.ComponentModel.DataAnnotations.Schema.Table("tblXRayFluorescenceSpectroscopy")]
    public partial class tblXRayFluorescenceSpectroscopy 
    {
        [Key, PrimaryKey, BsonId]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int xrfIdPk { get; set; }

        [Required]
        [StringLength(255)]
        public string xrfType { get; set; }

        public double? xrfSiO2 { get; set; }

        public double? xrfTiO2 { get; set; }

        public double? xrfAl2O3 { get; set; }

        public double? xrfFe2O3 { get; set; }

        public double? xrfCaO { get; set; }

        public double? xrfMgO { get; set; }

        public double? xrfMnO { get; set; }

        public double? xrfK2O { get; set; }

        public double? xrfNa2O { get; set; }

        public double? xrfP2O5 { get; set; }

        public double? xrfAs { get; set; }

        public double? xrfBa { get; set; }

        public double? xrfBi { get; set; }

        public double? xrfCd { get; set; }

        public double? xrfCe { get; set; }

        public double? xrfCl { get; set; }

        public double? xrfCo { get; set; }

        public double? xrfCr { get; set; }

        public double? xrfCs { get; set; }

        public double? xrfCu { get; set; }

        public double? xrfDy { get; set; }

        public double? xrfEr { get; set; }

        public double? xrfEu { get; set; }

        public double? xrfF { get; set; }

        public double? xrfGa { get; set; }

        public double? xrfGd { get; set; }

        public double? xrfHf { get; set; }

        public double? xrfHo { get; set; }

        public double? xrfLa { get; set; }

        public double? xrfMo { get; set; }

        public double? xrfNb { get; set; }

        public double? xrfNd { get; set; }

        public double? xrfNi { get; set; }

        public double? xrfPb { get; set; }

        public double? xrfPr { get; set; }

        public double? xrfRb { get; set; }

        public double? xrfS { get; set; }

        public double? xrfSb { get; set; }

        public double? xrfSc { get; set; }

        public double? xrfSm { get; set; }

        public double? xrfSn { get; set; }

        public double? xrfSr { get; set; }

        public double? xrfTa { get; set; }

        public double? xrfTb { get; set; }

        public double? xrfTh { get; set; }

        public double? xrfU { get; set; }

        public double? xrfV { get; set; }

        public double? xrfW { get; set; }

        public double? xrfY { get; set; }

        public double? xrfYb { get; set; }

        public double? xrfZn { get; set; }

        public double? xrfZr { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public double? xrfSum { get; set; }

        public double? xrfTime { get; set; }

        [StringLength(255)]
        public string xrfMode { get; set; }

        public double? xrfTi { get; set; }

        public bool? xrfCalibrationPassed { get; set; }

        public virtual tblMeasurement tblMeasurement { get; set; }
    }

    public enum Molecule
    {
        SiO2,
        TiO2,
        Al2O,
        Fe2O,
        CaO,
        MgO,
        MnO,
        K2O,
        Na2O,
        P2O5,
        As,
        Ba,
        Bi,
        Cd,
        Ce,
        Cl,
        Co,
        Cr,
        Cs,
        Cu,
        Dy,
        Er,
        Eu,
        F,
        Ga,
        Gd,
        Hf,
        Ho,
        La,
        Mo,
        Nb,
        Nd,
        Ni,
        Pb,
        Pr,
        Rb,
        S,
        Sb,
        Sc,
        Sm,
        Sn,
        Sr,
        Ta,
        Tb,
        Th,
        U,
        V,
        W,
        Y,
        Yb,
        Zn
    }
}
