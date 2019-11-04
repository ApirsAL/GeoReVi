using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace GeoReVi
{
    public class PositioningValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Select(x => (double)x).Sum();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
