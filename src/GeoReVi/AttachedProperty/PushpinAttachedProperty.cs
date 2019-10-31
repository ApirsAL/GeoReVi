using System.Data;

namespace GeoReVi
{
    /// <summary>
    /// Overriding the pushpin class to attach a data table
    /// </summary>
    public partial class Pushpin : Microsoft.Maps.MapControl.WPF.Pushpin
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsClicked { get; set; }
        public DataTable Data { get; set; }
    }
}