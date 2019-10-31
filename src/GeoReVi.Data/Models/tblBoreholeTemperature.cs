using LiteDB;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoReVi
{
    [Table("tblBoreholeTemperature")]
    public class tblBoreholeTemperature
    {
        [Key, BsonId]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int btfimeIdFk { get; set; }

        public double btValue { get; set; }

        public double btMeasurementTime { get; set; }

        public double btDeviceVelocity { get; set; }

        public virtual tblMeasurement tblMeasurement { get; set; }
    }
}
