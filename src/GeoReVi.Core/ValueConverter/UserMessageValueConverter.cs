using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoReVi
{
    public static class UserMessageValueConverter
    {
        //Convert to int
        public static int Convert(object value)
        {
            switch ((string)value)
            {

                case "An unexpected error occured.":
                    return 1;
                case "Unknown":
                    return 0;
                default:
                    return 0;
            }
        }

        //Convert to int
        public static string ConvertBack(int value)
        {
            switch ((int)value)
            {
                case 1:
                    return "An unexpected error occured.";
                case 0:
                    return "Unknown";
                default:
                    return "Unknown";
            }
        }
    }
}
