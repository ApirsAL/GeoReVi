using System;
using System.Globalization;
using System.Windows;

namespace GeoReVi
{
    /// <summary>
    /// Converts a boolean to a <see cref="Visibility"/>
    /// </summary>
    public class BooleanToInverseVisibilityConverter : BaseValueConverter<BooleanToInverseVisibilityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
                //if the value is true, hide the item, else show it
                return (bool)value ? Visibility.Hidden : Visibility.Visible;
            else
                return (bool)value ? Visibility.Visible : Visibility.Hidden;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
