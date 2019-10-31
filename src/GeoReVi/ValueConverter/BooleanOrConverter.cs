using System;
using System.Windows.Data;

namespace GeoReVi
{
    /// <summary>
    /// A multivalueconverter for user interface
    /// FROM https://stackoverflow.com/questions/7434245/validation-error-style-in-wpf-similar-to-silverlight
    /// </summary>
    public class BooleanOrConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            foreach (object value in values)
            {
                if ((bool)value == true)
                {
                    return true;
                }
            }
            return false;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
