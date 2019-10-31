using System;
using System.Globalization;
using System.Windows;

namespace GeoReVi
{
    /// <summary>
    /// Converts a visibility to an inverse boolean
    /// </summary>
    public class VisibilityToInverseBooleanConverter : BaseValueConverter<VisibilityToInverseBooleanConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
                //if the value is true, hide the item, else show it
                return (Visibility)value == Visibility.Visible ? false : true;
            else
                //if the value is true, hide the item, else show it
                return (Visibility)value == Visibility.Visible ? false : true;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
