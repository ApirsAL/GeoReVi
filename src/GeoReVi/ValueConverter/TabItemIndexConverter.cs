using System;
using System.Globalization;
using System.Windows.Data;

namespace GeoReVi
{
    [ValueConversion(typeof(object), typeof(int))]
    public class TabConverter : BaseValueConverter<TabConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
