using LiteDB;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoReVi
{
    [Table("tblSonicLog")]
    public class tblSonicLog
    {
        [Key, BsonId]
        public int slfimeIdFk { get; set; }

        public double slValue { get; set; }

        public double slMeasurementTime { get; set; }

        public double slDeviceVelocity { get; set; }

        public virtual tblMeasurement tblMeasurement { get; set; }
    }
}
