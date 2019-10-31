using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GeoReVi
{
    /// <summary>
    /// Value equality converter
    /// </summary>
    public class EnumValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            if (value.ToString() == parameter.ToString())
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || parameter == null)
                return null;
            var rtnValue = parameter.ToString();
            try
            {
                object returnEnum = Enum.Parse(targetType, rtnValue);
                return returnEnum;
            }
            catch
            {
                return null;
            }
        }
    }
}
