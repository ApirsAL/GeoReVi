using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoReVi
{
    public class GrainSizeToIntConverter : BaseValueConverter<GrainSizeToIntConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((string)value)
            {
                case "Boulder":
                    return 11;
                case "Cobble":
                    return 10;
                case "Pebble":
                    return 9;
                case "Granule":
                    return 8;
                case "Very coarse sand":
                    return 7;
                case "Coarse sand":
                    return 6;
                case "Medium sand":
                    return 5;
                case "Fine sand":
                    return 4;
                case "Very fine sand":
                    return 3;
                case "Silt":
                    return 2;
                case "Mud":
                    return 1;
                case "Giant (>30 mm)":
                    return 7;
                case "Huge (>10 – 30 mm)":
                    return 6;
                case "Coarse (>3 – 10 mm)":
                    return 5;
                case "Medium (>1 – 3 mm)":
                    return 4;
                case "Small (>0.3 – 1 mm)":
                    return 3;
                case "Fine (>0.1 – 0.3 mm)":
                    return 2;
                case "Dense (>0.1 mm)":
                    return 1;
                case "Unknown":
                    return 0;
                default:
                    return 0;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        //Convert to int
        public int Convert(object value)
        {
            switch ((string)value)
            {
                case "Boulder":
                    return 11;
                case "Cobble":
                    return 10;
                case "Pebble":
                    return 9;
                case "Granule":
                    return 8;
                case "Very coarse sand":
                    return 7;
                case "Coarse sand":
                    return 6;
                case "Medium sand":
                    return 5;
                case "Fine sand":
                    return 4;
                case "Very fine sand":
                    return 3;
                case "Silt":
                    return 2;
                case "Mud":
                    return 1;
                case "Giant (>30 mm)":
                    return 7;
                case "Huge (>10 – 30 mm)":
                    return 6;
                case "Coarse (>3 – 10 mm)":
                    return 5;
                case "Medium (>1 – 3 mm)":
                    return 4;
                case "Small (>0.3 – 1 mm)":
                    return 3;
                case "Fine (>0.1 – 0.3 mm)":
                    return 2;
                case "Dense (>0.1 mm)":
                    return 1;
                case "Unknown":
                    return 1;
                default:
                    return 1;
            }
        }

        //Convert to int
        public string ConvertBack(int value)
        {
            switch ((int)value)
            {
                case 11:
                    return "Boulder";
                case 10:
                    return "Cobble";
                case 9:
                    return "Pebble";
                case 8:
                    return "Granule";
                case 7:
                    return "Vcs/Giant";
                case 6:
                    return "Cs/Huge";
                case 5:
                    return "Ms/Coarse";
                case 4:
                    return "Fs/Medium";
                case 3:
                    return "Vfs/Small";
                case 2:
                    return "Silt/Fine";
                case 1:
                    return "Mud/Dense";
                case 0:
                    return "Unknown";
                default:
                    return "Unknown";
            }
        }
    }
}
