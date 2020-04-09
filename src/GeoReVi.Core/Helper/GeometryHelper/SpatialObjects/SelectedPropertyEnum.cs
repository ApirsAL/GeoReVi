using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace GeoReVi
{
    public enum SelectedPropertyEnum
    {
        [Description("Property 1")]
        Property1 = 0,
        [Description("Property 2")]
        Property2 = 1,
        [Description("Property 3")]
        Property3 = 2,
        [Description("Property 4")]
        Property4 = 3,
        [Description("Property 5")]
        Property5 = 4,
        [Description("Property 6")]
        Property6 = 5,
        [Description("Property 7")]
        Property7 = 6,
        [Description("Property 8")]
        Property8 = 7,
        [Description("Property 9")]
        Property9 = 8,
        [Description("Property 10")]
        Property10 = 9,
        [Description("X coordinate")]
        XAxis = 10,
        [Description("Y coordinate")]
        YAxis = 11,
        [Description("Z coordinate")]
        ZAxis=12,

    }
}