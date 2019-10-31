using Caliburn.Micro;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace GeoReVi
{
    public class LocationValueToPointCollectionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return new PointCollection(((BindableCollection<LocationTimeValue>)values[0]).Select(x => new System.Windows.Point(x.X + 0.5 * (double)values[1], x.Y + 0.5 * (double)values[1])));
        }
    
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
