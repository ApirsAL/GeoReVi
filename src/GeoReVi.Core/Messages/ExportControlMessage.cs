using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GeoReVi
{
    public class ExportControlMessage
    {
        public ExportControlMessage(UIElement element, string format)
        {
            Element = element;
            Format = format;
        }

        public UIElement Element
        {
            get;
            private set;
        }

        public string Format
        {
            get;
            private set;
        }
    }
}
