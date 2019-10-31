namespace GeoReVi
{
    using LiteDB;
    using SQLite;
    using System.ComponentModel.DataAnnotations;

    public partial class tblFaciesObservation 
    {
        [Key, BsonId]
        public int foIdPk { get; set; }

        public int fofacIdFk { get; set; }

        public double? foWidthMeter { get; set; }

        public double? foHeightMeter { get; set; }

        public double? foDipAngle { get; set; }

        public double? foDipDirection { get; set; }

        public int? foPersonIdFk { get; set; }

        public double? foLatitude { get; set; }

        public double? foLongitude { get; set; }

        public double? foAltitude { get; set; }

        [Ignore]
        public virtual tblFacy tblFacy { get; set; }
        [Ignore]
        public virtual tblPerson tblPerson { get; set; }
    }
}
