using System;
using System.Windows.Data;
using System.Windows.Media;

namespace GeoReVi
{
    [ValueConversion(typeof(Color), typeof(SolidColorBrush))]
    public class ColorToSolidBrushValueConverter : BaseValueConverter<ColorToSolidBrushValueConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (null == value)
            {
                return null;
            }
            // You can support here more source types if you wish
            if(value is SolidColorBrush)
            {
                SolidColorBrush color = (SolidColorBrush)value;
                return color.Color;

            }

            Type type = value.GetType();
            throw new InvalidOperationException("Unsupported type [" + type.Name + "]");
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // If necessary, here you can convert back. Check if which brush it is (if its one),
            // get its Color-value and return it.
            if (null == value)
            {
                return null;
            }
            // For a more sophisticated converter, check also the targetType and react accordingly..
            if (value is Color)
            {
                Color color = (Color)value;
                return new SolidColorBrush(color);
            }

            throw new NotImplementedException();
        }

    }
}
