
using System;
using System.Diagnostics;
using System.Globalization;

namespace GeoReVi
{
    public class ActiveItemValueConverter : BaseValueConverter<ActiveItemValueConverter>
    {
        /// <summary>
        /// Converts a ActiveItem object to a viewmodel
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            switch((ActiveItemView)value)
            {
                case ActiveItemView.HomeView:
                    return new HomeViewModel();
                default:
                    Debugger.Break();
                    return null;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
