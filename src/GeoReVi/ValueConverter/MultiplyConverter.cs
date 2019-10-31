using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GeoReVi
{
    /// <summary>
    /// FROM https://www.codeproject.com/Articles/248112/Templating-WPF-Expander-Control
    /// </summary>
    public class MultiplyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType,
               object parameter, CultureInfo culture)
        {
            double result = 1.0;
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] is double)
                    result *= (double)values[i];
            }

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes,
               object parameter, CultureInfo culture)
        {
            throw new Exception("Not implemented");
        }
    }
}
