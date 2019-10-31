using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace GeoReVi
{
    [ValueConversion(typeof(int), typeof(string))]
    public class IntegerToStringConverter : BaseValueConverter<IntegerToStringConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((int)value)
            {
                case 0:
                    return "New record";
                default:
                    return value;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((string)value)
            {
                case "New record":
                    return 0;
                default:
                    return (int)value;
            }
        }
    }
}
