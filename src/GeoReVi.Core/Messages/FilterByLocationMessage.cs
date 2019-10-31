using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoReVi
{
    /// <summary>
    /// A model event to change to another user
    /// </summary>
    public class FilterByLocationMessage
    {
        public FilterByLocationMessage(Location _selectionStart, Location _selectionEnd)
        {
            SelectionEndLocation = _selectionEnd;
            SelectionStartLocation = _selectionStart;
        }

        /// <summary>
        /// Selection properties
        /// </summary>
        public Location SelectionStartLocation
        { get; set; }
        public Location SelectionEndLocation
        { get; set; }
    }
}
